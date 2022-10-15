using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace Dystir.Views
{
    public partial class DystirPage : ContentPage
    {
        private readonly DystirService _dystirService;
        private DystirViewModel _viewModel = new DystirViewModel();
        private int _lastSelectedPage = 0;

        public DystirPage(DystirViewModel viewModel)
        {
            _dystirService = DependencyService.Get<DystirService>();
            _viewModel = viewModel;
            _viewModel.PageTitle = Properties.Resources.Matches;
            Analytics.TrackEvent("Matches");
            InitializeComponent();
            BindingContext = _viewModel;
            AddViews(_viewModel);
        }

        private void AddViews(DystirViewModel viewModel)
        {
            List<ContentView> pagesList = new List<ContentView>();
            pagesList.Add(new MatchesPage(viewModel));
            pagesList.Add(new ResultsPage(viewModel));
            pagesList.Add(new FixturesPage(viewModel));
            pagesList.Add(new StandingsPage(viewModel));
            pagesList.Add(new StatisticsPage(viewModel));
            pagesList.Add(new SelectedMatchDetailsView()
            {
                BindingContext = viewModel
            });
            DystirCarouselView.ItemsSource = pagesList; 
            ShowSelectedPage(0);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var parameter = (e as TappedEventArgs).Parameter.ToString();
            int index = int.Parse(parameter);
            ShowSelectedPage(index);
            _lastSelectedPage = index;
            SetPageTitle();
            foreach (View btn in ((sender as View).Parent as Grid).Children)
            {
                Label btnLabel = (btn as StackLayout).Children.First() as Label;
                btnLabel.TextColor = Color.White;
            }
            ((sender as StackLayout).Children.First() as Label).TextColor = Color.LimeGreen;
        }

        private void ShowSelectedPage(int index)
        {
            DystirCarouselView.ScrollTo(index, -1, ScrollToPosition.MakeVisible, false);
        }

        internal void SeeMatchDetailsAsync(Match selectedMatch, bool refreshMatchesBySelectedDate)
        {
            
            _viewModel.SelectedMatch = selectedMatch;
            
            if (refreshMatchesBySelectedDate)
            {
                ShowSelectedPage(5);
                ShowMainTab(false);
            }
            else
            {
                _viewModel.SelectedMatch.Details = new MatchDetails();
            }

            _ = _dystirService.LoadMatchDetailsAsync(_viewModel.SelectedMatch);

            if (refreshMatchesBySelectedDate)
            {
                //_viewModel.SetMatchesBySelectedDate();
            }
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    MenuMatchesScrollView.BackgroundColor = Color.Transparent;
            //    int matchDetailsIndex = _viewModel.MatchesBySelectedDate.IndexOf(_viewModel.MatchesBySelectedDate.FirstOrDefault(x => x.MatchID == selectedMatch?.MatchID));
            //    if (matchDetailsIndex > -1)
            //    {
            //        MenuMatchesScrollView.ScrollTo(matchDetailsIndex, -1, ScrollToPosition.Center, true);
            //    }
            //});

            //if (_viewModel.SelectedMatch?.Details != null && !_viewModel.SelectedMatch?.Details.IsDataLoaded)
            //{
            //    _viewModel.IsLoadingSelectedMatch = true;
            //    _ = (Application.Current as App).ReloadAsync(LoadDataType.MatchDataOnly);
            //}

            AnalyticsMatchDetails(selectedMatch);
        }

        private void MatchSelect_Tapped(object sender, EventArgs e)
        {
            Match selectedMatch = (Match)(e as TappedEventArgs).Parameter;
            if (selectedMatch?.MatchID != _viewModel.SelectedMatch?.MatchID)
            {
                SeeMatchDetailsAsync(selectedMatch, false);
            }
        }

        private void BackToMatchesView_Tapped(object sender, EventArgs e)
        {
            //_viewModel.AllMatchesWithDetails.ToList().ForEach(x => x.IsSelected = false);
            _viewModel.MatchesBySelectedDate = new ObservableCollection<Match>();
            _viewModel.SelectedMatch = null;
            ShowMainTab(true);
            SetPageTitle();
            ShowSelectedPage(_lastSelectedPage);
        }

        private async void RefreshSelectedMatch_Tapped(object sender, EventArgs e)
        {
            _viewModel.IsLoadingSelectedMatch = true;
            await (Application.Current as App).ReloadAsync(LoadDataType.MatchDataOnly);
        }

        private void ShowMainTab(bool isMainTab)
        {
            MenuMatchesScrollView.BackgroundColor = Color.DimGray;
            HeaderMatchDetails.IsVisible = !isMainTab;
            MenuMatchesScrollView.IsVisible = !isMainTab;
            Header.IsVisible = isMainTab;
            MenuButtomItemsView.IsVisible = isMainTab;
        }

        internal void SetPageTitle()
        {
            switch (_lastSelectedPage)
            {
                case 0:
                    _viewModel.PageTitle = Properties.Resources.Matches;
                    Analytics.TrackEvent("Matches");
                    break;
                case 1:
                    _viewModel.PageTitle = Properties.Resources.Results;
                    Analytics.TrackEvent("Results");
                    break;
                case 2:
                    _viewModel.PageTitle = Properties.Resources.Fixtures;
                    Analytics.TrackEvent("Fixtures");
                    break;
                case 3:
                    _viewModel.PageTitle = Properties.Resources.Standings;
                    Analytics.TrackEvent("Standings");
                    break;
                case 4:
                    _viewModel.PageTitle = Properties.Resources.StatisticsTab;
                    Analytics.TrackEvent("Statistics");
                    break;
                case 5:
                    break;
            }
        }

        private void AnalyticsMatchDetails(Match match)
        {
            string homeTeam = $"{match?.HomeTeam} {match?.HomeCategoriesName} {match?.HomeSquadName}".Trim();
            string awayTeam = $"{match?.AwayTeam} {match?.AwayCategoriesName} {match?.AwaySquadName}".Trim();
            string matchDetails = $"{homeTeam} vs {awayTeam} ({match.MatchTypeName})";
            Analytics.TrackEvent("Selected Match", new Dictionary<string, string> { { "Match", matchDetails } });
        }
    }
}
