using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleMQTTClient
{
    class Program
    {
        private static MQTTSettings _mqttSettings = new MQTTSettings();

        private static IConfiguration _configuration;


        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            ConfigurationBinder.Bind(_configuration.GetSection("MQTTSettings"), _mqttSettings);

            var serviceCollection = new ServiceCollection();
            var mqttAppSettingsConfig = _configuration.GetSection("MQTTSettings");
            serviceCollection.Configure<MQTTSettings>(mqttAppSettingsConfig);
            serviceCollection.AddSingleton<MQTTClient>();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            var serviceProvider = serviceCollection.BuildServiceProvider();
            

            var mqttClient = ActivatorUtilities.CreateInstance<MQTTClient>(serviceProvider);

            Console.WriteLine($"Connect client to host {_mqttSettings.BrokerHostName}:{_mqttSettings.BrokerPort} ?");
            Console.ReadKey();
            await mqttClient.StartMqttClientAsync();

            Console.ReadKey();
        }
    }
}
