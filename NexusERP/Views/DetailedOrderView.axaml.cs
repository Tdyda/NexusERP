using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;
using ReactiveUI;

namespace NexusERP.Views;

public partial class DetailedOrderView : ReactiveUserControl<DetailedOrderViewModel>
{
    public DetailedOrderView()
    {
        InitializeComponent();
        this.WhenActivated(disposables => { /* Aktywacja widoku */ });
    }
}