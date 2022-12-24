using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using Dystir.Models;
using Dystir.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dystir.Services
{
    public class DystirService
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        static readonly object lockUpdateData = new object();
        static readonly SemaphoreSlim semaphoreLoadMatchDetails = new SemaphoreSlim(1, 1);

        private readonly HubConnection hubConnection;
        private readonly DataLoadService _dataLoadService;

        public ObservableCollection<Match> AllMatches = new ObservableCollection<Match>();
        public ObservableCollection<Sponsor> AllSponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Standing> Standings;
        public ObservableCollection<CompetitionStatistic> CompetitionStatistics;
        public ObservableCollection<MatchDetails> AllMatchesDetails = new ObservableCollection<MatchDetails>();
        public ObservableCollection<MatchDetailsViewModel> AllMatchesDetailViewModels = new ObservableCollection<MatchDetailsViewModel>();

        //**************************//
        //          EVENTS          //
        //**************************//
        public event Action OnShowLoading;
        public void ShowLoading() => OnShowLoading?.Invoke();

        public event Action OnDisconnected;
        public void HubConnectionDisconnected() => OnDisconnected?.Invoke();

        public event Action OnFullDataLoaded;
        public void FullDataLoaded() => OnFullDataLoaded?.Invoke();

        public event Action<MatchDetails> OnMatchDetailsLoaded;
        public void MatchDetailsLoaded(MatchDetails matchDetails) => OnMatchDetailsLoaded?.Invoke(matchDetails);

        //**************************//
        //        CONSTRUCTOR       //
        //**************************//
        public DystirService(DataLoadService dataLoader)
        {
            _dataLoadService = dataLoader;

            hubConnection = new HubConnectionBuilder().WithUrl("https://www.dystir.fo/DystirHub").Build();

            hubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
            {
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                UpdateDataAsync(matchDetails);
            });
            hubConnection.On("RefreshData", () =>
            {
                _ = LoadDataAsync(true);
            });
            hubConnection.Closed += DystirHubConnection_Closed;
        }

        //**************************//
        //      PUBLIC METHODS      //
        //**************************//
        public async Task LoadDataAsync(bool loadFullData)
        {
            try
            {
                if(loadFullData)
                {
                    ShowLoading();
                }
                var loadAllMatchesTask = LoadAllMatchesAsync();
                var loadSponsorsTask = loadFullData ? LoadSponsorsAsync() : Task.CompletedTask;
                await Task.WhenAll(loadAllMatchesTask, loadSponsorsTask);
                FullDataLoaded();
                _ = StartDystirHubAsync();
            }
            catch (Exception)
            {
                await Task.Delay(500);
                _ = LoadDataAsync(loadFullData);
            }
            await Task.CompletedTask;
        }

        public async Task<MatchDetails> GetMatchDetailsAsync(int matchID)
        {
            MatchDetails matchDetails = null;
            try
            {
                matchDetails = await _dataLoadService.GetMatchDetailsAsync(matchID);
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
            }
            return matchDetails;
        }

        //**************************//
        //      PRIVATE METHODS     //
        //**************************//
        private async Task LoadAllMatchesAsync()
        {
            var matches = await _dataLoadService.GetMatchesAsync();
            AllMatches = new ObservableCollection<Match>(matches ?? new ObservableCollection<Match>());
        }

        private async Task LoadSponsorsAsync()
        {
            AllSponsors = await _dataLoadService.GetSponsorsAsync();
        }

        private async Task StartDystirHubAsync()
        {
            try
            {
                if (hubConnection.State == HubConnectionState.Disconnected)
                {
                    await hubConnection.StartAsync();
                    await LoadDataAsync(false);
                }
            }
            catch (Exception)
            {
                await Task.Delay(500);
                _ = StartDystirHubAsync();
            }
            await Task.CompletedTask;
        }

        private async Task DystirHubConnection_Closed(Exception ex)
        {
            ShowLoading();
            _ = StartDystirHubAsync();
            await Task.CompletedTask;
        }

        private async void UpdateDataAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateData)
            {
                try
                {
                    if (matchDetails?.Match != null)
                    {
                        Match match = matchDetails?.Match;

                        var allMatches = AllMatches.ToList();
                        allMatches.RemoveAll(x => x.MatchID == match.MatchID);
                        allMatches.Add(match);
                        AllMatches = new ObservableCollection<Match>(allMatches);

                        MatchDetailsLoaded(matchDetails);
                    }
                }
                catch(Exception ex)
                {
                    var exception = ex;
                }
                
            }
            await Task.CompletedTask;
        }
        
        private async Task<ObservableCollection<Standing>> GetStandings()
        {
            var standingsArray = await _dataLoadService.GetStandingsAsync();
            Standings = new ObservableCollection<Standing>(standingsArray);
            return Standings;
        }

        private async Task<ObservableCollection<CompetitionStatistic>> GetCompetitionStatistics()
        {
            var competitionStatisticsArray = await _dataLoadService.GetStatisticsAsync();
            CompetitionStatistics = new ObservableCollection<CompetitionStatistic>(competitionStatisticsArray);
            return CompetitionStatistics;
        }

        private ObservableCollection<Match> GetDemoData()
        {
            var demoMatches = new ObservableCollection<Match>();
            for (int i = 0; i < 8; i++)
            {
                //demoMatches.Add(new Match
                //{
                //    Time = DateTime.UtcNow,
                //    HomeTeam = "Team1" + i,
                //    AwayTeam = "Team2" + i,
                //    Location = "location" + i,
                //    MatchTypeName = "Betri"
                //});
            }

            return demoMatches;

        }
    }
}
