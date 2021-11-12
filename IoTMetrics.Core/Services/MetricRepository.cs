using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class MetricRepository : Repository<Metric>, IMetricRepository
    {
        private readonly SensorContext _context;
        public MetricRepository(SensorContext context) : base(context)
        {
            _context = context;
        }

        public virtual async Task<ICollection<Metric>> Include(int? deviceId)
        {
            try
            {
                return await _context.Set<Metric>().Include(p => p.Device).Where(p => p.DeviceId == deviceId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities by deviceId '{deviceId}': {ex.Message}", ex);
            }
        }

        public virtual async Task<ICollection<Metric>> GetMetricsBetweenDates(int deviceId, DateTime startDay, DateTime endDay, string name)
        {
            try
            {
                return await _context.Set<Metric>().Include(p => p.Device)
                    .Where(p => p.DeviceId == deviceId 
                        && p.Time > startDay 
                        && p.Time < endDay 
                        && p.Name == name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities between dates by deviceId '{deviceId}' and name '{name}' : {ex.Message}", ex);
            }
        }
    }
}
