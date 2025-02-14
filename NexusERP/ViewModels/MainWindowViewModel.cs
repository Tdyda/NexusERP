using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;

namespace NexusERP.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> ShowAddOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> ShowOrderList { get; }

        public MainWindowViewModel()
        {
            ShowAddOrder = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new AddOrderViewModel(this)));

            ShowOrderList = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new OrderListViewModel(this)));
        }
        public void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl != null)
            {
                var selectedTab = tabControl.SelectedIndex;
                if (selectedTab == 0)
                {
                    ShowAddOrder.Execute().Subscribe();
                }
                else if (selectedTab == 1)
                {
                    ShowOrderList.Execute().Subscribe();
                }
            }
        }

    }
}
