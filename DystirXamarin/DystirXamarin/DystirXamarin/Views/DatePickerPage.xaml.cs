using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using System.Collections.Generic;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatePickerPage : ContentPage
    {
        private bool _isPressed = false;

        public Label MatchDateLabel { get; private set; }
        public DateTime TotalDate { get; private set; }

        public DatePickerPage(Label matchDateLabel)
        {
            InitializeComponent();
            MatchDateLabel = matchDateLabel;
            TotalDate = GetDateFromString(matchDateLabel.Text);
            SetDate(TotalDate);
            BindingContext = this;
        }

        private void DaysUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseDays);
            _isPressed = true;
        }

        private void DaysDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseDays);
            _isPressed = true;
        }

        private void MonthsUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseMonths);
            _isPressed = true;
        }

        private void MonthsDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseMonths);
            _isPressed = true;
        }

        private void YearsDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseYears );
            _isPressed = true;
        }

        private void YearsUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseYears);
            _isPressed = true;
        }

        private void SetDate(DateTime dateValue)
        {
            Days.Text = dateValue.ToString("dd");
            Months.Text = dateValue.ToString("MM");
            Years.Text = dateValue.ToString("yyyy");
            DateLabel.Text = dateValue.ToString("dd.MM.yyyy");
            string dayText = dateValue.ToString("dddd");
            if (dateValue.Date == DateTime.Now.Date)
                dayText = "Today";
            if(dateValue.Date == DateTime.Now.AddDays(1).Date)
                dayText = "Tommorow";
            DayLabel.Text = dayText;
        }

        private DateTime GetDateFromString(string dateTimeString)
        {
            return DateTime.ParseExact(dateTimeString, "dd.MM.yyyy", null);
        }

        private bool IncreaseDays()
        {
            TotalDate = TotalDate.AddDays(1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private bool DecreaseDays()
        {
            TotalDate = TotalDate.AddDays(-1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private bool IncreaseMonths()
        {
            TotalDate = TotalDate.AddMonths(1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private bool DecreaseMonths()
        {
            TotalDate = TotalDate.AddMonths(-1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private bool IncreaseYears()
        {
            TotalDate = TotalDate.AddYears(1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private bool DecreaseYears()
        {
            TotalDate = TotalDate.AddYears(-1);
            SetDate(TotalDate);
            return _isPressed;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isPressed = false;
        }

        private void OK_Tapped(object sender, EventArgs e)
        {
            MatchDateLabel.Text = DateLabel.Text;
            Navigation.PopAsync(false);
        }
    }
}