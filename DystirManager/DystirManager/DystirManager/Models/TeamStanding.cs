using DystirManager.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DystirManager.Models
{
    public class TeamStanding
    {
        [JsonProperty("Team")]
        public string Team { get; internal set; }
        [JsonProperty("TeamID")]
        public int TeamID { get; internal set; }
        [JsonProperty("MatchesNo")]
        public int? MatchesNo { get; internal set; } = 0;
        [JsonProperty("Points")]
        public int? Points { get; internal set; } = 0;
        [JsonProperty("GoalScored")]
        public int? GoalScored { get; internal set; } = 0;
        [JsonProperty("GoalAgainst")]
        public int? GoalAgainst { get; internal set; } = 0;
        [JsonProperty("GoalDifference")]
        public int? GoalDifference { get; internal set; } = 0;
        [JsonProperty("PointsProcent")]
        public double? PointsProcent { get; internal set; }
        [JsonProperty("Victories")]
        public int Victories { get; internal set; }
        [JsonProperty("Draws")]
        public int Draws { get; internal set; }
        [JsonProperty("Losses")]
        public int Losses { get; internal set; }
        [JsonProperty("CompetitionName")]
        public string CompetitionName { get; internal set; }
        [JsonProperty("Position")]
        public int Position { get; internal set; }
        [JsonProperty("PositionColor")]
        public string PositionColor { get; internal set; }
    }
}