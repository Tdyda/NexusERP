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
using Microsoft.Extensions.Logging;
using NexusERP.Data;
using NexusERP.Services;
using NexusERP.ViewModels;
using NexusERP.Views;
using ReactiveUI;
using Splat;
using Serilog;
using NexusERP.Models;
using Avalonia.Platform;

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
            //DisableAvaloniaDataAnnotationValidation();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

                // Skonfiguruj po³¹czenie do bazy danych tutaj
                optionsBuilder.UseMySql(
                    "Server=10.172.111.78;Database=order_warehouse_db;User=tdyda;Password=Online1234!;",
                    new MySqlServerVersion(new Version(10, 11, 6))
                );

                var httpClient = new HttpClient();

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog(logger);
                });

                Locator.CurrentMutable.RegisterConstant(new AppDbContext(optionsBuilder.Options), typeof(AppDbContext));
                Locator.CurrentMutable.RegisterConstant(new PhmDbContext(), typeof(PhmDbContext));
                Locator.CurrentMutable.RegisterConstant(loggerFactory, typeof(ILoggerFactory));
                Locator.CurrentMutable.RegisterLazySingleton(() => loggerFactory.CreateLogger<AuthService>(), typeof(AuthService));
                Locator.CurrentMutable.RegisterLazySingleton(() => new AuthService(httpClient, loggerFactory.CreateLogger<AuthService>()), typeof(AuthService));
                Locator.CurrentMutable.RegisterLazySingleton(() => new UserSession(), typeof(UserSession));
                Locator.CurrentMutable.RegisterLazySingleton(() => new JwtDecoder(), typeof(JwtDecoder));
                Locator.CurrentMutable.RegisterLazySingleton(() => new AppDbContext(optionsBuilder.Options), typeof(AppDbContext));
                Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                });

                desktop.MainWindow = Locator.Current.GetService<MainWindow>();
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
