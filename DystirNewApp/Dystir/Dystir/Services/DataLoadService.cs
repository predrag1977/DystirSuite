using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dystir.Models;
using Newtonsoft.Json;

namespace Dystir.Services
{
    public class DataLoadService
    {
        private readonly HttpClient httpClient;

        //private const string Url = "http://localhost:51346/api/";
        private const string Url = "https://www.dystir.fo/api/";

        //**************************//
        //        CONSTRUCTOR       //
        //**************************//
        public DataLoadService()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 10);
        }

        public async Task<ObservableCollection<Match>> GetMatchesAsync()
        {
            ObservableCollection<Match> matches = new ObservableCollection<Match>();
            var responseMatches = await httpClient.GetAsync(Url + "Matches");
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
            HttpResponseMessage responseMatch = await httpClient.GetAsync(Url + "Matches/" + matchID);

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
            List<Standing> standingsList = new List<Standing>();
            var responseStanding = await httpClient.GetAsync(Url + "Standings");
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
            List<CompetitionStatistic> competitionStatisticList = new List<CompetitionStatistic>();
            var responseCompetitionStatistics = await httpClient.GetAsync(Url + "Statistics");
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
            HttpResponseMessage responseMatchDetails = await httpClient.GetAsync(Url + "MatchDetails/" + matchID?.ToString() ?? "0");

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
            List<Sponsor> sponsorsList = new List<Sponsor>();
            var responseSponsors = await httpClient.GetAsync(Url + "Sponsors");
            if (responseSponsors.IsSuccessStatusCode)
            {
                var resultSponsors = await responseSponsors.Content.ReadAsStringAsync();
                sponsorsList = JsonConvert.DeserializeObject<List<Sponsor>>(resultSponsors);
            }
            foreach (Sponsor sponsor in sponsorsList)
            {
                sponsors.Add(sponsor);
            }

            return await Task.FromResult(sponsors);
        }

        public async Task<ObservableCollection<MatchCompetition>> GetCompetitionsAsync()
        {
            ObservableCollection<MatchCompetition> competitions = new ObservableCollection<MatchCompetition>();
            var responseCompetitions = await httpClient.GetAsync(Url + "MatchTypes");
            if (responseCompetitions.IsSuccessStatusCode)
            {
                var resultCompetitions = await responseCompetitions.Content.ReadAsStringAsync();
                var competitionsList = JsonConvert.DeserializeObject<List<MatchCompetition>>(resultCompetitions);
                foreach (MatchCompetition competition in competitionsList)
                {
                    competitions.Add(competition);
                }
            }
            return await Task.FromResult(competitions);
        }
    }
}