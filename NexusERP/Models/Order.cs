using NexusERP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Index { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string ProdLine { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
    }
}
