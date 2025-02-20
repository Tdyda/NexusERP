using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;
using Splat;
using System.Diagnostics;

namespace NexusERP.Views;

public partial class OrderListView : ReactiveUserControl<OrderListViewModel>
{
    public OrderListView()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<OrderListViewModel>();
        Debug.WriteLine($"DataContext: {DataContext?.GetType().Name}");
    }
}