using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirManager.Models;
using DystirManager.ViewModels;
using System.Linq;
using DystirManager.Converter;
using System.Globalization;
using System.Threading.Tasks;
using System.ComponentModel;
using DystirManager.Helper;
using System.Collections.Generic;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class UpdateAndNewMatchPage : ContentPage
    {
        private MatchesViewModel _viewModel;
        private TypePages _typeOfPage;

        public UpdateAndNewMatchPage(MatchesViewModel viewModel, TypePages typePreviousPage)
        {
            _viewModel = viewModel;
            _typeOfPage = typePreviousPage;
            _viewModel.SetMatchAdditionalDetails();
            InitializeComponent();
            BindingContext = _viewModel.SelectedLiveMatch;
            SetPageButtonsAndTitle(typePreviousPage);
            CreateMatchActionButtons();
        }

        private void SetPageButtonsAndTitle(TypePages typePreviousPage)
        {
            switch (typePreviousPage)
            {
                case TypePages.UpdatePage:
                    PageNameLabel.Text = "UPDATE MATCH";
                    break;

                case TypePages.NewPage:
                    PageNameLabel.Text = "NEW MATCH";
                    break;
            }
        }

        private void CreateMatchActionButtons()
        {
            //Do not show Delete button in NewMatch page
            if (_typeOfPage == TypePages.NewPage)
            {
                return;
            }

            Grid menuView = MenuView;
            if (menuView.Children != null)
            {
                menuView.Children.Clear();
                menuView.ColumnDefinitions.Clear();
                menuView.RowDefinitions.Clear();
            }

            RowDefinition rd = new RowDefinition
            {
                Height = new GridLength(50)
            };
            menuView.RowDefinitions.Add(rd);
            List<string> menuItemList = new List<string>()
            {
                "DELETE MATCH",
                //"SAVE MATCH",
            };

            int i = 0;
            foreach (string item in menuItemList)
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                menuView.ColumnDefinitions.Add(cd);

                DetailMenuItem view = new DetailMenuItem();
                view.BindingContext = item;
                Label menuTextLabel = (Label)view.FindByName("MenuTextLabel");
                StackLayout detailMenuItemPanel = (StackLayout)view.FindByName("DetailMenuItemPanel");
                detailMenuItemPanel.BackgroundColor = i == 0 ? Color.DarkRed : Color.DarkKhaki;
                menuTextLabel.TextColor = Color.White;
                menuTextLabel.FontSize = 10;
                Grid.SetColumn(view, i);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += MatchActionSelected_Tapped;
                tapGestureRecognizer.CommandParameter = i;
                view.GestureRecognizers.Add(tapGestureRecognizer);
                menuView.Children.Add(view);
                i++;
            }
        }

        private void MatchActionSelected_Tapped(object sender, EventArgs e)
        {
            int index = (int)(e as TappedEventArgs).Parameter;
            switch (index)
            {
                case 0:
                    DeleteMatchAsync();
                    break;
                case 1:
                    SaveMatchAsync();
                    break;
            }
        }

        private void Save_Tapped(object sender, EventArgs e)
        {
            SaveMatchAsync();
        }

        private async void SaveMatchAsync()
        {
            SetMatchPropertiesValue();
            await Navigation.PopAsync(false);
            switch (_typeOfPage)
            {
                case TypePages.UpdatePage:
                    await _viewModel.SelectedLiveMatch.UpdateMatch();
                    break;

                case TypePages.NewPage:
                    await _viewModel.SelectedLiveMatch.AddMatch();
                    break;
            }
        }

        private async void DeleteMatchAsync()
        {
            var answer = await DisplayAlert("Delete match", "Do you want to delete match\n" + string.Format("{0} - {1}", _viewModel.SelectedLiveMatch.HomeTeam, _viewModel.SelectedLiveMatch.AwayTeam), "yes", "cancel");
            if (answer)
            {
                _viewModel.SelectedLiveMatch.MatchActivation = 1;
                await Navigation.PopAsync(false);
                if (_typeOfPage == TypePages.UpdatePage)
                {
                    await _viewModel.SelectedLiveMatch.DeleteMatch();
                }
            }
        }

        private void SetMatchPropertiesValue()
        {
            _viewModel.SelectedLiveMatch.MatchTypeName = MatchTypeName.Text;
            _viewModel.SelectedLiveMatch.MatchTypeID = _viewModel.SelectedLiveMatch.MatchTypes?.FirstOrDefault(x => x.MatchTypeName?.ToLower() == _viewModel.SelectedLiveMatch.MatchTypeName?.ToLower())?.MatchTypeID ?? 100;
            _viewModel.SelectedLiveMatch.Location = LocationLabel.Text;
            _viewModel.SelectedLiveMatch.HomeTeam = HomeTeam.Text;
            _viewModel.SelectedLiveMatch.HomeTeamID = _viewModel.SelectedLiveMatch.Teams?.FirstOrDefault(x => x.TeamName == _viewModel.SelectedLiveMatch.HomeTeam)?.TeamID;
            _viewModel.SelectedLiveMatch.HomeCategoriesName = new CategorieConverter()?.Convert(HomeCategoriesName.Text, null, "home", CultureInfo.CurrentCulture)?.ToString();
            string homeSquadName = _viewModel.SelectedLiveMatch.Squads?.FirstOrDefault(x => x.SquadName?.ToLower() == HomeSquadName.Text?.ToLower())?.SquadShortName ?? HomeSquadName.Text;
            _viewModel.SelectedLiveMatch.HomeSquadName = new SquadConverter()?.Convert(homeSquadName, null, "home", CultureInfo.CurrentCulture)?.ToString();
            _viewModel.SelectedLiveMatch.AwayTeam = AwayTeam.Text;
            _viewModel.SelectedLiveMatch.AwayTeamID = _viewModel.SelectedLiveMatch.Teams?.FirstOrDefault(x => x.TeamName == _viewModel.SelectedLiveMatch.AwayTeam)?.TeamID;
            _viewModel.SelectedLiveMatch.AwayCategoriesName = new CategorieConverter().Convert(AwayCategoriesName.Text, null, "away", CultureInfo.CurrentCulture)?.ToString(); ;
            string awaySquadName = _viewModel.SelectedLiveMatch.Squads?.FirstOrDefault(x => x.SquadName?.ToLower() == AwaySquadName.Text?.ToLower())?.SquadShortName ?? AwaySquadName.Text;
            _viewModel.SelectedLiveMatch.AwaySquadName = new SquadConverter()?.Convert(awaySquadName, null, "away", CultureInfo.CurrentCulture)?.ToString();

            string statusName = MatchStatus.Text;
            int? statusID = _viewModel.SelectedLiveMatch.Statuses?.Where(x => x.StatusID == 0 || x.StatusID == 13 || x.StatusID == 14).FirstOrDefault(x => x.StatusName?.ToLower() == statusName?.ToLower())?.StatusID;
            if (statusID == 0)
            {
                statusName = LiveMatchPeriod.Text ?? _viewModel.SelectedLiveMatch.Statuses?.FirstOrDefault(x => x.StatusID == 1).StatusName;
                statusID = _viewModel.SelectedLiveMatch.Statuses?.Where(x => x.StatusID > 0 && x.StatusID < 13).FirstOrDefault(x => x.StatusName?.ToLower() == statusName?.ToLower())?.StatusID;
            }
            if (statusID != null)
            {
                _viewModel.SelectedLiveMatch.StatusID = statusID;
                _viewModel.SelectedLiveMatch.StatusName = statusName;
            }

            DateTime matchDateTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(MatchDate.Text))
                matchDateTime = DateTime.ParseExact(MatchDate.Text, "dd.MM.yyyy", null);
            if (!string.IsNullOrWhiteSpace(MatchTime.Text))
            {
                TimeSpan ts = TimeSpan.TryParse(MatchTime.Text, out TimeSpan value) ? value : new TimeSpan();
                matchDateTime = matchDateTime.Add(ts);
            }
            _viewModel.SelectedLiveMatch.Time = matchDateTime.ToUniversalTime();

            _viewModel.SelectedLiveMatch.HomeTeamScore = Convert.ToInt32(HomeScore.Text);
            _viewModel.SelectedLiveMatch.AwayTeamScore = Convert.ToInt32(AwayScore.Text);
        }

        // TAPPED EVENTS 
        //#region TAPPED EVENTS

        private void HomeTeamControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Team, HomeTeam);
        }

        private void AwayTeamControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Team, AwayTeam);
        }

        private void HomeCategorieControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Categorie, HomeCategoriesName);
        }

        private void AwayCategorieControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Categorie, AwayCategoriesName);
        }

        private void HomeSquadControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Squad, HomeSquadName);
        }

        private void AwaySquadControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Squad, AwaySquadName);
        }

        private void MatchTypeNameControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.MatchType, MatchTypeName);
        }

        private void RoundControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Round, RoundLabel);
        }

        private void LocationControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Location, LocationLabel);
        }

        private void DateControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Date, MatchDate);
        }

        private void TimeControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Time, MatchTime);
        }

        private void MatchStatusControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.MatchStatus, MatchStatus);
        }

        private void MatchStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_viewModel.SelectedLiveMatch != null)
            {
                int? statusID = _viewModel.SelectedLiveMatch?.Statuses?.FirstOrDefault(x => x.StatusName?.ToLower() == MatchStatus.Text?.ToLower())?.StatusID;
                ResultLayout.IsVisible = statusID >= 0 && statusID <= 13 ? true : false;
                LiveMatchPeriodLayout.IsVisible = statusID >= 0 && statusID <= 12 ? true : false;
                int? statusIDLivePeriod = _viewModel.SelectedLiveMatch?.Statuses?.FirstOrDefault(x => x.StatusName?.ToLower() == LiveMatchPeriod.Text?.ToLower())?.StatusID;
                if (statusID == 0)
                {
                    if (statusIDLivePeriod > 13)
                    {
                        LiveMatchPeriod.Text = _viewModel.SelectedLiveMatch?.Statuses?.FirstOrDefault(x => x.StatusID == 1)?.StatusName;
                    }
                    if (statusIDLivePeriod == 1 || statusIDLivePeriod == 13)
                    {
                        LiveMatchPeriod.Text = _viewModel.SelectedLiveMatch.StatusName;
                    }
                }
            }
        }

        private void LiveMatchPeriodControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.LiveMatchPeriod, LiveMatchPeriod);
        }

        private void ResultControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Result, sender as Label);
        }

        private async void NavigateToDetails(TypeDetails typeDetails, Label label)
        {
            switch (typeDetails)
            {
                case TypeDetails.Date:
                    await Navigation.PushAsync(new DatePickerPage(label), false);
                    break;
                case TypeDetails.Time:
                    await Navigation.PushAsync(new TimePickerPage(label), false);
                    break;
                case TypeDetails.Result:
                    await Navigation.PushAsync(new ResultPage(_viewModel.SelectedLiveMatch, HomeScore, AwayScore), false);
                    break;
                case TypeDetails.Round:
                    await Navigation.PushAsync(new RoundPickerPage(_viewModel.SelectedLiveMatch, label), false);
                    break;
                default:
                    await Navigation.PushAsync(new DetailPickerPage(_viewModel.SelectedLiveMatch, typeDetails, label), false);
                    break;
            }
        }

        private void BackToMatchesView_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        //#endregion
    }
}