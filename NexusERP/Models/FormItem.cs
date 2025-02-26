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
        private string _searchText;
        private bool _isDropDownOpen;

        public string Index { get; set; }
        public double? Quantity { get; set; }
        public string Name {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public string? Comment { get; set; }
        public string OrderBatch { get; set; }
        public ObservableCollection<string> AvalivableOptions { get; }
        public ObservableCollection<string> AllOptions { get; set; }
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set => this.RaiseAndSetIfChanged(ref _isDropDownOpen, value);
        }

        public string SearchText
        { 
            get => _searchText;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
                FilterOptions();
            }
        }

        public FormItem(ObservableCollection<string> availableOptions, ObservableCollection<string> allOptions)
        {
            AvalivableOptions = availableOptions;
            AllOptions = allOptions;

            this.WhenAnyValue(x => x.SearchText)
           .Subscribe(text =>
           {
               IsDropDownOpen = true;
           });
        }

        private void FilterOptions()
        {
            var filtered = AllOptions.Where(option => option.ToLower().Contains(SearchText.ToLower())).ToList();
            AvalivableOptions.Clear();
            foreach (var item in filtered)
            {
                AvalivableOptions.Add(item);
            }
        }

        public static bool IsValid(FormItem formItem)
        {
            if (string.IsNullOrEmpty(formItem.Index) ||
                formItem.Quantity <= 0 ||
                string.IsNullOrEmpty(formItem.Name) ||                
                string.IsNullOrEmpty(formItem.OrderBatch))
            {
                return false;
            }
            return true;
        }
    }
}
