using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Remote.Protocol.Input;
using NexusERP.ViewModels;
using ReactiveUI;
using Splat;
using System.Diagnostics;

namespace NexusERP.Views;

public partial class OrderListView : ReactiveUserControl<OrderListViewModel>
{
    public OrderListView()
    {
        InitializeComponent();
        DataContext = new OrderListViewModel(Locator.Current.GetService<IScreen>());
    }
}