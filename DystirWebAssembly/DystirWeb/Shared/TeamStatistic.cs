namespace DystirWeb.Shared
{
    public class Statistic
    {
        public TeamStatistic HomeTeamStatistic { get; set; } = new TeamStatistic();
        public TeamStatistic AwayTeamStatistic { get; set; } = new TeamStatistic();
    }

    public class TeamStatistic
    {
        public string TeamName { get; set; }
        public int Goal { get; set; } = 0;
        public int YellowCard { get; set; } = 0;
        public int RedCard { get; set; } = 0;
        public int Corner { get; set; } = 0;
        public int OnTarget { get; set; } = 0;
        public int OffTarget { get; set; } = 0;
        public int BlockedShot { get; set; } = 0;
        public int BigChance { get; set; } = 0;
        public int GoalProcent { get; set; } = 50;
        public int YellowCardProcent { get; set; } = 50;
        public int RedCardProcent { get; set; } = 50;
        public int CornerProcent { get; set; } = 50;
        public int OnTargetProcent { get; set; } = 50;
        public int OffTargetProcent { get; set; } = 50;
        public int BlockedShotProcent { get; set; } = 50;
        public int BigChanceProcent { get; set; } = 50;

    }
}