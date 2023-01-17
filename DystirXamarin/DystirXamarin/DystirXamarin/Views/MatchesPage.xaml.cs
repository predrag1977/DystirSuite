using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using DystirXamarin.Converter;
using System.Globalization;
using System.Collections.ObjectModel;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchesPage : ContentPage
    {
        private MatchesViewModel _viewModel;

        public MatchesPage(MatchesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
            BindingContext = _viewModel;
            _viewModel.IsLoading = true;
            Populate();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.SetMatches();
            PopulateMatchList();
        }

        private async void Populate()
        {
            Administrator administratorLoggedIn = _viewModel.AdministratorLoggedIn;
            Title = administratorLoggedIn.AdministratorFirstName + " " + administratorLoggedIn.AdministratorLastName;
            await _viewModel.GetFullData();
            PopulateMatchList();
            if (_viewModel.AdministratorLoggedIn?.AdministratorTeamID == 0)
            {
                NewMatchButton.IsVisible = true;
            }
            else
            {
                NewMatchButton.IsVisible = false;
            }
            //TODO Change this
            VersionLabel.Text = "4.0.0.53";
        }

        private void PopulateMatchList()
        {
            BeforeLayout.BackgroundColor = Color.DarkKhaki;
            TodayLayout.BackgroundColor = Color.DarkKhaki;
            NextLayout.BackgroundColor = Color.DarkKhaki;

            switch (_viewModel.SelectedMatchListType)
            {
                case MatchListType.Before:
                    BeforeLayout.BackgroundColor = Color.White;
                    break;
                case MatchListType.Today:
                    TodayLayout.BackgroundColor = Color.White;
                    break;
                case MatchListType.Next:
                    NextLayout.BackgroundColor = Color.White;
                    break;
            }
        }

        private void MatchesListView_Refreshing(object sender, EventArgs e)
        {
            _viewModel.IsLoading = true;
            Populate();
        }

        private async void NewMatch_Tapped(object sender, EventArgs e)
        {
            Match match = new Match
            {
                Time = DateTime.Now.Date.AddHours(12),
                StatusID = 14,
                HomeTeamScore = 0,
                AwayTeamScore = 0
            };
            await Navigation.PushAsync(new UpdateAndNewMatchPage(match, _viewModel, TypePages.NewPage), false);
        }

        private void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.MainException))
            {
                if (_viewModel.MainException != null)
                {
                    ErrorLayout.IsVisible = true;
                    Device.StartTimer(new TimeSpan(0, 0, 0, 5), ShowError);
                }
            }
        }

        private bool ShowError()
        {
            ErrorLayout.IsVisible = false;
            return false;
        }

        private void LiveMatchDetails_Tapped(object sender, EventArgs e)
        {
            _viewModel.SelectedLiveMatch = (e as TappedEventArgs).Parameter as Match;
            Navigation.PushAsync(new EventsOfMatchPage(_viewModel), false);
        }

        private void MatchesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        private void TeamNamesLayout_Tapped(object sender, EventArgs e)
        {
            if (_viewModel.AdministratorLoggedIn?.AdministratorTeamID == 0)
            {
                Match selectedMatch = _viewModel.Matches.FirstOrDefault(x => x.MatchID == (int)(e as TappedEventArgs).Parameter);
                Navigation.PushAsync(new UpdateAndNewMatchPage(selectedMatch, _viewModel, TypePages.UpdatePage), false);
            }
            else
            {
                return;
            }
        }

        private async void LogOut_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Log out", "Do you want to log out?", "yes", "cancel");
            if (answer)
            {
                Application.Current.Properties.Remove("username");
                Application.Current.Properties.Remove("password");
                Application.Current.MainPage = new NavigationPage(new LogInPage(_viewModel));
            }
        }

        private void MatchTimeLayout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StackLayout matchTimeLayout = sender as StackLayout;
            Label matchTimeLabel = matchTimeLayout.Children?.FirstOrDefault() as Label;
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Match selectedMatch = matchTimeLabel.BindingContext as Match;
                string totalEventMinutesAndSeconds = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(selectedMatch, null, null, CultureInfo.CurrentCulture)?.ToString();
                string fullMatchTime = new LiveMatchTimeConverter()?.Convert(totalEventMinutesAndSeconds, null, selectedMatch?.StatusID, CultureInfo.CurrentCulture)?.ToString();
                matchTimeLabel.Text = fullMatchTime;
                return true;
            });
        }

        private void Before_Tapped(object sender, EventArgs e)
        {
            _viewModel.SelectedMatchListType = MatchListType.Before;
            _viewModel.Matches = new ObservableCollection<Match>(_viewModel.AllMatches.Where(x => x.Time.Value.ToLocalTime().Date < DateTime.Now.ToLocalTime().Date));
            PopulateMatchList();
        }

        private void Today_Tapped(object sender, EventArgs e)
        {
            _viewModel.SelectedMatchListType = MatchListType.Today;
            _viewModel.Matches = new ObservableCollection<Match>(_viewModel.AllMatches.Where(x => x.Time.Value.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date));
            PopulateMatchList();
        }

        private void Next_Tapped(object sender, EventArgs e)
        {
            _viewModel.SelectedMatchListType = MatchListType.Next;
            _viewModel.Matches = new ObservableCollection<Match>(_viewModel.AllMatches.Where(x => x.Time.Value.ToLocalTime().Date > DateTime.Now.ToLocalTime().Date));
            PopulateMatchList();
        }
    }
}