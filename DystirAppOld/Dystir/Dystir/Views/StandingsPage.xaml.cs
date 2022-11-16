using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dystir.Models;
using Dystir.ViewModels;
using System.Collections.ObjectModel;
using Dystir.Helper;
using System.Globalization;
using Microsoft.AppCenter.Analytics;

namespace Dystir.Views
{
    [DesignTimeVisible(true)]
    public partial class StandingsPage : ContentView
    {
        public DystirViewModel _viewModel;

        public StandingsPage(DystirViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        void StandingCompetitionsMenuView_BindingContextChanged(object sender, EventArgs e)
        {
            PopulateCompetitionMenu();
        }

        private void PopulateCompetitionMenu()
        {
            if (StandingCompetitionsMenuView.Children != null)
            {
                StandingCompetitionsMenuView.Children.Clear();
            }
            foreach (string competititionName in _viewModel.StandingsCompetitions)
            {
                if (string.IsNullOrWhiteSpace(_viewModel.StandingsCompetitionSelected))
                {
                    _viewModel.StandingsCompetitionSelected = competititionName;
                }
                CompetitionMenuItemView view = new CompetitionMenuItemView()
                {
                    BindingContext = competititionName
                };
                view.BackgroundColor = Color.DimGray;
                if (_viewModel.StandingsCompetitionSelected == competititionName)
                {
                    view.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Label competitionNameLabel = (Label)view.FindByName("CompetitionNameLabel");
                competitionNameLabel.TextColor = Color.White;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += CompetitionName_Tapped;
                tapGestureRecognizer.CommandParameter = competititionName;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                StandingCompetitionsMenuView.Children.Add(view);
            }
        }

        private void CompetitionName_Tapped(object sender, EventArgs e)
        {
            string competitionName = (e as TappedEventArgs).Parameter.ToString();
            _viewModel.StandingsCompetitionSelected = competitionName;
            PopulateCompetitionMenu();
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}