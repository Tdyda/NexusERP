using Avalonia.Platform;
using NexusERP.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

        public IScreen HostScreen { get; }

        public LoginViewModel(IScreen screen)
        {
            _authService = Locator.Current.GetService<AuthService>();
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
            this.WhenAnyValue(x => x.Username, x => x.Password).Subscribe();
            HostScreen = screen;
        }
        private string? _username;

        [Required]
        public string? Username 
        {
            get { return _username; }
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string? _password;

        [Required]
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
                var success = await _authService.LoginUser(Username, Password);
                if (success)
                {
                    var mainViewModel = new MainWindowViewModel();
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
