using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Models.Models
{
    public class Metric : IDBEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Time { get; set; }

        public int DeviceId { get; set; }
        public Device Device { get; set; }
    }
}
