using DystirWeb.Server.DystirDB;
using DystirWeb.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DystirWeb.Services
{
    public class MatchDetailsService
    {
        private readonly DystirService _dystirService;
        private readonly DystirDBContext _dystirDBContext;
        static readonly object lockGetMatchDetails = new object();

        public MatchDetailsService (DystirService dystirService, DystirDBContext dystirDBContext)
        {
            _dystirService = dystirService;
            _dystirDBContext = dystirDBContext;
        }

        public MatchDetails GetMatchDetails(int matchID, bool isDatabaseUpdated)
        {
            lock (lockGetMatchDetails)
            {
                var matchDetails = isDatabaseUpdated ? null : _dystirService.AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == matchID);
                if (matchDetails == null)
                {
                    var eventsOfMatch = GetEventsOfMatches(matchID);
                    var playersOfMatch = GetPlayersOfMatches(matchID);
                    matchDetails = new MatchDetails()
                    {
                        MatchDetailsID = matchID,
                        Match = _dystirDBContext.Matches.Find(matchID),
                        EventsOfMatch = eventsOfMatch?
                        .OrderBy(x => x.EventPeriodId ?? 0)
                        .ThenBy(x => x.EventTotalTime)
                        .ThenBy(x => x.EventMinute)
                        .ThenBy(x => x.EventOfMatchId).ToList(),
                        PlayersOfMatch = playersOfMatch?.Where(x => x.PlayingStatus != 3).ToList()
                    };
                    _dystirService.UpdateDataAsync(matchDetails);
                }
                return matchDetails;
            }
        }

        public IEnumerable<EventsOfMatches> GetEventsOfMatches(int matchID)
        {
            IEnumerable<EventsOfMatches> eventsList = _dystirDBContext.EventsOfMatches.Where(x => x.MatchId == matchID);
            var sortedEventList = eventsList?
                .OrderByDescending(x => x.EventPeriodId ?? 0)
                .ThenByDescending(x => x.EventTotalTime)
                .ThenByDescending(x => x.EventMinute)
                .ThenByDescending(x => x.EventOfMatchId);
            return sortedEventList ?? Enumerable.Empty<EventsOfMatches>();
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
    }
}
