using IoTMetrics.Core.DTO.Options;
using IoTMetrics.Core.Interfaces;
using IoTMetrics.Models.Models;
//using MailKit.Net.Smtp;
//using MimeKit;
//using System.Security.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class EmailNotification : IEmailNotification
    {
        private readonly EmailOptions _emailOptions;
        private readonly IUnitOfWork _unitOfWork;

        public EmailNotification(IOptions<EmailOptions> emailOptions, IUnitOfWork unitOfWork)
        {
            _emailOptions = emailOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task CheckMetric(string name, int value)
        {
            Notification notification = await _unitOfWork.NotificationRepository.GetByName(name);
            string body = "";
            if (notification == null)
            {
                return;
            }
            if (notification.MinValue != null && value <= notification.MinValue)
            {
                body += await EmailTemplate(name,value, notification.MinValue.ToString(), "MinValue");
            }
            if (notification.MaxValue != null && value >= notification.MaxValue)
            {
                body += await EmailTemplate(name, value, notification.MaxValue.ToString(), "MaxValue");
            }
            if (!string.IsNullOrWhiteSpace(notification.Condition) && await IsMatchCondition(notification.Condition, value))
            {
                body += await EmailTemplate(name, value, notification.Condition, "Condition");
            }
            if (!string.IsNullOrEmpty(body))
            {
                await SendEmail(notification.Email, body, "IoTMetrics");
            }
            
        }

        public async Task<(bool IsSucces, string improvedСondition)> CheckCorrectConditionAsunc(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                return await Task.FromResult((true, condition));
            }
            string[] values = Regex.Split(condition, @"<=|>=|<|>|==|!=|\|\||&&");
            if (values.Length < 2)
            {
                return await Task.FromResult((false, ""));
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (Int32.TryParse(values[i], out int _) || values[i] == "value")
                {
                    if (values[i].Trim() != values[i])
                    {
                        condition = Regex.Replace(condition, values[i], values[i].Trim());
                    }
                    continue;
                }
                if (string.IsNullOrWhiteSpace(values[i]))
                {
                    return await Task.FromResult((false, ""));
                }
                condition = Regex.Replace(condition, values[i], "value");
            }
            return await Task.FromResult((true, condition));
        }

        async Task<bool> IsMatchCondition(string condition, int value)
        {
            try
            {
                string[] values = Regex.Split(condition, @"<=|>=|<|>|==|!=|\|\||&&");
                for (int i = 0; i < values.Length; i++)
                {
                    if (Int32.TryParse(values[i], out int _) || values[i].Trim() == "@0")
                    {
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(values[i]))
                    {
                        throw new Exception("wrong condition");
                    }
                    condition = Regex.Replace(condition, values[i], "@0");
                }

                var fn = DynamicExpressionParser.ParseLambda<bool>(null, false, condition, value).Compile();
                return await Task.FromResult(fn());
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<string> EmailTemplate(string name, int value, string checkValue, string checkName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div style=\"color:red;\">{0} = {1}</div>", name, value);
            switch (checkName)
            {
                case "MinValue":
                    sb.AppendFormat("<div style=\"color:red;\">{0} below minimum value({1})</div>", name, checkValue);
                    break;
                case "MaxValue":
                    sb.AppendFormat("<div style=\"color:red;\">{0} is higher than maximum value({1})</div>", name, checkValue);
                    break;
                case "Condition":
                    sb.AppendFormat("<div style=\"color:red;\">{0} matched condition expression({1})</div>", name, checkValue);
                    break;
                
            }
            
            return await Task.FromResult(sb.ToString());
        }

        public async Task SendEmail(string email, string body, string subject)
        {
            if (!Int32.TryParse(_emailOptions.Port, out int port))
            {
                return;
            }
            try
            {
                #region MailKit Don`t Work
                //var emailMessage = new MimeMessage();

                //emailMessage.From.Add(new MailboxAddress(subject, _emailOptions.EmailAddress));
                //emailMessage.To.Add(new MailboxAddress("", email));
                //emailMessage.Subject = "Message from " + subject;
                //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                //{
                //    Text = body
                //};

                //using (var client = new SmtpClient())
                //{
                //    //MailKit.Security.SecureSocketOptions.SslOnConnect
                //    //client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                //    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                //    await client.ConnectAsync(_emailOptions.Host, port, false);
                //    client.AuthenticationMechanisms.Remove("XOAUTH2");
                //    await client.AuthenticateAsync(_emailOptions.EmailAddress, _emailOptions.Password);
                //    await client.SendAsync(emailMessage);

                //    await client.DisconnectAsync(true);
                //}
                #endregion

                #region System SmtpClient

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(_emailOptions.EmailAddress, subject);
                message.To.Add(email);
                message.Subject = $"Message from {subject}";
                message.Body = body;
                using (SmtpClient client = new SmtpClient(_emailOptions.Host))
                {
                    client.Credentials = new NetworkCredential(_emailOptions.EmailAddress, _emailOptions.Password);
                    client.Port = port;
                    client.EnableSsl = true;
                    //client.Send(message);
                    await client.SendMailAsync(message);
                }

                #endregion
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }
            
        }
    }
}
