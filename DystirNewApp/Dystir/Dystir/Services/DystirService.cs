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
using Xamarin.Forms;
using System.Diagnostics;
using Dystir.Pages;

namespace Dystir.Services
{
    public class DystirService
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        static readonly object lockUpdateData = new object();
        static readonly SemaphoreSlim semaphoreLoadMatchDetails = new SemaphoreSlim(1, 1);

        private readonly HubConnection _hubConnection;
        private readonly DataLoadService _dataLoadService;
        public ObservableCollection<MatchDetails> AllMatches = new ObservableCollection<MatchDetails>();
        public ObservableCollection<Sponsor> AllSponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<MatchCompetition> AllCompetitions = new ObservableCollection<MatchCompetition>();
        public ObservableCollection<Standing> Standings;
        public ObservableCollection<CompetitionStatistic> CompetitionStatistics;

        public object AllMatchesDetailsPages { get; internal set; }

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

            _hubConnection = new HubConnectionBuilder().WithUrl("https://www.dystir.fo/DystirHub").Build();

            _hubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
            {
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                
                if (matchDetails?.Match != null)
                {
                    UpdateDataAsync(matchDetails);
                    MatchDetailsLoaded(matchDetails);
                }
            });
            _hubConnection.On("RefreshData", () =>
            {
                _ = LoadDataAsync(true);
            });
            _hubConnection.Closed += DystirHubConnection_Closed;
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
                var loadCompetitionsTask = loadFullData ? LoadCompetitionsAsync() : Task.CompletedTask;
                await Task.WhenAll(loadAllMatchesTask, loadSponsorsTask, loadCompetitionsTask);
                FullDataLoaded();
                _ = StartDystirHubAsync();
            }
            catch (Exception ex)
            {
                var t = ex.Message;
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

        private async Task StartDystirHubAsync()
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
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
                demoMatches.Add(new Match
                {
                    Time = DateTime.UtcNow,
                    HomeTeam = "Team1" + i,
                    AwayTeam = "Team2" + i,
                    Location = "location" + i,
                    MatchTypeName = "Betri"
                });
            }

            return demoMatches;

        }
    }
}
