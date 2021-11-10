using IoTMetrics.Core.Interfaces;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IoTMetrics.Core.Services
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly SensorContext _context;
        public NotificationRepository(SensorContext context) : base(context)
        {
            _context = context;
        }
        public virtual async Task<Notification> GetByName(string name)
        {
            try
            {
                return await _context.Set<Notification>().Where(n => n.Name == name).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }
    }
}
