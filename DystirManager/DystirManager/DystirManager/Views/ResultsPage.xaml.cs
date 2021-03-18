using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using DystirManager.Models;
using DystirManager.ViewModels;
using System.Collections.Generic;
using DystirManager.Helper;
using System.Threading.Tasks;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class ResultsPage : ContentPage
    {
        public MatchesViewModel _viewModel;
        private bool isNavigate;

        public ResultsPage(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = viewModel;
            Title = Properties.Resources.Results;
            AddMenuView();
            PopulateView(_viewModel);
        }

        public void PopulateView(MatchesViewModel viewModel)
        {
            PopulateCompetitionMenu(viewModel.ResultsMatches.GroupBy(x => x.MatchTypeName));
        }

        private void PopulateCompetitionMenu(IEnumerable<IGrouping<string, Match>> matchesGroupList)
        {
            if (ResultsCompetitionsMenuView.Children != null)
            {
                ResultsCompetitionsMenuView.Children.Clear();
            }
            foreach (var group in matchesGroupList)
            {
                string competititionName = group.Key;
                if (string.IsNullOrWhiteSpace(_viewModel.ResultsCompetitionSelected))
                {
                    _viewModel.ResultsCompetitionSelected = competititionName;
                }
                CompetitionMenuItemView view = new CompetitionMenuItemView()
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    BindingContext = competititionName
                };
                view.BackgroundColor = Color.DimGray;
                if (_viewModel.ResultsCompetitionSelected == competititionName)
                {
                    view.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Label competitionNameLabel = (Label)view.FindByName("CompetitionNameLabel");
                competitionNameLabel.TextColor = Color.White;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += CompetitionName_Tapped;
                tapGestureRecognizer.CommandParameter = competititionName;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                ResultsCompetitionsMenuView.Children.Add(view);
            }
        }

        private void CompetitionName_Tapped(object sender, EventArgs e)
        {
            string competitionName = (e as TappedEventArgs).Parameter.ToString();
            _viewModel.ResultsCompetitionSelected = competitionName;
            PopulateView(_viewModel);
        }

        private void AddMenuView()
        {
            new MenuView().PopulateMenuView(MenuView, 1);
        }

        public async Task EditMatchDetails(Match selectedMatch)
        {
            if (!isNavigate)
            {
                isNavigate = true;
                _viewModel.SelectedLiveMatch = selectedMatch;
                await Navigation.PushAsync(new UpdateAndNewMatchPage(_viewModel, TypePages.UpdatePage), true);
                isNavigate = false;
            }
        }

        public async Task SeeMoreMatchDetail(Match selectedMatch)
        {
            if (!isNavigate)
            {
                isNavigate = true;
                _viewModel.SelectedLiveMatch = selectedMatch;
                await Navigation.PushAsync(new SelectedMatchDetailsView(_viewModel));
                isNavigate = false;
            }
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        private async void Refresh_Tapped(object sender, EventArgs e)
        {
            _viewModel.IsLoading = true;
            await (Application.Current as App).RefreshPageData();
        }

        private void Language_Tapped(object sender, EventArgs e)
        {
            (Application.Current as App).ChangeLanguage();
            AddMenuView();
        }
    }
}