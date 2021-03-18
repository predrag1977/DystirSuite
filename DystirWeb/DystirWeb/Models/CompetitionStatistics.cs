using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DystirWeb.Models
{
    [DataContract]
    public class CompetitionStatistic
    {
        [DataMember]
        public string CompetitionName { get; internal set; }

        [DataMember]
        public List<TeamStatistic> TeamStatistics { get; internal set; } = new List<TeamStatistic>();

        [DataMember]
        public List<PlayersOfMatches> GoalPlayers { get; internal set; } = new List<PlayersOfMatches>();

        [DataMember]
        public List<PlayersOfMatches> AssistPlayers { get; internal set; } = new List<PlayersOfMatches>();
    }
}