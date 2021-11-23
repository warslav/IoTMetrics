using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMQTTClient
{
    public class MQTTSettings
    {
        public string ClientId { get; set; }
        public string BrokerHostName { get; set; }
        public int BrokerPort { get; set; }
        public string TopicName { get; set; }
    }
}
