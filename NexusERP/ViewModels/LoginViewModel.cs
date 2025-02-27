using Avalonia.Platform;
using NexusERP.Models;
using NexusERP.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NexusERP.ViewModels
{
    public class LoginViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly AuthService _authService;
        public string UrlPathSegment => "login";
        private string? _Text;

        public string? Text
        {
            get { return _Text; }
            set
            {
                this.RaiseAndSetIfChanged(ref _Text, value);
            }
        }

        public IScreen HostScreen { get; }

        public LoginViewModel(IScreen screen)
        {
            _authService = Locator.Current.GetService<AuthService>();
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
            this.WhenAnyValue(x => x.Username, x => x.Password).Subscribe();
            HostScreen = screen;

            this.WhenAnyValue(o => o.Text).Subscribe(x => Debug.WriteLine(x));
        }
        private string? _username;

        [Required(ErrorMessage = "Login jest wymagany")]
        public string? Username 
        {
            get { return _username; }
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string? _password;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Password
        {
            get { return _password; }
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        private async Task Login()
        {
            try
            {
                UpdateCheckerService.CheckForUpdatesAsync();

                var success = await _authService.LoginUser(Username, Password);
                if (success)
                {
                    Debug.WriteLine("Poprawnie zalogowano");
                }
            }
            catch (Exception ex)
            {
                //ErrorMessage = $"Wystąpił błąd: {ex.Message}";
            }
        }
    }
}
