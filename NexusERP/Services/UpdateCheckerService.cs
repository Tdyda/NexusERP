using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using NexusERP.Models;
using NexusERP.Views;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NexusERP.Services
{
    public class UpdateCheckerService
    {
        private static readonly HttpClient _httpClient = Locator.Current.GetService<HttpClient>() ??
                                                         throw new InvalidOperationException(
                                                             "HttpClient service not found.");

        private const string UpdateUrl = "http://10.172.111.78/NexusERP_latest.json";

        public static void CheckForUpdatesAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NexusERP-Updater/1.0");
                var response = _httpClient.GetStringAsync(UpdateUrl).Result;
                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if (updateInfo != null && CompareVersions(currentVersion, updateInfo.Version) < 0)
                {
                    ShowUpdateDialog(updateInfo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd sprawdzania aktualizacji: {ex.Message}");
            }
        }

        private static int CompareVersions(string currentVersion, string latestVersion)
        {
            var currentVersionParts = currentVersion.Split(".");
            var latestVersionParta = latestVersion.Split(".");

            for (int i = 0; i < currentVersionParts.Length; i++)
            {
                if (int.Parse(currentVersionParts[i]) < int.Parse(latestVersionParta[i]))
                    return -1;
                else if (int.Parse(currentVersionParts[i]) > int.Parse(latestVersionParta[i]))
                    return 1;
            }

            return 0;
        }

        private static async void ShowUpdateDialog(UpdateInfo updateInfo)
        {
            var mainWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            var message = "Dostępna jest aktualizacja programu NexusERP. Czy chcesz ją zainstalować?";
            var warningDialog = new WarningDialog(message);

            if (mainWindow != null) await warningDialog.ShowDialog(mainWindow);
            if (!warningDialog.IsConfirmed) return;

            try
            {
                string fileUrl = updateInfo.Url;
                string tempFolder = Path.Combine(Path.GetTempPath(), "NexusERP_Update");
                string zipFilePath = Path.Combine(tempFolder, "NexusERP.zip");
                // string appDirectory = AppContext.BaseDirectory;
                string appDirectory = "C://NexusERP";
                string updaterPath = Path.Combine(tempFolder, "Updater.bat");

                // Tworzenie katalogu tymczasowego
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                Debug.WriteLine($"Pobieranie aktualizacji do: {zipFilePath}");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fileUrl, zipFilePath);
                }

                Debug.WriteLine("Pobieranie zakończone, kopiowanie Updater.bat...");

                File.WriteAllText(updaterPath, GetUpdaterScript(appDirectory, zipFilePath));

                Debug.WriteLine("Uruchamianie Updater.bat...");

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterPath,
                    UseShellExecute = true,
                });

                Environment.Exit(0); // Zamknięcie aplikacji
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas aktualizacji: {ex.Message}");
            }
        }

        private static string GetUpdaterScript(string appDirectory, string zipFilePath)
        {
            var exePath = Path.Combine(appDirectory, "NexusERP.exe");
            return $@"
        timeout /t 3 /nobreak >nul
        echo Rozpakowywanie aktualizacji...
        powershell -ExecutionPolicy Bypass -NoProfile -Command ""Expand-Archive -Path '{zipFilePath}' -DestinationPath '{appDirectory}' -Force""
        echo Uruchamianie nowej wersji...
        powershell -NoProfile -Command ""Start-Process '{exePath}'""
        ";
        }
    }
}