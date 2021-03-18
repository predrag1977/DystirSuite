using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DystirManager.Models;
using DystirManager.ViewModels;
using Newtonsoft.Json;

[assembly: Xamarin.Forms.Dependency(typeof(DystirManager.Services.DataLoader))]
namespace DystirManager.Services
{
    public class DataLoader : IDataLoader<Match>
    {
        //private const string Url = "https://www.faroekickoff.com/api/";
        //private const string Url = "http://localhost:4621/api/";
        //private const string Url = "http://localhost:6061/api/";
        //private const string Url = "http://localhost:11896/api/";
        private const string Url = "https://www.dystir.fo/api/";

        public async Task<ObservableCollection<Match>> GetMatchesAsync(string typeOfMatches, MatchesViewModel viewModel)
        {
            ObservableCollection<Match> matches = new ObservableCollection<Match>();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0,0,20);
            List<Match> matchesList = new List<Match>();
            var responseMatches = await client.GetAsync(Url + "Matches?action=" + typeOfMatches);
            if (responseMatches.IsSuccessStatusCode)
            {
                var resultMatches = await responseMatches.Content.ReadAsStringAsync();
                matchesList = JsonConvert.DeserializeObject<List<Match>>(resultMatches);
            }

            foreach (Match match in matchesList?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time))
            {
                matches.Add(match);
            }
            return await Task.FromResult(matches);
        }

        public async Task<Match> GetMatchAsync(string matchID)
        {
            Match Match = new Match();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            HttpResponseMessage responseMatch = await client.GetAsync(Url + "Matches/" + matchID);

            if (responseMatch.IsSuccessStatusCode)
            {
                var resultMatch = await responseMatch.Content.ReadAsStringAsync();
                Match = JsonConvert.DeserializeObject<Match>(resultMatch);
            }
            return await Task.FromResult(Match);
        }

        public async Task<ObservableCollection<Standing>> GetStandingsAsync()
        {
            ObservableCollection<Standing> standings = new ObservableCollection<Standing>();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            List<Standing> standingsList = new List<Standing>();
            var responseStanding = await client.GetAsync(Url + "Standings");
            if (responseStanding.IsSuccessStatusCode)
            {
                var resultStanding = await responseStanding.Content.ReadAsStringAsync();
                standingsList = JsonConvert.DeserializeObject<List<Standing>>(resultStanding);
            }
            foreach (Standing standing in standingsList)
            {
                standings.Add(standing);
            }
            return await Task.FromResult(standings);
        }

        public async Task<MatchDetails> GetMatchDetailsAsync(string matchID)
        {
            MatchDetails matchDetails = new MatchDetails();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            HttpResponseMessage responseMatchDetails = await client.GetAsync(Url + "MatchDetails/" + matchID);

            if (responseMatchDetails.IsSuccessStatusCode)
            {
                var resultMatchDetails = await responseMatchDetails.Content.ReadAsStringAsync();
                matchDetails = JsonConvert.DeserializeObject<MatchDetails>(resultMatchDetails);
            }
            return await Task.FromResult(matchDetails);
        }

        public async Task<ObservableCollection<Administrator>> GetAdministratorsAsync()
        {
            ObservableCollection<Administrator> administrators = new ObservableCollection<Administrator>();
            HttpClient client = new HttpClient();
            List<Administrator> administratorsList = new List<Administrator>();
            var responseAdmistrators = await client.GetAsync(Url + "Administrators");
            if (responseAdmistrators.IsSuccessStatusCode)
            {
                var resultAdministrators = await responseAdmistrators.Content.ReadAsStringAsync();
                administratorsList = JsonConvert.DeserializeObject<List<Administrator>>(resultAdministrators);
            }
            foreach (Administrator administrator in administratorsList)
            {
                administrators.Add(administrator);
            }
            return await Task.FromResult(administrators);
        }

        public async Task<ObservableCollection<Team>> GetTeamsAsync()
        {
            ObservableCollection<Team> teams = new ObservableCollection<Team>();
            HttpClient client = new HttpClient();
            List<Team> teamsList = new List<Team>();
            var responseTeams = await client.GetAsync(Url + "Teams");
            if (responseTeams.IsSuccessStatusCode)
            {
                var resultTeam = await responseTeams.Content.ReadAsStringAsync();
                teamsList = JsonConvert.DeserializeObject<List<Team>>(resultTeam);
            }
            foreach (Team team in teamsList?.OrderBy(x => x.TeamName))
            {
                teams.Add(team);
            }
            return await Task.FromResult(teams);
        }

        public async Task<ObservableCollection<Categorie>> GetCategoriesAsync()
        {
            ObservableCollection<Categorie> categories = new ObservableCollection<Categorie>();
            HttpClient client = new HttpClient();
            List<Categorie> categoriesList = new List<Categorie>();
            var responseCategories = await client.GetAsync(Url + "Categories");
            if (responseCategories.IsSuccessStatusCode)
            {
                var resultCategories = await responseCategories.Content.ReadAsStringAsync();
                categoriesList = JsonConvert.DeserializeObject<List<Categorie>>(resultCategories);
            }
            foreach (Categorie categorie in categoriesList)
            {
                categories.Add(categorie);
            }
            return await Task.FromResult(categories);
        }

        public async Task<ObservableCollection<MatchType>> GetMatchTypesAsync()
        {
            ObservableCollection<MatchType> matchTypes = new ObservableCollection<MatchType>();
            HttpClient client = new HttpClient();
            List<MatchType> matchTypesList = new List<MatchType>();
            var responseMatchType = await client.GetAsync(Url + "MatchTypes");
            if (responseMatchType.IsSuccessStatusCode)
            {
                var resultMatchType = await responseMatchType.Content.ReadAsStringAsync();
                matchTypesList = JsonConvert.DeserializeObject<List<MatchType>>(resultMatchType);
            }
            foreach (MatchType matchType in matchTypesList)
            {
                matchTypes.Add(matchType);
            }
            return await Task.FromResult(matchTypes);
        }

        public async Task<ObservableCollection<Squad>> GetSquadsAsync()
        {
            ObservableCollection<Squad> squads = new ObservableCollection<Squad>();
            HttpClient client = new HttpClient();
            List<Squad> squadsList = new List<Squad>();
            var responseSquad = await client.GetAsync(Url + "Squads");
            if (responseSquad.IsSuccessStatusCode)
            {
                var resultSquad = await responseSquad.Content.ReadAsStringAsync();
                squadsList = JsonConvert.DeserializeObject<List<Squad>>(resultSquad);
            }
            foreach (Squad squad in squadsList)
            {
                squads.Add(squad);
            }
            return await Task.FromResult(squads);
        }

        public async Task<ObservableCollection<Status>> GetStatusesAsync()
        {
            ObservableCollection<Status> statuses = new ObservableCollection<Status>();
            HttpClient client = new HttpClient();
            List<Status> statusesList = new List<Status>();
            var responseStatus = await client.GetAsync(Url + "Statuses");
            if (responseStatus.IsSuccessStatusCode)
            {
                var resultStatus = await responseStatus.Content.ReadAsStringAsync();
                statusesList = JsonConvert.DeserializeObject<List<Status>>(resultStatus);
            }
            foreach (Status status in statusesList)
            {
                statuses.Add(status);
            }
            return await Task.FromResult(statuses);
        }

        public async Task<ObservableCollection<Round>> GetRoundsAsync()
        {
            ObservableCollection<Round> rounds = new ObservableCollection<Round>();
            HttpClient client = new HttpClient();
            List<Round> roundsList = new List<Round>();
            var responseRound = await client.GetAsync(Url + "Rounds");
            if (responseRound.IsSuccessStatusCode)
            {
                var resultRound = await responseRound.Content.ReadAsStringAsync();
                roundsList = JsonConvert.DeserializeObject<List<Round>>(resultRound);
            }
            foreach (Round round in roundsList)
            {
                rounds.Add(round);
            }
            return await Task.FromResult(rounds);
        }

        public async Task<bool> AddMatchAsync(Match match)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(match), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(Url + "Matches", content);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> UpdateMatchAsync(Match match)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(match), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(Url + "Matches/" + match.MatchID, content);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(true);
            }
        }

        public async Task<bool> DeleteMatchAsync(Match match)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.DeleteAsync(Url + "Matches/" + match.MatchID);

            if (response.IsSuccessStatusCode)
            {

            }
            return await Task.FromResult(true);
        }

        public async Task<ObservableCollection<PlayerOfMatch>> GetPlayersOfMatchAsync(Match match)
        {
            ObservableCollection<PlayerOfMatch> playersOfMatch = new ObservableCollection<PlayerOfMatch>();
            HttpClient client = new HttpClient();

            var responsePlayers = await client.GetAsync(Url + "PlayersOfMatches?hometeamname=" + match?.HomeTeam?.Trim() + "&&awayteamname=" + match?.AwayTeam?.Trim() + "&&competitionID=" + match?.MatchTypeID + "&&selectedmatchID=" + match?.MatchID);
            if (responsePlayers.IsSuccessStatusCode)
            {
                var resultPlayers = await responsePlayers.Content.ReadAsStringAsync();
                playersOfMatch = JsonConvert.DeserializeObject<ObservableCollection<PlayerOfMatch>>(resultPlayers);
            }
            return await Task.FromResult(playersOfMatch);
        }

        public async Task<bool> AddPlayerOfMatchAsync(PlayerOfMatch playerOfMatch)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(playerOfMatch), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(Url + "PlayersOfMatches", content);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> UpdatePlayerOfMatchAsync(PlayerOfMatch player)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(player), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(Url + "PlayersOfMatches?id=" + player.PlayerOfMatchID, content);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(true);
            }
        }

        public async Task<bool> AddEventOfMatchAsync(EventOfMatch eventOfMatch)
        {
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(eventOfMatch), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(Url + "EventsOfMatches", content);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> UpdateEventOfMatchAsync(EventOfMatch eventOfMatch)
        {
            try
            {
                HttpClient client = new HttpClient();
                StringContent content = new StringContent(JsonConvert.SerializeObject(eventOfMatch), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(Url + "EventsOfMatches/" + eventOfMatch.EventOfMatchID, content);

                if (response.IsSuccessStatusCode)
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEventOfMatchAsync(EventOfMatch eventOfMatch)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.DeleteAsync(Url + "EventsOfMatches/" + eventOfMatch.EventOfMatchID);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(true);
            }
        }
    }
}
