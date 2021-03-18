using Dystir.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dystir.Models
{
    public class CompetitionStatistic
    {
        [JsonProperty("CompetitionName")]
        public string CompetitionName { get; internal set; }

        [JsonProperty("TeamStatistics")]
        public List<TeamStatistic> TeamStatistics { get; internal set; } = new List<TeamStatistic>();

        [JsonProperty("GoalPlayers")]
        public List<PlayerOfMatch> GoalPlayers { get; internal set; } = new List<PlayerOfMatch>();

        [JsonProperty("AssistPlayers")]
        public List<PlayerOfMatch> AssistPlayers { get; internal set; } = new List<PlayerOfMatch>();

    }
}