using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(SensorContext context) : base(context)
        {
        }
    }
}
