﻿using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using System.Collections.Generic;
using DystirXamarin.ViewModels;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPickerPage : ContentPage
    {
        public List<String> DetailList { get; private set; }
        private Match _match { get; set; }
        private TypeDetails _typeDetails { get; set; }
        public Label ValueLabel { get; set; }
        public string SelectedValue { get; private set; }

        public DetailPickerPage(Match match, TypeDetails typeDetails, Label valueLabel)
        {
            InitializeComponent();
            _match = match;
            _typeDetails = typeDetails;
            ValueLabel = valueLabel;
            SelectedValue = ValueLabel.Text;
            Populate(match, typeDetails);
            BindingContext = this;
        }

        private void Populate(Match match, TypeDetails typeDetails)
        {
            DetailList = new List<String>();
            switch (typeDetails)
            {
                case TypeDetails.MatchType:
                    Title = "Pick competition";
                    DetailList = match.MatchTypes.Select(x => x.MatchTypeName).ToList();
                    break;
                case TypeDetails.Team:
                    Title = "Pick team";
                    DetailList = match.Teams.OrderBy(x => x.TeamName).Select(x => x.TeamName).ToList();
                    break;
                case TypeDetails.Categorie:
                    Title = "Pick team categorie";
                    DetailList = match.Categories.Select(x => x.CategorieName).ToList();
                    break;
                case TypeDetails.Squad:
                    Title = "Pick team squad";
                    DetailList = match.Squads.Select(x => x.SquadName).ToList();
                    break;
                case TypeDetails.Location:
                    Title = "Pick match location";
                    DetailList = match.Teams.OrderBy(x => x.TeamLocation).Select(x => x.TeamLocation).Distinct().ToList();
                    break;
                case TypeDetails.MatchStatus:
                    Title = "Pick match status";
                    DetailList = match.Statuses?.Where(x => x.StatusID == 0 || x.StatusID == 13 || x.StatusID == 14).Select(x => x.StatusName).ToList();
                    break;
                case TypeDetails.LiveMatchPeriod:
                    Title = "Pick live match status";
                    DetailList = match.Statuses?.Where(x => x.StatusID > 0 && x.StatusID < 13).Select(x => x.StatusName).ToList();
                    break;
            }
        }

        private void DetailsItem_Tapped(object sender, EventArgs e)
        {
            string selectedValue = (e as TappedEventArgs).Parameter?.ToString() ?? string.Empty;
            if (_typeDetails == TypeDetails.Squad)
                ValueEntry.Text = _match.Squads.FirstOrDefault(x => x.SquadName?.ToLower() == selectedValue.ToLower()).SquadShortName;
            else
                ValueEntry.Text = selectedValue;
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            ValueLabel.Text = ValueEntry.Text;
            await Navigation.PopAsync(false);
        }

        private void DetailsMatchListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}