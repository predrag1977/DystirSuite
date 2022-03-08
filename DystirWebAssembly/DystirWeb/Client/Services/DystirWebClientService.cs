using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DystirWeb.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json;

namespace DystirWeb.Services
{
    public class DystirWebClientService
    {
        static readonly object lockUpdateData = new object();
        static SemaphoreSlim semaphoreLoadMatchDetails = new SemaphoreSlim(1, 1);

        private readonly HubConnection _hubConnection;
        private readonly HttpClient _httpClient;

        public List<Matches> AllMatches;
        public List<MatchDetails> AllMatchesDetails;
        public ObservableCollection<Teams> AllTeams;
        public ObservableCollection<Sponsors> AllSponsors;
        public ObservableCollection<Standing> Standings;
        public ObservableCollection<CompetitionStatistic> CompetitionStatistics;

        public event Action OnFullDataLoaded;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<MatchDetails> OnRefreshMatchDetails;
        public void FullDataLoaded() => OnFullDataLoaded?.Invoke();
        public void HubConnectionConnected() => OnConnected?.Invoke();
        public void HubConnectionDisconnected() => OnDisconnected?.Invoke();
        public void RefreshMatchDetails(MatchDetails matchDetails) => OnRefreshMatchDetails?.Invoke(matchDetails);

        public DystirWebClientService (HubConnection hubConnection, HttpClient httpClient)
        {
            _hubConnection = hubConnection;
            _httpClient = httpClient;
            _hubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
            {
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                UpdateDataAsync(matchDetails);
            });
            _hubConnection.On("RefreshData", () =>
            {
                _ = StartUpAsync();
            });
            _hubConnection.Closed += DystirHubConnection_Closed;
        }

        public async Task StartUpAsync()
        {
            try
            {
                _ = LoadDataAsync();
            }
            catch (Exception)
            {
                Thread.Sleep(2000);
                await StartUpAsync();
            }
        }

        public async Task LoadDataAsync()
        {
            var loadAllMatchesTask = LoadAllMatchesAsync();
            var loadAllTeamsTask = LoadAllTeamsAsync();
            var loadSponsorsTask = LoadSponsorsAsync();
            await Task.WhenAll(loadAllMatchesTask, loadAllTeamsTask, loadSponsorsTask);
            FullDataLoaded();
            _ = StartDystirHubAsync();
        }

        public async Task LoadAllMatchesAsync()
        {
            var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var matches = await _httpClient.GetFromJsonAsync<Matches[]>("api/matches");
            AllMatches = matches.Where(y => y.Time > fromDate
                    && y.MatchActivation != 1
                    && y.MatchActivation != 2).ToList();
            AllMatchesDetails = new List<MatchDetails>();
        }

        private async Task LoadAllTeamsAsync()
        {
            var teams =  await _httpClient.GetFromJsonAsync<Teams[]>("api/teams");
            AllTeams = new ObservableCollection<Teams>(teams);
        }

        private async Task LoadSponsorsAsync()
        {
            var sponsors = await _httpClient.GetFromJsonAsync<Sponsors[]>("api/sponsors");
            AllSponsors = new ObservableCollection<Sponsors>(sponsors);
        }

        public async Task StartDystirHubAsync()
        {
            try
            {
                if(_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                    await LoadAllMatchesAsync();
                    HubConnectionConnected();
                }
            }
            catch (Exception)
            {
                HubConnectionDisconnected();
                Thread.Sleep(2000);
                _ = StartDystirHubAsync();
            }
        }

        private async Task DystirHubConnection_Closed(Exception ex)
        {
            HubConnectionDisconnected();
            await StartDystirHubAsync();
        }

        public async void UpdateDataAsync(MatchDetails matchDetails)
        {
            lock (lockUpdateData)
            {
                Matches match = matchDetails?.Match;
                if (matchDetails?.Match == null) return;
                    
                AllMatches.RemoveAll(x => x.MatchID == matchDetails?.MatchDetailsID);
                AllMatches.Add(match);
                    
                AllMatchesDetails.RemoveAll(x=>x.MatchDetailsID == matchDetails?.MatchDetailsID);
                AllMatchesDetails.Add(matchDetails);

                RefreshMatchDetails(matchDetails);
            }
            await Task.CompletedTask;
        }

        public async Task<FullMatchDetailsModelView> LoadMatchDetailsAsync(int matchID)
        {
            await semaphoreLoadMatchDetails.WaitAsync();
            var fullMatchDetails = new FullMatchDetailsModelView();
            try
            {
                var matchDetails = AllMatchesDetails?.FirstOrDefault(x => x.MatchDetailsID == matchID);
                if (matchDetails == null)
                {
                    matchDetails = await _httpClient.GetFromJsonAsync<MatchDetails>("api/matchDetails/" + matchID);

                    if (matchDetails != null)
                    {
                        AllMatchesDetails.Add(matchDetails);
                    }
                }
                fullMatchDetails = GetFullMatchDetails(matchDetails);
            }
            finally
            {
                semaphoreLoadMatchDetails.Release();
            }
            return fullMatchDetails;
        }

        public FullMatchDetailsModelView GetFullMatchDetails(MatchDetails matchDetails)
        {
            var fullMatchDetails = new FullMatchDetailsModelView();
            if (matchDetails != null)
            {
                fullMatchDetails.MatchDetails = matchDetails;
                fullMatchDetails.Summary = GetSummary(matchDetails);
                fullMatchDetails.Commentary = GetCommentary(matchDetails);
                fullMatchDetails.Statistics = GetStatistics(matchDetails.EventsOfMatch, matchDetails.Match);
            }
            return fullMatchDetails;
        }

        public List<Matches> GetMatchesListSameDay(Matches match)
        {
            if(match != null)
            {
                DateTime date = match.Time.Value.Date;
                return AllMatches?.Where(x => x.Time.Value.Date == date && x.MatchID != match.MatchID && x.StatusID < 13)
                        .OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).ThenBy(x => x.MatchID).ToList();
            } 
            else
            {
                return new List<Matches>();
            }
        }

        private List<SummaryEventOfMatch> GetSummary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetSummaryEventsList(matchDetails);
            if (matchDetails.Match?.StatusID < 12)
            {
                listSummaryEvents.Reverse();
            }
            return listSummaryEvents;
        }

        internal List<SummaryEventOfMatch> GetSummaryEventsList(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> summaryEventOfMatchesList = new List<SummaryEventOfMatch>();
            Matches selectedMatch = matchDetails.Match;
            var homeTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.HomeTeam.Trim());
            var awayTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.AwayTeam.Trim());
            var eventsList = matchDetails.EventsOfMatch?.Where(x =>
                 x.EventName == "GOAL"
                 || x.EventName == "OWNGOAL"
                 || x.EventName == "PENALTYSCORED"
                 || x.EventName == "PENALTYMISSED"
                 || x.EventName == "YELLOW"
                 || x.EventName == "RED"
                 || x.EventName == "BIGCHANCE"
                 || x.EventName == "PLAYEROFTHEMATCH"
                 || x.EventName == "ASSIST").ToList();
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventsOfMatches>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch()
                {
                    EventOfMatch = eventOfMatch,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0,
                    HomeTeam = eventOfMatch?.EventTeam == selectedMatch.HomeTeam ? selectedMatch.HomeTeam : string.Empty,
                    AwayTeam = eventOfMatch?.EventTeam == selectedMatch.AwayTeam ? selectedMatch.AwayTeam : string.Empty,
                    EventName = eventOfMatch?.EventName,
                    EventMinute = eventOfMatch?.EventMinute,

                };
                PlayersOfMatches mainPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == eventOfMatch.MainPlayerOfMatchId);
                string mainPlayerFullName = (mainPlayerOfMatch?.FirstName?.Trim() + " " + mainPlayerOfMatch?.Lastname?.Trim())?.Trim();
                PlayersOfMatches secondPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == eventOfMatch.SecondPlayerOfMatchId);
                string secondPlayerFullName = (secondPlayerOfMatch?.FirstName?.Trim() + " " + secondPlayerOfMatch?.Lastname?.Trim())?.Trim();
                if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                {
                    summaryEventOfMatch.HomeMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.HomeSecondPlayer = secondPlayerFullName;
                }
                else
                {
                    summaryEventOfMatch.AwayMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.AwaySecondPlayer = secondPlayerFullName;
                }
                summaryEventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeTeam);
                summaryEventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwayTeam);
                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count)
                    {
                        EventsOfMatches nextEvent = eventsList[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            PlayersOfMatches assistPlayerOfMatch = matchDetails.PlayersOfMatch.Find(x => x.PlayerOfMatchId == nextEvent.MainPlayerOfMatchId);
                            string assistPlayerFullName = (assistPlayerOfMatch?.FirstName?.Trim() + " " + assistPlayerOfMatch?.Lastname?.Trim())?.Trim();
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
                summaryEventOfMatchesList.Add(summaryEventOfMatch);
            }

            return summaryEventOfMatchesList.ToList();
        }

        bool IsGoal(EventsOfMatches matchEvent)
        {
            return matchEvent.EventName == "GOAL"
                || matchEvent.EventName == "OWNGOAL"
                || matchEvent.EventName == "PENALTYSCORED";
        }

        private List<SummaryEventOfMatch> GetCommentary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> summaryEventOfMatchesList = new List<SummaryEventOfMatch>();
            var eventsList = matchDetails.EventsOfMatch?.Where(x => x != null).ToList();
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventsOfMatches>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch()
                {
                    EventOfMatch = eventOfMatch,
                    HomeTeam = eventOfMatch.EventTeam == matchDetails.Match.HomeTeam ? eventOfMatch.EventTeam : null,
                    AwayTeam = eventOfMatch.EventTeam == matchDetails.Match.AwayTeam ? eventOfMatch.EventTeam : null,
                    EventMinute = eventOfMatch.EventMinute,
                    EventName = eventOfMatch.EventName,
                    HomeTeamScore = 0,
                    AwayTeamScore = 0
                };
                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == matchDetails.Match.HomeTeam.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == matchDetails.Match.AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;
                summaryEventOfMatchesList.Add(summaryEventOfMatch);
            }
            summaryEventOfMatchesList.Reverse();
            return summaryEventOfMatchesList;
        }

        internal Statistic GetStatistics(List<EventsOfMatches> eventsDataList, Matches match)
        {
            Statistic statistic = new Statistic();
            foreach (EventsOfMatches matchEvent in eventsDataList)
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
                statistic.HomeTeamStatistic.GoalProcent = statistic.HomeTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
                statistic.AwayTeamStatistic.GoalProcent = statistic.AwayTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
            }
            if (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard != 0)
            {
                statistic.HomeTeamStatistic.YellowCardProcent = statistic.HomeTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
                statistic.AwayTeamStatistic.YellowCardProcent = statistic.AwayTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
            }
            if (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard != 0)
            {
                statistic.HomeTeamStatistic.RedCardProcent = statistic.HomeTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
                statistic.AwayTeamStatistic.RedCardProcent = statistic.AwayTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
            }
            if (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner != 0)
            {
                statistic.HomeTeamStatistic.CornerProcent = statistic.HomeTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
                statistic.AwayTeamStatistic.CornerProcent = statistic.AwayTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
            }
            if (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget != 0)
            {
                statistic.HomeTeamStatistic.OnTargetProcent = statistic.HomeTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
                statistic.AwayTeamStatistic.OnTargetProcent = statistic.AwayTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
            }
            if (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget != 0)
            {
                statistic.HomeTeamStatistic.OffTargetProcent = statistic.HomeTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
                statistic.AwayTeamStatistic.OffTargetProcent = statistic.AwayTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
            }
            if (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot != 0)
            {
                statistic.HomeTeamStatistic.BlockedShotProcent = statistic.HomeTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
                statistic.AwayTeamStatistic.BlockedShotProcent = statistic.AwayTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
            }
            if (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance != 0)
            {
                statistic.HomeTeamStatistic.BigChanceProcent = statistic.HomeTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
                statistic.AwayTeamStatistic.BigChanceProcent = statistic.AwayTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
            }

            return statistic;
        }

        private void AddTeamStatistic(EventsOfMatches matchEvent, TeamStatistic teamStatistic)
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

        internal async Task<ObservableCollection<Standing>> GetStandings()
        {
            var standingsArray = await _httpClient.GetFromJsonAsync<Standing[]>("api/standings");
            Standings = new ObservableCollection<Standing>(standingsArray);
            return Standings;
        }
        
        internal async Task<ObservableCollection<CompetitionStatistic>> GetCompetitionStatistics()
        {
            var competitionStatisticsArray = await _httpClient.GetFromJsonAsync<CompetitionStatistic[]>("api/Statistics");
            CompetitionStatistics = new ObservableCollection<CompetitionStatistic>(competitionStatisticsArray);
            return CompetitionStatistics;
        }
    }
}
