using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;

namespace NexusERP.Views;

public partial class TestView : ReactiveUserControl<TestViewModel>
{
    public TestView()
    {
        InitializeComponent();
    }
}