using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Dystir.Models
{
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

        public GridLength GoalProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength YellowCardProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength RedCardProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength CornerProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength OnTargetProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength OffTargetProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength BlockedShotProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);
        public GridLength BigChanceProcentWidth { get; set; } = new GridLength(50, GridUnitType.Star);

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

