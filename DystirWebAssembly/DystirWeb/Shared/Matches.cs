using Newtonsoft.Json;
using System;

namespace DystirWeb.Shared
{
    public partial class Matches
    {
        [JsonProperty("HomeTeam")]
        public string HomeTeam { get; set; }

        [JsonProperty("AwayTeam")]
        public string AwayTeam { get; set; }

        [JsonProperty("HomeCategoriesName")]
        public string HomeCategoriesName { get; set; }

        [JsonProperty("AwayCategoriesName")]
        public string AwayCategoriesName { get; set; }

        [JsonProperty("HomeSquadName")]
        public string HomeSquadName { get; set; }

        [JsonProperty("AwaySquadName")]
        public string AwaySquadName { get; set; }

        [JsonProperty("MatchID")]
        public int MatchID { get; set; }

        [JsonProperty("Time")]
        public DateTime? Time { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("StatusID")]
        public int? StatusID { get; set; }

        [JsonProperty("HomeTeamScore")]
        public int? HomeTeamScore { get; set; }

        [JsonProperty("AwayTeamScore")]
        public int? AwayTeamScore { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; set; }

        [JsonProperty("HomeTeamID")]
        public int? HomeTeamID { get; set; }

        [JsonProperty("AwayTeamID")]
        public int? AwayTeamID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }

        [JsonProperty("MatchActivation")]
        public int? MatchActivation { get; set; }

        [JsonProperty("StatusTime")]
        public DateTime? StatusTime { get; set; }

        [JsonProperty("MatchTypeID")]
        public int? MatchTypeID { get; set; }

        [JsonProperty("TeamAdminID")]
        public int? TeamAdminID { get; set; }

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }

        [JsonProperty("ExtraMinutes")]
        public int ExtraMinutes { get; set; }

        [JsonProperty("ExtraSeconds")]
        public int ExtraSeconds { get; set; }

        [JsonProperty("HomeTeamOnTarget")]
        public int? HomeTeamOnTarget { get; set; }

        [JsonProperty("AwayTeamOnTarget")]
        public int? AwayTeamOnTarget { get; set; }

        [JsonProperty("HomeTeamCorner")]
        public int? HomeTeamCorner { get; set; }

        [JsonProperty("AwayTeamCorner")]
        public int? AwayTeamCorner { get; set; }
    }
}
