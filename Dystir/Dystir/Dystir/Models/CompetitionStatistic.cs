using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    [DataContract]
    public class CompetitionStatistic
    {
        [DataMember]
        public string CompetitionName { get; set; }

        [DataMember]
        public List<TeamStatistic> TeamStatistics { get; set; } = new List<TeamStatistic>();

        [DataMember]
        public List<PlayerOfMatch> GoalPlayers { get; set; } = new List<PlayerOfMatch>();

        [DataMember]
        public List<PlayerOfMatch> AssistPlayers { get; set; } = new List<PlayerOfMatch>();
    }
}