namespace Dystir.Models
{
    public class Statistic
    {
        public TeamStatistic HomeTeamStatistic { get; set; } = new TeamStatistic();
        public TeamStatistic AwayTeamStatistic { get; set; } = new TeamStatistic();
    }

    public class TeamStatistic
    {
        public string TeamName { get; set; }
        public double Goal { get; set; } = 0;
        public double YellowCard { get; set; } = 0;
        public double RedCard { get; set; } = 0;
        public double Corner { get; set; } = 0;
        public double OnTarget { get; set; } = 0;
        public double OffTarget { get; set; } = 0;
        public double BlockedShot { get; set; } = 0;
        public double BigChance { get; set; } = 0;

        public double GoalProcent { get; set; } = 0.5;
        public double YellowCardProcent { get; set; } = 0.5;
        public double RedCardProcent { get; set; } = 0.5;
        public double CornerProcent { get; set; } = 0.5;
        public double OnTargetProcent { get; set; } = 0.5;
        public double OffTargetProcent { get; set; } = 0.5;
        public double BlockedShotProcent { get; set; } = 0.5;
        public double BigChanceProcent { get; set; } = 0.5;
    }
}