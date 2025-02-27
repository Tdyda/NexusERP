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
        private static readonly HttpClient _httpClient = Locator.Current.GetService<HttpClient>() ?? throw new InvalidOperationException("HttpClient service not found.");
        private const string UpdateUrl = "http://10.172.111.78/NexusERP_latest.json";

        public static void CheckForUpdatesAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NexusERP-Updater/1.0");
                var response = _httpClient.GetStringAsync(UpdateUrl).Result;
                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(response);

                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if(updateInfo != null && CompareVersions(currentVersion, updateInfo.Version) < 0)
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

            for(int i = 0; i < currentVersionParts.Length; i++)
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

                await warningDialog.ShowDialog(mainWindow);

                if (!warningDialog.IsConfirmed)
                {
                    return;
                }

            try
            {

                string fileUrl = updateInfo.Url;
                string tempFilePath = Path.Combine(Path.GetTempPath(), "NexusERP_install.exe");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(fileUrl, tempFilePath);
                }

                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = tempFilePath,
                    UseShellExecute = true
                });

                Environment.Exit(0);

                if (process != null)
                {
                    process.WaitForExit();
                }

                // Po zakończeniu procesu instalacji usuwamy plik
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                    Debug.WriteLine("Plik instalacyjny został usunięty.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas uruchamiania instalatora: {ex.Message}");
            }
        }
    }
}
