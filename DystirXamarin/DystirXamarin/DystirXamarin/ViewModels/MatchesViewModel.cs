using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DystirXamarin.Models;
using System.Linq;

namespace DystirXamarin.ViewModels
{
    public class MatchesViewModel : BaseViewModel
    {
        //public Command RefreshPlayersOfMatchCommand { get; private set; }
        public Administrator AdministratorLoggedIn { get; set; }
        public static string Token = null;

        public MatchesViewModel()
        {
            //RefreshPlayersOfMatchCommand = new Command(async () =>
            //{
                
            //});
        }

        public async void GetFullData()
        {
            IsLoading = true;
            MainException = null;
            try
            {
                var loadMatchesTask = GetMatches();
                var loadCategoriesTask = GetCategories();
                var loadTeamsTask = GetTeams();
                var loadMatchTypesTask = GetMatchTypes();
                var loadSquadsTask = GetSquads();
                var loadStatusesTask = GetStatuses();
                var loadRoundsTask = GetRounds();
                await Task.WhenAll(loadMatchesTask, loadCategoriesTask, loadTeamsTask, loadMatchTypesTask, loadSquadsTask, loadStatusesTask, loadRoundsTask);
            }
            catch (Exception ex)
            {
                MainException = ex;
                //Match matchCache = new Match();
                //var tempMatches = new ObservableCollection<Match>();
                //var cacheProp = Application.Current.Properties;
                //if (cacheProp.Count > 0)
                //{
                //    matchCache = (Match)Application.Current.Properties.FirstOrDefault(x => x.Key == "match").Value;
                //    tempMatches.Add(matchCache);
                //}
                //Matches = tempMatches;

            }
            IsLoading = false;
        }

        internal async Task GetAdministrators()
        {
            Administrators = await GetDataStore().GetAdministratorsAsync();
        }

        public async Task GetMatches()
        {
            AllMatches = await GetDataStore().GetMatchesAsync("active", this);
            if (AdministratorLoggedIn.AdministratorTeamID == 0)
            {
                Matches = new ObservableCollection<Match>(AllMatches.Where(x => x.Time.Value.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date));
            }
            else
            {
                Matches = new ObservableCollection<Match>(AllMatches);
            }
        }

        public async Task GetTeams()
        {
            Teams = await GetDataStore().GetTeamsAsync();
        }

        public async Task GetCategories()
        {
            Categories = await GetDataStore().GetCategoriesAsync();
        }

        public async Task GetMatchTypes()
        {
            MatchTypes = await GetDataStore().GetMatchTypesAsync();
        }

        public async Task GetSquads()
        {
            Squads = await GetDataStore().GetSquadsAsync();
        }

        public async Task GetStatuses()
        {
            Statuses = await GetDataStore().GetStatusesAsync();
        }

        public async Task GetRounds()
        {
            Rounds= await GetDataStore().GetRoundsAsync();
        }

        internal async Task<bool> GetSelectedLiveMatch(Match selectedMatch, bool loadFullListPlayers)
        {
            bool result = true;
            IsLoading = true;
            MainException = null;
            try
            {
                if (selectedMatch?.MatchID > 0)
                {
                    SelectedLiveMatch = await GetMatchDetailsAsync(selectedMatch);
                    if (loadFullListPlayers)
                    {
                        await GetPlayersOfMatchAsync(selectedMatch);
                    }
                    else
                    {
                        SelectedLiveMatch.PlayersOfMatch = selectedMatch.PlayersOfMatch;
                    }
                    SetMatchAdditionalDetails();
                }
            }
            catch (Exception ex)
            {
                MainException = ex;
                result = false;
            }
            IsLoading = false;
            return result;
        }

        public async Task GetPlayersOfMatchAsync(Match selectedMatch)
        {
            SelectedLiveMatch.PlayersOfMatch = await GetDataStore().GetPlayersOfMatchAsync(selectedMatch);
        }

        internal async Task GetEventsOfMatchAsync(Match selectedLiveMatch)
        {
            SelectedLiveMatch.EventsOfMatch = await GetDataStore().GetEventsOfMatchAsync(selectedLiveMatch);
        }

        internal async Task UpdateMatchAsync(Match match, bool changeMatchTime)
        {
            IsLoading = true;
            MainException = null;
            try
            {
                if (!changeMatchTime)
                {
                    match.ExtraMinutes = 0;
                    match.ExtraSeconds = 0;
                }
                match = await GetDataStore().UpdateMatchAsync(match);
            }
            catch (Exception ex)
            {
                MainException = ex;
            }
            var mat = Matches.FirstOrDefault(m => m.MatchID == match.MatchID);
            if (mat != null)
            {
                Matches.Remove(mat);
            }
            Matches.Add(match);
            IsLoading = false;
        }

        internal async Task NewMatch(Match match)
        {
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().AddMatchAsync(match);
            }
            catch (Exception ex)
            {
                MainException = ex;
                Matches.Add(match);
            }
            IsLoading = false;
        }

        internal async Task DeleteMatch(Match match)
        {
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().DeleteMatchAsync(match);
            }
            catch (Exception ex)
            {
                MainException = ex;
            }
            IsLoading = false;
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

        internal async Task<bool> AddEventOfMatch(EventOfMatch eventOfMatch, Match selectedMatch)
        {
            bool result = true;
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().AddEventOfMatchAsync(eventOfMatch);
                await GetSelectedLiveMatch(selectedMatch, false);
            }
            catch (Exception ex)
            {
                MainException = ex;
                result = false;
            }
            IsLoading = false;
            return result;
        }

        internal async Task<bool> UpdateEventOfMatchAsync(EventOfMatch eventOfMatch, Match selectedMatch)
        {
            bool result = true;
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().UpdateEventOfMatchAsync(eventOfMatch);
                await GetSelectedLiveMatch(selectedMatch, false);
            }
            catch (Exception ex)
            {
                MainException = ex;
                result = false;
            }
            IsLoading = false;
            return result;
        }

        internal async Task DeleteEventOfMatchAsync(EventOfMatch eventOfMatch, Match selectedMatch)
        {
            IsLoading = true;
            MainException = null;
            try
            {
                await GetDataStore().DeleteEventOfMatchAsync(eventOfMatch);
                await GetSelectedLiveMatch(selectedMatch, false);
            }
            catch (Exception ex)
            {
                MainException = ex;
            }
            IsLoading = false;
        }

        private async Task<Match> GetMatchDetailsAsync(Match selectedMatch)
        {
            MatchDetails matchDetails = await GetDataStore().GetMatchDetailsAsync(selectedMatch?.MatchID.ToString());
            Match match = matchDetails.Match;
            match.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
            return match;
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
}