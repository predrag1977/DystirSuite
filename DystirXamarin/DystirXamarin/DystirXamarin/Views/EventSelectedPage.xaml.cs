using DystirXamarin.Converter;
using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventSelectedPage : ContentPage
    {
        private MatchesViewModel _viewModel;
        private string _teamName;
        private string _matchEventName;
        private Match _selectedMatch;
        private List<PlayerOfMatch> _playersList;
        private string _totalEventMinutesAndSeconds;
        private bool _isUpdateEvent;
        private string _previousTotalTime;
        private int _previousPeriodID;
        private int _previousMainPlayerID;
        private int _previousSecondPlayerID;
        private int _statusID;
        public EventOfMatch _eventOfMatch;
        private string _eventText;
        private bool _isConfirmChanges;
        private PlayerOfMatch _mainSelectedPlayer;
        private PlayerOfMatch _secondSelectedPlayer;

        public EventSelectedPage(MatchesViewModel viewModel, string matchEventName, EventOfMatch eventOfMatch)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _eventOfMatch = eventOfMatch;
            _eventText = "";
            _teamName = eventOfMatch?.EventTeam ?? string.Empty;
            _matchEventName = matchEventName;
            _selectedMatch = viewModel.SelectedLiveMatch;
            _isUpdateEvent = _eventOfMatch != null;
            _isConfirmChanges = false;
            if (_isUpdateEvent)
            {
                _previousTotalTime = _eventOfMatch.EventTotalTime;
                _previousPeriodID = _eventOfMatch.EventPeriodID;
                _previousMainPlayerID = _eventOfMatch.MainPlayerOfMatchID;
                _previousSecondPlayerID = _eventOfMatch.SecondPlayerOfMatchID;
                if (_matchEventName?.ToUpper().Trim() == "CORNER")
                {
                    _playersList = new List<PlayerOfMatch>();
                }
            }
            else
            {
                _eventOfMatch = new EventOfMatch()
                {
                    EventTotalTime = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(_selectedMatch, null, null, CultureInfo.CurrentCulture)?.ToString(),
                    EventPeriodID = (int)_selectedMatch.StatusID
                };
                DeleteEventView.IsVisible = false;
            }
            BindingContext = _viewModel.SelectedLiveMatch;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_matchEventName.ToUpper() == "COMMENTARY" || _isUpdateEvent)
            {
                PopulateEventData();
                PopulateView();
            }
            Title = _matchEventName.ToUpper();
        }

        private void PopulateView()
        {
            Title = string.Format("{0} - {1}", _matchEventName.ToUpper(), _teamName);
            TeamEventView.IsVisible = false;
            MatchTimeLayout.IsVisible = true;
            //MatchTimeLabel.Text = _eventOfMatch.EventTotalTime;
            _totalEventMinutesAndSeconds = GetTotalEventMinutesAndSeconds(_eventOfMatch.EventTotalTime, 0);
            MatchTime(_totalEventMinutesAndSeconds, _eventOfMatch.EventPeriodID);

            if (_matchEventName.ToUpper() == "GOAL" || _matchEventName.ToUpper() == "DIRECTFREEKICKGOAL" || _matchEventName?.ToUpper().Trim() == "OWNGOAL")
            {
                GoalEventView.IsVisible = true;
                if (_matchEventName.ToUpper() == "GOAL" && _isUpdateEvent)
                {
                    AddAssistButtonLayout.IsVisible = true;
                }
            }
            else if (_matchEventName.ToUpper() == "SHOT" || _matchEventName?.ToUpper().Trim() == "ONTARGET"
                || _matchEventName?.ToUpper().Trim() == "OFFTARGET" || _matchEventName?.ToUpper().Trim() == "BLOCKEDSHOT")
            {
                BigChanceNameLabel.Text = string.Format("BIG CHANCE: {0}", _eventOfMatch.SecondPlayerOfMatchID != 0 ? "Yes" : "No");
                ChooseEventView.IsVisible = true;
                BigChanceView.IsVisible = false;
            }
            else if (_matchEventName.ToUpper() == "COMMENTARY")
            {
                CommentaryView.IsVisible = true;
                CommentaryEntry.Text = _eventOfMatch.EventText?.Replace("HENDINGAR:", "").Trim();
            }
            else
            {
                MainView.IsVisible = true;
            }
        }

        private string GetTotalEventMinutesAndSeconds(string eventTotalTime, int minuteExtra)
        {
            string minutes = "000";
            string seconds = "00";
            try
            {
                string[] totalTimeArray = eventTotalTime.Split(':');
                if (totalTimeArray != null && totalTimeArray.Length == 2)
                {
                    minutes = (int.Parse(totalTimeArray[0].TrimStart('0').TrimStart('0')) + minuteExtra).ToString();
                    seconds = totalTimeArray[1].TrimStart('0').TrimStart('0');
                    minutes = string.IsNullOrWhiteSpace(minutes) ? "0" : minutes;
                    seconds = string.IsNullOrWhiteSpace(seconds) ? "0" : seconds;
                }
                int min = int.Parse(minutes);
                int sec = int.Parse(seconds);
                if (min < 100)
                {
                    minutes = "0" + min;
                    if (min < 10)
                    {
                        minutes = "0" + minutes;
                    }
                    if (sec < 10)
                    {
                        seconds = "0" + seconds;
                    }
                }
            }
            catch { }
            return minutes + ":" + seconds;
        }

        private void MatchTime(string totalEventMinutesAndSeconds, int? statusID)
        {
            _statusID = (int)statusID;
            string matchTime = new LiveMatchTimeConverter()?.Convert(totalEventMinutesAndSeconds, null, statusID, CultureInfo.CurrentCulture)?.ToString();
            MatchTimeLabel.Text = matchTime;
        }

        private void PopulateEventData()
        {
            _playersList = _matchEventName == "CORNER" || _matchEventName == "PENALTY" || _matchEventName == "COMMENTARY" ? null : GetPlayersList();
            MainEventNameLabel.Text = _matchEventName.ToUpper();
            _mainSelectedPlayer = _playersList?.FirstOrDefault(x => x.PlayerOfMatchID == _eventOfMatch.MainPlayerOfMatchID);
            MainSelectedPlayerLabel.Text = (_mainSelectedPlayer?.FirstName + " " + _mainSelectedPlayer?.LastName)?.Trim();
            _secondSelectedPlayer = _playersList?.FirstOrDefault(x => x.PlayerOfMatchID == _eventOfMatch.SecondPlayerOfMatchID);
            SecondEventNameLabel.Text = _matchEventName.ToUpper();
            SecondSelectedPlayerLabel.Text = (_secondSelectedPlayer?.FirstName + " " + _secondSelectedPlayer?.LastName)?.Trim();
            MainPlayersListView.ItemsSource = _playersList;
            if (_matchEventName == "SUBSTITUTION")
            {
                Title = string.Format("{0} - {1}", "SUBS OUT", _teamName);
                MainEventNameLabel.Text = "SUBS OUT";
                SecondEventNameLabel.Text = "SUBS OUT";
                SecondPlayersListView.ItemsSource = _playersList;
            }
        }

        private void MainViewSendEvent_Clicked(object sender, EventArgs e)
        {
            if (_isUpdateEvent)
            {
                UpdateEvent();
            }
            else
            {
                AddNewEvent();
            }

            if (_matchEventName == "SUBSTITUTION")
            {
                Title = string.Format("{0} - {1}", "SUBS IN", _teamName);
                MainEventNameLabel.Text = "SUBS IN";
                SecondEventNameLabel.Text = "SUBS IN";
                MainView.IsVisible = false;
                SecondView.IsVisible = true;
            }
            else
            {
                SendEventChanges();
            }
        }

        private void SecondViewSendEvent_Clicked(object sender, EventArgs e)
        {
            PlayerOfMatch secondPlayer = _secondSelectedPlayer;
            _eventOfMatch.SecondPlayerOfMatchID = secondPlayer?.PlayerOfMatchID ?? 0;
            _eventOfMatch.SecondPlayerOfMatchNumber = secondPlayer?.Number?.ToString();
            SendEventChanges();
        }

        private void CommentarySendEvent_Clicked(object sender, EventArgs e)
        {
            _matchEventName = "COMMENTARY";
            _eventText = CommentaryEntry.Text;
            if (_isUpdateEvent)
            {
                UpdateEvent();
            }
            else
            {
                AddNewEvent();
            }
            SendEventChanges();
        }

        private void UpdateEvent()
        {
            _eventOfMatch.MainPlayerOfMatchID = _mainSelectedPlayer?.PlayerOfMatchID ?? 0;
            _eventOfMatch.MainPlayerOfMatchNumber = _mainSelectedPlayer?.Number?.ToString();
            _eventOfMatch.EventPeriodID = _statusID;
            _eventOfMatch.EventTotalTime = _totalEventMinutesAndSeconds;
            _eventOfMatch.EventText = _eventText;
            _eventOfMatch.EventName = _matchEventName;
        }

        private void AddNewEvent()
        {
            _eventOfMatch.MatchID = _selectedMatch.MatchID;
            _eventOfMatch.EventName = _matchEventName;
            _eventOfMatch.EventText = _eventText;
            _eventOfMatch.MainPlayerOfMatchID = _mainSelectedPlayer?.PlayerOfMatchID ?? 0;
            _eventOfMatch.MainPlayerOfMatchNumber = _mainSelectedPlayer?.Number?.ToString();
            _eventOfMatch.SecondPlayerOfMatchID = 0;
            _eventOfMatch.EventTeam = _teamName;
        }

        private async void SendEventChanges()
        {
            _isConfirmChanges = true;
            if (_isUpdateEvent)
            {
                await Navigation.PopAsync(false);
                _ = _viewModel.UpdateEventOfMatchAsync(_eventOfMatch, _selectedMatch);
            }
            else
            {
                if (_matchEventName != "GOAL")
                {
                    await Navigation.PopAsync(false);
                }
                _ = _viewModel.AddEventOfMatch(_eventOfMatch, _selectedMatch);
                if (_matchEventName == "GOAL")
                {
                    _matchEventName = "ASSIST";
                    MainView.IsVisible = false;
                    AssistView.IsVisible = true;
                }
            }
        }

        private async void DeleteMatch_Tapped(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete event", "Do you want to delete event?", "yes", "cancel");
            if (answer)
            {
                await Navigation.PopAsync(false);
                await _viewModel.DeleteEventOfMatchAsync(_eventOfMatch, _selectedMatch);
            }
        }

        private void PlayersListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        private void Player_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;

            if (MainView.IsVisible)
            {
                if (_mainSelectedPlayer == playerOfMatch)
                {
                    _mainSelectedPlayer = null;
                }
                else
                {
                    _mainSelectedPlayer = playerOfMatch;
                }
                MainSelectedPlayerLabel.Text = (_mainSelectedPlayer?.FirstName + " " + _mainSelectedPlayer?.LastName)?.Trim();
            }
            else if (SecondView.IsVisible)
            {
                if (_secondSelectedPlayer == playerOfMatch)
                {
                    _secondSelectedPlayer = null;
                }
                else
                {
                    _secondSelectedPlayer = playerOfMatch;
                }
                SecondSelectedPlayerLabel.Text = (_secondSelectedPlayer?.FirstName + " " + _secondSelectedPlayer?.LastName)?.Trim();
            }
        }

        private async void EventTime_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MatchTimeAndPeriodPage(_viewModel, _eventOfMatch), false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Page currentPage = Navigation.NavigationStack.LastOrDefault();
            if (!_isConfirmChanges && currentPage == this)
            {
                _eventOfMatch.EventTotalTime = _previousTotalTime;
                _eventOfMatch.EventPeriodID = _previousPeriodID;
                _eventOfMatch.MainPlayerOfMatchID = _previousMainPlayerID;
                _eventOfMatch.SecondPlayerOfMatchID = _previousSecondPlayerID;
            }
        }

        private void ShotEventTarget_Clicked(object sender, EventArgs e)
        {
            _matchEventName = (sender as Button).CommandParameter?.ToString();
            ChooseEventView.IsVisible = false;
            BigChanceView.IsVisible = true;
            PopulateEventData();
        }

        private void BigChance_Clicked(object sender, EventArgs e)
        {
            string bigChance = (sender as Button).CommandParameter?.ToString();
            BigChanceNameLabel.Text = string.Format("BIG CHANCE: {0}", !string.IsNullOrEmpty(bigChance) ? "Yes" : "No");
            SecondEventLabel.Text = string.Format("BIG CHANCE: {0}", !string.IsNullOrEmpty(bigChance) ? "Yes" : "No");
            SecondEventLabel.IsVisible = true;
            _eventText = bigChance;
            BigChanceView.IsVisible = false;
            MainView.IsVisible = true;
        }

        private void GoalEvent_Clicked(object sender, EventArgs e)
        {
            _matchEventName = (sender as Button).CommandParameter?.ToString();
            _playersList = GetPlayersList();
            GoalEventView.IsVisible = false;
            MainView.IsVisible = true;
            if(_matchEventName == "ASSIST")
            {
                _isUpdateEvent = false;
                var eventTotalTime = _eventOfMatch.EventTotalTime;
                var eventPeriodID = (int)_eventOfMatch.EventPeriodID;
                _eventOfMatch = new EventOfMatch()
                {
                    EventTotalTime = eventTotalTime,
                    EventPeriodID = eventPeriodID
                };
                DeleteEventView.IsVisible = false;
            }
            PopulateEventData();
        }

        private string GetOppositeTeam(string teamName)
        {
            string oppositeTeamName;
            if (teamName == _viewModel.SelectedLiveMatch.HomeTeam)
            {
                oppositeTeamName = _viewModel.SelectedLiveMatch.AwayTeam;
            }
            else
            {
                oppositeTeamName = _viewModel.SelectedLiveMatch.HomeTeam;
            }
            return oppositeTeamName;
        }

        private List<PlayerOfMatch> GetPlayersList() {
            string teamName = _matchEventName == "OWNGOAL" ? GetOppositeTeam(_teamName) : _teamName;
            var playersList = _viewModel.SelectedLiveMatch.PlayersOfMatch?
                .Where(x => x.TeamName?.ToUpper().Trim() == teamName?.ToUpper().Trim() && (x.PlayingStatus < 3))
                .OrderBy(x => GetOrder(x.SubIN, x.SubOUT, x.PlayingStatus)).ToList();
            return playersList;
        }

        private object GetOrder(int? subIN, int? subOUT, int? playingStatus)
        {
            switch (playingStatus)
            {
                case 1:
                    //if (subOUT != null && subOUT > -1) return 2;
                    return 1;
                case 2:
                    //if (subIN != null && subIN > -1) return 1;
                    return 2;
                case 0:
                    return 3;
                default:
                    return 4;
            }
        }

        private async void Assist_Clicked(object sender, EventArgs e)
        {
            string eventName = (sender as Button).CommandParameter?.ToString();
            if(!string.IsNullOrWhiteSpace(eventName))
            {
                _matchEventName = eventName;
                AssistView.IsVisible = false;
                MainView.IsVisible = true;
                PopulateEventData();
            }
            else if(_isUpdateEvent)
            {
                var answer = await DisplayAlert("Remove assist", "Do you want to remove assist?", "yes", "cancel");
                if (answer)
                {
                    await Navigation.PopAsync(false);
                    await _viewModel.DeleteEventOfMatchAsync(_eventOfMatch, _selectedMatch);
                }
            }
            else
            {
                await Navigation.PopAsync(false);
            }
        }

        private void ChooseTeam_Clicked(object sender, EventArgs e)
        {
            _teamName = (sender as Button).CommandParameter?.ToString();
            PopulateEventData();
            PopulateView();
        }
    }
}