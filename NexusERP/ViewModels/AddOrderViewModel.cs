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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Linq;

namespace NexusERP.ViewModels
{
    public class AddOrderViewModel : ViewModelBase, IAuthorizedViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "addOrder";
        public IScreen HostScreen { get; }
        private PhmDbContext _phmDbContext;

        public string[] RequiredRoles => ["sk", "admin"];
        public string _name;
        private AppDbContext _appDbContext;
        private ILogger<AddOrderViewModel> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private ObservableCollection<MaterialRequestModel> _materialRequests;
        private ObservableCollection<string> _clientFilters;
        private ObservableCollection<DateOnly> _shippingDateFilters;
        private HashSet<string> _selectedClients = new HashSet<string>();
        private HashSet<DateOnly> _selectedShippingDates = new HashSet<DateOnly>();
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
        public ICommand SubmitCommand { get; }
        private ObservableCollection<string> _avalivableOptions;
        private ObservableCollection<string> _allOptions;
        private string _searchText;

        public ObservableCollection<MaterialRequestModel> MaterialRequests
        {
            get => _materialRequests;
            set => this.RaiseAndSetIfChanged(ref _materialRequests, value);
        }

        public ObservableCollection<string> ClientFilters
        {
            get => _clientFilters;
            set => this.RaiseAndSetIfChanged(ref _clientFilters, value);
        }

        public ObservableCollection<DateOnly> ShippingDateFilters
        {
            get => _shippingDateFilters;
            set => this.RaiseAndSetIfChanged(ref _shippingDateFilters, value);
        }

        public HashSet<string> SelectedClients
        {
            get => _selectedClients;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedClients, value);
                ApplyFilters();
            }
        }

        public HashSet<DateOnly> SelectedShippingDates
        {
            get => _selectedShippingDates;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedShippingDates, value);
                ApplyFilters();
            }
        }       
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
            _logger = Locator.Current.GetService<ILogger<AddOrderViewModel>>() ?? throw new Exception("Logger service not found.");
            AvalivableOptions = new ObservableCollection<string>();
            AllOptions = new ObservableCollection<string>();
            MaterialRequests = new ObservableCollection<MaterialRequestModel>();

            this.WhenActivated(disposable =>
            {
                Observable.FromAsync(LoadMaterialRequests)
                    .Subscribe()
                    .DisposeWith(disposable);
            });

            this.WhenActivated(disposables =>
            {
                Observable.FromAsync(() => LoadRawMaterials())
                          .Subscribe()
                          .DisposeWith(disposables);
            });            
        }

        private async Task LoadMaterialRequests()
        {
            var materialRequests = await _appDbContext.MaterialsRequest.ToListAsync();

            foreach (var item in materialRequests)
            {
                MaterialRequests.Add(item);
            }

            ClientFilters = [.. MaterialRequests.Select(r => r.Client).Distinct()];
            ShippingDateFilters = [.. MaterialRequests.Select(r => r.ShippingDate).Distinct()];
        }
        private void ApplyFilters()
        {
            var filteredRequests = MaterialRequests.Where(r =>
                (SelectedClients.Count == 0 || SelectedClients.Contains(r.Client)) &&
                (SelectedShippingDates.Count == 0 || SelectedShippingDates.Contains(r.ShippingDate))
            ).ToList();

            MaterialRequests = new ObservableCollection<MaterialRequestModel>(filteredRequests);
        }
        private async Task LoadRawMaterials()
        {
            await _semaphore.WaitAsync();

            try
            {
                var mtlMaterials = await _phmDbContext.MtlMaterials.ToListAsync();

                foreach (var material in mtlMaterials)
                {
                    AvalivableOptions.Add(material.MaterialId);
                    AllOptions.Add(material.MaterialId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
