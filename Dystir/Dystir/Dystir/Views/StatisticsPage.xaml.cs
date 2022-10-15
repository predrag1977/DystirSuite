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
    public partial class StatisticsPage : ContentView
    {
        public DystirViewModel _viewModel;

        public StatisticsPage(DystirViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        void StatisticsCompetitionsMenuView_BindingContextChanged(System.Object sender, System.EventArgs e)
        {
            PopulateCompetitionMenu();
        }

        private void PopulateCompetitionMenu()
        {
            if (StatisticsCompetitionsMenuView.Children != null)
            {
                StatisticsCompetitionsMenuView.Children.Clear();
            }
            foreach (string competititionName in _viewModel.StatisticCompetitions)
            {
                if (string.IsNullOrWhiteSpace(_viewModel.StatisticsCompetitionSelected))
                {
                    _viewModel.StatisticsCompetitionSelected = competititionName;
                }
                CompetitionMenuItemView view = new CompetitionMenuItemView()
                {
                    BindingContext = competititionName
                };
                view.BackgroundColor = Color.DimGray;
                if (_viewModel.StatisticsCompetitionSelected == competititionName)
                {
                    view.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Label competitionNameLabel = (Label)view.FindByName("CompetitionNameLabel");
                competitionNameLabel.TextColor = Color.White;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += CompetitionName_Tapped;
                tapGestureRecognizer.CommandParameter = competititionName;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                StatisticsCompetitionsMenuView.Children.Add(view);
            }
        }

        private void CompetitionName_Tapped(object sender, EventArgs e)
        {
            string competitionName = (e as TappedEventArgs).Parameter.ToString();
            _viewModel.StatisticsCompetitionSelected = competitionName;
            PopulateCompetitionMenu();
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }


    }
}