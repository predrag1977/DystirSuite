using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DystirWeb.DystirDB;
using DystirWeb.Shared;

namespace DystirWeb.Services
{
    public class MatchDetailsService
    {
        private readonly DystirService _dystirService;
        private readonly StandingService _standingService;
        private readonly MatchStatisticService _matchStatisticService;
        private readonly DystirDBContext _dystirDBContext;
        static readonly object lockGetMatchDetails = new object();

        public MatchDetailsService(
            DystirService dystirService,
            StandingService standingService,
            MatchStatisticService matchStatisticService,
            DystirDBContext dystirDBContext)
        {
            _dystirService = dystirService;
            _standingService = standingService;
            _matchStatisticService = matchStatisticService;
            _dystirDBContext = dystirDBContext;
        }

        public MatchDetails FindMatchDetails(int matchID)
        {
            var matchDetails = _dystirService.AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == matchID);
            if(matchDetails == null)
            {
                var match = GetMatchFromDB(matchID);
                matchDetails = GetMatchDetails(match);
            }
            else
            {
                matchDetails.Matches = GetLiveMatches();
            }
            return matchDetails;
        }

        public MatchDetails GetMatchDetails(Matches match)
        {
            lock (lockGetMatchDetails)
            {
                var matchID = match.MatchID;
                var matchDetails = new MatchDetails()
                {
                    MatchDetailsID = matchID,
                    Match = match,
                    EventsOfMatch = GetEventsOfMatches(matchID)?
                    .OrderBy(x => x.EventPeriodId ?? 0)
                    .ThenBy(x => x.EventTotalTime)
                    .ThenBy(x => x.EventMinute)
                    .ThenBy(x => x.EventOfMatchId).ToList(),
                    PlayersOfMatch = GetPlayersOfMatches(matchID)?.Where(x => x.PlayingStatus != 3).ToList(),
                };
                matchDetails.Standings = _standingService.GetStandings().ToList();
                matchDetails.Matches = GetLiveMatches();
                matchDetails.Statistic = _matchStatisticService.GetStatistic(matchDetails.EventsOfMatch, matchDetails.Match);
                return matchDetails;
            }
        }

        public List<Matches> GetLiveMatches()
        {
            return _dystirService.AllMatches.Where(x =>
                x.Time > DateTime.UtcNow.AddDays(-2) &&
                x.Time < DateTime.UtcNow.AddDays(2)).ToList();
        }

        public IEnumerable<EventsOfMatches> GetEventsOfMatches(int matchID)
        {
            IEnumerable<EventsOfMatches> eventsList = _dystirDBContext.EventsOfMatches.Where(x => x.MatchId == matchID);
            var sortedEventList = eventsList?
                .OrderByDescending(x => x.EventPeriodId ?? 0)
                .ThenByDescending(x => x.EventTotalTime)
                .ThenByDescending(x => x.EventMinute)
                .ThenByDescending(x => x.EventOfMatchId);
            var fullEventsOfMatchList = GetFullEventsList(sortedEventList ?? Enumerable.Empty<EventsOfMatches>(), matchID);
            return fullEventsOfMatchList;
        }

        public IEnumerable<PlayersOfMatches> GetPlayersOfMatches(int matchID)
        {
            var playersOfMatchList = _dystirDBContext.PlayersOfMatches.Where(x => x.MatchId == matchID);
            return playersOfMatchList?
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
        }

        private IEnumerable<EventsOfMatches> GetFullEventsList(IEnumerable<EventsOfMatches> sortedEventList, int matchID)
        {
            var match = GetMatchFromDB(matchID);
            int homeTeamScore = 0;
            int awayTeamScore = 0;
            int homeTeamPenaltiesScore = 0;
            int awyTeamPenaltiesScore = 0;
            foreach (EventsOfMatches eventOfMatch in sortedEventList.Reverse())
            {
                if(eventOfMatch.EventName == "GOAL" || eventOfMatch.EventName == "OWNGOAL" || eventOfMatch.EventName == "PENALTYSCORED")
                {
                    if (eventOfMatch.EventPeriodId != 10)
                    {
                        if (match.HomeTeam == eventOfMatch.EventTeam)
                        {
                            homeTeamScore++;
                        }
                        else if (match.AwayTeam == eventOfMatch.EventTeam)
                        {
                            awayTeamScore++;
                        }
                    }
                    else
                    {
                        if (match.HomeTeam == eventOfMatch.EventTeam)
                        {
                            homeTeamPenaltiesScore++;
                        }
                        else if (match.AwayTeam == eventOfMatch.EventTeam)
                        {
                            awyTeamPenaltiesScore++;
                        }
                    }
                }
                eventOfMatch.HomeTeamScore = homeTeamScore;
                eventOfMatch.AwayTeamScore = awayTeamScore;
                eventOfMatch.HomeTeamPenaltiesScore =  homeTeamPenaltiesScore;
                eventOfMatch.AwayTeamPenaltiesScore = awyTeamPenaltiesScore;
            }
            return sortedEventList;
        }

        private Matches GetMatchFromDB(int matchID)
        {
            var match = _dystirDBContext.Matches.Find(matchID);
            _dystirService.SetTeamLogoInMatch(match);

            return match;
        }
    }
}
