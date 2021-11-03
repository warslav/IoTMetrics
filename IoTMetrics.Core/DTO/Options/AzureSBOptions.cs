using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.DTO.Options
{
    public class AzureSBOptions
    {
        public bool UseTopic { get; set; }

        public String ServiceBusConnectionString { get; set; }

        public String QueueName { get; set; }

        public String TopicName { get; set; }

        public String SubscriptionName { get; set; }
    }
}
