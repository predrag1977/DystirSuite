using System;
using System.ComponentModel;

namespace Dystir.Models
{
    public class EventOfMatch
    {
        //[JsonProperty("EventOfMatchID")]
        //public int EventOfMatchID { get; set; }

        //[JsonProperty("EventName")]
        //public string EventName { get; internal set; }

        //[JsonProperty("EventText")]
        //public string EventText { get; internal set; }

        //[JsonProperty("MainPlayerOfMatchID")]
        //public int MainPlayerOfMatchID { get; set; }

        //[JsonProperty("SecondPlayerOfMatchID")]
        //public int SecondPlayerOfMatchID { get; set; }

        //[JsonProperty("MatchID")]
        //public int MatchID { get; set; }

        //[JsonProperty("EventPeriodID")]
        //public int EventPeriodID { get; set; }

        //[JsonProperty("EventTeam")]
        //public string EventTeam { get; internal set; }

        //[JsonProperty("EventMinute")]
        //public string EventMinute { get; set; }

        //[JsonProperty("EventTotalTime")]
        //public string EventTotalTime { get; set; }

        //[JsonProperty("EventTime")]
        //public DateTime? EventTime { get; set; }

        //[JsonProperty("MainPlayerOfMatchNumber")]
        //public string MainPlayerOfMatchNumber { get; set; }

        //[JsonProperty("SecondPlayerOfMatchNumber")]
        //public string SecondPlayerOfMatchNumber { get; set; }

        public int HomeTeamScore { get; internal set; }
        public int AwayTeamScore { get; internal set; }
        public string HomeTeam { get; internal set; }
        public string AwayTeam { get; internal set; }
        public string HomeMainPlayer { get; internal set; }
        public string HomeSecondPlayer { get; internal set; }
        public string AwayMainPlayer { get; internal set; }
        public string AwaySecondPlayer { get; internal set; }
        public bool HomeTeamVisible { get; internal set; }
        public bool AwayTeamVisible { get; internal set; }
        public bool ShowResult { get; internal set; }
        public bool ShowSecondPlayer { get; internal set; }

        public int EventOfMatchID { get; set; }
        public int? MainPlayerOfMatchID { get; set; }
        public string EventText { get; set; }
        public string EventName { get; set; }
        public int? MatchID { get; set; }
        public string EventTeam { get; set; }
        public string EventMinute { get; set; }
        public DateTime? EventTime { get; set; }
        public int? EventPeriodID { get; set; }
        public string EventTotalTime { get; set; }
        public int? SecondPlayerOfMatchID { get; set; }
        public string MainPlayerOfMatchNumber { get; set; }
        public string SecondPlayerOfMatchNumber { get; set; }
    }
}