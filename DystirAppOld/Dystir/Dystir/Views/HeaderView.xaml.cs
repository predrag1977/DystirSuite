using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderView : ContentView
    {
        private DystirViewModel _viewModel;
        private readonly DystirService _dystirService;

        public HeaderView()
        {
            _dystirService = DependencyService.Get<DystirService>();
            InitializeComponent();
        }

        private void ContentView_BindingContextChanged(object sender, EventArgs e)
        {
            _viewModel = BindingContext as DystirViewModel;
            ShowLanguageFlag();
        }

        private void Refresh_Tapped(object sender, EventArgs e)
        {
            DystirViewModel viewModel = (e as TappedEventArgs).Parameter as DystirViewModel;
            RefreshAllData(viewModel);
        }

        private async void RefreshAllData(DystirViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.IsLoading = true;
            }
            await _dystirService.LoadDataAsync();
            //await (Application.Current as App).ReloadAsync(LoadDataType.MainData);
        }

        private async void Language_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                if (_viewModel.LanguageCode.ToString() == "fo-FO")
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    _viewModel.LanguageCode = CultureInfo.GetCultureInfo("en-US");
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fo-FO");
                    _viewModel.LanguageCode = CultureInfo.GetCultureInfo("fo-FO");
                }
                ShowLanguageFlag();
                ((Application.Current.MainPage as NavigationPage).CurrentPage as DystirPage).SetPageTitle();
                Analytics.TrackEvent("Language", new Dictionary<string, string> { { "Language code", Thread.CurrentThread.CurrentUICulture.Name } });
                Application.Current.Properties.Remove("languageCode");
                Application.Current.Properties.Add("languageCode", _viewModel.LanguageCode.ToString());
            });
            await Application.Current.SavePropertiesAsync();
        }

        private void ShowLanguageFlag()
        {
            FaroeseFlag.IsVisible = Thread.CurrentThread.CurrentUICulture == CultureInfo.GetCultureInfo("en-US");
            EnglishFlag.IsVisible = Thread.CurrentThread.CurrentUICulture == CultureInfo.GetCultureInfo("fo-FO");
        }
    }
}