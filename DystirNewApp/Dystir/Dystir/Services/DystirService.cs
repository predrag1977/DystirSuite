using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Threading;
using Dystir.Models;
using Xamarin.Forms;
using System.Diagnostics;

namespace Dystir.Services
{
    public class DystirService
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        static readonly object lockUpdateData = new object();
        //static readonly SemaphoreSlim semaphoreLoadMatchDetails = new SemaphoreSlim(1, 1);

        private readonly DataLoadService _dataLoadService;
        public readonly HubConnection HubConnection;
        public ObservableCollection<MatchDetails> AllMatches;
        public ObservableCollection<Sponsor> AllSponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<MatchCompetition> AllCompetitions = new ObservableCollection<MatchCompetition>();
        public ObservableCollection<Standing> Standings;
        public ObservableCollection<CompetitionStatistic> CompetitionStatistics;

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
        public DystirService()
        {
            _dataLoadService = DependencyService.Get<DataLoadService>();

            HubConnection = new HubConnectionBuilder()
                .WithUrl("https://www.dystir.fo/DystirHub")
                //.WithAutomaticReconnect()
                .Build();

            HubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
            {
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                
                if (matchDetails?.Match != null)
                {
                    UpdateDataAsync(matchDetails);
                    MatchDetailsLoaded(matchDetails);
                }
            });
            HubConnection.On("RefreshData", () =>
            {
                _ = LoadDataAsync(true);
            });
            HubConnection.Closed += DystirHubConnection_Closed;
        }

        //**************************//
        //      PUBLIC METHODS      //
        //**************************//
        public async Task LoadDataAsync(bool loadFullData)
        {
            try
            {
                ShowLoading();

                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                }

                var loadAllMatchesTask = LoadAllMatchesAsync();
                var loadSponsorsTask = loadFullData ? LoadSponsorsAsync() : Task.CompletedTask;
                var loadCompetitionsTask = loadFullData ? LoadCompetitionsAsync() : Task.CompletedTask;
                await Task.WhenAll(
                    loadAllMatchesTask,
                    loadSponsorsTask,
                    loadCompetitionsTask
                    );

                FullDataLoaded();
            }
            catch (Exception ex)
            {
                var t = ex.Message;
                await Task.Delay(500);
                await LoadDataAsync(loadFullData);
            }
        }

        public async Task<MatchDetails> GetMatchDetailsAsync(int matchID)
        {
            MatchDetails matchDetails = null;
            try
            {
                matchDetails = await _dataLoadService.GetMatchDetailsAsync(matchID);
                UpdateDataAsync(matchDetails);
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
            }
            return matchDetails;
        }

        public async void UpdateDataAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateData)
            {
                try
                {
                    var allMatches = AllMatches.ToList();
                    allMatches.RemoveAll(x => x.Match.MatchID == matchDetails.Match.MatchID);
                    allMatches.Add(matchDetails);
                    AllMatches = new ObservableCollection<MatchDetails>(allMatches);
                    matchDetails.IsDataLoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            await Task.CompletedTask;
        }

        //**************************//
        //      PRIVATE METHODS     //
        //**************************//
        private async Task LoadAllMatchesAsync()
        {
            var matches = await _dataLoadService.GetMatchesAsync();
            var matchesWithDetails = new ObservableCollection<MatchDetails>();
            foreach (Match match in matches ?? new ObservableCollection<Match>())
            {
                var matchDetails = new MatchDetails()
                {
                    Match = match,
                    MatchDetailsID = match.MatchID,
                    IsDataLoaded = false
                };
                matchesWithDetails.Add(matchDetails);
            }
            AllMatches = new ObservableCollection<MatchDetails>(matchesWithDetails);
        }

        private async Task LoadSponsorsAsync()
        {
            AllSponsors = await _dataLoadService.GetSponsorsAsync();
        }

        private async Task LoadCompetitionsAsync()
        {
            AllCompetitions = await _dataLoadService.GetCompetitionsAsync();
        }

        private async Task DystirHubConnection_Closed(Exception ex)
        {
            await LoadDataAsync(false);
        }

        private async Task<ObservableCollection<CompetitionStatistic>> GetCompetitionStatistics()
        {
            var competitionStatisticsArray = await _dataLoadService.GetStatisticsAsync();
            CompetitionStatistics = new ObservableCollection<CompetitionStatistic>(competitionStatisticsArray);
            return CompetitionStatistics;
        }

        private ObservableCollection<MatchDetails> GetDemoData()
        {
            var demoMatches = new ObservableCollection<MatchDetails>();
            for (int i = 0; i < 8; i++)
            {
                demoMatches.Add(new MatchDetails
                {
                    Match = new Match
                    {
                        Time = DateTime.UtcNow,
                        HomeTeamFullName = "Team1" + i,
                        AwayTeamFullName = "Team2" + i,
                        MatchInfo = "location" + i,
                        MatchTypeName = "Betri",
                        MatchID = i,
                        HomeTeamScore = 10,
                        AwayTeamScore = 0,
                        StatusTime = DateTime.UtcNow,
                        StatusID = 2,
                        HomeTeamPenaltiesScore = 4,
                        AwayTeamPenaltiesScore = 5
                    },
                    MatchDetailsID = i
                });
            }

            return demoMatches;

        }
    }
}
