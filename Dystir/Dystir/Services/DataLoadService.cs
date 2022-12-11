using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.ViewModels;
using Newtonsoft.Json;

namespace Dystir.Services
{
    public class DataLoadService
    {
        //private const string Url = "https://www.faroekickoff.com/api/";
        //private const string Url = "http://localhost:4621/api/";
        //private const string Url = "http://localhost:6061/api/";
        private const string Url = "https://www.dystir.fo/api/";

        public async Task<ObservableCollection<Match>> GetMatchesAsync()
        {
            ObservableCollection<Match> matches = new ObservableCollection<Match>();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            var responseMatches = await client.GetAsync(Url + "Matches");
            if (responseMatches.IsSuccessStatusCode)
            {
                var resultMatches = await responseMatches.Content.ReadAsStringAsync();

                var matchesList = JsonConvert.DeserializeObject<List<Match>>(resultMatches);

                foreach (Match match in matchesList?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time))
                {
                    matches.Add(match);
                }
            }
            return await Task.FromResult(matches);
        }

        public async Task<Match> GetMatchAsync(string matchID)
        {
            Match match = null; 
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            HttpResponseMessage responseMatch = await client.GetAsync(Url + "Matches/" + matchID);

            if (responseMatch.IsSuccessStatusCode)
            {
                var resultMatch = await responseMatch.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(resultMatch);
            }
            return await Task.FromResult(match);
        }

        public async Task<ObservableCollection<Standing>> GetStandingsAsync()
        {
            ObservableCollection<Models.Standing> standings = new ObservableCollection<Standing>();
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

        public async Task<ObservableCollection<CompetitionStatistic>> GetStatisticsAsync()
        {
            ObservableCollection<CompetitionStatistic> competitionStatistics = new ObservableCollection<CompetitionStatistic>();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            List<CompetitionStatistic> competitionStatisticList = new List<CompetitionStatistic>();
            var responseCompetitionStatistics = await client.GetAsync(Url + "Statistics");
            if (responseCompetitionStatistics.IsSuccessStatusCode)
            {
                var resultCompetitionStatistics = await responseCompetitionStatistics.Content.ReadAsStringAsync();
                competitionStatisticList = JsonConvert.DeserializeObject<List<CompetitionStatistic>>(resultCompetitionStatistics);
            }
            foreach (CompetitionStatistic competitionStatistic in competitionStatisticList)
            {
                competitionStatistics.Add(competitionStatistic);
            }
            return await Task.FromResult(competitionStatistics);
        }

        public async Task<MatchDetails> GetMatchDetailsAsync(int? matchID)
        {
            MatchDetails matchDetails = new MatchDetails();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 30);
            HttpResponseMessage responseMatchDetails = await client.GetAsync(Url + "MatchDetails/" + matchID?.ToString() ?? "0");

            if (responseMatchDetails.IsSuccessStatusCode)
            {
                var resultMatchDetails = await responseMatchDetails.Content.ReadAsStringAsync();
                matchDetails = JsonConvert.DeserializeObject<MatchDetails>(resultMatchDetails);
            }
            return await Task.FromResult(matchDetails);
        }

        public async Task<ObservableCollection<Sponsor>> GetSponsorsAsync()
        {
            ObservableCollection<Sponsor> sponsors = new ObservableCollection<Sponsor>();
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            List<Sponsor> sponsorsList = new List<Sponsor>();
            var responseStanding = await client.GetAsync(Url + "Sponsors");
            if (responseStanding.IsSuccessStatusCode)
            {
                var resultSponsors = await responseStanding.Content.ReadAsStringAsync();
                sponsorsList = JsonConvert.DeserializeObject<List<Sponsor>>(resultSponsors);
            }
            foreach (Sponsor sponsor in sponsorsList)
            {
                sponsors.Add(sponsor);
            }

            return await Task.FromResult(sponsors);
        }
    }
}