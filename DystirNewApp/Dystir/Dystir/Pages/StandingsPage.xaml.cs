using System;
using Xamarin.Forms;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages
{
    public partial class StandingsPage : ContentPage
    {
        private readonly StandingsViewModel standingsViewModel;
        private readonly AnalyticsService analyticsService;

        public StandingsPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();

            InitializeComponent();
            BindingContext = standingsViewModel = new StandingsViewModel();
            standingsViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            analyticsService.Standings();

            _ = standingsViewModel.LoadDataAsync();
        }

        async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (standingsViewModel.IsLoading == false)
            {
                await standingsViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}
