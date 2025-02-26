using Microsoft.Extensions.Logging;
using NexusERP.Models;
using Splat;
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
        private UserSession _userSession;
        private readonly JwtDecoder _jwtDecoder;

        public AuthService(HttpClient httpClient)
        {
            _logger = Locator.Current.GetService<ILogger<AuthService>>() ?? throw new Exception("Logger service not found.");
            _httpClient = httpClient;
            _userSession = Locator.Current.GetService<UserSession>() ?? throw new InvalidOperationException("UserSession service not found.");
            _jwtDecoder = Locator.Current.GetService<JwtDecoder>() ?? throw new InvalidOperationException("JwtDecoder service not found.");
        }

        public async Task<bool> RegisterUser(string username, string password)
        {
            _logger.LogInformation("RegisterUser called.");

            var registerData = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("http://10.172.111.78/api/account/register", registerData);

            _logger.LogInformation(response.ToString());
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
            _logger.LogInformation($"ID: {_userSession.UserId}");
            _logger.LogInformation($"Zalogowany: {_userSession.IsAuthenticated}");

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

                    _userSession.IsAuthenticated = true;
                    _userSession.UserId = responseData.UserId;
                    _userSession.LocationName = responseData.LocationName;

                    var roles = _jwtDecoder.GetRolesFromToken(_authToken);

                    foreach (var role in roles)
                    {
                        _userSession.AddRole(role);
                    }

                    _logger.LogInformation($"ID: {_userSession.UserId}");
                    _logger.LogInformation($"Zalogowany: {_userSession.IsAuthenticated}");

                    foreach (var role in roles)
                    {
                        _logger.LogInformation($"Role: {role}");
                    }

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
                var response = await _httpClient.PostAsJsonAsync("http://10.172.111.78/api/account/refresh-token", refreshData);

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
