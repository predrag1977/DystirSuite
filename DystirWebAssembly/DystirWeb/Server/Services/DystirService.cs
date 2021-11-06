using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DystirWeb.Shared;
using DystirWeb.Server.DystirDB;
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
        public ObservableCollection<Matches> AllMatches;
        public ObservableCollection<MatchDetails> AllMatchesDetails;
        public ObservableCollection<Teams> AllTeams;
        public ObservableCollection<Sponsors> AllSponsors;
        public ObservableCollection<PlayersOfMatches> AllPlayersOfMatches;

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
            catch (Exception)
            {
                Thread.Sleep(1000);
                await StartupAsync();
            }
        }

        public async Task LoadDataAsync()
        {
            DystirDBContext = new DystirDBContext(_dbContextOptions);
            AllMatchesDetails = new ObservableCollection<MatchDetails>();
            var allMatchesTask = GetAllMatchesAsync();
            var allTeamsTask = GetAllTeamsAsync();
            var allSponsorsTask = GetAllSponsorsAsync();
            await Task.WhenAll(allMatchesTask, allTeamsTask, allSponsorsTask);
        }

        public async Task GetAllMatchesAsync()
        {
            var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            AllMatches = new ObservableCollection<Matches>(DystirDBContext.Matches
                .Where(y => y.Time > fromDate
                && y.MatchActivation != 1
                && y.MatchActivation != 2));
            await GetPlayersOfMatchesAsync();
        }

        private async Task GetAllTeamsAsync()
        {
            AllTeams = new ObservableCollection<Teams>(DystirDBContext.Teams);
            await Task.CompletedTask;
        }

        private async Task GetAllSponsorsAsync()
        {
            AllSponsors = new ObservableCollection<Sponsors>(DystirDBContext.Sponsors);
            await Task.CompletedTask;
        }

        private async Task GetPlayersOfMatchesAsync()
        {
            AllPlayersOfMatches = new ObservableCollection<PlayersOfMatches>(DystirDBContext.PlayersOfMatches?
                .Where(x => x.Goal > 0 || x.Assist > 0).ToList().Where(p => AllMatches.Any(m => m.MatchID == p.MatchId)));
            await Task.CompletedTask;
        }

        public async void UpdateDataAsync(MatchDetails matchDetails)
        {
            Matches match = matchDetails?.Match;
            if (matchDetails?.Match == null) return;

            var updateAllMatchesTask = UpdateAllMatchesAsync(match);
            var updateAllMatchesDetailsTask = UpdateAllMatchesDetailsAsync(matchDetails);
            var updateAllPlayersOfMatchesTask = UpdateAllPlayersOfMatchesAsync(matchDetails);
            await Task.WhenAll(updateAllMatchesTask, updateAllMatchesDetailsTask, updateAllPlayersOfMatchesTask);
        }

        public async Task UpdateAllMatchesAsync(Matches match)
        {
            lock (lockUpdateAllMatches)
            {
                var findMatch = AllMatches.FirstOrDefault(x => x.MatchID == match.MatchID);
                if (findMatch != null)
                {
                    AllMatches.Remove(findMatch);
                }
                AllMatches.Add(match);
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
                    foreach (PlayersOfMatches playersOfMatches in findMatchDetails.PlayersOfMatch ?? new List<PlayersOfMatches>())
                    {
                        var findPlayersOfMatches = AllPlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == playersOfMatches?.PlayerOfMatchId);
                        if (findPlayersOfMatches != null)
                        {
                            AllPlayersOfMatches.Remove(findPlayersOfMatches);
                        }
                        AllPlayersOfMatches.Add(playersOfMatches);
                    }
                    AllPlayersOfMatches = new ObservableCollection<PlayersOfMatches>(AllPlayersOfMatches?
                        .Where(x => x.Goal > 0 || x.Assist > 0).ToList().Where(p => AllMatches.Any(m => m.MatchID == p.MatchId)));
                }
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
                    AllMatchesDetails.Remove(findMatchDetails);
                }
                AllMatchesDetails.Add(matchDetails);
            }
            await Task.CompletedTask;
        }

        public async Task Refresh()
        {
            await StartupAsync();
        }
    }
}
