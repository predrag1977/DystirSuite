using System;
using System.Collections.Generic;

namespace DystirWeb.Shared
{
    public partial class PlayersOfMatches
    {
        public int PlayerOfMatchId { get; set; }
        public int? MatchId { get; set; }
        public string? FirstName { get; set; }
        public string? Lastname { get; set; }
        public string? TeamName { get; set; }
        public int? Number { get; set; }
        public int? Goal { get; set; }
        public int? OwnGoal { get; set; }
        public int? YellowCard { get; set; }
        public int? RedCard { get; set; }
        public int? Assist { get; set; }
        public int? SubIn { get; set; }
        public int? SubOut { get; set; }
        public int? PlayingStatus { get; set; }
        public int? TeamId { get; set; }
        public int? PlayerId { get; set; }
        public string? Position { get; set; }
        public int? Captain { get; set; }
        public string? MatchTypeName { get; set; }
        public int? MatchTypeId { get; set; }
    }
}
