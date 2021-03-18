using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dystir.ViewModels;
using System.Collections.ObjectModel;

namespace Dystir.Views
{
    [DesignTimeVisible(true)]
    public partial class MatchesPage : ContentView
    {
        private MatchesViewModel _viewModel;

        public MatchesPage(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        private void DaysView_BindingContextChanged(object sender, EventArgs e)
        {
            PopulateMatchDays();
        }

        private void PopulateMatchDays()
        {
            if (DaysView.Children != null)
            {
                DaysView.Children.Clear();
                DaysView.ColumnDefinitions.Clear();
                DaysView.RowDefinitions.Clear();
            }

            int i = 0;
            foreach (DateTime date in _viewModel.MatchesDays)
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                DaysView.ColumnDefinitions.Add(cd);
                DayView view = new DayView(date);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += MatchDay_Tapped;
                tapGestureRecognizer.CommandParameter = date;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                Grid.SetColumn(view, i);
                view.BackgroundColor = Color.DimGray;
                if (date.Date == _viewModel.MatchesDaySelected.Date)
                {
                    view.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Label dayLabel = (Label)view.FindByName("DayLabel");
                dayLabel.TextColor = Color.White;
                Label daySecondLabel = (Label)view.FindByName("DaySecondLabel");
                daySecondLabel.TextColor = Color.White;
                DaysView.Children.Add(view);
                i++;
            }
            RowDefinition rd = new RowDefinition
            {
                //Height = new GridLength(50)
            };
            DaysView.RowDefinitions.Add(rd);
        }

        private void MatchDay_Tapped(object sender, EventArgs e)
        {
            _viewModel.MatchesDaySelected = (DateTime)(e as TappedEventArgs)?.Parameter;
            PopulateMatchDays();
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}