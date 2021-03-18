using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel;
using DystirManager.ViewModels;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class LogInPage : ContentPage
    {
        private MatchesViewModel _viewModel;

        public LogInPage(MatchesViewModel viewModel, bool autoLogIn)
        {
            InitializeComponent();
            _viewModel = viewModel;
            if(autoLogIn)
            {
                AutoLogIn();
            }
            else
            {
                LoadingView.IsVisible = false;
                LogInView.IsVisible = true;
            }
        }

        private void AutoLogIn()
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
                await _viewModel.GetAdministrators();
                _viewModel.AdministratorLoggedIn = _viewModel.Administrators.FirstOrDefault(x => x.AdministratorEmail == userName && Decrypt(x.AdministratorPassword) == password);
                if (_viewModel.AdministratorLoggedIn != null)
                {
                    Application.Current.Properties.Remove("username");
                    Application.Current.Properties.Remove("password");
                    Application.Current.Properties.Add("username", userName);
                    Application.Current.Properties.Add("password", password);
                    await Application.Current.SavePropertiesAsync();
                    string administratorName = $"{_viewModel.AdministratorLoggedIn.AdministratorFirstName} {_viewModel.AdministratorLoggedIn.AdministratorLastName}";
                    Analytics.TrackEvent("LoggedIn", new Dictionary<string, string> { { "Administrator", administratorName } });
                    (Application.Current as App).StartApplication();
                    return;
                }
                else if (isLogInButtonPress)
                {
                    await DisplayAlert("Error message", "Wrong username or password", "OK");
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
            TryToLogIn(UserNameEntry.Text, PasswordEntry.Text, true);
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}