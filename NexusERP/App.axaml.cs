using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using NexusERP.Data;
using NexusERP.Services;
using NexusERP.ViewModels;
using NexusERP.Views;
using ReactiveUI;
using Splat;

namespace NexusERP
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

                // Skonfiguruj po³¹czenie do bazy danych tutaj
                optionsBuilder.UseMySql(
                    "Server=10.172.111.78;Database=order_warehouse_db;User=tdyda;Password=Online1234!;",
                    new MySqlServerVersion(new Version(10, 11, 6))
                );

                var httpClient = new HttpClient();

                Locator.CurrentMutable.RegisterConstant(new AppDbContext(optionsBuilder.Options), typeof(AppDbContext));
                Locator.CurrentMutable.RegisterLazySingleton(() => new AuthService(httpClient), typeof(AuthService));


                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}