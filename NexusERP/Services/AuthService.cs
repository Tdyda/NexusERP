using Microsoft.Extensions.Logging;
using NexusERP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> RegisterUser(string username, string password)
        {
            _logger.LogInformation("RegisterUser called.");

            var registerData = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("http://nexusERP.local/api/account/register", registerData);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User registered successfully.");
                return true;
            }
            else
            {
                _logger.LogError("Failed to register user.");
                return false;
            }
        }

        public async Task<bool> LoginUser(string username, string password)
        {
            _logger.LogInformation("LoginUser called.");
            var loginData = new { Email = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("http://nexusERP.local/api/account/login", loginData);

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

                    _logger.LogInformation($"Logged in as {_userId}");

                    return true;
                }
            }
            _logger.LogError("Failed to log in.");
            return false;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                _logger.LogInformation("RefreshTokenAsync called.");

                if (string.IsNullOrEmpty(_refreshToken))
                {
                    _logger.LogError("No refresh token available.");
                    return false;
                }

                var refreshData = new { RefreshToken = _refreshToken };
                var response = await _httpClient.PostAsJsonAsync("http://nexusERP.local/api/account/refresh-token", refreshData);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
                    if (responseData != null)
                    {
                        _authToken = responseData.AccessToken;
                        _refreshToken = responseData.RefreshToken;

                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

                        _logger.LogInformation("Token refreshed successfully.");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh token.");
            }

            return false;
        }
    }
}
