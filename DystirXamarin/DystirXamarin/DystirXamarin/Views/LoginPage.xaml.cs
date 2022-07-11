using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using DystirXamarin.ViewModels;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using DystirXamarin.Services;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        private readonly MatchesViewModel _viewModel;
        private readonly EncryptorService _encryptorService;

        public LogInPage()
        {
            InitializeComponent();
            _viewModel = new MatchesViewModel();
            _encryptorService = DependencyService.Get<EncryptorService>();
            GetAdministrators();
        }

        public LogInPage(MatchesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            LoadingView.IsVisible = false;
            LogInView.IsVisible = true;
            _encryptorService = DependencyService.Get<EncryptorService>();
        }

        private void GetAdministrators()
        {
            var propertyUserName = Application.Current.Properties.FirstOrDefault(x => x.Key == "username");
            string userName = propertyUserName.Value?.ToString();
            var propertyPassword = Application.Current.Properties.FirstOrDefault(x => x.Key == "password");
            string password = propertyPassword.Value?.ToString();
            TryToLogIn(userName, password, false);
        }

        private async void TryToLogIn(string userName, string password, bool isLogInButtonPress)
        {
            LogInBtn.IsEnabled = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    string token = _encryptorService.Encrypt(string.Format("{0}{1}", userName, password));
                    _viewModel.AdministratorLoggedIn = await _viewModel.LoginAsync(token);
                    if (_viewModel.AdministratorLoggedIn != null)
                    {
                        Application.Current.Properties.Remove("username");
                        Application.Current.Properties.Remove("password");
                        Application.Current.Properties.Add("username", userName);
                        Application.Current.Properties.Add("password", password);
                        await Application.Current.SavePropertiesAsync();
                        string administratorName = $"{_viewModel.AdministratorLoggedIn.AdministratorFirstName} {_viewModel.AdministratorLoggedIn.AdministratorLastName}";
                        Analytics.TrackEvent("LoggedIn", new Dictionary<string, string> { { "Administrator", administratorName } });
                        Application.Current.MainPage = new NavigationPage(new MatchesPage(_viewModel));
                        return;
                    }
                    else if (isLogInButtonPress)
                    {
                        await DisplayAlert("Error message", "Wrong username or password", "OK");
                    }
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error message", "No connection to server", "OK");
            }
            LoadingView.IsVisible = false;
            LogInView.IsVisible = true;
            LogInBtn.IsEnabled = true;
        }

        private void LogIn_Tapped(object sender, EventArgs e)
        {
            TryToLogIn(UserNameEntry.Text.Trim(), PasswordEntry.Text, true);
            Analytics.TrackEvent("TryToLoggedIn", new Dictionary<string, string> { { "UserName", UserNameEntry.Text } });
        }
    }
}