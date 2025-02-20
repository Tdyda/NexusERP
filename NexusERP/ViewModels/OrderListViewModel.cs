using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using NexusERP.Enums;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using System;

namespace NexusERP.ViewModels
{
    public class OrderListViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "orderList";
        public IScreen HostScreen { get; }
        public string[] RequiredRoles => [];
        private readonly AppDbContext _appDbContext;
        public ObservableCollection<Order> Orders { get; set; } = new();
        public ICommand ChangeStatusCommand { get; }
        public OrderListViewModel(IScreen screen)
        {
            HostScreen = screen;
            _appDbContext = Locator.Current.GetService<AppDbContext>();

            LoadOrders();

            ChangeStatusCommand = ReactiveCommand.Create<Tuple<Order, string>>(ChangeStatus);
        }

        private async void ChangeStatus(Tuple<Order, string> param)
        {
            if (param == null)
                return;

            var (selectedOrder, newStatus) = param;

            if (selectedOrder == null)
                return;

            var orders = _appDbContext.Orders.ToList();

            foreach(var order in orders)
            if (order.Index == selectedOrder.Index)
            {
                order.Status = Enum.Parse<OrderStatus>(newStatus);
                _appDbContext.Orders.Update(order);

                selectedOrder.Status = order.Status;
            }

            await _appDbContext.SaveChangesAsync();
            LoadOrders();
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
                    OrderDate = g.Max(x => x.OrderDate),
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
