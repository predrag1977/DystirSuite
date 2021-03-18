using DystirManager.Converter;
using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedEventPage : ContentPage
    {
        public Match SelectedMatch { get; private set; }
        public EventOfMatch FocusedEventOfMatch { get; private set; }

        public SelectedEventPage(Match selectedMatch, EventOfMatch eventOfMatch)
        {
            InitializeComponent();
            SelectedMatch = selectedMatch;
            FocusedEventOfMatch = eventOfMatch;
            SetAdditionalEventOfMatch();
            BindingContext = FocusedEventOfMatch;
        }

        public SelectedEventPage(Match selectedLiveMatch, string teamName, string v, List<PlayerOfMatch> playersList, object p)
        {

        }

        public void SetAdditionalEventOfMatch()
        {
            FocusedEventOfMatch.MatchID = SelectedMatch.MatchID;
            if (FocusedEventOfMatch?.EventOfMatchID == 0)
            {
                FocusedEventOfMatch.EventTotalTime = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(SelectedMatch, null, null, CultureInfo.CurrentCulture)?.ToString();
                FocusedEventOfMatch.EventPeriodID = (int)SelectedMatch.StatusID;
            }
            string totalEventMinutesAndSeconds = GetTotalEventMinutesAndSeconds(FocusedEventOfMatch.EventTotalTime);
            FocusedEventOfMatch.MatchTime = new LiveMatchTimeConverter()?.Convert(totalEventMinutesAndSeconds, null, FocusedEventOfMatch.EventPeriodID, CultureInfo.CurrentCulture)?.ToString();
        }

        private string GetTotalEventMinutesAndSeconds(string eventTotalTime)
        {
            string minutes = "000";
            string seconds = "00";
            try
            {
                string[] totalTimeArray = eventTotalTime.Split(':');
                if (totalTimeArray != null && totalTimeArray.Length == 2)
                {
                    minutes = (int.Parse(totalTimeArray[0].TrimStart('0').TrimStart('0'))).ToString();
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
            catch (Exception)
            {
            }
            return minutes + ":" + seconds;
        }

        private void PlayersListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }

        private void Player_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;
            if (FocusedEventOfMatch.MainPlayerOfMatchID == playerOfMatch.PlayerOfMatchID)
            {
                FocusedEventOfMatch.MainPlayerOfMatchID = 0;
                FocusedEventOfMatch.MainPlayerOfMatchNumber = "";
                playerOfMatch = null;
            }
            else
            {
                FocusedEventOfMatch.MainPlayerOfMatchID = playerOfMatch.PlayerOfMatchID;
                FocusedEventOfMatch.MainPlayerOfMatchNumber = playerOfMatch.Number?.ToString() ?? "";
            }
            MainSelectedPlayerLabel.Text = (playerOfMatch?.FirstName + " " + playerOfMatch?.LastName)?.Trim();
        }

        private async void Set_Time_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetMatchTimeAndPeriodPage(SelectedMatch, FocusedEventOfMatch), false);
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            SelectedMatch.IsLoadingSelectedMatch = true;
            await Navigation.PopAsync(false);
            if (FocusedEventOfMatch?.EventOfMatchID == 0)
            {
                await FocusedEventOfMatch.AddEventOfMatch();
            }
            else
            {
                await FocusedEventOfMatch.UpdateEventOfMatchAsync();
            }
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}