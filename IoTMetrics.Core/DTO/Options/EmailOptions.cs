using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.DTO.Options
{
    public class EmailOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
