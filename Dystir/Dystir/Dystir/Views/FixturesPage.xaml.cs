using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dystir.ViewModels;

namespace Dystir.Views
{
    [DesignTimeVisible(true)]
    public partial class FixturesPage : ContentView
    {
        public DystirViewModel _viewModel;

        public FixturesPage(DystirViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        void FixturesCompetitionsMenuView_BindingContextChanged(object sender, EventArgs e)
        {
            PopulateCompetitionMenu();
        }

        private void PopulateCompetitionMenu()
        {
            if (FixturesCompetitionsMenuView.Children != null)
            {
                FixturesCompetitionsMenuView.Children.Clear();
            }
            foreach (string competititionName in _viewModel.FixturesCompetitions)
            {
                CompetitionMenuItemView view = new CompetitionMenuItemView()
                {
                    BindingContext = competititionName
                };
                view.BackgroundColor = Color.DimGray;
                if (_viewModel.FixturesCompetitionSelected == competititionName)
                {
                    view.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Label competitionNameLabel = (Label)view.FindByName("CompetitionNameLabel");
                competitionNameLabel.TextColor = Color.White;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += CompetitionName_Tapped;
                tapGestureRecognizer.CommandParameter = competititionName;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                FixturesCompetitionsMenuView.Children.Add(view);
            }
        }

        private void CompetitionName_Tapped(object sender, EventArgs e)
        {
            string competitionName = (e as TappedEventArgs).Parameter.ToString();
            _viewModel.FixturesCompetitionSelected = competitionName;
            PopulateCompetitionMenu();
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}