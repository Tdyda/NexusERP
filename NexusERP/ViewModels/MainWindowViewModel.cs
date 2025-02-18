using NexusERP.Models;
using NexusERP.ViewModels;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace NexusERP.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IScreen
    {
        private readonly UserSession _userSession;
        public ICommand Launch { get; }
        public RoutingState Router { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> ShowAddOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ShowOrderList { get; }
        public List<string> Roles => _userSession.Roles;

        private bool _CanAccessAddOrder;
        public bool CanAccessAddOrder
        {
            get => _CanAccessAddOrder;
            set
            {
                Debug.WriteLine($"Setting CanAccessAddOrder to {value}");
                this.RaiseAndSetIfChanged(ref _CanAccessAddOrder, value);
            }
        }
        public MainWindowViewModel()
        {
            Launch = ReactiveCommand.Create(() =>
            {
                Debug.WriteLine($"CanAccessAddOrder: {CanAccessAddOrder}");
                CanAccessAddOrder = !CanAccessAddOrder;
                Debug.WriteLine($"CanAccessAddOrder: {CanAccessAddOrder}");
            });

            _userSession = Locator.Current.GetService<UserSession>() ?? throw new Exception("UserSession service not found.");

            Router.Navigate.Execute(new LoginViewModel(this));

            ShowAddOrder = ReactiveCommand.CreateFromObservable(
                () => NavigateWithAuthorization(new AddOrderViewModel(this)));

            ShowOrderList = ReactiveCommand.CreateFromObservable(
                () => NavigateWithAuthorization(new OrderListViewModel(this)));

            this.WhenAnyValue(us => us._userSession)
                .Where(session => session != null)
                .Select(session => session.Roles.Contains("admin"))
                .Subscribe(x =>
                {
                    Debug.WriteLine($"CanAccessAddOrder: {CanAccessAddOrder}");
                    this.RaiseAndSetIfChanged(ref _CanAccessAddOrder, x);
                    Debug.WriteLine($"CanAccessAddOrder: {CanAccessAddOrder}");
                });
        }

        private IObservable<IRoutableViewModel> NavigateWithAuthorization(IRoutableViewModel viewModel)
        {
            if (viewModel is IAuthorizedViewModel authViewModel)
            {
                if (!authViewModel.RequiredRoles.Any(role => Roles.Contains(role)))
                {
                    Debug.WriteLine("Brak uprawnień, przekierowanie na stronę logowania.");
                    return Router.Navigate.Execute(new LoginViewModel(this));
                }
            }

            return Router.Navigate.Execute(viewModel);
        }
    }
}
