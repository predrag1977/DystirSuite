﻿using Acr.UserDialogs;
using DystirXamarin.Converter;
using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsOfMatchPage : ContentPage
    {
        private readonly MatchesViewModel _viewModel;
        private readonly bool _openFromNotification;
        private readonly string _notificationText;

        public EventsOfMatchPage(MatchesViewModel viewModel, bool openFromNotification = false, string notificationText = "")
        {
            InitializeComponent();
            _viewModel = viewModel;
            _openFromNotification = openFromNotification;
            _notificationText = notificationText;
            BindingContext = _viewModel;
            LoadMatchData();
            PopulateMatchTime();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_openFromNotification)
            {
                UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
                {
                    Message = _notificationText,
                    Cancel = new ActionSheetOption("Close", null, null)
                });
            }
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                PopulateMatchTime();
                Page currentPage = Navigation.NavigationStack.LastOrDefault();
                if (currentPage == this)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _ = _viewModel.GetMatches();
        }

        private async void LoadMatchData()
        {
            LoadingDataLabel.IsVisible = true;
            ErrorLoadingDataLabel.IsVisible = false;
            TryAgainBottom.IsVisible = false;
            EventsGrid.IsVisible = false;
            IsEnabled = false;

            _viewModel.SelectedLiveMatch.Statuses = _viewModel.Statuses;
            bool loadResult = await _viewModel.GetSelectedLiveMatch(_viewModel.SelectedLiveMatch, true);
            if (loadResult)
            {
                if(_openFromNotification)
                {
                    ShowMainEvents();
                }
                LoadingMatchDataGrid.IsVisible = false;
                EventsOfMatchGrid.IsVisible = true;
            }
            else
            {
                LoadingDataLabel.IsVisible = false;
                ErrorLoadingDataLabel.IsVisible = true;
                TryAgainBottom.IsVisible = true;
            }
            EventsGrid.IsVisible = !_openFromNotification;
            IsEnabled = true;
            OpenCommetButton.IsVisible = _openFromNotification;
        }

        private void PopulateMatchTime()
        {
            Match selectedMatch = MatchTimeLabel.BindingContext as Match;
            string totalEventMinutesAndSeconds = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(selectedMatch, null, null, CultureInfo.CurrentCulture)?.ToString();
            string fullMatchTime = new LiveMatchTimeConverter()?.Convert(totalEventMinutesAndSeconds, null, selectedMatch?.StatusID, CultureInfo.CurrentCulture)?.ToString();
            MatchTimeLabel.Text = fullMatchTime;
            if(_viewModel.SelectedLiveMatch.StatusID >= 12)
            {
                PlayerOfTheMatchContentView.IsVisible = true;
                if(EventsGrid.RowDefinitions.Count == 4)
                {
                    EventsGrid.RowDefinitions.Add(new RowDefinition() { Height = 55});
                }
            }
            else
            {
                PlayerOfTheMatchContentView.IsVisible = false;
                if (EventsGrid.RowDefinitions.Count == 5)
                {
                    EventsGrid.RowDefinitions.RemoveAt(4);
                }
            }
            
        }

        private void ShowMainEvents()
        {
            var mainEventsOfMatch = _viewModel.SelectedLiveMatch.EventsOfMatch.Where(x => MainEventName(x.EventName)).Reverse();
            _viewModel.SelectedLiveMatch.EventsOfMatch = new ObservableCollection<EventOfMatch>(mainEventsOfMatch);
        }

        private bool MainEventName(string eventName)
        {
            switch(eventName.ToUpper())
            {
                case "GOAL":
                case "OWNGOAL":
                case "PENALTYSCORED":
                case "PENALTyMISSED":
                case "YELLOW":
                case "RED":
                case "SUBSTITUTION":
                    return true;
                default:
                    return false;
            }
        }

        private async void MatchTime_Clicked(object sender, EventArgs e)
        {
            if(_openFromNotification)
            {
                return;
            }
            await Navigation.PushAsync(new MatchTimeAndPeriodPage(_viewModel), false);
        }

        private async void EditEvent_Tapped(object sender, EventArgs e)
        {
            if (_openFromNotification)
            {
                return;
            }
            EventOfMatch eventOfMatch = (e as TappedEventArgs).Parameter as EventOfMatch;
            await Navigation.PushAsync(new EventSelectedPage(_viewModel, eventOfMatch.EventName, eventOfMatch), false);
        }

        private async void EventsListView_Refreshing(object sender, EventArgs e)
        {
            await _viewModel.GetSelectedLiveMatch(_viewModel.SelectedLiveMatch, false);
            if (_openFromNotification)
            {
                ShowMainEvents();
            }
        }

        private async void OpenCommet_Tapped(object sender, EventArgs e)
        {
            //Open Comet application
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Launcher.OpenAsync("https://newcometmobile.page.link/redirect");
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                await Launcher.OpenAsync("https://newcometmobile.page.link/redirect");
            }
        }

        private void TryLoadAgain_Tapped(object sender, EventArgs e)
        {
            LoadMatchData();
        }

        private async void LineUp_Tapped(object sender, EventArgs e)
        {
            string teamName = (e as TappedEventArgs).Parameter?.ToString();
            await Navigation.PushAsync(new LineUpsPage(_viewModel, teamName), false);
        }

        private async void Event_Tapped(object sender, EventArgs e)
        {
            string eventName = (e as TappedEventArgs).Parameter?.ToString();
            await Navigation.PushAsync(new EventSelectedPage(_viewModel, eventName, null), false);
        }
    }
}