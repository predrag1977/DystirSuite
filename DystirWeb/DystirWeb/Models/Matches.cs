using System;

namespace DystirWeb.Models
{
    public partial class Matches
    {
        public int ExtraMinutes { get; set; }
        public int ExtraSeconds { get; set; }

        public int MatchID { get; set; }
        public DateTime? Time { get; set; }
        public string Location { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int? TeamAdminId { get; set; }
        public string HomeCategoriesName { get; set; }
        public string AwayCategoriesName { get; set; }
        public string HomeSquadName { get; set; }
        public int? StatusID { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public int? HomeTeamOnTarget { get; set; }
        public int? AwayTeamOnTarget { get; set; }
        public int? HomeTeamCorner { get; set; }
        public int? AwayTeamCorner { get; set; }
        public string AwaySquadName { get; set; }
        public string MatchTypeName { get; set; }
        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }
        public string StatusName { get; set; }
        public int? MatchActivation { get; set; }
        public DateTime? StatusTime { get; set; }
        public int? MatchTypeID { get; set; }
        public int? RoundID { get; set; }
        public string RoundName { get; set; }
    }
}
