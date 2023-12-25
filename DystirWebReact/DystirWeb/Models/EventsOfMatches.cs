using System;

namespace DystirWeb.Shared
{
    public partial class EventsOfMatches
    {
        public int EventOfMatchId { get; set; }
        public int? MainPlayerOfMatchId { get; set; }
        public string EventText { get; set; }
        public string EventName { get; set; }
        public int? MatchId { get; set; }
        public string EventTeam { get; set; }
        public string EventMinute { get; set; }
        public DateTime? EventTime { get; set; }
        public int? EventPeriodId { get; set; }
        public string EventTotalTime { get; set; }
        public int? SecondPlayerOfMatchId { get; set; }
        public string MainPlayerOfMatchNumber { get; set; }
        public string SecondPlayerOfMatchNumber { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public int HomeTeamPenaltiesScore { get; set; }
        public int AwayTeamPenaltiesScore { get; set; }
    }
}
