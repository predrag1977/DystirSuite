using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DystirWeb.Shared
{
    [DataContract]
    public class CompetitionStatistic
    {
        [DataMember]
        public string CompetitionName { get; set; }

        [DataMember]
        public List<TeamStatistic> TeamStatistics { get; set; } = new List<TeamStatistic>();

        [DataMember]
        public List<PlayersOfMatches> GoalPlayers { get; set; } = new List<PlayersOfMatches>();

        [DataMember]
        public List<PlayersOfMatches> AssistPlayers { get; set; } = new List<PlayersOfMatches>();
    }
}