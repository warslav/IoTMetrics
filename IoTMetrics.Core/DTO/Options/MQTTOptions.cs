using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.DTO.Options
{
    public class MQTTOptions
    {
        public string BrokerHostName { get; set; }
        public int BrokerPort { get; set; }
        public bool UseSSL { get; set; }
        public SslProtocols MqttSslProtocol { get; set; }
        public string TopicName { get; set; }
    }
}
