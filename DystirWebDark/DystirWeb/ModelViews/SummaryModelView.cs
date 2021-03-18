using System;
using DystirWeb.Models;

namespace DystirWeb.ModelViews
{
    public class SummaryEventOfMatch
    {
        public EventsOfMatches EventOfMatch { get; internal set; }
        public int HomeTeamScore { get; internal set; }
        public int AwayTeamScore { get; internal set; }
        public string HomeTeam { get; internal set; }
        public string AwayTeam { get; internal set; }
        public string EventName { get; internal set; }
        public string EventMinute { get; internal set; }
        public string HomeMainPlayer { get; internal set; }
        public string HomeSecondPlayer { get; internal set; }
        public string AwayMainPlayer { get; internal set; }
        public string AwaySecondPlayer { get; internal set; }
        public bool HomeTeamVisible { get; internal set; }
        public bool AwayTeamVisible { get; internal set; }
    }
}