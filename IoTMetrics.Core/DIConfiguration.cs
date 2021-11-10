using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using IoTMetrics.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IoTMetrics.Models;
using IoTMetrics.Core.Interfaces;
using IoTMetrics.Core.Services;
using IoTMetrics.Core.DTO.Options;

namespace IoTMetrics.Core
{
    public static class DIConfiguration
    {
        
        
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<SensorContext>(options => options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<IMetricRepository, MetricRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();

            var azureAppSettingsConfig = configuration.GetSection("AzureSBSettings");
            services.Configure<AzureSBOptions>(azureAppSettingsConfig);
            services.AddHostedService<ReceiverAzureSB>();

            var emailAppSettingsConfig = configuration.GetSection("EmailSettings");
            services.Configure<EmailOptions>(emailAppSettingsConfig);
            services.AddTransient<IEmailNotification, EmailNotification>();

            services.AddTransient<SenderAzureSB>();
        }
    }
}
