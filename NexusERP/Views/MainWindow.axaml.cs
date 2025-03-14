using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.Services;
using NexusERP.ViewModels;
using ReactiveUI;


namespace NexusERP.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);

            this.AttachDevTools();
        }
    }
}