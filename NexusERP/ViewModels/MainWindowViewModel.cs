using Avalonia.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NexusERP.Data;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace NexusERP.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IScreen
    {
        private readonly AppDbContext _context;
        public RoutingState Router { get; } = new RoutingState();
       
        public ReactiveCommand<Unit, IRoutableViewModel> ShowAddOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ShowOrderList { get; }

        public MainWindowViewModel()
        {
            _context = Locator.Current.GetService<AppDbContext>();

            Router.Navigate.Execute(new LoginViewModel(this));          

            ShowAddOrder = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new AddOrderViewModel(this)));

            ShowOrderList = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new OrderListViewModel(this)));
        }       
    }
}
