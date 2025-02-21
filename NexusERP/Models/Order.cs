using Microsoft.EntityFrameworkCore.Metadata;
using NexusERP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace NexusERP.Models
{
    public class Order : ReactiveObject
    {
        private OrderStatus _status;

        public int Id { get; set; }
        public string Index { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string ProdLine { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Comment { get; set; }
        public OrderStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    this.RaiseAndSetIfChanged(ref _status, value);
                }
            }
        }
    }
}
