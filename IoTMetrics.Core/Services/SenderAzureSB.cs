using Azure.Messaging.ServiceBus;
using IoTMetrics.Core.DTO.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class SenderAzureSB
    {
        private readonly AzureSBOptions _azureSBOptions;
        private readonly ServiceBusClient _serviceBusClient;

        public SenderAzureSB(IOptions<AzureSBOptions> azureSBOptions, ServiceBusClient serviceBusClient)
        {
            _azureSBOptions = azureSBOptions.Value;
            _serviceBusClient = serviceBusClient;
        }

        public async Task SendMessagesAsync(string name, int value, DateTime dateTime, int deviceId)
        {
            var queueOrTopicName = _azureSBOptions.UseTopic ? _azureSBOptions.TopicName : _azureSBOptions.QueueName;
            ServiceBusSender sender = _serviceBusClient.CreateSender(queueOrTopicName);

            var messageObj = new
            {
                Name = name,
                Value = value,
                Time = dateTime,
                DeviceId = deviceId
            };
            var message = JsonSerializer.Serialize(messageObj);

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message);
            serviceBusMessage.ContentType = "application/json";
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
