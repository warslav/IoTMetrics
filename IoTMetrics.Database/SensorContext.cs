using IoTMetrics.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Database
{
    public class SensorContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Metric> Metrics { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite(Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        //}
        //public SensorContext(DbContextOptions<SensorContext> options,IConfiguration configuration):base(options)
        //{
        //    Configuration = configuration;
        //}
        //public IConfiguration Configuration { get; }

        public SensorContext(DbContextOptions<SensorContext> options) : base(options)
        {
            
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .HasMany<Metric>(d => d.Metrics)
                .WithOne(m => m.Device)
                .HasForeignKey(m => m.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
