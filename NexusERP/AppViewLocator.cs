using NexusERP.ViewModels;
using NexusERP.Views;
using ReactiveUI;
using Splat;
using System;

namespace NexusERP
{
    public class AppViewLocator : IViewLocator
    {
        public AppViewLocator() 
        {
            Locator.CurrentMutable.Register(() => new AddOrderView(), typeof(IViewFor<AddOrderViewModel>));
            Locator.CurrentMutable.Register(() => new OrderListView(), typeof(IViewFor<OrderListViewModel>));
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
        }
        public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
        {
            AddOrderViewModel context => new AddOrderView { DataContext = context },
            OrderListViewModel context => new OrderListView { DataContext = context },
            LoginViewModel context => new LoginView { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}