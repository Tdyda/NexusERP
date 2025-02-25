using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Remote.Protocol.Input;
using Avalonia.VisualTree;
using NexusERP.Models;
using NexusERP.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Linq;

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
