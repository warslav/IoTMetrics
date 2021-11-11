using IoTMetrics.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Interfaces
{
    public interface IEmailNotification
    {
        public Task CheckMetric(string name, int value);
        Task<string> EmailTemplate(string name, int value, string checkValue, string checkName);
        Task SendEmail(string email, string body, string subject);
        Task<(bool IsSucces, string improvedСondition)> CheckCorrectConditionAsunc(string condition);
    }
}
