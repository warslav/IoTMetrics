using IoTMetrics.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IDeviceRepository DeviceRepository { get; }
        public IMetricRepository MetricRepository { get; }
        Task<int> SaveAsync();
    }
}
