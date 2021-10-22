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

namespace IoTMetrics.Core
{
    public static class DIConfiguration
    {
        
        
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            
            //services.AddDbContext<SensorContext>();
            services.AddDbContext<SensorContext>(options => options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
