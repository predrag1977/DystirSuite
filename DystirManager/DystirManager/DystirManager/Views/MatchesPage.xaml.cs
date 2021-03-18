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
    public partial class MatchesPage : ContentPage
    {
        public MatchesViewModel _viewModel;
        private bool isNavigate;

        public MatchesPage(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = viewModel;
            Title = Properties.Resources.Matches;
            AddMenuView();
            PopulateView(_viewModel);
        }

        public void PopulateView(MatchesViewModel viewModel)
        {
            PopulateMatchDays();
            NoMatchesPanel.IsVisible = viewModel.MatchesGroupList.Select(x=>x).Count() == 0 ? true : false;
        }

        private void PopulateMatchDays()
        {
            if (_viewModel.MatchesDaySelected == DateTime.MinValue)
            {
                _viewModel.MatchesDaySelected = DateTime.Now.ToLocalTime();
            }
            
            if (DayMenuView.Children != null)
            {
                DayMenuView.Children.Clear();
                DayMenuView.ColumnDefinitions.Clear();
                DayMenuView.RowDefinitions.Clear();
            }

            int i = 0;
            for (DateTime date = DateTime.Now.AddDays(-3); date <= DateTime.Now.AddDays(3); date = date.AddDays(1))
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                DayMenuView.ColumnDefinitions.Add(cd);
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
                DayMenuView.Children.Add(view);
                i++;
            }
            RowDefinition rd = new RowDefinition
            {
                Height = new GridLength(50)
            };
            DayMenuView.RowDefinitions.Add(rd);
        }

        private void AddMenuView()
        {
            new MenuView().PopulateMenuView(MenuView, 0);
        }

        private void MatchDay_Tapped(object sender, EventArgs e)
        {
            _viewModel.MatchesDaySelected = (DateTime)(e as TappedEventArgs)?.Parameter;
            PopulateView(_viewModel);
        }

        private async void AddNewMatch_Tapped(object sender, EventArgs e)
        {
            if (!isNavigate)
            {
                isNavigate = true;
                _viewModel.SelectedLiveMatch = new Match()
                {
                    Time = DateTime.UtcNow.Date
                };
                await Navigation.PushAsync(new UpdateAndNewMatchPage(_viewModel, TypePages.NewPage), true);
                isNavigate = false;
            }
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
                await Navigation.PushAsync(new SelectedMatchDetailsView(_viewModel), true);
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

        private async void Logout_Tapped(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Log out", "Do you want to log out?", "yes", "cancel");
            if (answer)
            {
                (Application.Current as App).Logout();
            }
        }

        private void Language_Tapped(object sender, EventArgs e)
        {
            (Application.Current as App).ChangeLanguage();
            PopulateMatchDays();
            AddMenuView();
        }
    }
}