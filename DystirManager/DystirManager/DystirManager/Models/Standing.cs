using DystirManager.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DystirManager.Models
{
    public class Standing
    {
        [JsonProperty("StandingCompetitionName")]
        public string StandingCompetitionName { get; set; }

        [JsonProperty("TeamStandings")]
        public IEnumerable<TeamStanding> TeamStandings { get; internal set; }
        
    }
}