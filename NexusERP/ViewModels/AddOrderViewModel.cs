using NexusERP.Data;
using NexusERP.Interfaces;
using NexusERP.Models;
using NexusERP.Services;
using NexusERP.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Splat;
using NexusERP.Enums;
using System.Linq;
using NexusERP.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NexusERP.ViewModels
{
    public class AddOrderViewModel : ViewModelBase, IAuthorizedViewModel
    {
        public string UrlPathSegment => "addOrder";
        public IScreen HostScreen { get; }
        private PhmDbContext _phmDbContext;

        public string[] RequiredRoles => ["sk", "admin"];

        private string _index;
        public string _name;
        private double? _quantity;
        private string? _comment;
        private string _errorMessage;
        private AppDbContext _appDbContext;
        private ObservableCollection<FormItem> _formItems;
        private UserSession? _userSession;
        private ILogger<AddOrderViewModel> _logger;
        private List<Order> _ordersList;
        private string _orderBatch;

        public ICommand SubmitCommand { get; }
        public ICommand DeleteLastFormItemCommand { get; }
        public ICommand AddFormItemCommand { get; }

        private ObservableCollection<string> _avalivableOptions;
        private ObservableCollection<string> _allOptions;
        private string _searchText;

        public ObservableCollection<string> AvalivableOptions
        {
            get => _avalivableOptions;
            set => this.RaiseAndSetIfChanged(ref _avalivableOptions, value);
        }

        public ObservableCollection<string> AllOptions
        {
            get => _allOptions;
            set => this.RaiseAndSetIfChanged(ref _allOptions, value);
        }

        public string SearchText 
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public AddOrderViewModel(IScreen screen)
        {
            HostScreen = screen;
            _phmDbContext = Locator.Current.GetService<PhmDbContext>() ?? throw new Exception("PhmDbContext service not found.");
            _appDbContext = Locator.Current.GetService<AppDbContext>() ?? throw new Exception("AppDbContext service not found.");
            _userSession = Locator.Current.GetService<UserSession>() ?? throw new Exception("UserSession service not found");
            _logger = Locator.Current.GetService<ILogger<AddOrderViewModel>>() ?? throw new Exception("Logger service not found.");
            SubmitCommand = ReactiveCommand.Create(Submit);
            _formItems = new ObservableCollection<FormItem>();
            AddFormItemCommand = ReactiveCommand.Create(AddFormItem);
            DeleteLastFormItemCommand = ReactiveCommand.Create(DeleteLastFormItem);
            AvalivableOptions = new ObservableCollection<string>();
            AllOptions = new ObservableCollection<string>();

            _ = LoadRawMaterials();
        }

        public string Index
        {
            get => _index;
            set => this.RaiseAndSetIfChanged(ref _index, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public double? Quantity
        {
            get => _quantity;
            set
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    ErrorMessage = "Musisz podać liczbę!";
                    _quantity = value;
                    return;
                }

                if (!int.TryParse(value.ToString(), out int parsedValue) || parsedValue < 0)
                {
                    ErrorMessage = "Zamówienie musi być większe od 0!";
                }
                else
                {
                    ErrorMessage = string.Empty;
                    _quantity = value;
                }

                this.RaiseAndSetIfChanged(ref _quantity, value);
            }
        }
        public string? Comment
        {
            get => _comment;
            set => this.RaiseAndSetIfChanged(ref _comment, value);
        }
        public string OrderBatch
        {
            get => _orderBatch;
            set => this.RaiseAndSetIfChanged(ref _orderBatch, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ObservableCollection<FormItem> FormItems
        {
            get => _formItems;
            set => this.RaiseAndSetIfChanged(ref _formItems, value);
        }
        private async void AddFormItem()
        {
            FormItems.Add(new FormItem(AvalivableOptions, AllOptions));
        }

        private void DeleteLastFormItem()
        {
            if (FormItems.Count > 0)
            {
                FormItems.RemoveAt(FormItems.Count - 1);
            }
        }

        private async Task LoadRawMaterials()
        {
            var mtlMaterials = await _phmDbContext.MtlMaterials.ToListAsync();

            foreach (var material in mtlMaterials)
            {
                AvalivableOptions.Add(material.MaterialId);
                AllOptions.Add(material.MaterialId);
            }
        }       
        private async void Submit()
        {
            _ordersList = new List<Order>();
            FormItem? currentItem = null;
            try
            {
                foreach (var item in FormItems)
                {
                    currentItem = item;

                    var order = new Order
                    {
                        Index = item.Index.ToUpper(),
                        Name = item.Name.ToUpper(),
                        Quantity = (double)item.Quantity,
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.NotAccepted,
                        ProdLine = _userSession.LocationName,
                        Comment = item.Comment,
                        OrderBatch = item.OrderBatch.ToUpper()
                    };
                    _ordersList.Add(order);

                }
                var checkOrders = _appDbContext.Orders
                    .Where(o => o.OrderDate.Date == DateTime.Today)
                    .GroupBy(o => new { o.Index, o.OrderBatch })
                    .Select(g => new Order
                    {
                        Index = g.Key.Index,
                        OrderBatch = g.Key.OrderBatch,
                        Quantity = g.Sum(o => o.Quantity),
                        OrderDate = DateTime.Today
                    })
                    .ToList();


                foreach (var order in _ordersList)
                {
                    checkOrders.Add(order);
                }

                var duplicateOrders = checkOrders
                    .GroupBy(o => new { o.Index, o.OrderBatch })
                    .Where(g => g.Count() > 1)
                    .Select(g => new { g.Key.Index, g.Key.OrderBatch, Count = g.Count() })
                    .ToList();

                if (duplicateOrders.Any())
                {
                    string errorMessage = "Znaleziono duplikaty zamówień dla:\n" +
                        string.Join("\n", duplicateOrders.Select(d => $"Indeks: {d.Index}, Numer partii: {d.OrderBatch}"));

                    // Pobieramy referencję do MainWindow
                    var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                    if (mainWindow != null)
                    {
                        var warningDialog = new WarningDialog(errorMessage);
                        await warningDialog.ShowDialog(mainWindow);

                        if (!warningDialog.IsConfirmed)
                        {
                            return;
                        }
                    }
                }

                foreach (var item in _ordersList)
                {
                    await _appDbContext.Orders.AddAsync(item);
                }
                await _appDbContext.SaveChangesAsync();
                HostScreen.Router.Navigate.Execute(new AddOrderViewModel(HostScreen));

            }
            catch (Exception ex)
            {
                string itemInfo = currentItem != null ? $"Indeks: {currentItem.Index}, Nazwa: {currentItem.Name}, OrderBatch: {currentItem.OrderBatch}" : "Brak danych o elemencie";
                _logger.LogError($"Błąd podczas przetwarzania elementu: {itemInfo}. Exception: {ex.Message}");

                var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                if (mainWindow != null)
                {
                    var errorMessage = "Wystąpił błąd podczas przetwarzania zamówień. Spróbuj ponownie.";
                    var warningDialog = new WarningDialog(errorMessage);
                    await warningDialog.ShowDialog(mainWindow);

                    if (warningDialog.IsConfirmed)
                    {
                        return;
                    }
                }
            }
        }
    }
}
