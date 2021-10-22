using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Models.Models
{
    public class Device : IDBEntity
    {
        public Device()
        {
            this.Metrics = new HashSet<Metric>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }

        public ICollection<Metric> Metrics { get; set;  }
    }
}
