using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dystir.Models
{
    public class CompetitionStatistic
    {
        [JsonProperty("CompetitionName")]
        public string CompetitionName { get; set; }

        //[JsonProperty("TeamStatistics")]
        //public List<TeamStatistic> TeamStatistics { get; set; } = new List<TeamStatistic>();

        [JsonProperty("GoalPlayers")]
        public List<PlayerOfMatch> GoalPlayers { get; set; } = new List<PlayerOfMatch>();

        [JsonProperty("AssistPlayers")]
        public List<PlayerOfMatch> AssistPlayers { get; set; } = new List<PlayerOfMatch>();
    }
}