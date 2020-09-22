using Prism.Commands;
using Prism.Navigation;
using PruebaProsegur.Models;
using PruebaProsegur.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PruebaProsegur.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;
        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;

            Title = "Iniciar Sesión";
            IsEnabled = true;
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Introduce el email.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Introduce la contraseña.", "Aceptar");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var connection = await _apiService.CheckConnection(url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Comprueba la conexión a internet", "Aceptar");
                return;
            }

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var response = await _apiService.GetTokenAsync(url, "api", "/Login", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await App.Current.MainPage.DisplayAlert("Error", "Email o contraseña incorrectos", "Aceptar");
                Password = string.Empty;
                return;
            }

            var token = (TokenResponse)response.Result;
            var responseUser = await _apiService.GetUserByEmailAsync(url, "api", "/Users/GetUserByEmail", "bearer", token.Token, Email);

            if (!responseUser.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await App.Current.MainPage.DisplayAlert("Error", "Error fatal con el usuario", "Aceptar");
                return;
            }

            var user = (UserResponse)responseUser.Result;
            var parameters = new NavigationParameters
            {
                { "user", user }
            };

            IsRunning = false;
            IsEnabled = true;

            await _navigationService.NavigateAsync("ExplicationPage", parameters);
            Password = string.Empty;
        }
    }
}