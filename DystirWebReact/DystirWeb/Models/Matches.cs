using Newtonsoft.Json;

namespace DystirWeb.Shared
{
    public partial class Matches
    {
        [JsonProperty(nameof(HomeTeam))]
        public string HomeTeam { get; set; }

        [JsonProperty(nameof(AwayTeam))]
        public string AwayTeam { get; set; }

        [JsonProperty(nameof(HomeCategoriesName))]
        public string HomeCategoriesName { get; set; }

        [JsonProperty(nameof(AwayCategoriesName))]
        public string AwayCategoriesName { get; set; }

        [JsonProperty(nameof(HomeSquadName))]
        public string HomeSquadName { get; set; }

        [JsonProperty(nameof(AwaySquadName))]
        public string AwaySquadName { get; set; }

        [JsonProperty(nameof(MatchID))]
        public int MatchID { get; set; }

        [JsonProperty(nameof(Time))]
        public DateTime? Time { get; set; }

        [JsonProperty(nameof(Location))]
        public string Location { get; set; }

        [JsonProperty(nameof(StatusID))]
        public int? StatusID { get; set; }

        [JsonProperty(nameof(HomeTeamScore))]
        public int? HomeTeamScore { get; set; }

        [JsonProperty(nameof(AwayTeamScore))]
        public int? AwayTeamScore { get; set; }

        [JsonProperty(nameof(HomeTeamPenaltiesScore))]
        public int? HomeTeamPenaltiesScore { get; set; }

        [JsonProperty(nameof(AwayTeamPenaltiesScore))]
        public int? AwayTeamPenaltiesScore { get; set; }

        [JsonProperty(nameof(MatchTypeName))]
        public string MatchTypeName { get; set; }

        [JsonProperty(nameof(HomeTeamID))]
        public int? HomeTeamID { get; set; }

        [JsonProperty(nameof(AwayTeamID))]
        public int? AwayTeamID { get; set; }

        [JsonProperty(nameof(StatusName))]
        public string StatusName { get; set; }

        [JsonProperty(nameof(MatchActivation))]
        public int? MatchActivation { get; set; }

        [JsonProperty(nameof(StatusTime))]
        public DateTime? StatusTime { get; set; }

        [JsonProperty(nameof(MatchTypeID))]
        public int? MatchTypeID { get; set; }

        [JsonProperty(nameof(TeamAdminID))]
        public int? TeamAdminID { get; set; }

        [JsonProperty(nameof(RoundID))]
        public int? RoundID { get; set; }

        [JsonProperty(nameof(RoundName))]
        public string RoundName { get; set; }

        [JsonProperty(nameof(ExtraMinutes))]
        public int ExtraMinutes { get; set; }

        [JsonProperty(nameof(ExtraSeconds))]
        public int ExtraSeconds { get; set; }

        [JsonProperty(nameof(HomeTeamOnTarget))]
        public int? HomeTeamOnTarget { get; set; }

        [JsonProperty(nameof(AwayTeamOnTarget))]
        public int? AwayTeamOnTarget { get; set; }

        [JsonProperty(nameof(HomeTeamCorner))]
        public int? HomeTeamCorner { get; set; }

        [JsonProperty(nameof(AwayTeamCorner))]
        public int? AwayTeamCorner { get; set; }

        [JsonProperty(nameof(HomeTeamLogo))]
        public string HomeTeamLogo { get; set; }

        [JsonProperty(nameof(AwayTeamLogo))]
        public string AwayTeamLogo { get; set; }

        [JsonProperty(nameof(FullMatchDetails))]
        public FullMatchDetailsModelView FullMatchDetails { get; set; }
        
    }
}
