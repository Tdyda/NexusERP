using Microsoft.EntityFrameworkCore;
using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class DetailedOrderViewModel : ViewModelBase, IAuthorizedViewModel, IActivatableViewModel
    {
        public string[] RequiredRoles => ["mecalux", "admin"];
        public string? UrlPathSegment => "detailedOrderView";
        public IScreen HostScreen { get; }
        private readonly AppDbContext _appDbContext;
        public ObservableCollection<Order> Orders { get; set; }
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public DetailedOrderViewModel(IScreen screen, string index)
        {
            HostScreen = screen;
            _appDbContext = Locator.Current.GetService<AppDbContext>() ?? throw new Exception("AppDbContext service not found.");
            Orders = new ObservableCollection<Order>();

            this.WhenActivated(disposables =>
            {
                Observable.FromAsync(() => LoadDetailedOrders(index))
                          .Subscribe()
                          .DisposeWith(disposables);
            });
        }

        public async Task LoadDetailedOrders(string index)
        {
            var orders = await _appDbContext.Orders.Where(x => x.Index == index && x.Status == Enums.OrderStatus.Accepted).ToListAsync();

            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }
    }
}
