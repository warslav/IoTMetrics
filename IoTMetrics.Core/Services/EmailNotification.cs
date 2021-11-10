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
using System.Net;
using System.Net.Mail;
using System.Text;
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
            if (notification == null || (value > notification.MinValue && value < notification.MaxValue))
            {
                return;
            }
            else if (value <= notification.MinValue)
            {
                body = await EmailTemplate(name,value, true);
            }
            else
            {
                body = await EmailTemplate(name, value, false);
            }
            await SendEmail(notification.Email, body, "IoTMetrics");
        }

        public async Task<string> EmailTemplate(string name, int value, bool minValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div style=\"color:red;\">{0} = {1}</div>", name, value);
            if (minValue)
            {
                sb.AppendFormat("<div style=\"color:red;\">{0} below minimum value</div>", name);
            }
            else
            {
                sb.AppendFormat("<div style=\"color:red;\">{0} is higher than maximum value</div>", name);
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
