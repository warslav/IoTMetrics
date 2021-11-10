using Azure.Messaging.ServiceBus;
using IoTMetrics.Core.DTO.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class SenderAzureSB
    {
        private readonly AzureSBOptions _azureSBOptions;
        public SenderAzureSB(IOptions<AzureSBOptions> azureSBOptions)
        {
            _azureSBOptions = azureSBOptions.Value;
        }

        public async Task SendMessagesAsync(string name, int value, DateTime dateTime, int deviceId)
        {
            ServiceBusClient client = new ServiceBusClient(_azureSBOptions.ServiceBusConnectionString);
            string queueOrTopicName = _azureSBOptions.UseTopic ? _azureSBOptions.TopicName : _azureSBOptions.QueueName;
            ServiceBusSender sender = client.CreateSender(queueOrTopicName);

            string date = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            string message = $"{{\"Name\":\"{name}\",\"Value\":\"{value}\",\"Time\":\"{date}\",\"DeviceId\":\"{deviceId}\"}}";
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message);
            serviceBusMessage.ContentType = "application/json";
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
