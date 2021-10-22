using IoTMetrics.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Database
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using(var context = new SensorContext(serviceProvider.GetRequiredService<DbContextOptions<SensorContext>>()))
            {
                if (!context.Devices.Any())
                {
                    context.Devices.AddRange(
                        new Device
                        {
                            Name = "Room sensor",
                            Company = "Honeywell"
                        },
                        new Device
                        {
                            Name = "Kitchen sensor",
                            Company = "Endress+Hauser"
                        }
                    );
                    context.SaveChanges();
                }
                if (!context.Metrics.Any())
                {
                    context.Metrics.AddRange(
                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 20,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 50,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 500,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },

                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 20,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 50,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 500,
                            Time = DateTime.Parse("2021-10-21 15:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },

                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 21,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 50,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 400,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },

                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 19,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 40,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 300,
                            Time = DateTime.Parse("2021-10-21 16:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },

                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 22,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 50,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 500,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Room sensor" && d.Company == "Honeywell").Id
                        },

                        new Metric
                        {
                            Name = "temperature",//Range: -40..+85°C  более 54 °С менее 12
                            Value = 20,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "relative humidity",//Range: 0..100%RH до 20 и более 70 Ограниченно-допустимые
                            Value = 50,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "air pressure",//Range: 300..1200 mBar /751
                            Value = 751,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        },
                        new Metric
                        {
                            Name = "illuminance",//Range: 0 до 200 000 Lux /100-750
                            Value = 500,
                            Time = DateTime.Parse("2021-10-21 17:00"),
                            DeviceId = context.Devices.FirstOrDefault(d => d.Name == "Kitchen sensor" && d.Company == "Endress+Hauser").Id
                        }
                        );
                    context.SaveChanges();
                }
            }
        }
    }
}
