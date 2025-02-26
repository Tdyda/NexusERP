using Microsoft.EntityFrameworkCore;
using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NexusERP.ViewModels
{
    public class UserOrdersViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string[] RequiredRoles => ["sk", "admin"];
        public string? UrlPathSegment => "userOrders";
        public IScreen HostScreen { get; }

        private readonly AppDbContext _appDbContext;
        private readonly UserSession _userSession;
        public ObservableCollection<Order> Orders { get; set; } = new();

        public UserOrdersViewModel(IScreen screen)
        {
            HostScreen = screen;
            _appDbContext = Locator.Current.GetService<AppDbContext>() ?? throw new Exception("AppDbContext service not found.");
            _userSession = Locator.Current.GetService<UserSession>() ?? throw new Exception("UserSession service not found.");

            _ = LoadOrders();
        }

        public async Task LoadOrders()
        {
            Orders.Clear();
            var orders = await _appDbContext.Orders
                .AsNoTracking()
                .Where(x => x.ProdLine == _userSession.LocationName && x.OrderDate.Date >= DateTime.Today)
                .ToListAsync();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }
    }
}
