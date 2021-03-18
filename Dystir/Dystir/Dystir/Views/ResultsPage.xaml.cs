using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dystir.Models;
using Dystir.ViewModels;

namespace Dystir.Views
{
    [DesignTimeVisible(true)]
    public partial class ResultsPage : ContentView
    {
        public MatchesViewModel _viewModel;
        private bool isNavigate;

        public ResultsPage(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        void ResultsCompetitionsMenuView_BindingContextChanged(object sender, System.EventArgs e)
        {
            PopulateCompetitionMenu();
        }

        private void PopulateCompetitionMenu()
        {
            if (ResultsCompetitionsMenuView.Children != null)
            {
                ResultsCompetitionsMenuView.Children.Clear();
            }
            foreach (string competititionName in _viewModel.ResultsCompetitions)
            {
                CompetitionMenuItemView view = new CompetitionMenuItemView()
                {
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
            PopulateCompetitionMenu();
        }

        public void SeeMoreMatchDetail(Match selectedMatch)
        {
            if (!isNavigate)
            {
                isNavigate = true;
                _viewModel.SelectedMatch = selectedMatch;

                isNavigate = false;
            }
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}