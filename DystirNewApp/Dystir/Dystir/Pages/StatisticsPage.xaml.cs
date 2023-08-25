using System;
using Xamarin.Forms;
using Dystir.ViewModels;
using Dystir.Services;
using Dystir.Models;
using System.Linq;

namespace Dystir.Pages
{
    public partial class StatisticsPage : ContentPage
    {
        private readonly StatisticsViewModel statisticsViewModel;
        private readonly AnalyticsService analyticsService;

        public StatisticsPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();

            InitializeComponent();
            BindingContext = statisticsViewModel = new StatisticsViewModel();
            statisticsViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            analyticsService.Statistics();

            _ = statisticsViewModel.LoadDataAsync();
        }

        async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (statisticsViewModel.IsLoading == false)
            {
                statisticsViewModel.IsLoading = true;
                await statisticsViewModel.LoadDataAsync();
            }
        }

        //void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        try
        //        {
        //            if (StatisticListView.ItemsSource != null && StatisticListView.ItemsSource.Cast<PlayersInLineups>().Count() > 0)
        //            {
        //                var firstItem = StatisticListView.ItemsSource.Cast<PlayersInLineups>().FirstOrDefault();
        //                if (firstItem != null)
        //                {
        //                    StatisticListView.ScrollTo(firstItem, ScrollToPosition.Start, false);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.ToString());
        //        }
        //    });
        //}

        //async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;
        //    await App.Current.MainPage.Navigation.ShowPopupAsync(new PlayerInfoPopupView(playerOfMatch));
        //}
    }
}
