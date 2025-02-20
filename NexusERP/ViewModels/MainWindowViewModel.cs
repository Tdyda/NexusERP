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
using DynamicData.Binding;
using System.Collections.ObjectModel;
using NexusERP.Interfaces;

namespace NexusERP.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IScreen
    {
        private UserSession _userSession;
        private bool _canAccessOrderList;
        private bool _canAccessAddOrder;
        private bool _canAccessLogout;
        private OrderListViewModel _orderListViewModel;
        public RoutingState Router { get; } = new RoutingState();
        public ReactiveCommand<Unit, IRoutableViewModel> ShowAddOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ShowOrderList { get; }
        public ReadOnlyObservableCollection<string> Roles => _userSession.Roles;
        public ICommand Logout { get; }
      
        public bool CanAccessAddOrder
        {
            get => _canAccessAddOrder;
            set => this.RaiseAndSetIfChanged(ref _canAccessAddOrder, value);
        }

        public bool CanAccessOrderList
        {
            get => _canAccessOrderList;
            set => this.RaiseAndSetIfChanged(ref _canAccessOrderList, value);
        }
        public bool CanAccessLogout
        {
            get => _canAccessLogout;
            set => this.RaiseAndSetIfChanged(ref _canAccessLogout, value);
        }
        public MainWindowViewModel()
        {           
            _userSession = Locator.Current.GetService<UserSession>() ?? throw new Exception("UserSession service not found.");
            _orderListViewModel = Locator.Current.GetService<OrderListViewModel>() ?? throw new Exception("OrderListViewModel not found");

            Router.Navigate.Execute(new LoginViewModel(this));

            ShowAddOrder = ReactiveCommand.CreateFromObservable(
                () => NavigateWithAuthorization(new AddOrderViewModel(this)));

            ShowOrderList = ReactiveCommand.CreateFromObservable(
                () => NavigateWithAuthorization(_orderListViewModel));

            _userSession.Roles
                 .ObserveCollectionChanges()
                 .Subscribe(_ =>
                 {
                     CanAccessAddOrder = _userSession.Roles.Contains("admin");
                 });

            _userSession
                .WhenAnyValue(x => x.IsAuthenticated)
                .Subscribe(isAuthenticated =>
                {
                    CanAccessOrderList = isAuthenticated;
                    CanAccessLogout = isAuthenticated;
                });

            Logout = ReactiveCommand.Create(() =>
            {
                _userSession.Logout();
                Router.Navigate.Execute(new LoginViewModel(this));
            });
        }

        private IObservable<IRoutableViewModel> NavigateWithAuthorization(IRoutableViewModel viewModel)
        {
            if (viewModel is IAuthorizedViewModel authViewModel)
            {
                if (!_userSession.IsAuthenticated || (authViewModel.RequiredRoles.Any() && !authViewModel.RequiredRoles.Any(role => Roles.Contains(role))))
                {
                    Debug.WriteLine("Brak uprawnień, przekierowanie na stronę logowania.");
                    return Router.Navigate.Execute(new LoginViewModel(this));
                }
            }
            Debug.WriteLine($"Navigating to: {viewModel.GetType().Name}");

            return Router.Navigate.Execute(viewModel);
        }
    }
}
