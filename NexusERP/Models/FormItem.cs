using NexusERP.Data;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Models
{
    public class FormItem : ReactiveObject
    {
        private string _name;

        public string Index { get; set; }
        public double? Quantity { get; set; }
        public string Name {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string? Comment { get; set; }
        public string OrderBatch { get; set; }
        public ObservableCollection<string> AvalivableOptions { get; }

        public FormItem(ObservableCollection<string> availableOptions)
        {
            AvalivableOptions = availableOptions;
        }
    }
}
