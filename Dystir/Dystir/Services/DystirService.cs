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
        public ObservableCollection<MatchDetails> AllMatchesDetails = new ObservableCollection<MatchDetails>();
        public ObservableCollection<Sponsor> AllSponsors = new ObservableCollection<Sponsor>();
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

        public event Action<Match> OnMatchDetailsLoaded;
        public void MatchDetailsLoaded(Match match) => OnMatchDetailsLoaded?.Invoke(match);

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

        public async Task LoadMatchDetailsAsync(Match selectedMatch)
        {
            try
            {
                var matchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == selectedMatch.MatchID);
                if (matchDetails == null)
                {
                    matchDetails = await _dataLoadService.GetMatchDetailsAsync(selectedMatch.MatchID);
                    matchDetails = GetFullMatchDetails(matchDetails);
                    AllMatchesDetails.Add(matchDetails);
                }
                selectedMatch.MatchDetails = matchDetails;
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
            }
            await Task.CompletedTask;
        }

        public async Task RefreshSelectedMatchAsync(Match selectedMatch)
        {
            try
            {
                selectedMatch.IsLoading = true;
                var matchDetails = await _dataLoadService.GetMatchDetailsAsync(selectedMatch.MatchID);
                selectedMatch.IsLoading = false;
                UpdateDataAsync(matchDetails);
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
            await Task.CompletedTask;
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
                        match.MatchDetails = GetFullMatchDetails(matchDetails);

                        var allMatches = AllMatches.ToList();
                        allMatches.RemoveAll(x => x.MatchID == match.MatchID);
                        allMatches.Add(match);
                        AllMatches = new ObservableCollection<Match>(allMatches);

                        var findMatchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == match.MatchID);
                        if (findMatchDetails != null)
                        {
                            AllMatchesDetails.Remove(findMatchDetails);
                        }
                        AllMatchesDetails.Add(match.MatchDetails);

                        MatchDetailsLoaded(match);
                    }
                }
                catch(Exception ex)
                {
                    var exception = ex;
                }
                
            }
            await Task.CompletedTask;
        }

        private MatchDetails GetFullMatchDetails(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                SetLineups(matchDetails);
                matchDetails.Summary = GetSummary(matchDetails);
                matchDetails.Commentary = GetCommentary(matchDetails);
                matchDetails.Statistics = GetStatistics(matchDetails.EventsOfMatch, matchDetails.Match);
            }
            return matchDetails;
        }

        private static void SetLineups(MatchDetails matchDetails)
        {
            var awayTeam = matchDetails?.Match?.AwayTeam;
            var lineUps = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 1 || x.PlayingStatus == 2).OrderBy(x=>x.Number);
            var homeLineUps = lineUps.Where(x => x.TeamName == matchDetails?.Match?.HomeTeam);
            var awayLineUps = lineUps.Where(x => x.TeamName == matchDetails?.Match?.AwayTeam);
            matchDetails.HomeTeamLineups = new ObservableCollection<PlayerOfMatch>(homeLineUps);
            matchDetails.AwayTeamLineups = new ObservableCollection<PlayerOfMatch>(awayLineUps);
        }

        private static ObservableCollection<SummaryEventOfMatch> GetSummary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetEventOfMatchList(matchDetails, true);
            if (matchDetails.Match?.StatusID < 12)
            {
                listSummaryEvents.Reverse();
            }
            return new ObservableCollection<SummaryEventOfMatch>(listSummaryEvents);
        }
        
        private static ObservableCollection<SummaryEventOfMatch> GetCommentary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listCommentaryEvents = GetEventOfMatchList(matchDetails, false);
            listCommentaryEvents.Reverse();
            return new ObservableCollection<SummaryEventOfMatch>(listCommentaryEvents);
        }

        private static List<SummaryEventOfMatch> GetEventOfMatchList(MatchDetails matchDetails, bool isSummaryList)
        {
            List<SummaryEventOfMatch> eventOfMatchesList = new List<SummaryEventOfMatch>();
            Match selectedMatch = matchDetails.Match;
            var homeTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.HomeTeam.Trim());
            var awayTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.AwayTeam.Trim());
            var eventsList = isSummaryList ? matchDetails.EventsOfMatch?
                .Where(x => x.EventName == "GOAL"
                || x.EventName == "OWNGOAL"
                || x.EventName == "PENALTYSCORED"
                || x.EventName == "PENALTYMISSED"
                || x.EventName == "YELLOW"
                || x.EventName == "RED"
                || x.EventName == "SUBSTITUTION"
                || x.EventName == "PLAYEROFTHEMATCH"
                || x.EventName == "ASSIST").ToList() : matchDetails.EventsOfMatch.ToList();

            int homeScore = 0;
            int awayScore = 0;
            int homeTeamPenaltiesScore = 0;
            int awayTeamPenaltiesScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventOfMatch>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch(eventOfMatch, selectedMatch); ;
                PlayerOfMatch mainPlayerOfMatch = matchDetails.PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.MainPlayerOfMatchID);
                string mainPlayerFullName = (mainPlayerOfMatch?.FirstName?.Trim() + " " + mainPlayerOfMatch?.LastName?.Trim())?.Trim();
                PlayerOfMatch secondPlayerOfMatch = matchDetails.PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.SecondPlayerOfMatchID);
                string secondPlayerFullName = (secondPlayerOfMatch?.FirstName?.Trim() + " " + secondPlayerOfMatch?.LastName?.Trim())?.Trim();
                if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                {
                    summaryEventOfMatch.IsHomeTeamEvent = true;
                    summaryEventOfMatch.HomeMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.HomeSecondPlayer = secondPlayerFullName;
                }
                else
                {
                    summaryEventOfMatch.IsAwayTeamEvent = true;
                    summaryEventOfMatch.AwayMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.AwaySecondPlayer = secondPlayerFullName;
                }
                summaryEventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeTeam);
                summaryEventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwayTeam);
                summaryEventOfMatch.IsGoal = eventOfMatch.EventName == "GOAL"
                    || eventOfMatch.EventName == "OWNGOAL"
                    || eventOfMatch.EventName == "PENALTYSCORED";
                if (summaryEventOfMatch.IsGoal)
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                    {
                        if (eventOfMatch.EventPeriodID != 10)
                        {
                            homeScore += 1;
                        }
                        else
                        {
                            homeTeamPenaltiesScore += 1;
                        }
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                    {
                        if (eventOfMatch.EventPeriodID != 10)
                        {
                            awayScore += 1;
                        }
                        else
                        {
                            awayTeamPenaltiesScore += 1;
                        }
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;
                summaryEventOfMatch.HomeTeamPenaltiesScore = homeTeamPenaltiesScore;
                summaryEventOfMatch.AwayTeamPenaltiesScore = awayTeamPenaltiesScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count)
                    {
                        EventOfMatch nextEvent = eventsList[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            PlayerOfMatch assistPlayerOfMatch = matchDetails.PlayersOfMatch?.First(x => x.PlayerOfMatchID == nextEvent.MainPlayerOfMatchID);
                            string assistPlayerFullName = (assistPlayerOfMatch?.FirstName?.Trim() + " " + assistPlayerOfMatch?.LastName?.Trim())?.Trim();
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.HomeSecondPlayer = assistPlayerFullName;
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.AwaySecondPlayer = assistPlayerFullName;
                            }

                        }
                    }
                }
                else if (eventOfMatch.EventName == "ASSIST")
                {
                    continue;
                }
                else if (eventOfMatch.EventName == "PLAYEROFTHEMATCH")
                {
                    summaryEventOfMatch.ShowMinutes = false;
                }
                else if (eventOfMatch.EventName == "SUBSTITUTION")
                {
                    var homeMainPlayer = summaryEventOfMatch.HomeMainPlayer;
                    var awayMainPlayer = summaryEventOfMatch.AwayMainPlayer;
                    summaryEventOfMatch.HomeMainPlayer = summaryEventOfMatch.HomeSecondPlayer;
                    summaryEventOfMatch.AwayMainPlayer = summaryEventOfMatch.AwaySecondPlayer;
                    summaryEventOfMatch.HomeSecondPlayer = homeMainPlayer;
                    summaryEventOfMatch.AwaySecondPlayer = awayMainPlayer;
                }
                eventOfMatchesList.Add(summaryEventOfMatch);
            }

            return eventOfMatchesList.ToList();
        }

        private Statistic GetStatistics(ObservableCollection<EventOfMatch> eventsDataList, Match match)
        {
            Statistic statistic = new Statistic();
            foreach (EventOfMatch matchEvent in eventsDataList)
            {
                if (matchEvent?.EventTeam?.ToLower().Trim() == match?.HomeTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.HomeTeamStatistic);
                }
                else if (matchEvent?.EventTeam?.ToLower().Trim() == match?.AwayTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.AwayTeamStatistic);
                }
            }

            if (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal != 0)
            {
                statistic.HomeTeamStatistic.GoalProcent = statistic.HomeTeamStatistic.Goal / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
                statistic.AwayTeamStatistic.GoalProcent = statistic.AwayTeamStatistic.Goal / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
            }
            if (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard != 0)
            {
                statistic.HomeTeamStatistic.YellowCardProcent = statistic.HomeTeamStatistic.YellowCard / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
                statistic.AwayTeamStatistic.YellowCardProcent = statistic.AwayTeamStatistic.YellowCard / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
            }
            if (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard != 0)
            {
                statistic.HomeTeamStatistic.RedCardProcent = statistic.HomeTeamStatistic.RedCard / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
                statistic.AwayTeamStatistic.RedCardProcent = statistic.AwayTeamStatistic.RedCard / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
            }
            if (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner != 0)
            {
                statistic.HomeTeamStatistic.CornerProcent = statistic.HomeTeamStatistic.Corner / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
                statistic.AwayTeamStatistic.CornerProcent = statistic.AwayTeamStatistic.Corner / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
            }
            if (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget != 0)
            {
                statistic.HomeTeamStatistic.OnTargetProcent = statistic.HomeTeamStatistic.OnTarget / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
                statistic.AwayTeamStatistic.OnTargetProcent = statistic.AwayTeamStatistic.OnTarget / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
            }
            if (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget != 0)
            {
                statistic.HomeTeamStatistic.OffTargetProcent = statistic.HomeTeamStatistic.OffTarget / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
                statistic.AwayTeamStatistic.OffTargetProcent = statistic.AwayTeamStatistic.OffTarget / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
            }
            if (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot != 0)
            {
                statistic.HomeTeamStatistic.BlockedShotProcent = statistic.HomeTeamStatistic.BlockedShot / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
                statistic.AwayTeamStatistic.BlockedShotProcent = statistic.AwayTeamStatistic.BlockedShot / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
            }
            if (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance != 0)
            {
                statistic.HomeTeamStatistic.BigChanceProcent = statistic.HomeTeamStatistic.BigChance / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
                statistic.AwayTeamStatistic.BigChanceProcent = statistic.AwayTeamStatistic.BigChance / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
            }

            return statistic;
        }

        private void AddTeamStatistic(EventOfMatch matchEvent, TeamStatistic teamStatistic)
        {
            switch (matchEvent?.EventName?.ToLower())
            {
                case "goal":
                case "penaltyscored":
                    teamStatistic.Goal += 1;
                    teamStatistic.OnTarget += 1;
                    break;
                case "owngoal":
                    teamStatistic.Goal += 1;
                    break;
                case "yellow":
                    teamStatistic.YellowCard += 1;
                    break;
                case "red":
                    teamStatistic.RedCard += 1;
                    break;
                case "corner":
                    teamStatistic.Corner += 1;
                    break;
                case "ontarget":
                    teamStatistic.OnTarget += 1;
                    break;
                case "offtarget":
                    teamStatistic.OffTarget += 1;
                    break;
                case "blockedshot":
                    teamStatistic.BlockedShot += 1;
                    break;
                case "bigchance":
                    teamStatistic.BigChance += 1;
                    break;
            }
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
