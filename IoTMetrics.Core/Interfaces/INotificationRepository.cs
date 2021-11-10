using IoTMetrics.Models.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Core.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<Notification> GetByName(string name);
    }
}
