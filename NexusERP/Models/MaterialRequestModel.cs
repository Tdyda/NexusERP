using NexusERP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    public class MaterialRequestModel
    {
        public int Id { get; set; }
        public string Client { get; set; }
        public string Index { get; set; }
        public DateOnly ShippingDate { get; set; }
        public string Name { get; set; }
        public MaterialStatus Status { get; set; }
        public int DemandedQuantity { get; set; }
        public int? OrderQuantity { get; set; }
        public string? Comment { get; set; }
    }
}
