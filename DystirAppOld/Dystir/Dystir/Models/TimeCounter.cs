using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dystir.ViewModels;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class TimeCounter
    {
        

        internal string MatchPeriod(Match match)
        {
            try
            {
                if (match.StatusTime == null)
                {
                    return "";
                }
                double secondsToStart = (double)(match.Time?.ToLocalTime() - DateTime.Now.ToLocalTime())?.TotalSeconds;
                double totalSeconds = (double)(DateTime.Now.ToLocalTime() - match.StatusTime?.ToLocalTime())?.TotalSeconds;
                int minutes = (int)Math.Floor(totalSeconds / 60);
                int seconds = (int)totalSeconds - minutes * 60;
                int? matchStatus = match.StatusID;
                var addtime = "";
                switch (matchStatus)
                {
                    case 0:
                        return GetTimeToStart(secondsToStart, "--:--");
                    case 1:
                        return GetTimeToStart(secondsToStart, "00:00");
                    case 2:
                        if (minutes > 45)
                        {
                            addtime = "45+";
                            minutes -= 45;
                        }
                        break;
                    case 3:
                        return Properties.Resources.HalfTime;
                    case 4:
                        minutes += 45;
                        if (minutes > 90)
                        {
                            addtime = "90+";
                            minutes -= 90;
                        }
                        break;
                    case 5:
                        return Properties.Resources.FullTime;
                    case 6:
                        minutes += 90;
                        if (minutes > 105)
                        {
                            addtime = "105+";
                            minutes -= 105;
                        }
                        break;
                    case 7:
                        return Properties.Resources.ExtraTimePause;
                    case 8:
                        minutes += 105;
                        if (minutes > 120)
                        {
                            addtime = "120+";
                            minutes -= 120;
                        }
                        break;
                    case 9:
                        return Properties.Resources.ExtraTimeFinished;
                    case 10:
                        return Properties.Resources.Penalties;
                    default:
                        return Properties.Resources.Finished;
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                    min = "0" + min;
                if (seconds < 10)
                    sec = "0" + sec;
                return addtime + " " + min + ":" + sec;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetTimeToStart(double secToStart, string defaultText)
        {
            var minutesToStart = Math.Ceiling(secToStart / 60);
            if (minutesToStart > 0)
            {
                var days = Math.Floor(minutesToStart / 1440);
                var hours = Math.Floor((minutesToStart - days * 1440) / 60);
                var minutes = minutesToStart - days * 1440 - hours * 60;

                if (days > 0)
                {
                    return days + " d.";
                }
                else
                {
                    var hoursText = hours > 0 ? hours + " " + Properties.Resources.HoursShort + " " : "";
                    return hoursText + minutes + " m.";
                }
            }
            else
            {
                return defaultText;
            }
        }

        private void ShowLiveColor(Match match, bool isBlinkingStatus)
        {
            switch (match.StatusID)
            {
                case 1:
                    match.StatusColor = "DarkKhaki";
                    if(isBlinkingStatus)
                    {
                        match.StatusBlinkingColor = "DarkKhaki";
                    }
                    break;
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                    match.StatusColor = "Green";
                    if (isBlinkingStatus)
                    {
                        match.StatusBlinkingColor = match.StatusBlinkingColor == "LightGreen" ? "LimeGreen" : "LightGreen";
                    }
                    break;
                case 3:
                case 5:
                case 7:
                case 9:
                    match.StatusColor = "Green";
                    if (isBlinkingStatus)
                    {
                        match.StatusBlinkingColor = "LightGreen";
                    }
                    break;
                default:
                    match.StatusColor = "DarkRed";
                    if (isBlinkingStatus)
                    {
                        match.StatusBlinkingColor = "DarkRed";
                    }
                    break;
            }
        }

        private void ShowLiveStandingsColorBlinking(TeamStanding teamStanding)
        {
            if (teamStanding.LiveColor == "LightGreen")
                teamStanding.LiveColor = "LimeGreen";
            else
                teamStanding.LiveColor = "LightGreen";
        }
    }
}
