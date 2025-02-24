using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Remote.Protocol.Input;
using NexusERP.Models;
using NexusERP.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;

namespace NexusERP.Views;

public partial class OrderListView : ReactiveUserControl<OrderListViewModel>
{
    public OrderListView()
    {
        InitializeComponent();
        DataContext = new OrderListViewModel(Locator.Current.GetService<IScreen>());
    }

    private void OnOrderDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (sender is DataGrid grid && grid.SelectedItem is Order selectedOrder)
        {
            if (DataContext is OrderListViewModel viewModel)
            {
                viewModel.NavigateToDetailedOrderCommand.Execute(selectedOrder).Subscribe();
            }
        }
    }
}
