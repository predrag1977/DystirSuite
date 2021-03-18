using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using DystirManager.Models;
using System.Collections.Generic;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummaryEventsView : ContentView
    {
        private Match _selectedLiveMatch;
        public SummaryEventsView()
        {
            InitializeComponent();
        }

        private void ContentView_BindingContextChanged(object sender, EventArgs e)
        {
            _selectedLiveMatch = BindingContext as Match;
        }

        private async void Edit_Event_Tapped(object sender, EventArgs e)
        {
            EventOfMatch eventOfMatch = (e as TappedEventArgs).Parameter as EventOfMatch;
            var playersList = GetPlayersList(eventOfMatch.EventTeam);
            await Navigation.PushAsync(new SelectedEventPage(_selectedLiveMatch, eventOfMatch.EventTeam, eventOfMatch.EventName, playersList, eventOfMatch), false);
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

        private async void Delete_Event_Tapped(object sender, EventArgs e)
        {
            EventOfMatch eventOfMatch = (e as TappedEventArgs).Parameter as EventOfMatch;
            var answer = await Application.Current.MainPage.DisplayAlert("Delete event", "Do you want to delete event\n\n" + eventOfMatch.EventText, "yes", "cancel");
            if (answer)
            {
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