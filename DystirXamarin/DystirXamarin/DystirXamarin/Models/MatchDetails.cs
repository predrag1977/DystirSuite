using Newtonsoft.Json;
using System.Collections.Generic;

namespace DystirXamarin.Models
{
    public class MatchDetails
    {
        [JsonProperty("Match")]
        public Match Match { get; set; }

        [JsonProperty("EventsOfMatch")]
        public IEnumerable<EventOfMatch> EventsOfMatch { get; internal set; }

        [JsonProperty("PlayersOfMatch")]
        public IEnumerable<PlayerOfMatch> PlayersOfMatch { get; internal set; }

    }
}