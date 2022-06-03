using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DystirWeb.Shared
{

    public class StandingsModelView
    {
        public string SelectedCompetition { get; set; }
        public Standing Standing { get; set; }
        public IEnumerable<string> CompetitionsList { get; set; }
    }

    [DataContract]
    public class Standing
    {
        [DataMember]
        public string StandingCompetitionName { get; set; }
        [DataMember]
        public IEnumerable<TeamStanding> TeamStandings { get; set; }
    }

    [DataContract]
    public class TeamStanding
    {
        [DataMember]
        public string Team { get; set; }
        [DataMember]
        public int TeamID { get; set; }
        [DataMember]
        public int? MatchesNo { get; set; } = 0;
        [DataMember]
        public int? Points { get; set; } = 0;
        [DataMember]
        public int? GoalScored { get; set; } = 0;
        [DataMember]
        public int? GoalAgainst { get; set; } = 0;
        [DataMember]
        public int? GoalDifference { get; set; } = 0;
        [DataMember]
        public double? PointsProcent { get; set; }
        [DataMember]
        public int Victories { get; set; }
        [DataMember]
        public int Draws { get; set; }
        [DataMember]
        public int Losses { get; set; }
        [DataMember]
        public string CompetitionName { get; set; }
        [DataMember]
        public int Position { get; set; }
        [DataMember]
        public string PositionColor { get; set; }
        [DataMember]
        public bool IsLive { get; set; }
    }
}