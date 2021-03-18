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
    public partial class ResultPage : ContentPage
    {
        private Match _match { get; set; }
        public Label HomeScoreLabel { get; private set; }
        public Label AwayScoreLabel { get; private set; }
        public string TotalHomeScore { get; private set; }
        public string TotalAwayScore { get; private set; }

        public ResultPage(Match match, Label homeScoreLabel, Label awayScoreLabel)
        {
            InitializeComponent();
            _match = match;
            HomeScoreLabel = homeScoreLabel;
            AwayScoreLabel = awayScoreLabel;
            TotalHomeScore = homeScoreLabel.Text;
            TotalAwayScore = awayScoreLabel.Text;
            TitleLabel.Text = "RESULT";
            BindingContext = this;
        }

        private void HomeScoreUp_Tapped(object sender, EventArgs e)
        {
            int homeScore = Convert.ToInt32(HomeScore.Text);
            homeScore++;
            HomeScore.Text = homeScore.ToString();
        }

        private void HomeScoreDown_Tapped(object sender, EventArgs e)
        {
            int homeScore = Convert.ToInt32(HomeScore.Text);
            if (homeScore > 0)
                homeScore--;
            HomeScore.Text = homeScore.ToString();
        }

        private void AwayScoreUp_Tapped(object sender, EventArgs e)
        {
            int awayScore = Convert.ToInt32(AwayScore.Text);
            awayScore++;
            AwayScore.Text = awayScore.ToString();
        }

        private void AwayScoreDown_Tapped(object sender, EventArgs e)
        {
            int awayScore = Convert.ToInt32(AwayScore.Text);
            if (awayScore > 0)
                awayScore--;
            AwayScore.Text = awayScore.ToString();
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            HomeScoreLabel.Text = HomeScore.Text;
            AwayScoreLabel.Text = AwayScore.Text;
            await Navigation.PopAsync(false);
        }

        private void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}