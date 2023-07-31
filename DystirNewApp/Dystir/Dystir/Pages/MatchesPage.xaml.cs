using System;
using Xamarin.Forms;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages
{
    public partial class MatchesPage : ContentPage
    {
        private readonly LanguageService languageService;
        private readonly MatchesViewModel matchesViewModel;
        private readonly AnalyticsService analyticsService;

        public MatchesPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();
            languageService = DependencyService.Get<LanguageService>();
            languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;

            InitializeComponent();
            BindingContext = matchesViewModel = new MatchesViewModel();
            matchesViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            analyticsService.Matches();

            _ = matchesViewModel.LoadDataAsync();
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (matchesViewModel.IsLoading == false)
            {
                await matchesViewModel.DystirService.LoadDataAsync(false);
            }
        }

        void Language_Clicked(object sender, EventArgs e)
        {
            languageService.LanguageChange();
        }

        private async void LanguageServiceOnLanguageChanged()
        {
            await matchesViewModel.DystirService.LoadDataAsync(false);
            await matchesViewModel.LoadDataAsync();
        }
    }
}
