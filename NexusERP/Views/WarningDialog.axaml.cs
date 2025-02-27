using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;
using Tmds.DBus.Protocol;

namespace NexusERP.Views;

public partial class WarningDialog : ReactiveWindow<MainWindowViewModel>
{
    public bool IsConfirmed { get; private set; }

    public WarningDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }
    private void CloseDialog(object? sender, RoutedEventArgs e)
    {
        IsConfirmed = sender is Button button && button.Content.ToString() == "OK";
        Close();
    }
}