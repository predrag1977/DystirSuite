using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using System.Linq;
using DystirXamarin.Converter;
using System.Globalization;
using System.Threading.Tasks;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateAndNewMatchPage : ContentPage
    {
        public Match Match { get; set; }

        private MatchesViewModel _viewModel;
        private TypePages _typePreviousPage;

        public UpdateAndNewMatchPage(Match match, MatchesViewModel viewModel, TypePages typePreviousPage)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _typePreviousPage = typePreviousPage;
            SetPageButtonsAndTitle(typePreviousPage);
            Match = match;
            Match.Teams = viewModel.Teams;
            Match.Categories = viewModel.Categories;
            Match.MatchTypes = viewModel.MatchTypes;
            Match.Squads = viewModel.Squads;
            Match.Statuses = viewModel.Statuses;
            Match.Rounds = viewModel.Rounds;
            BindingContext = this;
        }

        private void SetPageButtonsAndTitle(TypePages typePreviousPage)
        {
            switch (typePreviousPage)
            {
                case TypePages.UpdatePage:
                    DeleteButton.IsVisible = true;
                    Title = "Update match";
                    break;

                case TypePages.NewPage:
                    DeleteButton.IsVisible = false;
                    Title = "New match";
                    break;
            }
        }

        private void Save_Tapped(object sender, EventArgs e)
        {
            switch (_typePreviousPage)
            {
                case TypePages.UpdatePage:
                    UpdateMatch();
                    break;

                case TypePages.NewPage:
                    NewMatch();
                    break;
            }
        }

        private async void UpdateMatch()
        {
            SetMatchPropertiesValue();
            await Navigation.PopAsync(false);
            await _viewModel.UpdateMatchAsync(Match, false);
            await _viewModel.GetMatches();
        }

        private async void NewMatch()
        {
            SetMatchPropertiesValue();
            await Navigation.PopAsync(false);
            await _viewModel.NewMatch(Match);
            await _viewModel.GetMatches();
        }

        private async void DeleteMatch_Tapped(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete match", "Do you want to delete match?", "yes", "cancel");
            if (answer)
            {
                Match.MatchActivation = 1;
                await Navigation.PopAsync(false);
                await _viewModel.UpdateMatchAsync(Match, false);
                await _viewModel.GetMatches();
            }
        }

        private void SetMatchPropertiesValue()
        {
            Match.MatchTypeName = MatchTypeName.Text;
            Match.MatchTypeID = Match.MatchTypes?.FirstOrDefault(x => x.MatchTypeName?.ToLower() == Match.MatchTypeName?.ToLower())?.MatchTypeID ?? 100;
            Match.Location = LocationLabel.Text;
            Match.HomeTeam = HomeTeam.Text;
            Match.HomeTeamID = Match.Teams?.FirstOrDefault(x => x.TeamName == Match.HomeTeam)?.TeamID;
            Match.HomeCategoriesName = new CategorieConverter()?.Convert(HomeCategoriesName.Text, null, "home", CultureInfo.CurrentCulture)?.ToString();
            string homeSquadName = Match.Squads?.FirstOrDefault(x => x.SquadName?.ToLower() == HomeSquadName.Text?.ToLower())?.SquadShortName ?? HomeSquadName.Text;
            Match.HomeSquadName = new SquadConverter()?.Convert(homeSquadName, null, "home", CultureInfo.CurrentCulture)?.ToString();
            Match.AwayTeam = AwayTeam.Text;
            Match.AwayTeamID = Match.Teams?.FirstOrDefault(x => x.TeamName == Match.AwayTeam)?.TeamID;
            Match.AwayCategoriesName = new CategorieConverter().Convert(AwayCategoriesName.Text, null, "away", CultureInfo.CurrentCulture)?.ToString(); ;
            string awaySquadName = Match.Squads?.FirstOrDefault(x => x.SquadName?.ToLower() == AwaySquadName.Text?.ToLower())?.SquadShortName ?? AwaySquadName.Text;
            Match.AwaySquadName = new SquadConverter()?.Convert(awaySquadName, null, "away", CultureInfo.CurrentCulture)?.ToString();

            string statusName = MatchStatus.Text;
            int? statusID = Match.Statuses?.Where(x => x.StatusID == 0 || x.StatusID == 13 || x.StatusID == 14).FirstOrDefault(x => x.StatusName?.ToLower() == statusName?.ToLower())?.StatusID;
            if (statusID == 0)
            {
                statusName = LiveMatchPeriod.Text ?? Match.Statuses?.FirstOrDefault(x => x.StatusID == 1).StatusName;              
                statusID = Match.Statuses?.Where(x => x.StatusID > 0 && x.StatusID < 13).FirstOrDefault(x => x.StatusName?.ToLower() == statusName?.ToLower())?.StatusID;
            }
            Match.StatusID = statusID;
            Match.StatusName = statusName;

            DateTime matchDateTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(MatchDate.Text))
                matchDateTime = DateTime.ParseExact(MatchDate.Text, "dd.MM.yyyy", null);
            if (!string.IsNullOrWhiteSpace(MatchTime.Text))
            {
                TimeSpan ts = TimeSpan.TryParse(MatchTime.Text, out TimeSpan value) ? value : new TimeSpan();
                matchDateTime = matchDateTime.Add(ts);
            }
            Match.Time = matchDateTime.ToUniversalTime();

            Match.HomeTeamScore = Convert.ToInt32(HomeScore.Text);
            Match.AwayTeamScore = Convert.ToInt32(AwayScore.Text);
        }

        // TAPPED EVENTS 
        #region TAPPED EVENTS

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

        private void MatchStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Match != null)
            {
                int? statusID = Match?.Statuses?.FirstOrDefault(x => x.StatusName?.ToLower() == MatchStatus.Text?.ToLower())?.StatusID;
                ResultLayout.IsVisible = statusID >= 0 && statusID <= 13 ? true : false;
                LiveMatchPeriodLayout.IsVisible = statusID >= 0 && statusID < 12 ? true : false;
                int? statusIDLivePeriod = Match?.Statuses?.FirstOrDefault(x => x.StatusName?.ToLower() == LiveMatchPeriod.Text?.ToLower())?.StatusID;
                if (statusID == 0)
                {
                    if(statusIDLivePeriod > 13)
                    {
                        LiveMatchPeriod.Text = Match?.Statuses?.FirstOrDefault(x => x.StatusID == 1)?.StatusName;
                    }
                    if (statusIDLivePeriod == 1 || statusIDLivePeriod == 13)
                    {
                        LiveMatchPeriod.Text = Match.StatusName;
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

        private void ManagerControl_Tapped(object sender, EventArgs e)
        {
            NavigateToDetails(TypeDetails.Manager, ManagersLabel);
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
                    await Navigation.PushAsync(new ResultPage(Match, HomeScore, AwayScore), false);
                    break;
                case TypeDetails.Round:
                    await Navigation.PushAsync(new RoundPickerPage(Match, label), false);
                    break;
                case TypeDetails.Manager:
                    await Navigation.PushAsync(new ManagerPickerPage(_viewModel, Match, label), false);
                    break;
                default:
                    await Navigation.PushAsync(new DetailPickerPage(Match, typeDetails, label), false);
                    break;
            }
        }

        #endregion
    }
}