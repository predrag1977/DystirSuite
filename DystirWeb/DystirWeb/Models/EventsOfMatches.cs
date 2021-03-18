using System;
using System.Collections.Generic;

namespace DystirWeb.Models
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
    }
}
