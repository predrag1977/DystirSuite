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
    public partial class RoundPickerPage : ContentPage
    {
        public Match Match { get; private set; }
        public Label MainRoundLabel { get; private set; }
        public Round Round { get; private set; }

        public RoundPickerPage(Match match, Label mainRoundLabel)
        {
            InitializeComponent();
            Match = match;
            MainRoundLabel = mainRoundLabel;
            Round = new Round()
            {
                RoundID = match.RoundID,
                RoundName = match.RoundName
            };
            SetRoundToText(Round);
            BindingContext = this;
        }

        private void RoundUp_Tapped(object sender, EventArgs e)
        {
            if (Round.RoundID < 1 || Round.RoundID > 999)
            {
                Round.RoundID = 0;
            }
            Round.RoundID++;
            Round.RoundName = GetRoundName(Round.RoundID);
            SetRoundToText(Round);
        }

        private void RoundDown_Tapped(object sender, EventArgs e)
        {
            if (Round.RoundID < 1 || Round.RoundID > 999)
            {
                Round.RoundID = 0;
                RoundNameEntry.Text = "";
            }
            Round.RoundID--;
            Round.RoundName = GetRoundName(Round.RoundID);
            SetRoundToText(Round);
        }

        private string GetRoundName(int? roundID)
        {
            string roundName = "";
            if (roundID != null)
            {
                roundName = roundID + ". umfar";
            }
            return roundName;
        }

        private void RoundItem_Tapped(object sender, EventArgs e)
        {
            
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Round = new Round()
            {
                RoundID = ((Round)(e as TappedEventArgs).Parameter).RoundID,
                RoundName = ((Round)(e as TappedEventArgs).Parameter).RoundName
            };
            RoundIDLabel.Text = "";
            RoundNameEntry.Text = Round.RoundName;
        }

        private void SetRoundToText(Round round)
        {
            if (round.RoundID > 999 || round.RoundID < 1 || round.RoundID == null)
            {
                RoundIDLabel.Text = "";
                RoundNameEntry.Text = "";
                Round.RoundID = 0;
                Round.RoundName = "";
            }
            else
            {
                RoundIDLabel.Text = round.RoundID.ToString();
                RoundNameEntry.Text = round.RoundName;
            }
        }

        private void OK_Tapped(object sender, EventArgs e)
        {
            Match.RoundID = Round.RoundID;
            Match.RoundName = Round.RoundName;
            MainRoundLabel.Text = Round.RoundName;
            Navigation.PopAsync(false);
        }

        private void RoundMatchListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}