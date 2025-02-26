using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NexusERP.ViewModels;
using FluentAvalonia.UI.Controls;
using NexusERP.Data;
using System;
using Splat;
using Microsoft.EntityFrameworkCore;
using NexusERP.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace NexusERP.Views;

public partial class AddOrderView : ReactiveUserControl<AddOrderViewModel>
{
    private CancellationTokenSource _cts;
    private readonly PhmDbContext _phmDbContext = Locator.Current.GetService<PhmDbContext>() ?? throw new Exception("PhmDbContext service not found.");
    public AddOrderView()
    {
        this.InitializeComponent();
    }

    private async void IndexComboBox_LostFocus(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var comboBox = sender as FAComboBox;
        if (comboBox == null) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            await Task.Delay(800, token); // Debounce - czekaj 500ms
        }
        catch (TaskCanceledException)
        {
            return; // Jeœli anulowane, po prostu zakoñcz funkcjê bez b³êdu
        }
        if (token.IsCancellationRequested) return;
        var stopwatch = Stopwatch.StartNew();
        var order = await _phmDbContext.MtlMaterials.FirstOrDefaultAsync(x => x.MaterialId == comboBox.Text);
        stopwatch.Stop();

        Debug.WriteLine($"Query time: {stopwatch.ElapsedMilliseconds}ms");

        // Pobranie FormItem powi¹zanego z aktualnym ComboBoxem
        if (comboBox.DataContext is FormItem formItem && order != null)
        {
            formItem.Name = order.MaterialDesc;
        }
    }


    private void IndexComboBox_PointerPressed(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var comboBox = sender as FAComboBox;
        // Wymuszenie otwarcia dropdowna, kiedy kontrolka uzyska fokus
        if (comboBox != null)
        {
            comboBox.IsDropDownOpen = true;
        }
    }
}