using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;

namespace NexusERP.Views;

public partial class UserOrdersView : ReactiveUserControl<UserOrdersViewModel>
{
    public UserOrdersView()
    {
        InitializeComponent();
    }
}