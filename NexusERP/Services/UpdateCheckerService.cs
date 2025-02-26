using Newtonsoft.Json;
using NexusERP.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private const string UpdateUrl = "https://api.github.com/repos/tdyda/NexusERP/releases/latest";

        public static async Task CheckForUpdatesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(UpdateUrl);
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

        private static void ShowUpdateDialog(UpdateInfo updateInfo)
        {
            System.Diagnostics.Process.Start(updateInfo.Url);
        }
    }
}
