using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DystirWeb.Shared;
using DystirWeb.DystirDB;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DystirWeb.Services
{
    public class DystirService
    {
        static readonly object lockUpdateAllMatches = new object();
        static readonly object lockUpdateAllMatchesDetails = new object();
        static readonly object lockUpdateAllPlayersOfMatches = new object();
        private readonly DbContextOptions<DystirDBContext> _dbContextOptions;
        public DystirDBContext DystirDBContext;
        public ObservableCollection<Matches> AllMatches = new ObservableCollection<Matches>();
        public ObservableCollection<MatchDetails> AllMatchesDetails = new ObservableCollection<MatchDetails>();
        public ObservableCollection<Teams> AllTeams = new ObservableCollection<Teams>();
        public ObservableCollection<MatchTypes> AllCompetitions = new ObservableCollection<MatchTypes>();
        public ObservableCollection<Sponsors> AllSponsors = new ObservableCollection<Sponsors>();
        public ObservableCollection<PlayersOfMatches> AllPlayersOfMatches = new ObservableCollection<PlayersOfMatches>();
        public ObservableCollection<Requestor> AllRequestor = new ObservableCollection<Requestor>();

        public DystirService(DbContextOptions<DystirDBContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            _ = StartupAsync();
        }

        public async Task StartupAsync()
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                var t = ex.Message;
                Thread.Sleep(1000);
                await StartupAsync();
            }
        }

        public async Task LoadDataAsync()
        {
            DystirDBContext = new DystirDBContext(_dbContextOptions);
            AllMatchesDetails = new ObservableCollection<MatchDetails>();
            var allMatchesTask = GetAllMatchesAsync();
            var allSponsorsTask = GetAllSponsorsAsync();
            var allRequestorTask = GetAllRequestorsAsync();
            await Task.WhenAll(allMatchesTask, allSponsorsTask, allRequestorTask);
        }

        public async Task GetAllMatchesAsync()
        {
            var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            AllMatches = new ObservableCollection<Matches>(DystirDBContext.Matches
                .Where(y => y.Time > fromDate
                && y.MatchActivation != 1
                && y.MatchActivation != 2));
            var allTeamsTask = GetAllTeamsAsync();
            var allPlayersOfMatchesTask = GetPlayersOfMatchesAsync();
            var allCompetitionTask = GetAllCompetitionAsync();
            await Task.WhenAll(allTeamsTask, allPlayersOfMatchesTask, allCompetitionTask);
            foreach (Matches match in AllMatches)
            {
                SetTeamLogoInMatch(match);
            }
        }

        private async Task GetAllTeamsAsync()
        {
            AllTeams = new ObservableCollection<Teams>(DystirDBContext.Teams);
            await Task.CompletedTask;
        }

        private async Task GetAllCompetitionAsync()
        {
            AllCompetitions = new ObservableCollection<MatchTypes>(DystirDBContext.MatchTypes);
            await Task.CompletedTask;
        }

        private async Task GetAllSponsorsAsync()
        {
            AllSponsors = new ObservableCollection<Sponsors>(DystirDBContext.Sponsors);
            await Task.CompletedTask;
        }

        private async Task GetAllRequestorsAsync()
        {
            AllRequestor = new ObservableCollection<Requestor>(DystirDBContext.Requestor);
            await Task.CompletedTask;
        }

        private async Task GetPlayersOfMatchesAsync()
        {
            AllPlayersOfMatches = new ObservableCollection<PlayersOfMatches>(DystirDBContext.PlayersOfMatches
                .Where(x => x.Goal > 0 || x.Assist > 0).ToList().Where(p => AllMatches.Any(m => m.MatchID == p.MatchId)));
            await Task.CompletedTask;
        }

        public async Task UpdateDataAsync(MatchDetails matchDetails)
        {
            Matches match = matchDetails.Match;
            if (match == null) return;

            var updateAllMatchesTask = UpdateAllMatchesAsync(matchDetails);
            var updateAllMatchesDetailsTask = UpdateAllMatchesDetailsAsync(matchDetails);
            var updateAllPlayersOfMatchesTask = UpdateAllPlayersOfMatchesAsync(matchDetails);
            await Task.WhenAll(updateAllMatchesTask, updateAllMatchesDetailsTask, updateAllPlayersOfMatchesTask);
        }

        public async Task UpdateAllMatchesAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateAllMatches)
            {
                Matches match = matchDetails.Match;
                var findMatch = AllMatches.FirstOrDefault(x => x.MatchID == match.MatchID);
                if (findMatch != null)
                {
                    AllMatches.Remove(findMatch);
                }
                SetTeamLogoInMatch(match);
                AllMatches.Add(match);
                matchDetails.Matches = AllMatches.Where(x =>
                    x.Time > DateTime.UtcNow.AddDays(-2) &&
                    x.Time < DateTime.UtcNow.AddDays(2)
                ).ToList();
            }
            await Task.CompletedTask;
        }

        public async Task UpdateAllMatchesDetailsAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateAllMatchesDetails)
            {
                var findMatchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == matchDetails?.MatchDetailsID);
                if (findMatchDetails != null)
                {
                    AllMatchesDetails.Remove(findMatchDetails);
                }
                AllMatchesDetails.Add(matchDetails);
            }
            await Task.CompletedTask;
        }

        public async Task UpdateAllPlayersOfMatchesAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateAllPlayersOfMatches)
            {
                var findMatchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == matchDetails?.MatchDetailsID);
                if (findMatchDetails != null)
                {
                    foreach (PlayersOfMatches playersOfMatches in findMatchDetails.PlayersOfMatch ?? new List<PlayersOfMatches>())
                    {
                        var findPlayersOfMatches = AllPlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == playersOfMatches?.PlayerOfMatchId);
                        if (findPlayersOfMatches != null)
                        {
                            AllPlayersOfMatches.Remove(findPlayersOfMatches);
                        }
                        AllPlayersOfMatches.Add(playersOfMatches);
                    }
                    AllPlayersOfMatches = new ObservableCollection<PlayersOfMatches>(AllPlayersOfMatches
                        .Where(x => x.Goal > 0 || x.Assist > 0).ToList().Where(p => AllMatches.Any(m => m.MatchID == p.MatchId)));
                }
            }
            await Task.CompletedTask;
        }

        public void SetTeamLogoInMatch(Matches match)
        {
            if(match != null)
            {
                var homeTeamLogo = AllTeams?.FirstOrDefault(x => x.TeamId == match?.HomeTeamID)?.TeamLogo ?? "";
                match.HomeTeamLogo = homeTeamLogo;
                var awayTeamLogo = AllTeams?.FirstOrDefault(x => x.TeamId == match?.AwayTeamID)?.TeamLogo ?? "";
                match.AwayTeamLogo = awayTeamLogo;
            }
        }

        public async Task Refresh()
        {
            await StartupAsync();
        }
    }
}
