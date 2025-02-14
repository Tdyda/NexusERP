using NexusERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _authToken;
        private string _refreshToken;
        private string _userId;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterUser(string username, string password)
        {
            var registerData = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("http://10.172.111.78/api/account/register", registerData);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginUser(string username, string password)
        {
            var loginData = new { Email = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("http://10.172.111.78/api/account/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (responseData != null)
                {
                    _authToken = responseData.Token;
                    _refreshToken = responseData.RefreshToken;
                    _userId = responseData.UserId;

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

                    return true;
                }
            }

            return false;
        }

    }
}
