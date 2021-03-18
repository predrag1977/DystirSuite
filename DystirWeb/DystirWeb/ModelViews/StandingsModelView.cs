using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DystirWeb.Models;

namespace DystirWeb.ModelViews
{

    public class StandingsModelView
    {
        public string SelectedCompetition { get; internal set; }
        public Standing Standing { get; internal set; }
        public IEnumerable<string> CompetitionsList { get; internal set; }
    }

    [DataContract]
    public class Standing
    {
        [DataMember]
        public string StandingCompetitionName { get; internal set; }
        [DataMember]
        public IEnumerable<TeamStanding> TeamStandings { get; internal set; }
    }

    [DataContract]
    public class TeamStanding
    {
        [DataMember]
        public string Team { get; internal set; }
        [DataMember]
        public int TeamID { get; internal set; }
        [DataMember]
        public int? MatchesNo { get; internal set; } = 0;
        [DataMember]
        public int? Points { get; internal set; } = 0;
        [DataMember]
        public int? GoalScored { get; internal set; } = 0;
        [DataMember]
        public int? GoalAgainst { get; internal set; } = 0;
        [DataMember]
        public int? GoalDifference { get; internal set; } = 0;
        [DataMember]
        public double? PointsProcent { get; internal set; }
        [DataMember]
        public int Victories { get; internal set; }
        [DataMember]
        public int Draws { get; internal set; }
        [DataMember]
        public int Losses { get; internal set; }
        [DataMember]
        public string CompetitionName { get; internal set; }
        [DataMember]
        public int Position { get; internal set; }
        [DataMember]
        public string PositionColor { get; internal set; }
        [DataMember]
        public bool IsLive { get; internal set; } = false;
    }
}