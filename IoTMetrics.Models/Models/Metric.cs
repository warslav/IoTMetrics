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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        public int? DeviceId { get; set; }
        public Device Device { get; set; }
    }
}
