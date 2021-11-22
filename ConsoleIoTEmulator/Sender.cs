using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleIoTEmulator
{
    public static class Sender
    {
        public static async Task SendMessagesAsync(string url, string[] metrics, int deviceId)
        {
            Random random = new Random();
            int value = 0;
            for (int i = 0; i < metrics.Length; i++)
            {
                switch (metrics[i])
                {
                    case "temperature":
                        value = random.Next(12, 54);
                        break;
                    case "relative humidity":
                        value = random.Next(20, 70);
                        break;
                    case "air pressure":
                        value = random.Next(750, 753);
                        break;
                    case "illuminance":
                        value = random.Next(100, 750);
                        break;
                    default:
                        value = random.Next(1000);
                        break;
                }

                var messageObj = new
                {
                    Name = metrics[i],
                    Value = value,
                    Time = DateTime.Now,
                    DeviceId = deviceId
                };
                var message = JsonSerializer.Serialize(messageObj);

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(message);

                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = await request.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                try
                {
                    var httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = await streamReader.ReadToEndAsync();
                        Console.WriteLine(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.ReadKey();
            }

        }
    }
}
