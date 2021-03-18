using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using DystirManager.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendEventsView : ContentView
    {
        private Match _selectedLiveMatch;

        public SendEventsView()
        {
            InitializeComponent();
        }

        private void ContentView_BindingContextChanged(object sender, EventArgs e)
        {
            _selectedLiveMatch = BindingContext as Match;
        }

        private void Goal_Clicked(object sender, EventArgs e)
        {
            SetEvent("GOAL", (string)(e as TappedEventArgs).Parameter);
        }

        private void Shot_Clicked(object sender, EventArgs e)
        {
            SetEvent("SHOT", (string)(e as TappedEventArgs).Parameter);
        }

        private void Corner_Clicked(object sender, EventArgs e)
        {
            SetEvent("CORNER", (string)(e as TappedEventArgs).Parameter);
        }

        private void Penalty_Clicked(object sender, EventArgs e)
        {
            SetEvent("PENALTY", (string)(e as TappedEventArgs).Parameter);
        }

        private void Yellow_Clicked(object sender, EventArgs e)
        {
            SetEvent("YELLOW", (string)(e as TappedEventArgs).Parameter);
        }

        private void Red_Clicked(object sender, EventArgs e)
        {
            SetEvent("RED", (string)(e as TappedEventArgs).Parameter);
        }

        private async void SetEvent(string eventName, string teamName)
        {
            EventOfMatch newEvent = new EventOfMatch()
            {
                EventName = eventName,
                EventTeam = teamName,
                HomeTeam = _selectedLiveMatch.HomeTeam,
                AwayTeam = _selectedLiveMatch.AwayTeam
            };
            newEvent.PlayersList = new ObservableCollection<PlayerOfMatch>(GetPlayersList(teamName));
            newEvent.TeamsList = new ObservableCollection<string>(new string[] { newEvent.HomeTeam, newEvent.AwayTeam });
            await Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, newEvent), false);
        }




        private void Assist_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "ASSIST", playersList, null), false);
        }

        private void OwnGoal_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            string oppositeTeamName;
            if (teamName == _selectedLiveMatch.HomeTeam)
            {
                oppositeTeamName = _selectedLiveMatch.AwayTeam;
            }
            else
            {
                oppositeTeamName = _selectedLiveMatch.HomeTeam;
            }
            var playersList = GetPlayersList(oppositeTeamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "OWNGOAL", playersList, null), false);
        }

        private void PenaltyScored_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "PENALTYSCORED", playersList, null), false);
        }

        private void PenaltyMissed_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "PENALTYMISSED", playersList, null), false);
        }

        private void OffTarget_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "OFFTARGET", playersList, null), false);
        }

        private void BlockedShot_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "BLOCKEDSHOT", playersList, null), false);
        }

        private void Subs_Clicked(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "SUBSTITUTION", playersList, null), false);
        }

        private void BigChance_Tapped(object sender, EventArgs e)
        {
            string teamName = (string)(e as TappedEventArgs).Parameter;
            var playersList = GetPlayersList(teamName);
            Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, teamName, "BIGCHANCE", playersList, null), false);
        }

        private List<PlayerOfMatch> GetPlayersList(string teamName)
        {
            var playersList = _selectedLiveMatch.PlayersOfMatch?
                .Where(x => x.TeamName?.ToUpper().Trim() == teamName?.ToUpper().Trim() && (x.PlayingStatus < 3)).
                OrderBy(x => GetOrder(x.PlayingStatus)).ToList();
            return playersList;
        }

        private object GetOrder(int? playingStatus)
        {
            switch (playingStatus)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 0:
                    return 3;
                default:
                    return 4;
            }
        }

        private async void Edit_Event_Tapped(object sender, EventArgs e)
        {
            EventOfMatch eventOfMatch = (e as TappedEventArgs).Parameter as EventOfMatch;
            eventOfMatch.HomeTeam = _selectedLiveMatch.HomeTeam;
            eventOfMatch.AwayTeam = _selectedLiveMatch.AwayTeam;
            eventOfMatch.PlayersList = new ObservableCollection<PlayerOfMatch>(GetPlayersList(eventOfMatch.EventTeam));
            eventOfMatch.TeamsList = new ObservableCollection<string>(new string[] { eventOfMatch.HomeTeam, eventOfMatch.AwayTeam });
            await Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, eventOfMatch), false);
        }

        private async void Delete_Event_Tapped(object sender, EventArgs e)
        {
            EventOfMatch eventOfMatch = (e as TappedEventArgs).Parameter as EventOfMatch;
            var answer = await Application.Current.MainPage.DisplayAlert("Delete event", "Do you want to delete event\n\n" + eventOfMatch.EventText, "yes", "cancel");
            if (answer)
            {
                _selectedLiveMatch.IsLoadingSelectedMatch = true;
                await eventOfMatch.DeleteEventOfMatchAsync();
            }
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}