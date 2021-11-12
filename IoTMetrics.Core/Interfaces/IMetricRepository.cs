using IoTMetrics.Models.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Interfaces
{
    public interface IMetricRepository : IRepository<Metric>
    {
        Task<ICollection<Metric>> Include(int? deviceId);
        Task<ICollection<Metric>> GetMetricsBetweenDates(int deviceId, DateTime startDay, DateTime endDay, string name);
    }
}
