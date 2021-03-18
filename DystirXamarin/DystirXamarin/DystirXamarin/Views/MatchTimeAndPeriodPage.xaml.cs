using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using System.Collections.Generic;
using DystirXamarin.ViewModels;
using DystirXamarin.Converter;
using System.Globalization;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchTimeAndPeriodPage : ContentPage
    {
        private bool _isMatchTime;
        private MatchesViewModel _viewModel;
        private bool _isPressed = false;

        public List<Status> StatusList { get; private set; }
        private Match _match { get; set; }
        public EventOfMatch EventOfMatch { get; private set; }

        private int _periodID;
        private string _totalEventMinutesAndSeconds;
        private string _totalMatchTime;

        public DateTime TotalTime { get; private set; }

        public MatchTimeAndPeriodPage(MatchesViewModel viewModel)
        {
            _isMatchTime = true;
            _viewModel = viewModel;
            _match = new Match()
            {
                StatusID = viewModel.SelectedLiveMatch.StatusID,
                StatusTime = viewModel.SelectedLiveMatch.StatusTime
            };
            InitializeComponent();
            Populate(viewModel.SelectedLiveMatch);
            BindingContext = this;
        }

        public MatchTimeAndPeriodPage(MatchesViewModel viewModel, EventOfMatch eventOfMatch)
        {
            _isMatchTime = false;
            _match = viewModel.SelectedLiveMatch;
            EventOfMatch = eventOfMatch;
            if (eventOfMatch != null)
            {
                _periodID = eventOfMatch.EventPeriodID;
                _totalEventMinutesAndSeconds = eventOfMatch.EventTotalTime;
            }
            else
            {
                _periodID = (int)_match.StatusID;
                _totalEventMinutesAndSeconds = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(_match, null, null, CultureInfo.CurrentCulture)?.ToString();
            }
            InitializeComponent();
            Populate(_match);
            BindingContext = this;
        }

        private void Populate(Match match)
        {
            if (_isMatchTime)
            {
                Title = "Match time and period";
            }
            else
            {
                Title = "Event time and period";
            }
            SetTime(0, 0);
            StatusList = match.Statuses?.Where(x => x.StatusID > 0 && x.StatusID < 13).ToList();
        }

        private void StatusItem_Tapped(object sender, EventArgs e)
        {
            if (_isMatchTime)
            {
                _match.StatusID = (int?)(e as TappedEventArgs).Parameter;
                _match.StatusTime = DateTime.UtcNow;
                SetTime(0, 0);
            }
            else
            {
                _periodID = (int)(e as TappedEventArgs).Parameter;
                SetTime(0, 0);
            }
        }

        private void MinutesUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseMinutes);
            _isPressed = true;
        }

        private void MinutesDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseMinutes);
            _isPressed = true;
        }

        private void SecondsUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseSeconds);
            _isPressed = true;
        }

        private void SecondsDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseSeconds);
            _isPressed = true;
        }

        private bool IncreaseMinutes()
        {
            SetTime(1, 0);
            return _isPressed;
        }

        private bool DecreaseMinutes()
        {
            SetTime(-1, 0);
            return _isPressed;
        }

        private bool IncreaseSeconds()
        {
            SetTime(0, 1);
            return _isPressed;
        }

        private bool DecreaseSeconds()
        {
            SetTime(0, -1);
            return _isPressed;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isPressed = false;
        }

        private void SetTime(double minutes, double seconds)
        {
            if (_isMatchTime)
            {
                _match.StatusTime = _match.StatusTime.Value.AddMinutes(-minutes).AddSeconds(-seconds);
                _totalMatchTime = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(_match, null, null, CultureInfo.CurrentCulture)?.ToString();
                TimeLabelPopulate(_totalMatchTime, _match.StatusID);
            }
            else
            {
                _totalEventMinutesAndSeconds = AddMinutesAndSeconds(_totalEventMinutesAndSeconds, minutes, seconds);
                TimeLabelPopulate(_totalEventMinutesAndSeconds, _periodID);
            }
        }

        private string AddMinutesAndSeconds(string totalEventMinutesAndSeconds, double addMinutes, double addSeconds)
        {
            try
            {
                string[] valuesArray = totalEventMinutesAndSeconds.Split(':');
                string minutes = string.IsNullOrWhiteSpace(valuesArray[0].TrimStart('0')) ? "0" : valuesArray[0].TrimStart('0');
                string seconds = string.IsNullOrWhiteSpace(valuesArray[1].TrimStart('0')) ? "0" : valuesArray[1].TrimStart('0');
                double sec = double.Parse(seconds) + addSeconds;
                if (sec < 0)
                {
                    sec = 59;
                    addMinutes = -1;
                }
                if (sec > 59)
                {
                    sec = 0;
                    addMinutes = 1;
                }
                double min = double.Parse(minutes) + addMinutes;
                min = SyncTimeWithPeriod(min, _periodID);
                min = min < 0 ? 0 : min;
                minutes = min.ToString();
                seconds = sec.ToString();
                if (min < 100)
                {
                    minutes = "0" + minutes;
                    if (min < 10)
                    {
                        minutes = "0" + minutes;
                    }
                }
                if (sec < 10)
                {
                    seconds = "0" + seconds;
                }
                return minutes + ":" + seconds;
            }
            catch
            {
                return totalEventMinutesAndSeconds;
            }
        }

        private double SyncTimeWithPeriod(double min, int periodID)
        {
            switch (periodID)
            {
                case 2:
                    if (min < 0)
                    {
                        min = 0;
                    };
                    break;
                case 4:
                    if (min < 45)
                    {
                        min = 45;
                    };
                    break;
                case 6:
                    if (min < 90)
                    {
                        min = 90;
                    };
                    break;
                case 8:
                    if (min < 105)
                    {
                        min = 105;
                    };
                    break;
            }
            return min;
        }

        private void TimeLabelPopulate(string totalEventMinutesAndSeconds, int? statusID)
        {
            string fullMatchTime = new LiveMatchTimeConverter()?.Convert(totalEventMinutesAndSeconds, null, statusID, CultureInfo.CurrentCulture)?.ToString();
            _totalMatchTime = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(_match, null, null, CultureInfo.CurrentCulture)?.ToString();
            MatchTimeLabel.Text = fullMatchTime;
            if (fullMatchTime.Contains(":"))
            {
                string[] timeTextArray = fullMatchTime.Split(':') ?? new string[2];
                MinutesLabel.Text = timeTextArray[0];
                SecondsLabel.Text = timeTextArray[1];
            }
            else
            {
                MinutesLabel.Text = "";
                SecondsLabel.Text = "";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_isMatchTime)
            {
                Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                {
                    string totalEventMinutesAndSeconds = new TotalTimeFromSelectedMatchTimeConverter()?.Convert(_match, null, null, CultureInfo.CurrentCulture)?.ToString();
                    TimeLabelPopulate(totalEventMinutesAndSeconds, _match.StatusID);
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
        }

        private int GetMinutesOfMatchPeriod(int minutes, int? statusID)
        {
            switch(statusID)
            {
                case 4:
                    return minutes -= 45;
                case 6:
                    return minutes -= 90;
                case 8:
                    return minutes -= 105;
                default:
                    return minutes;
            }
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            if (_isMatchTime)
            {
                string[] timeTextArray = _totalMatchTime.Split(':') ?? new string[2];
                int.TryParse(timeTextArray[0], out int minutes);
                _viewModel.SelectedLiveMatch.ExtraMinutes = GetMinutesOfMatchPeriod(minutes, _match.StatusID);
                int.TryParse(timeTextArray[1], out int seconds);
                _viewModel.SelectedLiveMatch.ExtraSeconds = seconds + 1;
                _viewModel.SelectedLiveMatch.StatusID = _match.StatusID;
                _viewModel.SelectedLiveMatch.StatusTime = _match.StatusTime;
                await _viewModel.UpdateMatchAsync(_viewModel.SelectedLiveMatch, true);
            }
            else
            {
                EventOfMatch.EventTotalTime = _totalEventMinutesAndSeconds;
                EventOfMatch.EventPeriodID = _periodID;
            }
            await Navigation.PopAsync(false);
        }

        private void StatusListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}