using Microsoft.EntityFrameworkCore;
using NexusERP.Data;
using NexusERP.Enums;
using NexusERP.Interfaces;
using NexusERP.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NexusERP.ViewModels
{
    public class OrderListViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "orderList";
        public IScreen HostScreen { get; }
        public string[] RequiredRoles => ["mecalux", "admin"];
        private readonly AppDbContext _appDbContext;
        private DateTime _selectedDate;
        private TimeSpan _selectedTime;

        public ObservableCollection<Order> Orders { get; set; } = new();
        public ICommand ChangeStatusCommand { get; }
        public ICommand LoadOrdersCommand {  get; }
        public ReactiveCommand<Order, Unit> NavigateToDetailedOrderCommand { get; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => this.RaiseAndSetIfChanged(ref _selectedDate, value);
        }

        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set => this.RaiseAndSetIfChanged(ref _selectedTime, value);
        }
        public OrderListViewModel(IScreen screen)
        {
            HostScreen = screen;
            _appDbContext = Locator.Current.GetService<AppDbContext>();

            ChangeStatusCommand = ReactiveCommand.Create<Tuple<Order, string>>(ChangeStatus);
            LoadOrdersCommand = ReactiveCommand.Create<DateTime, Task>(
                (async time => await RefreshOrders()));

            _ = LoadOrders();

            NavigateToDetailedOrderCommand = ReactiveCommand.CreateFromTask<Order>(async order =>
            {
                if (order == null)
                    return;

                await HostScreen.Router.Navigate.Execute(new DetailedOrderViewModel(HostScreen, order.Index));
            });

        }

        private async void ChangeStatus(Tuple<Order, string> param)
        {
            if (param == null)
                return;

            var (selectedOrder, newStatus) = param;

            if (selectedOrder == null)
                return;

            selectedOrder.Status = Enum.Parse<OrderStatus>(newStatus);
            selectedOrder.RaisePropertyChanged(nameof(selectedOrder.Status));

            var index = Orders.IndexOf(selectedOrder);
            if (index >= 0)
            {
                Orders[index] = selectedOrder;
            }

            var orders = _appDbContext.Orders.Where(x => x.Status == Enums.OrderStatus.Accepted).ToList();

            foreach (var order in orders)
                if (order.Index == selectedOrder.Index)
                {
                    order.Status = Enum.Parse<OrderStatus>(newStatus);             
                    _appDbContext.Orders.Update(order);                   
                }

            await _appDbContext.SaveChangesAsync();
        }

        public async Task RefreshOrders()
        {
            await ChangeStatusOnLoad(SelectedTime);
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            var orders = await _appDbContext.Orders.Where(o => o.Status == Enums.OrderStatus.Accepted)
                .GroupBy(x => x.Index)
                .Select(g => new Order
                {
                    Index = g.Key,
                    Name = g.First().Name,
                    Quantity = g.Sum(x => x.Quantity),
                    OrderDate = g.Max(x => x.OrderDate),
                    Status = OrderStatus.Accepted,
                    ProdLine = g.First().ProdLine,
                    Comment = g.First().Comment,
                    HasComment = g.Any(x => !string.IsNullOrEmpty(x.Comment))
                }).ToListAsync();            

            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);

                order.WhenAnyValue(o => o.Status)
                   .Subscribe(status =>
                   {
                       var index = Orders.IndexOf(order);
                       if (index >= 0)
                       {
                           Orders[index] = order;
                       }
                   });
            }
        }

        private async Task ChangeStatusOnLoad(TimeSpan time)
        {
            var dateTime = DateTime.Today.Add(time);

            var orders = await _appDbContext.Orders.Where(x => x.Status == Enums.OrderStatus.NotAccepted && x.OrderDate <= dateTime).ToListAsync();

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Accepted;
                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();
            }
        }
    }
}
