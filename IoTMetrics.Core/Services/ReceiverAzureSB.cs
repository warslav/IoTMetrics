using Azure.Messaging.ServiceBus;
using IoTMetrics.Core.DTO.Options;
using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class ReceiverAzureSB : BackgroundService
    {
        private readonly AzureSBOptions _azureSBOptions;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceBusClient _serviceBusClient;

        public ReceiverAzureSB(IOptions<AzureSBOptions> azureSBOptions, IServiceScopeFactory scopeFactory, ServiceBusClient serviceBusClient)
        {
            _azureSBOptions = azureSBOptions.Value;
            _scopeFactory = scopeFactory;
            _serviceBusClient = serviceBusClient;
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            Metric metric = JsonSerializer.Deserialize<Metric>(args.Message.Body, options);
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                await _unitOfWork.MetricRepository.AddAsync(metric);
                await _unitOfWork.SaveAsync();
            }
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueOrTopicName = _azureSBOptions.UseTopic ? _azureSBOptions.TopicName : _azureSBOptions.QueueName;
            var subscriptionName = _azureSBOptions.UseTopic ? _azureSBOptions.SubscriptionName : string.Empty;

            ServiceBusProcessor processor = _azureSBOptions.UseTopic
                ? _serviceBusClient.CreateProcessor(queueOrTopicName, subscriptionName)
                : _serviceBusClient.CreateProcessor(queueOrTopicName);
            
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            processor.StartProcessingAsync();
            return Task.CompletedTask;
        }

    }
}
