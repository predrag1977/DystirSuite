using Newtonsoft.Json;
using System;
using Xamarin.Forms;

namespace DystirXamarin.Models
{
    public class EventOfMatch
    {
        [JsonProperty("EventOfMatchID")]
        public int EventOfMatchID { get; set; }

        [JsonProperty("EventName")]
        public string EventName { get; internal set; }

        [JsonProperty("EventText")]
        public string EventText { get; internal set; }

        [JsonProperty("MainPlayerOfMatchID")]
        public int MainPlayerOfMatchID { get; set; }

        [JsonProperty("SecondPlayerOfMatchID")]
        public int SecondPlayerOfMatchID { get; set; }

        [JsonProperty("MatchID")]
        public int MatchID { get; set; }

        [JsonProperty("EventPeriodID")]
        public int EventPeriodID { get; set; }

        [JsonProperty("EventTeam")]
        public string EventTeam { get; internal set; }

        [JsonProperty("EventMinute")]
        public string EventMinute { get; set; }

        [JsonProperty("EventTotalTime")]
        public string EventTotalTime { get; set; }

        [JsonProperty("EventTime")]
        public Nullable<DateTime> EventTime { get; set; }

        [JsonProperty("MainPlayerOfMatchNumber")]
        public string MainPlayerOfMatchNumber { get; set; }

        [JsonProperty("SecondPlayerOfMatchNumber")]
        public string SecondPlayerOfMatchNumber { get; set; }
        public bool HomeTeamVisible { get; internal set; }
        public bool AwayTeamVisible { get; internal set; }
        public string AdditionalText { get; internal set; }
        public object EventBackgroundColor { get; internal set; }
        public string EventIconSource { get; internal set; }
        public int EventIconSize { get; internal set; }
        public bool ShowEventIcon { get; internal set; }
    }
}