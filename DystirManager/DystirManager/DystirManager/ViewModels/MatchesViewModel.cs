using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using DystirManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace DystirManager.ViewModels
{
    public class MatchesViewModel : BaseViewModel
    {
        DateTime _matchesDaySelected;
        public DateTime MatchesDaySelected
        {
            get { return _matchesDaySelected; }
            set { SetProperty(ref _matchesDaySelected, value); GetMatches(value); }
        }

        private void GetMatches(DateTime matchesDaySelected)
        {
            MatchesGroupList = Matches.Where(x => x.Time.Value.Date == matchesDaySelected.Date).GroupBy(x => x.MatchTypeName);
        }

        string _resultsCompetitionSelected;
        public string ResultsCompetitionSelected
        {
            get { return _resultsCompetitionSelected; }
            set { SetProperty(ref _resultsCompetitionSelected, value); GetResults(value); }
        }

        private void GetResults(string resultsCompetitionSelected)
        {
            ResultsGroupList = ResultsMatches.Where(x => x.MatchTypeName == resultsCompetitionSelected).GroupBy(x => x.MatchTypeName);
        }

        string _fixturesCompetitionSelected;
        public string FixturesCompetitionSelected
        {
            get { return _fixturesCompetitionSelected; }
            set { SetProperty(ref _fixturesCompetitionSelected, value); GetFixtures(value); }
        }

        private void GetFixtures(string fixturesCompetitionSelected)
        {
            FixturesGroupList = FixturesMatches.Where(x => x.MatchTypeName == fixturesCompetitionSelected).GroupBy(x => x.MatchTypeName);
        }

        public async Task<bool> GetPageData()
        {
            bool success;
            try
            {
                await GetMatchesAsync();
                await GetStandingsAsync();
                await GetTeamsAsync();
                await GetCategoriesAsync();
                await GetMatchTypesAsync();
                await GetSquadsAsync();
                await GetStatusesAsync();
                await GetRoundsAsync();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Error: {ex.Message}");
            }
            return success;
        }

        public async Task GetMatchesAsync()
        {
            string loadPadameter = "active";
            AllMatches = await GetDataStore().GetMatchesAsync(loadPadameter, this);
            CreateMatchesForPages(AllMatches ?? new ObservableCollection<Match>());
        }

        internal async Task GetSelectedLiveMatch(Match selectedMatch)
        {
            try
            {
                if (selectedMatch?.MatchID > 0)
                {
                    SelectedLiveMatch = await GetMatchDetailsAsync(selectedMatch);
                    SetMatchAdditionalDetails();
                }
            }
            catch (Exception ex)
            {
                _ = (Application.Current as App).Reconnect();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        internal ObservableCollection<Match> MatchesByAdministrator(ObservableCollection<Match> allMatchList)
        {
            List<Match> retMatchList = allMatchList.ToList();
            if (AdministratorLoggedIn.AdministratorTeamID > 0)
            {
                retMatchList.RemoveAll(x => x.HomeTeamID != AdministratorLoggedIn.AdministratorTeamID && x.AwayTeamID != AdministratorLoggedIn.AdministratorTeamID);
            }
            foreach(Match match in retMatchList)
            {
                match.IsEditable = AdministratorLoggedIn.AdministratorTeamID == 0;
            }
            return new ObservableCollection<Match>(retMatchList);
        }

        private async Task<Match> GetMatchDetailsAsync(Match selectedMatch)
        {
            MatchDetails matchDetails = await GetDataStore().GetMatchDetailsAsync(selectedMatch?.MatchID.ToString());
            Match match = matchDetails.Match;
            match.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(matchDetails.PlayersOfMatch);
            match.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
            match.LiveTime = selectedMatch.LiveTime;
            return match;
        }

        public async Task GetStandingsAsync()
        {
            Standings = await GetDataStore().GetStandingsAsync();
        }

        internal async Task<Match> GetMatchByIDAsync(string matchID)
        {
            return await GetDataStore().GetMatchAsync(matchID);
        }

        internal async Task GetAdministrators()
        {
            Administrators = await GetDataStore().GetAdministratorsAsync();
        }

        public async Task GetTeamsAsync()
        {
            Teams = await GetDataStore().GetTeamsAsync();
        }

        public async Task GetCategoriesAsync()
        {
            Categories = await GetDataStore().GetCategoriesAsync();
        }

        public async Task GetMatchTypesAsync()
        {
            MatchTypes = await GetDataStore().GetMatchTypesAsync();
        }

        public async Task GetSquadsAsync()
        {
            Squads = await GetDataStore().GetSquadsAsync();
        }

        public async Task GetStatusesAsync()
        {
            Statuses = await GetDataStore().GetStatusesAsync();
        }

        public async Task GetRoundsAsync()
        {
            Rounds = await GetDataStore().GetRoundsAsync();
        }

        public async Task GetPlayersOfMatchAsync(Match selectedMatch)
        {
            SelectedLiveMatch.PlayersOfMatch = await GetDataStore().GetPlayersOfMatchAsync(selectedMatch);
        }

        public async Task<bool> AddPlayerOfMatch(PlayerOfMatch playerOfMatch)
        {
            bool result = true;
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().AddPlayerOfMatchAsync(playerOfMatch);
            }
            catch (Exception ex)
            {
                MainException = ex;
                result = false;
            }
            IsLoading = false;
            return result;
        }

        public async Task<bool> UpdatePlayerOfMatch(PlayerOfMatch player)
        {
            bool result = true;
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().UpdatePlayerOfMatchAsync(player);
            }
            catch (Exception ex)
            {
                MainException = ex;
                result = false;
            }
            IsLoading = false;
            return result;
        }

        internal void CreateMatchesForPages(ObservableCollection<Match> allMatches)
        {
            allMatches = MatchesByAdministrator(allMatches);
            TimeCounter.MatchesTime(this, allMatches);

            //Matches
            Matches = new ObservableCollection<Match>(allMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time));

            //Results
            var toDate = DateTime.Now.Date.AddDays(2);
            var fromDate = new DateTime(toDate.Year - 1, 12, 31);
            ResultsMatches = new ObservableCollection<Match>(allMatches?.OrderBy(x => x.MatchTypeID).ThenByDescending(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            && (x.StatusID == 13 || x.StatusID == 12)));

            //Fixtures
            fromDate = DateTime.Now.Date.AddDays(-2);
            toDate = DateTime.Now.Date.AddDays(360);
            FixturesMatches = new ObservableCollection<Match>(allMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            && (x.StatusID < 2 || x.StatusID == 14)));
        }

        internal void SetMatchAdditionalDetails()
        {
            SelectedLiveMatch.Teams = Teams;
            SelectedLiveMatch.Statuses = Statuses;
            SelectedLiveMatch.MatchTypes = MatchTypes;
            SelectedLiveMatch.Categories = Categories;
            SelectedLiveMatch.Squads = Squads;
            SelectedLiveMatch.Rounds = Rounds;
        }
    }

    public class SummaryEventOfMatch
    {
        public EventOfMatch EventOfMatch { get; internal set; }
        public int HomeTeamScore { get; internal set; }
        public int AwayTeamScore { get; internal set; }
        public string HomeTeam { get; internal set; }
        public string AwayTeam { get; internal set; }
        public string EventName { get; internal set; }
        public string EventMinute { get; internal set; }
        public string HomeMainPlayer { get; internal set; }
        public string HomeSecondPlayer { get; internal set; }
        public string AwayMainPlayer { get; internal set; }
        public string AwaySecondPlayer { get; internal set; }
        public bool HomeTeamVisible { get; internal set; }
        public bool AwayTeamVisible { get; internal set; }
    }

    public class Statistic
    {
        public TeamStatistic HomeTeamStatistic { get; internal set; } = new TeamStatistic();
        public TeamStatistic AwayTeamStatistic { get; internal set; } = new TeamStatistic();
    }

    public class TeamStatistic
    {
        public int Goal { get; internal set; } = 0;
        public int YellowCard { get; internal set; } = 0;
        public int RedCard { get; internal set; } = 0;
        public int Corner { get; internal set; } = 0;
        public int GoalProcent { get; internal set; } = 100;
        public int YellowCardProcent { get; internal set; } = 100;
        public int RedCardProcent { get; internal set; } = 100;
        public int CornerProcent { get; internal set; } = 100;
        public int OnTarget { get; internal set; } = 0;
        public int OnTargetProcent { get; internal set; } = 100;
        public int OffTarget { get; internal set; } = 0;
        public int OffTargetProcent { get; internal set; } = 100;
        public int BlockedShot { get; internal set; } = 0;
        public int BlockedShotProcent { get; internal set; } = 100;
        public int BigChance { get; internal set; } = 0;
        public int BigChanceProcent { get; internal set; } = 100;
    }

    public enum TypePages
    {
        UpdatePage,
        NewPage
    }

    public enum TypeDetails
    {
        Team,
        Categorie,
        Squad,
        MatchType,
        Location,
        MatchStatus,
        LiveMatchPeriod,
        Date,
        Time,
        Result,
        Round
    }
}