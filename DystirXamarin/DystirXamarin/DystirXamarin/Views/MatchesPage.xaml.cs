using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using DystirXamarin.Converter;
using System.Globalization;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Plugin.FirebasePushNotification;
using System.Collections.Generic;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchesPage : ContentPage
    {
        private readonly MatchesViewModel _viewModel;
        private Match _newMatch;

        public MatchesPage(MatchesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
            BindingContext = _viewModel;

            Version version = AppInfo.Version;
            string buildString = AppInfo.BuildString;
            VersionLabel.Text = $"{version.Major}.{version.Minor}.{version.Build}.{buildString}";
            

            AddManager(((App)Application.Current).DeviceToken);

            // Push message received event
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Received");
                OpenMatchAsync(p.Data, true);
            };

            //Push message received event
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
                OpenMatchAsync(p.Data, false);
            };
        }

        private void AddManager(string deviceToken)
        {
            if (!string.IsNullOrEmpty(deviceToken))
            {
                var administrator = _viewModel.AdministratorLoggedIn;
                var manager = new Manager()
                {
                    ManagerID = administrator.ID,
                    Name = $"{administrator.AdministratorFirstName} {administrator.AdministratorLastName}",
                    DeviceToken = deviceToken
                };
                _ = _viewModel.AddManagerAsync(manager);
            }
        }

        private async void OpenMatchAsync(IDictionary<string, object> data, bool showAlertDialog)
        {
            string matchID = data["matchID"]?.ToString();
            var selectedLiveMatch = _viewModel.AllMatches.FirstOrDefault(x => x.MatchID == int.Parse(matchID));
            if (selectedLiveMatch == null)
            {
                return;
            }
            var eventType = data["event"]?.ToString();
            if (showAlertDialog)
            {
                var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load(eventType.Equals("GOAL") ? "crowd.mp3" : "whistle.mp3");
                player.Play();
                var title = data["aps.alert.title"]?.ToString();
                var body = data["aps.alert.body"]?.ToString();
                var answer = await DisplayAlert(title, body, "Open", "Cancel");
                if (!answer)
                {
                    return;
                }
            }
            _viewModel.SelectedLiveMatch = selectedLiveMatch;
            await Navigation.PushAsync(new EventsOfMatchPage(_viewModel), false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.IsLoading = true;
            Populate();
        }

        private async void Populate()
        {
            Administrator administratorLoggedIn = _viewModel.AdministratorLoggedIn;
            Title = administratorLoggedIn.AdministratorFirstName + " " + administratorLoggedIn.AdministratorLastName;
            await _viewModel.GetFullData();
            _viewModel.SetMatches();
            PopulateMatchList();
            if (_viewModel.AdministratorLoggedIn?.AdministratorTeamID == 0)
            {
                NewMatchButton.IsVisible = true;
            }
            else
            {
                NewMatchButton.IsVisible = false;
            }
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
                    NoMatchesLabel.Text = "No previous match";
                    break;
                case MatchListType.Today:
                    TodayLayout.BackgroundColor = Color.White;
                    NoMatchesLabel.Text = "No today's match";
                    break;
                case MatchListType.Next:
                    NextLayout.BackgroundColor = Color.White;
                    NoMatchesLabel.Text = "No next match";
                    break;
            }

            NoMatchesLayout.IsVisible = !_viewModel.IsLoading && _viewModel.Matches.Count == 0;
        }

        private void MatchesListView_Refreshing(object sender, EventArgs e)
        {
            _viewModel.IsLoading = true;
            Populate();
        }

        private async void NewMatch_Tapped(object sender, EventArgs e)
        {
            if(_newMatch == null)
            {
                _newMatch = new Match
                {
                    Time = DateTime.UtcNow.Date,
                    StatusID = 14,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0
                };
            }
            _newMatch.HomeTeam = "";
            _newMatch.AwayTeam = "";
            _newMatch.Location = "";
            _newMatch.HomeSquadName = "";
            _newMatch.AwaySquadName = "";
            _newMatch.HomeCategoriesName = "";
            _newMatch.AwayCategoriesName = "";
            await Navigation.PushAsync(new UpdateAndNewMatchPage(_newMatch, _viewModel, TypePages.NewPage), false);
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