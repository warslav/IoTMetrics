using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTMetrics.Models.Models
{
    public class Notification : IDBEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You cannot leave the Name of metrics blank")]
        public string Name { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string Condition { get; set; }
        [Required(ErrorMessage = "Email Address is required")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
