using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleIoTEmulator
{
    class Program
    {
        static Endpoints endpoints = new Endpoints();

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            ConfigurationBinder.Bind(configuration.GetSection("Endpoints"), endpoints);
            var url = endpoints.UrlLocalhost;

            Console.WriteLine($"Send metrics to {endpoints.UrlLocalhost}?");
            Console.ReadKey();
            await Sender.SendMessagesAsync(url
                , new string[] { "temperature", "relative humidity", "air pressure", "illuminance" }
                , 12);
        }
    }
}
