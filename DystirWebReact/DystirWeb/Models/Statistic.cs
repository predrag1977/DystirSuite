using DystirWeb.Shared;

namespace DystirWeb.Models
{
    public class Statistic
    {
        public TeamStatistic HomeTeamStatistic { get; set; } = new TeamStatistic();
        public TeamStatistic AwayTeamStatistic { get; set; } = new TeamStatistic();
    }
}

