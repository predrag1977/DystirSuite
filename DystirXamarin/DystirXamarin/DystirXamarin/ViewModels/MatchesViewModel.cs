using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DystirXamarin.Models;
using System.Linq;
using Xamarin.Forms;
using DystirXamarin.Services;

namespace DystirXamarin.ViewModels
{
    public class MatchesViewModel : BaseViewModel
    {
        public Administrator AdministratorLoggedIn { get; set; }
        public static string Token;
        public MatchListType SelectedMatchListType = MatchListType.Today;
        private readonly DataLoaderService _dataLoaderService;

        public MatchesViewModel()
        {
            _dataLoaderService = DependencyService.Get<DataLoaderService>();
        }

        public Task<Administrator> LoginAsync(string token)
        {
            return _dataLoaderService.LoginAsync(token);
        }

        public async Task GetFullData()
        {
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

            }
            IsLoading = false;
        }

        internal async Task GetAdministrators()
        {
            Administrators = await _dataLoaderService.GetAdministratorsAsync();
        }

        public async Task GetMatches()
        {
            AllMatches = await _dataLoaderService.GetMatchesAsync("active", this);
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
            Teams = await _dataLoaderService.GetTeamsAsync();
        }

        public async Task GetCategories()
        {
            Categories = await _dataLoaderService.GetCategoriesAsync();
        }

        public async Task GetMatchTypes()
        {
            MatchTypes = await _dataLoaderService.GetMatchTypesAsync();
        }

        public async Task GetSquads()
        {
            Squads = await _dataLoaderService.GetSquadsAsync();
        }

        public async Task GetStatuses()
        {
            Statuses = await _dataLoaderService.GetStatusesAsync();
        }

        public async Task GetRounds()
        {
            Rounds= await _dataLoaderService.GetRoundsAsync();
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
            SelectedLiveMatch.PlayersOfMatch = await _dataLoaderService.GetPlayersOfMatchAsync(selectedMatch);
        }

        internal async Task GetEventsOfMatchAsync(Match selectedLiveMatch)
        {
            SelectedLiveMatch.EventsOfMatch = await _dataLoaderService.GetEventsOfMatchAsync(selectedLiveMatch);
        }

        internal async Task UpdateMatchAsync(Match match, bool changeMatchTime)
        {
            MainException = null;
            try
            {
                if (!changeMatchTime)
                {
                    match.ExtraMinutes = 0;
                    match.ExtraSeconds = 0;
                }
                match = await _dataLoaderService.UpdateMatchAsync(match);
            }
            catch (Exception ex)
            {
                MainException = ex;
            }
            var matches = new ObservableCollection<Match>(Matches);
            var mat = matches.FirstOrDefault(m => m.MatchID == match.MatchID);
            if (mat != null)
            {
                matches.Remove(mat);
            }
            matches.Add(match);
            Matches = new ObservableCollection<Match>(matches);
            IsLoading = false;
        }

        internal async Task NewMatch(Match match)
        {
            IsLoading = true;
            MainException = null;
            try
            {
                await _dataLoaderService.AddMatchAsync(match);
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
                await _dataLoaderService.DeleteMatchAsync(match);
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
                await _dataLoaderService.AddPlayerOfMatchAsync(playerOfMatch);
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
                await _dataLoaderService.UpdatePlayerOfMatchAsync(player);
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
                await _dataLoaderService.AddEventOfMatchAsync(eventOfMatch);
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
                await _dataLoaderService.UpdateEventOfMatchAsync(eventOfMatch);
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
                await _dataLoaderService.DeleteEventOfMatchAsync(eventOfMatch);
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
            MatchDetails matchDetails = await _dataLoaderService.GetMatchDetailsAsync(selectedMatch?.MatchID.ToString());
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