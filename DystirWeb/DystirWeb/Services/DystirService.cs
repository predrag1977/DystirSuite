using DystirWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using DystirWeb.Server.Hubs;

namespace DystirWeb.Services
{
    public class DystirService
    {
        private DystirDBContext _dystirDBContext;
        private StandingService _standingService;
        private DbContextOptions<DystirDBContext> _dbContextOptions;
        private StatisticCompetitionsService _statisticCompetitionsService;
        private HubConnection _hubConnection;
        private IHubContext<DystirHub> _hubContext;
        public ObservableCollection<Matches> AllMatches;
        public ObservableCollection<MatchDetails> AllMatchesDetails;
        public ObservableCollection<Teams> AllTeams;
        public ObservableCollection<Sponsors> Sponsors;
        public ObservableCollection<HandballMatches> AllHandballMatches;
        public ObservableCollection<Standing> HandballStandings;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public void HubConnectionConnected() => OnConnected?.Invoke();
        public void HubConnectionDisconnected() => OnDisconnected?.Invoke();

        public DystirService(
            DbContextOptions<DystirDBContext> dbContextOptions,
            StandingService standingService,
            StatisticCompetitionsService statisticCompetitionsService,
            HubConnection hubConnection, 
            IHubContext<DystirHub> hubContext)
        {
            _dbContextOptions = dbContextOptions;
            _dystirDBContext = new DystirDBContext(_dbContextOptions);
            _standingService = standingService;
            _statisticCompetitionsService = statisticCompetitionsService;
            _hubConnection = hubConnection;
            _hubContext = hubContext;
            AllMatchesDetails = new ObservableCollection<MatchDetails>();
            _ = StartupAsync();
            _hubConnection.On<string, string>("ReceiveMatchDetails", (matchID, matchDetailsJson) =>
            {
                MatchDetails matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                UpdateMatchDetails(matchDetails?.Match?.MatchID.ToString(), matchDetails);
                _dystirDBContext = new DystirDBContext(_dbContextOptions);
                var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                AllMatches = new ObservableCollection<Matches>(_dystirDBContext.Matches
                    .Where(y => y.Time > fromDate
                    && y.MatchActivation != 1
                    && y.MatchActivation != 2));
                new HubSender().SendUpdateCommand(_hubContext, matchID);
            });
            _hubConnection.Closed += DystirHubConnection_Closed;
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
            _dystirDBContext = new DystirDBContext(_dbContextOptions);
            var startDystirHubTask = StartDystirHub();
            var loadAllMatchesTask = LoadAllMatchesAsync();
            var loadAllTeamsTask = LoadAllTeamsAsync();
            var loadSponsorsTask = LoadSponsorsAsync();
            await Task.WhenAll(startDystirHubTask, loadAllMatchesTask, loadAllTeamsTask, loadSponsorsTask);
            if (!startDystirHubTask.Result)
            {
                Thread.Sleep(1000);
                await LoadDataAsync();
            }
            HubConnectionConnected();
        }

        public async Task LoadAllMatchesAsync()
        {
            var fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
            AllMatches = new ObservableCollection<Matches>(await Task.FromResult(_dystirDBContext.Matches
                .Where(y => y.MatchActivation != 1
                && y.MatchActivation != 2
                && y.Time > fromDate)));
        }

        private async Task LoadAllTeamsAsync()
        {
            AllTeams = new ObservableCollection<Teams>(await Task.FromResult(_dystirDBContext.Teams));
        }

        private async Task LoadSponsorsAsync()
        {
            Sponsors = new ObservableCollection<Sponsors>(await Task.FromResult(_dystirDBContext.Sponsors));
        }

        public async Task<bool> StartDystirHub()
        {
            try
            {
                if(_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task DystirHubConnection_Closed(Exception ex)
        {
            HubConnectionDisconnected();
            Thread.Sleep(1000);
            await StartupAsync();
        }

        private void UpdateMatchDetails(string matchID, MatchDetails matchDetailsJson)
        {
            var matchDetails = AllMatchesDetails.FirstOrDefault(x => x.Match?.MatchID.ToString() == matchID);
            if(matchDetails != null)
            {
                AllMatchesDetails.Remove(matchDetails);
            }
            AllMatchesDetails.Add(matchDetailsJson);
        }

        public async Task<FullMatchDetailsModelView> LoadMatchDetailsAsync(string matchID)
        {
            var fullMatchDetails = new FullMatchDetailsModelView();
            try
            {
                Matches match = AllMatches?.FirstOrDefault(x => x.MatchID.ToString() == matchID);
                var matchDetails = AllMatchesDetails.FirstOrDefault(x => x.Match?.MatchID.ToString() == matchID);
                if (matchDetails == null)
                {
                    var eventsOfMatchTask = GetEventsOfMatchAsync(matchID);
                    var playersOfMatchTask = GetPlayersOfMatch(matchID);
                    await Task.WhenAll(eventsOfMatchTask, playersOfMatchTask);
                    var eventsOfMatch = eventsOfMatchTask.Result;
                    var playersOfMatch = playersOfMatchTask.Result;
                    matchDetails = new MatchDetails()
                    {
                        Match = match,
                        EventsOfMatch = eventsOfMatch?
                        .OrderBy(x => x.EventPeriodId ?? 0)
                        .ThenBy(x => x.EventTotalTime)
                        .ThenBy(x => x.EventMinute)
                        .ThenBy(x => x.EventOfMatchId).ToList(),
                        PlayersOfMatch = playersOfMatch.Where(x => x.PlayingStatus != 3).ToList()
                    };
                    UpdateMatchDetails(matchID, matchDetails);
                }
                fullMatchDetails = GetFullMatchDetails(matchDetails);
                fullMatchDetails.MatchesListSelection = GetMatchesListSameDay(match);
            }
            catch (Exception)
            {

            }
            return fullMatchDetails;
        }

        public FullMatchDetailsModelView GetFullMatchDetails(MatchDetails matchDetails)
        {
            var fullMatchDetails = new FullMatchDetailsModelView();
            fullMatchDetails.MatchDetails = matchDetails;
            fullMatchDetails.Summary = GetSummary(matchDetails);
            fullMatchDetails.Commentary = GetCommentary(matchDetails);
            fullMatchDetails.Statistics = GetStatistics(matchDetails.EventsOfMatch, matchDetails.Match);
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

        private Task<List<EventsOfMatches>> GetEventsOfMatchAsync(string matchID)
        {
            var eventsList = _dystirDBContext.EventsOfMatches?.Where(x => x.MatchId.ToString() == matchID);
            var sortedEventList = eventsList?
                .OrderByDescending(x => x.EventPeriodId ?? 0)
                .ThenByDescending(x => x.EventTotalTime)
                .ThenByDescending(x => x.EventMinute)
                .ThenByDescending(x => x.EventOfMatchId);
            return Task.FromResult(sortedEventList?.ToList());
        }

        private Task<List<PlayersOfMatches>> GetPlayersOfMatch(string matchID)
        {
            var playersOfMatchList = _dystirDBContext.PlayersOfMatches?.Where(x => x.MatchId.ToString() == matchID);
            var sortedPlayersList = playersOfMatchList?
                .OrderBy(x => x.PlayingStatus == 3)
                .ThenBy(x => x.PlayingStatus == 0)
                .ThenBy(x => x.PlayingStatus == 2)
                .ThenBy(x => x.PlayingStatus == 1)
                .ThenByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number == null)
                .ThenBy(x => x.Number)
                .ThenBy(x => x.Position == null)
                .ThenBy(x => x.Position == "ATT")
                .ThenBy(x => x.Position == "MID")
                .ThenBy(x => x.Position == "DEF")
                .ThenBy(x => x.Position == "GK")
                .ThenBy(x => x.FirstName)
                .ThenBy(x => x.Lastname);
            return Task.FromResult(sortedPlayersList?.ToList());
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
                 //|| x.EventName == "SUBSTITUTION"
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

        internal IEnumerable<Standing> GetStandings()
        {
            return _standingService.GetStandings(AllTeams, AllMatches);
        }

        internal ObservableCollection<CompetitionStatistic> GetCompetitionStatistics()
        {
            return new ObservableCollection<CompetitionStatistic>(_statisticCompetitionsService.GetCompetitionsStatistic(AllMatches, _dystirDBContext));
        }
    }
}
