
namespace Dystir.Models
{
    public class SummaryEventOfMatch
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
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
        public bool IsGoal { get; set; }
        public bool IsHomeTeamEvent { get; set; }
        public bool IsAwayTeamEvent { get; set; }
        public string EventIconSource { get; set; }
        public bool ShowMinutes { get; set; }
        public Color TextColorOfEventMinute { get; set; } = Colors.Khaki;

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public SummaryEventOfMatch(EventOfMatch eventOfMatch, Match selectedMatch)
        {
            EventOfMatch = eventOfMatch;
            HomeTeam = eventOfMatch.EventTeam == selectedMatch.HomeTeam ? eventOfMatch.EventTeam : string.Empty;
            AwayTeam = eventOfMatch.EventTeam == selectedMatch.AwayTeam ? eventOfMatch.EventTeam : string.Empty;
            EventMinute = GetEventMinute(eventOfMatch);
            EventName = eventOfMatch.EventName;
            HomeTeamScore = 0;
            AwayTeamScore = 0;
            IsGoal = false;
            IsHomeTeamEvent = false;
            IsAwayTeamEvent = false;
            EventIconSource = (eventOfMatch.EventName + ".png").ToLower();
            ShowMinutes = true;
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private static string GetEventMinute(EventOfMatch eventOfMatch)
        {
            string liveMatchTime = string.Empty;
            if (eventOfMatch?.MatchID != null)
            {
                int minutes = 0;
                int seconds = 0;
                string[] totalTimeArray = eventOfMatch?.EventTotalTime?.Split(':');
                if (totalTimeArray != null && totalTimeArray.Length == 2)
                {
                    string totalMinutes = totalTimeArray[0].TrimStart('0').TrimStart('0');
                    totalMinutes = string.IsNullOrWhiteSpace(totalMinutes) ? "0" : totalMinutes;
                    minutes = int.Parse(totalMinutes) + 1;
                    string totalSeconds = totalTimeArray[1].TrimStart('0').TrimStart('0');
                    totalSeconds = string.IsNullOrWhiteSpace(totalSeconds) ? "0" : totalSeconds;
                    seconds = int.Parse(totalSeconds);
                }

                string addTime = string.Empty;
                switch (eventOfMatch.EventPeriodID)
                {
                    case 1:
                        return "00'";
                    case 2:
                        if (minutes > 45)
                        {
                            addTime = "45+";
                            minutes = minutes - 45;
                        }
                        break;
                    case 3:
                        return "46'";
                    case 4:
                        if (minutes > 90)
                        {
                            addTime = "90+";
                            minutes = minutes - 90;
                        }
                        break;
                    case 5:
                        return "90'";
                    case 6:
                        if (minutes > 105)
                        {
                            addTime = "105+";
                            minutes = minutes - 105;
                        }
                        break;
                    case 7:
                        return "106'";
                    case 8:
                        if (minutes > 120)
                        {
                            addTime = "120+";
                            minutes = minutes - 120;
                        }
                        break;
                    case 9:
                        return "120'";
                    case 10:
                        return "120'";
                    default:
                        return "120'";
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                {
                    min = "0" + minutes;
                }
                if (seconds < 10)
                {
                    sec = "0" + seconds;
                }

                liveMatchTime = addTime + " " + min + ":" + sec;
            }
            return (liveMatchTime.Split(':')?[0] + "'").Trim();
        }
    }
}