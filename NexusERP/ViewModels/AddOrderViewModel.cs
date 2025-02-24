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
using NexusERP.Enums;

namespace NexusERP.ViewModels
{
    public class AddOrderViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "addOrder";
        public IScreen HostScreen { get; }
        public string[] RequiredRoles => ["sk", "admin"];

        private string _index;
        private string _name;
        private double? _quantity;
        private string? _comment;
        private string _errorMessage;
        private AppDbContext _appDbContext;
        private ObservableCollection<FormItem> _formItems;
        private UserSession? _userSession;
        private List<Order> _ordersList;
        private string _orderBatch;

        public ICommand SubmitCommand { get; }
        public ICommand AddFormItemCommand { get; }

        public AddOrderViewModel(IScreen screen)
        {
            HostScreen = screen;
            SubmitCommand = ReactiveCommand.Create(Submit);
            _appDbContext = Locator.Current.GetService<AppDbContext>() ?? throw new Exception("AppDbContext service not found.");
            _formItems = new ObservableCollection<FormItem>();
            _userSession = Locator.Current.GetService<UserSession>() ?? throw new Exception("UserSession service not found");
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
        public string? Comment
        {
            get => _comment;
            set => this.RaiseAndSetIfChanged(ref _comment, value);
        }
        public string OrderBatch 
        {
            get => _orderBatch;
            set => this.RaiseAndSetIfChanged(ref _orderBatch, value);
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
                        Quantity = (double)item.Quantity,
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.NotAccepted,
                        ProdLine = _userSession.LocationName,
                        Comment = item.Comment,
                        OrderBatch = item.OrderBatch
                    };
                    _ordersList.Add(order);
                }
            }

            //var orderList = new OrderList(_ordersList);
            //var finalList = orderList.Calculate();

            foreach (var item in _ordersList)
            {
                await _appDbContext.Orders.AddAsync(item);
            }
            await _appDbContext.SaveChangesAsync();
        }
    }
}
