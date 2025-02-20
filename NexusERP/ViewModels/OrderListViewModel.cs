using Avalonia.Data;
using Avalonia.Media;
using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Controls;
using NexusERP.Enums;

namespace NexusERP.ViewModels
{
    public class OrderListViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "orderList";
        public IScreen HostScreen { get; }
        public string[] RequiredRoles => [];
        private readonly AppDbContext _appDbContext;
        private string _text;

        public ObservableCollection<Order> Orders { get; set; } = new();
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public OrderListViewModel(IScreen screen)
        {
            HostScreen = screen;
            _appDbContext = Locator.Current.GetService<AppDbContext>();

            Text = "test";

            LoadOrders();

            Debug.WriteLine($"Orders.Count = {Orders.Count}");
            foreach (var order in Orders)
            {
                Debug.WriteLine($"Order: {order.Index} - {order.Name} - {order.Quantity}");
            }
        }

        private void LoadOrders()
        {
            var orders = _appDbContext.Orders.Where(o => o.Status == Enums.OrderStatus.Pending)
                .GroupBy(x => x.Index)
                .Select(g => new Order
                {
                    Index = g.Key,
                    Name = g.First().Name,
                    Quantity = g.Sum(x => x.Quantity),
                    OrderDate = DateTimeOffset.Now,
                    Status = OrderStatus.Pending,
                    ProdLine = g.First().ProdLine
                }).ToList();

            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }
    }
}
