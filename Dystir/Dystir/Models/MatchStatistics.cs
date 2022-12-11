using System.Collections.ObjectModel;

namespace Dystir.Models
{
    public class MatchStatistics
    {
        public TeamStatistic HomeTeamStatistics { get; set; }
        public TeamStatistic AwayTeamStatistics { get; set; }

        public MatchStatistics(ObservableCollection<EventOfMatch> eventsDataList, Match match)
        {
            SetMatchStatistics(eventsDataList, match);
        }

        private void SetMatchStatistics(ObservableCollection<EventOfMatch> eventsDataList, Match match)
        {
            var homeTeamEventList = eventsDataList.Where(x => x.EventTeam?.ToLower().Trim() == match?.HomeTeam?.ToLower().Trim());
            HomeTeamStatistics = new TeamStatistic(new ObservableCollection<EventOfMatch>(homeTeamEventList));

            var awayTeamEventList = eventsDataList.Where(x => x.EventTeam?.ToLower().Trim() == match?.AwayTeam?.ToLower().Trim());
            AwayTeamStatistics = new TeamStatistic(new ObservableCollection<EventOfMatch>(awayTeamEventList));

            if (HomeTeamStatistics.Goal + AwayTeamStatistics.Goal != 0)
            {
                HomeTeamStatistics.GoalProcent = HomeTeamStatistics.Goal / (HomeTeamStatistics.Goal + AwayTeamStatistics.Goal);
                AwayTeamStatistics.GoalProcent = AwayTeamStatistics.Goal / (HomeTeamStatistics.Goal + AwayTeamStatistics.Goal);
            }
            if (HomeTeamStatistics.YellowCard + AwayTeamStatistics.YellowCard != 0)
            {
                HomeTeamStatistics.YellowCardProcent = HomeTeamStatistics.YellowCard / (HomeTeamStatistics.YellowCard + AwayTeamStatistics.YellowCard);
                AwayTeamStatistics.YellowCardProcent = AwayTeamStatistics.YellowCard / (HomeTeamStatistics.YellowCard + AwayTeamStatistics.YellowCard);
            }
            if (HomeTeamStatistics.RedCard + AwayTeamStatistics.RedCard != 0)
            {
                HomeTeamStatistics.RedCardProcent = HomeTeamStatistics.RedCard / (HomeTeamStatistics.RedCard + AwayTeamStatistics.RedCard);
                AwayTeamStatistics.RedCardProcent = AwayTeamStatistics.RedCard / (HomeTeamStatistics.RedCard + AwayTeamStatistics.RedCard);
            }
            if (HomeTeamStatistics.Corner + AwayTeamStatistics.Corner != 0)
            {
                HomeTeamStatistics.CornerProcent = HomeTeamStatistics.Corner / (HomeTeamStatistics.Corner + AwayTeamStatistics.Corner);
                AwayTeamStatistics.CornerProcent = AwayTeamStatistics.Corner / (HomeTeamStatistics.Corner + AwayTeamStatistics.Corner);
            }
            if (HomeTeamStatistics.OnTarget + AwayTeamStatistics.OnTarget != 0)
            {
                HomeTeamStatistics.OnTargetProcent = HomeTeamStatistics.OnTarget / (HomeTeamStatistics.OnTarget + AwayTeamStatistics.OnTarget);
                AwayTeamStatistics.OnTargetProcent = AwayTeamStatistics.OnTarget / (HomeTeamStatistics.OnTarget + AwayTeamStatistics.OnTarget);
            }
            if (HomeTeamStatistics.OffTarget + AwayTeamStatistics.OffTarget != 0)
            {
                HomeTeamStatistics.OffTargetProcent = HomeTeamStatistics.OffTarget / (HomeTeamStatistics.OffTarget + AwayTeamStatistics.OffTarget);
                AwayTeamStatistics.OffTargetProcent = AwayTeamStatistics.OffTarget / (HomeTeamStatistics.OffTarget + AwayTeamStatistics.OffTarget);
            }
            if (HomeTeamStatistics.BlockedShot + AwayTeamStatistics.BlockedShot != 0)
            {
                HomeTeamStatistics.BlockedShotProcent = HomeTeamStatistics.BlockedShot / (HomeTeamStatistics.BlockedShot + AwayTeamStatistics.BlockedShot);
                AwayTeamStatistics.BlockedShotProcent = AwayTeamStatistics.BlockedShot / (HomeTeamStatistics.BlockedShot + AwayTeamStatistics.BlockedShot);
            }
            if (HomeTeamStatistics.BigChance + AwayTeamStatistics.BigChance != 0)
            {
                HomeTeamStatistics.BigChanceProcent = HomeTeamStatistics.BigChance / (HomeTeamStatistics.BigChance + AwayTeamStatistics.BigChance);
                AwayTeamStatistics.BigChanceProcent = AwayTeamStatistics.BigChance / (HomeTeamStatistics.BigChance + AwayTeamStatistics.BigChance);
            }
        }
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

        public TeamStatistic(ObservableCollection<EventOfMatch> eventsDataList)
        {
            foreach (EventOfMatch eventOfMatch in eventsDataList)
            {
                SetTeamStatistics(eventOfMatch);
            }
        }

        private void SetTeamStatistics(EventOfMatch eventOfMatch)
        {
            switch (eventOfMatch?.EventName?.ToLower())
            {
                case "goal":
                case "penaltyscored":
                    Goal += 1;
                    OnTarget += 1;
                    break;
                case "owngoal":
                    Goal += 1;
                    break;
                case "yellow":
                    YellowCard += 1;
                    break;
                case "red":
                    RedCard += 1;
                    break;
                case "corner":
                    Corner += 1;
                    break;
                case "ontarget":
                    OnTarget += 1;
                    break;
                case "offtarget":
                    OffTarget += 1;
                    break;
                case "blockedshot":
                    BlockedShot += 1;
                    break;
                case "bigchance":
                    BigChance += 1;
                    break;
            }
        }
    }
}