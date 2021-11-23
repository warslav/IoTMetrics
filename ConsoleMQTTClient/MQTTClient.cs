using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ConsoleMQTTClient
{
    public class MQTTClient
    {
        MQTTSettings _mqttSettings;
        private static ILogger<MQTTClient> _logger;
        IManagedMqttClient _mqttClient;
        ManagedMqttClientOptions _options;

        public MQTTClient(IOptions<MQTTSettings> mqttSettings, ILogger<MQTTClient> logger)
        {
            _mqttSettings = mqttSettings.Value;
            _logger = logger;

            this.BuildClientOptions();
            this.CreateMqttClient();
        }

        private void BuildClientOptions()
        {
            try
            {
                MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                       .WithClientId(_mqttSettings.ClientId)
                                       .WithTcpServer(_mqttSettings.BrokerHostName, _mqttSettings.BrokerPort);

                _options = new ManagedMqttClientOptionsBuilder()
                                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                        .WithClientOptions(builder.Build())
                                        .Build();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateMqttClient()
        {
            try
            {
                _mqttClient = new MqttFactory().CreateManagedMqttClient();
                _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
                _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
                _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
                _mqttClient.SubscribeAsync(_mqttSettings.TopicName);
                _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task StartMqttClientAsync()
        {
            try
            {
                if (this._options == null)
                {
                    throw new ArgumentNullException(nameof(this._options));
                }

                if (_mqttClient.IsStarted)
                {
                    return;
                }
                await _mqttClient.StartAsync(this._options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private static void OnMessageReceived(MqttApplicationMessageReceivedEventArgs context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            _logger.LogInformation(
                "TimeStamp: {TimeStamp} -- Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);

        }

        private static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            _logger.LogInformation("Successfully connected.");
        }

        private static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            _logger.LogWarning("Couldn't connect to broker.");
        }

        private static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            _logger.LogInformation("Successfully disconnected.");
        }
    }
}
