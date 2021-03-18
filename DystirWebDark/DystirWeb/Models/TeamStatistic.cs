namespace DystirWeb.Models
{
    public class Statistic
    {
        public TeamStatistic HomeTeamStatistic { get; internal set; } = new TeamStatistic();
        public TeamStatistic AwayTeamStatistic { get; internal set; } = new TeamStatistic();
    }

    public class TeamStatistic
    {
        public string TeamName { get; internal set; }
        public int Goal { get; internal set; } = 0;
        public int YellowCard { get; internal set; } = 0;
        public int RedCard { get; internal set; } = 0;
        public int Corner { get; internal set; } = 0;
        public int GoalProcent { get; internal set; } = 0;
        public int YellowCardProcent { get; internal set; } = 0;
        public int RedCardProcent { get; internal set; } = 0;
        public int CornerProcent { get; internal set; } = 0;
        public int OnTarget { get; internal set; } = 0;
        public int OnTargetProcent { get; internal set; } = 0;
        public int OffTarget { get; internal set; } = 0;
        public int OffTargetProcent { get; internal set; } = 0;
        public int BlockedShot { get; internal set; } = 0;
        public int BlockedShotProcent { get; internal set; } = 0;
        public int BigChance { get; internal set; } = 0;
        public int BigChanceProcent { get; internal set; } = 0;

    }
}