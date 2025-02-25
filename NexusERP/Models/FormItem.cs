using NexusERP.Data;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    public class FormItem
    {
        public string Index { get; set; }
        public double? Quantity { get; set; }
        public string Name { get; set; }
        public string? Comment { get; set; }
        public string OrderBatch { get; set; }
        public ObservableCollection<string> AvalivableOptions { get; }

        public FormItem(ObservableCollection<string> availableOptions)
        {
            AvalivableOptions = availableOptions;
        }
    }
}
