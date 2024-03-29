﻿using DystirWeb.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace DystirWeb.ViewBases
{
    public class MatchBase : ComponentBase
    {
        [Parameter]
        public Matches MatchItem { get; set; }

        [Parameter]
        public bool IsMatchForSameDayList { get; set; }

        [Parameter]
        public bool IsMatchInDetails { get; set; }

        [Parameter]
        public int TimeZoneOffset { get; set; }

        [Parameter]
        public bool ShowMatchType { get; set; }

        [Parameter]
        public bool IsSelectedMatch { get; set; }

        [Parameter]
        public int NumberOfMatches { get; set; }

        [Parameter]
        public string Page { get; set; }


        public string GetMatchTime(DateTime? statusTime, DateTime? matchDateTime, int? statusId)
        {
            try {
                var timeNow = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
                var matchTime = TimeSpan.FromTicks(statusTime.Value.Ticks);
                var matchStart = TimeSpan.FromTicks(matchDateTime.Value.Ticks);
                double totalMiliseconds = (timeNow - matchTime).TotalMilliseconds;
                double seconds = Math.Floor(totalMiliseconds / 1000);
                double minutes = Math.Floor(seconds / 60);
                seconds -= minutes * 60;
                double milsecToStart = (matchStart - timeNow).TotalMilliseconds;
                return MatchPeriod(minutes, seconds, statusId, milsecToStart);
            } 
            catch
            {
                return string.Empty;
            }
        }

        public string MatchPeriod(double minutes, double seconds, int? matchStatus, double milsecToStart)
        {
            //bool isDisconnected = DystirService.DystirHubConnection?.State != HubConnectionState.Connected;
            bool isDisconnected = false;
            if (isDisconnected && matchStatus > 1 && matchStatus < 12)
            {
                return "";
            }

            var addtime = "";
            switch (matchStatus)
            {
                case 1:
                    return GetTimeToStart(milsecToStart, "00:00");
                case 2:
                    if (minutes >= 45)
                    {
                        addtime = "45+";
                        minutes = minutes - 45;
                    }
                    break;
                case 3:
                    return "hálvleikur";
                case 4:
                    minutes = minutes + 45;
                    if (minutes >= 90)
                    {
                        addtime = "90+";
                        minutes = minutes - 90;
                    }
                    break;
                case 5:
                    return "liðugt";
                case 6:
                    minutes = minutes + 90;
                    if (minutes >= 105)
                    {
                        addtime = "105+";
                        minutes = minutes - 105;
                    }
                    break;
                case 7:
                    return "longd leiktíð hálvleikur";
                case 8:
                    minutes = minutes + 105;
                    if (minutes >= 120)
                    {
                        addtime = "120+";
                        minutes = minutes - 120;
                    }
                    break;
                case 9:
                    return "longd leiktíð liðugt";
                case 10:
                    return "brotsspark";
                case 11:
                case 12:
                case 13:
                    return "liðugt";
                default:
                    return GetTimeToStart(milsecToStart, "-- : --");
            }
            string min = minutes.ToString();
            string sec = seconds.ToString();
            if (minutes < 10)
                min = "0" + minutes.ToString();
            if (seconds < 10)
                sec = "0" + seconds.ToString();
            return addtime + " " + min + ":" + sec;
        }

        private string GetTimeToStart(double milsecToStart, string defaultText)
        {
            var minutesToStart = Math.Ceiling(milsecToStart / 60000);
            if (minutesToStart > 0)
            {
                var days = Math.Floor(minutesToStart / 1440);
                var hours = Math.Floor((minutesToStart - days * 1440) / 60);
                var minutes = minutesToStart - days * 1440 - hours * 60;
                if (days > 0)
                {
                    return string.Format("{0} d. {1} t.", days, hours);
                }
                else
                {
                    var hoursText = hours > 0 ? hours + " t. " : "";
                    return hoursText + minutes + " m.";
                }
            }
            else
            {
                return defaultText;
            }
        }

        public string GetStatusColor(int? statusId)
        {
            switch (statusId)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return "limegreen";
                case 11:
                case 12:
                case 13:
                    return "salmon";
                default:
                    return "khaki";
            }
        }

        public string GetMatchInfoTime(DateTime? matchDateTime)
        {
            if(matchDateTime == null)
            {
                return "";
            }
            var ticks = (DateTime.UtcNow.Ticks - matchDateTime.Value.Ticks);
            return TimeSpan.FromTicks(ticks).TotalMinutes.ToString();
        }

        public string MatchItemWidth(string page, int numberOfMatches, bool isMatchInDetails)
        {
            if (page == "portal" || page == "midlar" || page == "roysni")
            {
                if (isMatchInDetails == true)
                {
                    if (numberOfMatches > 0)
                    {
                        return 100 / numberOfMatches - numberOfMatches * 0.1 + "%";
                    }
                }
                else
                {
                    return "99.9%";
                }
            }
            else if (numberOfMatches > 0)
            {
                return 100 / numberOfMatches - numberOfMatches * 0.1 + "%";
            }
            return "0px";
        }

        public string GetMatchDayName()
        {
            var dayName = MatchItem?.Time?.AddMinutes(-TimeZoneOffset).ToString("ddd", new CultureInfo("en-US"));
            return string.IsNullOrEmpty(dayName) ? "" : ConvertToFaroese(dayName) + ". ";
        }

        private string ConvertToFaroese(string day)
        {
            switch (day.ToLower()) {
                case "mon":
                    return "Mán";
                case "tue":
                    return "Týs";
                case "wed":
                    return "Mik";
                case "thu":
                    return "Hós";
                case "fri":
                    return "Frí";
                case "sat":
                    return "Ley";
                case "sun":
                    return "Sun";
                default:
                    return day;
            }
        }
    }
}
