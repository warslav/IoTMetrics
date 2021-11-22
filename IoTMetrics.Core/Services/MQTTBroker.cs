using IoTMetrics.Core.DTO.Options;
using IoTMetrics.Models.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class MQTTBroker : BackgroundService
    {
        private readonly MQTTOptions _mqttOptions;
        private IMqttServerOptions _mqttServerOptions;
        private IMqttServer _mqttServer;
        private static ILogger<MQTTBroker> _logger;

        public MQTTBroker(IOptions<MQTTOptions> mqttOptions, ILogger<MQTTBroker> logger)
        {
            _logger = logger;
            _mqttOptions = mqttOptions.Value;
            this.BuildServerOptions();
            this.CreateMqttServer();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.StartMqttServerAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void BuildServerOptions()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(this._mqttOptions.BrokerHostName);
                MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder();

                if (this._mqttOptions.UseSSL)
                {
                    optionsBuilder.WithClientCertificate()
                        .WithEncryptionSslProtocol(this._mqttOptions.MqttSslProtocol);
                }

                optionsBuilder.WithDefaultEndpointBoundIPAddress(ipAddress)
                    .WithDefaultEndpointPort(this._mqttOptions.BrokerPort)
                    .WithSubscriptionInterceptor(c =>
                    {
                        c.AcceptSubscription = true;
                    })
                    .WithApplicationMessageInterceptor(c =>
                    {
                        c.AcceptPublish = true;
                    });

                this._mqttServerOptions = optionsBuilder.Build();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateMqttServer()
        {
            try
            {
                _mqttServer = new MqttFactory().CreateMqttServer();

                _mqttServer.UseClientConnectedHandler(ClientConnectedHandler);
                _mqttServer.UseClientDisconnectedHandler(ClientDisconnectedHandler);
                _mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(args =>
                {
                    try
                    {
                        string clientID = args.ClientId;
                        MqttTopicFilter topicFilter = (MqttTopicFilter)args.TopicFilter;

                        string topicString = ConvertTopicFilterToString(topicFilter);
                        _logger.LogInformation($"[{DateTime.Now}] clientID: {clientID} subscribed topic: {topicString}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                });

                _mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(args =>
                {
                    try
                    {
                        string clientID = args.ClientId;
                        string topicFilter = args.TopicFilter;
                        _logger.LogInformation($"[{DateTime.Now}] clientID: {clientID} unsubscribed topic: {topicFilter}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private static string ConvertTopicFilterToString(MqttTopicFilter topicFilter)
        {
            string output = string.Empty;

            if (topicFilter != null)
            {
                output = $"{topicFilter.Topic} - {topicFilter.QualityOfServiceLevel.ToString()}";
            }

            return output;
        }

        private async Task StartMqttServerAsync()
        {
            try
            {
                if (this._mqttServerOptions == null)
                {
                    throw new ArgumentNullException(nameof(this._mqttServerOptions));
                }

                await _mqttServer.StartAsync(this._mqttServerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public static void ClientConnectedHandler(MqttServerClientConnectedEventArgs args)
        {
            try
            {
                string clientID = args.ClientId;
                _logger.LogInformation($"[{DateTime.Now}] Client '{clientID}' connected.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public static void ClientDisconnectedHandler(MqttServerClientDisconnectedEventArgs args)
        {
            try
            {
                string clientID = args.ClientId;
                MqttClientDisconnectType mqttClientDisconnectType = args.DisconnectType;
                _logger.LogInformation($"[{DateTime.Now}] Client '{clientID}' disconnected, disconnectType: {mqttClientDisconnectType}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task PublishMetricAsync(string name, int value, DateTime dateTime, int deviceId)
        {
            var messageObj = new
            {
                Name = name,
                Value = value,
                Time = dateTime,
                DeviceId = deviceId
            };
            var message = JsonSerializer.Serialize(messageObj);
            try
            {
                await _mqttServer.PublishAsync(_mqttOptions.TopicName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }
    }
}
