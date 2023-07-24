using Xamarin.Forms;
using Dystir.ViewModels;
using System;
using System.Threading.Tasks;
using Dystir.Services;
using System.Linq;

namespace Dystir.Pages
{
    [QueryProperty(nameof(MatchID), nameof(MatchID))]
    public partial class MatchDetailPage : ContentPage, IDisposable
    {
        private MatchDetailViewModel matchDetailViewModel;

        public int MatchID { get; set; }

        private readonly AnalyticsService analyticsService;

        public MatchDetailPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Shell.SetTabBarIsVisible(this, false);
            BindingContext = matchDetailViewModel = new MatchDetailViewModel(MatchID);
            _ = LoadDataAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Dispose();
        }

        private async Task LoadDataAsync()
        {
            matchDetailViewModel.IsLoading = true;
            
            await matchDetailViewModel.PopulateMatchDetailsTabs();
            await Task.Delay(100);
            await matchDetailViewModel.LoadMatchDetailAsync();

            analyticsService.MatchDetails(matchDetailViewModel.MatchDetails.Match);
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            Shell.SetTabBarIsVisible(this, true);
            await Navigation.PopAsync(true);
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (matchDetailViewModel.IsLoading == false)
            {
                await matchDetailViewModel.DystirService.LoadDataAsync(true);
            }
        }

        async void News_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(NewsPage)}");
        }

        public void Dispose()
        {
            matchDetailViewModel.Dispose();
        }
    }
}
