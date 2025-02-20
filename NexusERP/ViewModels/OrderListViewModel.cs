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

            ChangeStatusCommand = ReactiveCommand.Create<Order>(ChangeStatus);
        }

        private async void ChangeStatus(Order selectedOrder)
        {
            if (selectedOrder == null)
                return;

            Debug.WriteLine(selectedOrder);

            var orders = _appDbContext.Orders.ToList();

            foreach(var order in orders)
            {
                if (order != null)
                {
                    order.Status = selectedOrder.Status;
                    _appDbContext.Orders.Update(order);
                }
                await _appDbContext.SaveChangesAsync();
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
