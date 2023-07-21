using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages
{
    public partial class FixturesPage : ContentPage
    {
        private readonly FixturesViewModel fixturesViewModel;
        private readonly AnalyticsService analyticsService;

        public FixturesPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();

            InitializeComponent();
            BindingContext = fixturesViewModel = new FixturesViewModel();
            fixturesViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            analyticsService.Fixtures();

            _ = fixturesViewModel.LoadDataAsync();
            
        }

        void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (fixturesViewModel.IsLoading == false)
            {
                await fixturesViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}

