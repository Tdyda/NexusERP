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
using Avalonia.Threading;

namespace NexusERP.Views;

public partial class AddOrderView : ReactiveUserControl<AddOrderViewModel>
{
    private CancellationTokenSource _cts;
    private static readonly SemaphoreSlim _dbContextSemaphore = new SemaphoreSlim(1, 1);
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


        if (token.IsCancellationRequested) return;

        try
        {
            await _dbContextSemaphore.WaitAsync(token);

            var stopwatch = Stopwatch.StartNew();
            var order = await _phmDbContext.MtlMaterials
                .FirstOrDefaultAsync(x => x.MaterialId == comboBox.Text, token);
            stopwatch.Stop();

            Debug.WriteLine($"Query time: {stopwatch.ElapsedMilliseconds}ms");

            if (order == null)
            {
                Debug.WriteLine($"Nie znaleziono materia³u o ID: {comboBox.Text}");
                return;
            }

            if (comboBox.DataContext is FormItem formItem)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    formItem.Name = order.MaterialDesc;
                });
            }
        }
        catch (TaskCanceledException)
        {
            // Operacja zosta³a anulowana
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Wyst¹pi³ b³¹d: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            _dbContextSemaphore.Release();
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