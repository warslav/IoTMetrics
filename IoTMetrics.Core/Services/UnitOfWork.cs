using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private SensorContext _context;

        public IDeviceRepository DeviceRepository { get; }
        public IMetricRepository MetricRepository { get; }
        public INotificationRepository NotificationRepository { get; }

        public UnitOfWork(SensorContext context, IDeviceRepository deviceRepository, IMetricRepository metricRepository, INotificationRepository notificationRepository)
        {
            _context = context;
            DeviceRepository = deviceRepository;
            MetricRepository = metricRepository;
            NotificationRepository = notificationRepository;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public virtual async Task<int> SaveAsync()
        {
            int records = 0;
            IDbContextTransaction tx = null;

            try
            {
                using (tx = await _context.Database.BeginTransactionAsync())
                {
                    records = await _context.SaveChangesAsync();
                    tx.Commit();
                    return records;
                }
            }
            catch (Exception)
            {
                tx.Rollback();
            }
            
            return records;
        }
    }
}
