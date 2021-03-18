using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dystir.Models
{
    public class Standing
    {
        [JsonProperty("StandingCompetitionName")]
        public string StandingCompetitionName { get; set; }

        [JsonProperty("TeamStandings")]
        public IEnumerable<TeamStanding> TeamStandings { get; internal set; }
        
    }
}