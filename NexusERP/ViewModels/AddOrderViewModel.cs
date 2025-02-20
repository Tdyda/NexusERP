using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using NexusERP.Services;
using NexusERP.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Splat;

namespace NexusERP.ViewModels
{
    public class AddOrderViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "addOrder";
        public IScreen HostScreen { get; }
        public string[] RequiredRoles => ["admin", "user"];

        private string _index;
        private string _name;
        private double? _quantity;
        private string _errorMessage;
        private AppDbContext _appDbContext;
        private ObservableCollection<FormItem> _formItems;
        private List<Order> _ordersList;
        public ICommand SubmitCommand { get; }
        public ICommand AddFormItemCommand { get; }

        public AddOrderViewModel(IScreen screen)
        {
            HostScreen = screen;
            SubmitCommand = ReactiveCommand.Create(Submit);
            _appDbContext = Locator.Current.GetService<AppDbContext>() ?? throw new Exception("AppDbContext service not found.");
            _formItems = new ObservableCollection<FormItem>();
            AddFormItemCommand = ReactiveCommand.Create(AddFormItem);
        }

        public string Index
        {
            get => _index;
            set => this.RaiseAndSetIfChanged(ref _index, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public double? Quantity
        {
            get => _quantity;
            set
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    ErrorMessage = "Musisz podać liczbę!";
                    _quantity = value;
                    return;
                }

                if (!int.TryParse(value.ToString(), out int parsedValue) || parsedValue < 0)
                {
                    ErrorMessage = "Zamówienie musi być większe od 0!";
                }
                else
                {
                    ErrorMessage = string.Empty;
                    _quantity = value;
                }

                this.RaiseAndSetIfChanged(ref _quantity, value);
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ObservableCollection<FormItem> FormItems
        {
            get => _formItems;
            set => this.RaiseAndSetIfChanged(ref _formItems, value);
        }
        private void AddFormItem()
        {
            FormItems.Add(new FormItem());
        }

        private async void Submit()
        {
            _ordersList = new List<Order>();

            foreach (var item in FormItems)
            {
                if (item.Quantity > 0)
                {
                    var order = new Order
                    {
                        Index = item.Index,
                        Name = item.Name,
                        Quantity = (double)item.Quantity
                    };
                    _ordersList.Add(order);
                }
            }

            var orderList = new OrderList(_ordersList);
            var finalList = orderList.Calculate();

            foreach (var item in finalList)
            {
                await _appDbContext.Orders.AddAsync(item);
            }
            await _appDbContext.SaveChangesAsync();
        }
    }
}
