using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dystir.Services;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class Match : INotifyPropertyChanged
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        [JsonProperty("HomeTeam")]
        public string HomeTeam { get; internal set; }

        [JsonProperty("AwayTeam")]
        public string AwayTeam { get; internal set; }

        [JsonProperty("HomeCategoriesName")]
        public string HomeCategoriesName { get; internal set; }

        [JsonProperty("AwayCategoriesName")]
        public string AwayCategoriesName { get; internal set; }

        [JsonProperty("HomeSquadName")]
        public string HomeSquadName { get; internal set; }

        [JsonProperty("AwaySquadName")]
        public string AwaySquadName { get; internal set; }

        [JsonProperty("MatchID")]
        public int MatchID { get; set; }

        [JsonProperty("Time")]
        public DateTime? Time { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }
        
        private int? statusID;
        [JsonProperty("StatusID")]
        public int? StatusID
        {
            get { return statusID; }
            set { statusID = value; SetCorrectHomeTeamScore(); SetCorrectAwayTeamScore(); }
        }

        [JsonProperty("HomeTeamScore")]
        public int? HomeTeamScore { get; set; }

        [JsonProperty("AwayTeamScore")]
        public int? AwayTeamScore { get; set; }

        private int? homeTeamPenaltiesScore;
        [JsonProperty("HomeTeamPenaltiesScore")]
        public int? HomeTeamPenaltiesScore
        {
            get { return homeTeamPenaltiesScore; }
            set { homeTeamPenaltiesScore = value; SetCorrectHomeTeamScore(); }
        }

        private int? awayTeamPenaltiesScore;
        [JsonProperty("AwayTeamPenaltiesScore")]
        public int? AwayTeamPenaltiesScore
        {
            get { return awayTeamPenaltiesScore;  }
            set { awayTeamPenaltiesScore = value; SetCorrectAwayTeamScore(); }
        }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; set; }

        [JsonProperty("HomeTeamID")]
        public int? HomeTeamID { get; set; }

        [JsonProperty("AwayTeamID")]
        public int? AwayTeamID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }

        [JsonProperty("MatchActivation")]
        public int? MatchActivation { get; set; }

        [JsonProperty("MatchTypeID")]
        public int? MatchTypeID { get; set;}

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        private DateTime? statusTime;
        [JsonProperty("StatusTime")]
        public DateTime? StatusTime
        {
            get { return statusTime; }
            set { statusTime  = value; SetMatchTime();}
        }

        private string roundName;
        [JsonProperty("RoundName")]
        public string RoundName
        {
            get { return roundName;}
            set { roundName  = value; SetMatchAdditionalInfo();}
        }

        private string homeTeamLogo;
        [JsonProperty("HomeTeamLogo")]
        public string HomeTeamLogo
        {
            get { return homeTeamLogo; }
            set { homeTeamLogo = "https://www.dystir.fo/team_logos/" + value; }
        }

        private string awayTeamLogo;
        [JsonProperty("AwayTeamLogo")]
        public string AwayTeamLogo
        {
            get { return awayTeamLogo; }
            set { awayTeamLogo = "https://www.dystir.fo/team_logos/" + value; }
        }

        public string HomeTeamFullName { get; internal set; }
        public string AwayTeamFullName { get; internal set; }

        public string HomeTeamScoreText { get; set; }
        public string AwayTeamScoreText { get; set; }

        public string HomeTeamPenaltiesScoreText { get; set; }
        public string AwayTeamPenaltiesScoreText { get; set; }

        string matchTime = string.Empty;
        public string MatchTime
        {
            get { return matchTime; }
            set { matchTime = value; OnPropertyChanged(); }
        }

        Color statusColor = Color.Transparent;
        public Color StatusColor
        {
            get { return statusColor; }
            set { statusColor = value; OnPropertyChanged();}
        }

        string matchInfo = string.Empty;
        public string MatchInfo
        {
            get { return matchInfo; }
            set { matchInfo = value; OnPropertyChanged();}
        }

        string fullMatchInfo = string.Empty;
        public string FullMatchInfo
        {
            get { return fullMatchInfo; }
            set { fullMatchInfo = value; OnPropertyChanged(); }
        }

        bool goalVisible;
        public bool GoalVisible
        {
            get { return goalVisible; }
            set { goalVisible = value; OnPropertyChanged(); }
        }

        bool matchTimeVisible;
        public bool MatchTimeVisible
        {
            get { return matchTimeVisible; }
            set { matchTimeVisible = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public Match()
        {
            TimeService timeService = DependencyService.Get<TimeService>();
            timeService.OnTimerElapsed += OnTimerElapsed;
            LanguageService languageService = DependencyService.Get<LanguageService>();
            languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;
            SetMatchTime();
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private void OnTimerElapsed()
        {
            SetMatchTime();
        }

        private void LanguageServiceOnLanguageChanged()
        {
            
        }

        private void SetMatchTime()
        {
            MatchTime = GetMatchTime();
            StatusColor = GetTimeStatusColor();
            MatchTimeVisible = StatusID > 0 && StatusID < 14;
        }

        private string GetMatchTime()
        {
            try
            {
                if (StatusTime == null)
                {
                    return "";
                }
                double secondsToStart = (double)(Time?.ToLocalTime() - DateTime.Now.ToLocalTime())?.TotalSeconds;
                double totalSeconds = (double)(DateTime.Now.ToLocalTime() - StatusTime?.ToLocalTime())?.TotalSeconds;
                int minutes = (int)Math.Floor(totalSeconds / 60);
                int seconds = (int)totalSeconds - minutes * 60;
                int? matchStatus = StatusID;
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
                        return Resources.Localization.Resources.HalfTime;
                    case 4:
                        minutes += 45;
                        if (minutes > 90)
                        {
                            addtime = "90+";
                            minutes -= 90;
                        }
                        break;
                    case 5:
                        return Resources.Localization.Resources.FullTime;
                    case 6:
                        minutes += 90;
                        if (minutes > 105)
                        {
                            addtime = "105+";
                            minutes -= 105;
                        }
                        break;
                    case 7:
                        return Resources.Localization.Resources.ExtraTimePause;
                    case 8:
                        minutes += 105;
                        if (minutes > 120)
                        {
                            addtime = "120+";
                            minutes -= 120;
                        }
                        break;
                    case 9:
                        return Resources.Localization.Resources.ExtraTimeFinished;
                    case 10:
                        return Resources.Localization.Resources.Penalties;
                    case 11:
                    case 12:
                    case 13:
                        return Resources.Localization.Resources.Finished;
                    default:
                        return string.Empty;
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                    min = "0" + min;
                if (seconds < 10)
                    sec = "0" + sec;
                return addtime + " " + min + "'" + sec;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetTimeToStart(double secToStart, string defaultText)
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
                    var hoursText = hours > 0 ? hours + " " + Resources.Localization.Resources.HoursShort + " " : "";
                    return hoursText + minutes + " m.";
                }
            }
            else
            {
                return defaultText;
            }
        }

        private Color GetTimeStatusColor()
        {
            switch (StatusID)
            {
                case 1:
                    return Color.Khaki;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return Color.LimeGreen;
                case 11:
                case 12:
                case 13:
                    return Color.Salmon;
                default:
                    return Color.Transparent;
            }
        }

        private void SetMatchAdditionalInfo()
        {
            SetFullTeamsName();
            SetMatchInfo();
        }

        private void SetFullTeamsName()
        {
            HomeTeamFullName = string.Format($"{HomeTeam} {HomeCategoriesName} {HomeSquadName}").Trim();
            AwayTeamFullName = string.Format($"{AwayTeam} {AwayCategoriesName} {AwaySquadName}").Trim();
        }

        private void SetMatchInfo()
        {
            string matchTime = Time?.ToLocalTime().ToString("dd.MM. HH:mm - ");
            if (Time?.Hour == 0 && Time?.Minute == 0)
            {
                matchTime = Time?.ToLocalTime().ToString("dd.MM. - ");
            }
            string matchType = !string.IsNullOrWhiteSpace(MatchTypeName) ? MatchTypeName + " - " : string.Empty;
            string matchRound = RoundName ?? string.Empty;

            var matchInfo = matchTime + matchType + matchRound;
            MatchInfo = matchInfo.Trim();

            matchRound = !string.IsNullOrWhiteSpace(matchRound) ? matchRound + " - " : string.Empty;
            string location = Location;

            var fullMatchInfo = matchTime + matchType + matchRound + location;
            FullMatchInfo = fullMatchInfo.Trim();
        }

        private void SetCorrectHomeTeamScore()
        {
            if ((StatusID ?? 0) > 1 && (StatusID ?? 0) < 14)
            {
                var homeTeamScore = (HomeTeamScore ?? 0) - (HomeTeamPenaltiesScore ?? 0);
                HomeTeamScoreText = (homeTeamScore < 0 ? 0 : homeTeamScore).ToString();
                SetPenaltyScore();
            }
            else
            {
                HomeTeamScoreText = "-";
            }
        }

        private void SetCorrectAwayTeamScore()
        {
            if ((StatusID ?? 0) > 1 && (StatusID ?? 0) < 14)
            {
                var awayTeamScore = (AwayTeamScore ?? 0) - (AwayTeamPenaltiesScore ?? 0);
                AwayTeamScoreText = (awayTeamScore < 0 ? 0 : awayTeamScore).ToString();
                SetPenaltyScore();
            }
            else
            {
                AwayTeamScoreText = "-";
            }
        }

        private void SetPenaltyScore()
        {
            HomeTeamPenaltiesScoreText = string.Empty;
            AwayTeamPenaltiesScoreText = string.Empty;
            if ((HomeTeamPenaltiesScore ?? 0) > 0 || (AwayTeamPenaltiesScore ?? 0) > 0)
            {
                HomeTeamPenaltiesScoreText = string.Format("({0})", (HomeTeamPenaltiesScore ?? 0).ToString());
                AwayTeamPenaltiesScoreText = string.Format("({0})", (AwayTeamPenaltiesScore ?? 0).ToString());
            }
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
