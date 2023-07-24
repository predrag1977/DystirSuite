using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dystir.Services;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Dystir.Models
{

    public class MatchDetails
    {
        //*****************************//
        //         PROPERTIES          //
        //*****************************//
        public int MatchDetailsID { get; set; }

        public bool IsDataLoaded = false;

        public Match Match { get; set; }

        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; }

        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; }

        public ObservableCollection<SummaryEventOfMatch> Summary { get; set; }

        public ObservableCollection<PlayersInLineups> Lineups { get; set; }

        public ObservableCollection<SummaryEventOfMatch> Commentary { get; set; }

        public MatchStatistics MatchStatistics { get; set; }

        public Standing LiveStanding { get; set; }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task SetFullData()
        {
            await Task.WhenAll(
                LoadSummaryAsync(),
                LoadLineupsAsync(),
                LoadCommentaryAsync(),
                LoadMatchStatisticsAsync(),
                LoadLiveStandingAsync());
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private async Task LoadSummaryAsync()
        {
            await Task.Run(() =>
            {
                Summary = new ObservableCollection<SummaryEventOfMatch>(GetSummary());
            });
        }

        private async Task LoadLineupsAsync()
        {
            await Task.Run(() =>
            {
                var starterPlayers = PlayersOfMatch.Where(x => x.PlayingStatus == 1)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);
                var substitutionPlayers = PlayersOfMatch.Where(x => x.PlayingStatus == 2)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);

                var homeTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == Match.HomeTeam));
                var awayTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == Match.AwayTeam));
                var homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == Match.HomeTeam));
                var awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == Match.AwayTeam));

                var lineups = new ObservableCollection<PlayersInLineups>();

                var biggerLineups = homeTeamLineups.Count >= awayTeamLineups.Count ? homeTeamLineups : awayTeamLineups;
                for (int i = 0; i < biggerLineups.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamLineups.Count > i ? homeTeamLineups[i] : new PlayerOfMatch(),
                        AwayPlayer = awayTeamLineups.Count > i ? awayTeamLineups[i] : new PlayerOfMatch()
                    };
                    lineups.Add(playerInLineups);
                }

                var biggerSubstitution = homeTeamSubtitutions.Count >= awayTeamSubtitutions.Count ? homeTeamSubtitutions : awayTeamSubtitutions;
                for (int i = 0; i < biggerSubstitution.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamSubtitutions.Count > i ? homeTeamSubtitutions[i] : new PlayerOfMatch(),
                        AwayPlayer = awayTeamSubtitutions.Count > i ? awayTeamSubtitutions[i] : new PlayerOfMatch(),
                        IsFirstSub = i == 0
                    };
                    lineups.Add(playerInLineups);
                }

                Lineups = new ObservableCollection<PlayersInLineups>(lineups);
            });
        }

        private async Task LoadCommentaryAsync()
        {
            await Task.Run(() =>
            {
                Commentary = new ObservableCollection<SummaryEventOfMatch>(GetCommentary());
            });
        }

        private async Task LoadMatchStatisticsAsync()
        {
            MatchStatistics = new MatchStatistics(EventsOfMatch, Match);
            await Task.CompletedTask;
        }

        private async Task LoadLiveStandingAsync()
        {
            LiveStanding = DependencyService.Get<LiveStandingService>().GetStanding(Match?.MatchTypeName);
            await Task.CompletedTask;
        }

        private ObservableCollection<SummaryEventOfMatch> GetSummary()
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetEventOfMatchList(true);
            if (Match?.StatusID < 12)
            {
                listSummaryEvents.Reverse();
            }
            return new ObservableCollection<SummaryEventOfMatch>(listSummaryEvents);
        }

        private ObservableCollection<SummaryEventOfMatch> GetCommentary()
        {
            List<SummaryEventOfMatch> listCommentaryEvents = GetEventOfMatchList(false);
            listCommentaryEvents.Reverse();
            return new ObservableCollection<SummaryEventOfMatch>(listCommentaryEvents);
        }

        private List<SummaryEventOfMatch> GetEventOfMatchList(bool isSummaryList)
        {
            List<SummaryEventOfMatch> eventOfMatchesList = new List<SummaryEventOfMatch>();
            Match selectedMatch = Match;
            var homeTeamPlayers = PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.HomeTeam.Trim());
            var awayTeamPlayers = PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.AwayTeam.Trim());
            var eventsList = isSummaryList ? EventsOfMatch?
                .Where(x => x.EventName == "GOAL"
                || x.EventName == "OWNGOAL"
                || x.EventName == "PENALTYSCORED"
                || x.EventName == "PENALTYMISSED"
                || x.EventName == "YELLOW"
                || x.EventName == "RED"
                || x.EventName == "SUBSTITUTION"
                || x.EventName == "PLAYEROFTHEMATCH"
                || x.EventName == "ASSIST").ToList() : EventsOfMatch.ToList();

            int homeScore = 0;
            int awayScore = 0;
            int homeTeamPenaltiesScore = 0;
            int awayTeamPenaltiesScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventOfMatch>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch(eventOfMatch, selectedMatch); ;
                PlayerOfMatch mainPlayerOfMatch = PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.MainPlayerOfMatchID);
                string mainPlayerFullName = (mainPlayerOfMatch?.FirstName?.Trim() + " " + mainPlayerOfMatch?.LastName?.Trim())?.Trim();
                PlayerOfMatch secondPlayerOfMatch = PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.SecondPlayerOfMatchID);
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
                            PlayerOfMatch assistPlayerOfMatch = PlayersOfMatch?.FirstOrDefault(x => x.PlayerOfMatchID == nextEvent.MainPlayerOfMatchID);
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
                else if (eventOfMatch.EventName == "ASSIST" && isSummaryList)
                {
                    continue;
                }
                else if (eventOfMatch.EventName == "PLAYEROFTHEMATCH")
                {
                    summaryEventOfMatch.TextColorOfEventMinute = Color.DarkOrange;
                    summaryEventOfMatch.EventMinute = Resources.Localization.Resources.PlayerOfTheMatch;
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
                else if (eventOfMatch.EventName == "PENALTYSCORED")
                {
                    summaryEventOfMatch.HomeSecondPlayer = summaryEventOfMatch.AwaySecondPlayer = Resources.Localization.Resources.Penalty.ToLower();
                    summaryEventOfMatch.HomeSecondPlayerTextColor = summaryEventOfMatch.AwaySecondPlayerTextColor = Color.LimeGreen;
                }
                else if (eventOfMatch.EventName == "PENALTYMISSED")
                {
                    summaryEventOfMatch.HomeSecondPlayer = summaryEventOfMatch.AwaySecondPlayer = Resources.Localization.Resources.PenaltyMissed.ToLower();
                    summaryEventOfMatch.HomeSecondPlayerTextColor = summaryEventOfMatch.AwaySecondPlayerTextColor = Color.Red;
                }

                summaryEventOfMatch.IsHomeSecondPlayerVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeSecondPlayer);
                summaryEventOfMatch.IsAwaySecondPlayerVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwaySecondPlayer);

                eventOfMatchesList.Add(summaryEventOfMatch);
            }

            return eventOfMatchesList.ToList();
        }

    }
}