
namespace Dystir.Models
{
    public class SummaryEventOfMatch
    {
        public EventOfMatch EventOfMatch { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public int HomeTeamPenaltiesScore { get; set; }
        public int AwayTeamPenaltiesScore { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string EventName { get; set; }
        public string EventMinute { get; set; }
        public string HomeMainPlayer { get; set; }
        public string HomeSecondPlayer { get; set; }
        public string AwayMainPlayer { get; set; }
        public string AwaySecondPlayer { get; set; }
        public bool HomeTeamVisible { get; set; }
        public bool AwayTeamVisible { get; set; }
    }
}