using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dystir.Models;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Dystir.ViewModels
{
    public class MatchesViewModel : BaseViewModel
    {
        public async Task<bool> GetFullDataAsync()
        {
            bool success = true;
            try
            {
                var task1 = GetMatchesAsync();
                var task2 = GetStandingsAsync();
                var task3 = GetSponsorsAsync();
                var task4 = GetCompetitionStatisticsAsync();
                await Task.WhenAll(task1, task2, task3, task4);
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsLoading = false;
            return success;
        }

        public async Task GetMatchesAsync()
        {
            string loadPadameter = "active";
            AllMatches = await GetDataStore().GetMatchesAsync(loadPadameter, this);
            SetAllMatchesWithDetails();
        }

        internal async Task<bool> GetSelectedLiveMatch(Match selectedMatch)
        {
            bool success;
            try
            {
                await GetMatchDetailsAsync(selectedMatch);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Error: {ex.Message}");
            }
            IsLoadingSelectedMatch = false;
            return success;
        }

        private async Task GetMatchDetailsAsync(Match selectedMatch)
        {
            MatchDetails matchDetails = await GetDataStore().GetMatchDetailsAsync(selectedMatch?.MatchID.ToString());
            if (matchDetails != null)
            {
                MatchDetails selectedMatchDetails = AllMatchesWithDetails?.FirstOrDefault(x => x.Match?.MatchID == selectedMatch?.MatchID);
                selectedMatchDetails.DetailsMatchTabIndex = 0;
                selectedMatchDetails.Match = matchDetails.Match;
                selectedMatchDetails.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(matchDetails.PlayersOfMatch);
                selectedMatchDetails.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
                selectedMatchDetails.IsDataLoaded = true;
            }
        }

        public async Task GetStandingsAsync()
        {
            AllStandings = await GetDataStore().GetStandingsAsync();
        }

        public async Task GetCompetitionStatisticsAsync()
        {
            AllCompetitionStatistics = await GetDataStore().GetCompetitionStatisticsAsync();
        }

        public async Task GetSponsorsAsync()
        {
            var sponsors = await GetDataStore().GetSponsorsAsync();
            Sponsors = new ObservableCollection<Sponsor>(sponsors.OrderBy(a => Guid.NewGuid()));
            AllMainSponsors = new ObservableCollection<Sponsor>(sponsors.Where(x => x.SponsorID < 100).OrderBy(a => Guid.NewGuid()));
            SponsorsMain = new ObservableCollection<Sponsor>(AllMainSponsors?.Take(2));
        }

        internal async Task<Match> GetMatchByIDAsync(string matchID)
        {
            return await GetDataStore().GetMatchAsync(matchID);
        }

        internal void CopyProperties(Match target, Match source)
        {
            foreach (PropertyInfo property in typeof(Match).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(target, property.GetValue(source, null), null);
            }
        }

        // ALL MATCHES WITH DETAILS
        public void SetAllMatchesWithDetails()
        {
            var allMatchesWithDetails = new ObservableCollection<MatchDetails>();
            foreach (Match match in AllMatches)
            {
                MatchDetails matchDetails = AllMatchesWithDetails.FirstOrDefault(x => x.Match?.MatchID == match.MatchID);
                if (matchDetails != null)
                {
                    matchDetails.Match = match;
                }
                else
                {
                    matchDetails = new MatchDetails()
                    {
                        Match = match,
                        PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(),
                        EventsOfMatch = new ObservableCollection<EventOfMatch>()
                    };
                }
                allMatchesWithDetails.Add(matchDetails);
            }
            AllMatchesWithDetails = new ObservableCollection<MatchDetails>(allMatchesWithDetails);
        }
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
}