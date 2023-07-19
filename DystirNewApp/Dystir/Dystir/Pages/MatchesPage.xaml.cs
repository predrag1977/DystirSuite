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

        public MatchesPage()
        {
            languageService = DependencyService.Get<LanguageService>();
            languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;
            InitializeComponent();
            BindingContext = matchesViewModel = new MatchesViewModel();
        }

        protected override void OnAppearing()
        {
            _ = matchesViewModel.LoadDataAsync();
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (matchesViewModel.IsLoading == false)
            {
                await matchesViewModel.DystirService.LoadDataAsync(true);
            }
        }

        void Language_Clicked(object sender, EventArgs e)
        {
            languageService.LanguageChange();
        }

        private async void LanguageServiceOnLanguageChanged()
        {
            await matchesViewModel.DystirService.LoadDataAsync(true);
            await matchesViewModel.LoadDataAsync();
        }
    }
}
