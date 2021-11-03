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
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;

        public ReceiverAzureSB(IOptions<AzureSBOptions> azureSBOptions, IServiceScopeFactory scopeFactory)
        {
            _azureSBOptions = azureSBOptions.Value;
            _scopeFactory = scopeFactory;
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Message received: {body}");

            var options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            // deserialize
            
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
            ServiceBusClient client = new ServiceBusClient(_azureSBOptions.ServiceBusConnectionString);
            string queueOrTopicName = _azureSBOptions.UseTopic ? _azureSBOptions.TopicName : _azureSBOptions.QueueName;
            string subscriptionName = _azureSBOptions.UseTopic ? _azureSBOptions.SubscriptionName : string.Empty;

            // create a processor that we can use to process the messages, either with a queue or a topic/subscription
            ServiceBusProcessor processor = _azureSBOptions.UseTopic
                ? client.CreateProcessor(queueOrTopicName, subscriptionName, new ServiceBusProcessorOptions { })
                : client.CreateProcessor(queueOrTopicName, new ServiceBusProcessorOptions());
            
            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            // start processing 
            processor.StartProcessingAsync();
            return Task.CompletedTask;
        }

    }
}
