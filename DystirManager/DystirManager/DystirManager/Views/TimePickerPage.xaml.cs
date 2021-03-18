using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirManager.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class TimePickerPage : ContentPage
    {
        private bool _isPressed = false;

        public Label MatchTimeLabel { get; private set; }
        public DateTime TotalTime { get; private set; }

        public TimePickerPage(Label matchTimeLabel)
        {
            InitializeComponent();
            MatchTimeLabel = matchTimeLabel;
            TotalTime = GetTimeFromString(matchTimeLabel.Text);
            SetTime(TotalTime);
            BindingContext = this;
        }

        private void HoursUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseHours);
            _isPressed = true;
        }

        private void HoursDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseHours);
            _isPressed = true;
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

        private void SetTime(DateTime timeValue)
        {
            Hours.Text = timeValue.ToString("HH");
            Minutes.Text = timeValue.ToString("mm");
            TimeLabel.Text = timeValue.ToString("HH:mm");
        }

        private DateTime GetTimeFromString(string dateTimeString)
        {
            TimeSpan ts = TimeSpan.TryParse(dateTimeString, out TimeSpan value) ? value : new TimeSpan();
            return DateTime.Now.Date.Add(ts);
        }

        private bool IncreaseHours()
        {
            TotalTime = TotalTime.AddHours(1);
            SetTime(TotalTime);
            return _isPressed;
        }

        private bool DecreaseHours()
        {
            TotalTime = TotalTime.AddHours(-1);
            SetTime(TotalTime);
            return _isPressed;
        }

        private bool IncreaseMinutes()
        {
            TotalTime = TotalTime.AddMinutes(1);
            SetTime(TotalTime);
            return _isPressed;
        }

        private bool DecreaseMinutes()
        {
            TotalTime = TotalTime.AddMinutes(-1);
            SetTime(TotalTime);
            return _isPressed;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isPressed = false;
        }

        private void OK_Tapped(object sender, EventArgs e)
        {
            MatchTimeLabel.Text = TimeLabel.Text;
            Navigation.PopAsync(false);
        }

        private void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}