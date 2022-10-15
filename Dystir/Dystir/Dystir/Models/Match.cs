using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Dystir.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class Match : INotifyPropertyChanged
    {
        //----------------------------//
        //          Properties        //
        //----------------------------//
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

        [JsonProperty("StatusID")]
        public int? StatusID { get; set; }

        [JsonProperty("HomeTeamScore")]
        public int? HomeTeamScore { get; set; }

        [JsonProperty("AwayTeamScore")]
        public int? AwayTeamScore { get; set; }

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

        private DateTime? _statusTime;
        [JsonProperty("StatusTime")]
        public DateTime? StatusTime
        {
            get { return _statusTime; }
            set { SetProperty(ref _statusTime, value); SetMatchTime(); }
        }

        [JsonProperty("MatchTypeID")]
        public Nullable<int> MatchTypeID { get; set; }

        [JsonProperty("TeamAdminID")]
        public int? TeamAdminID { get; set; }

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        private string _roundName;
        [JsonProperty("RoundName")]
        public string RoundName
        {
            get { return _roundName;}
            set { SetProperty(ref _roundName, value); SetMatchInfo();}
        }

        private string homeTeamLogo;
        [JsonProperty("HomeTeamLogo")]
        public string HomeTeamLogo
        {
            get { return homeTeamLogo; }
            set { SetProperty(ref homeTeamLogo, "https://www.dystir.fo/team_logos/" + value); }
        }

        private string awayTeamLogo;
        [JsonProperty("AwayTeamLogo")]
        public string AwayTeamLogo
        {
            get { return awayTeamLogo; }
            set { SetProperty(ref awayTeamLogo, "https://www.dystir.fo/team_logos/" + value); }
        }

        string _matchTime = "";
        public string MatchTime
        {
            get { return _matchTime; }
            set { SetProperty(ref _matchTime, value); }
        }

        CultureInfo _languageCode;
        public CultureInfo LanguageCode
        {
            get { return _languageCode; }
            set { SetProperty(ref _languageCode, value); }
        }

        string _statusColor = "Transparent";
        public string StatusColor
        {
            get { return _statusColor; }
            set { SetProperty(ref _statusColor, value); }
        }

        string _statusBlinkingColor = "Transparent";
        public string StatusBlinkingColor
        {
            get { return _statusBlinkingColor; }
            set { SetProperty(ref _statusBlinkingColor, value); }
        }

        public string MatchInfo { get; set;}


        MatchDetails _details;
        public MatchDetails Details
        {
            get { return _details; }
            set { SetProperty(ref _details, value); }
        }

        FullMatchDetails _fullMatchDetails;
        public FullMatchDetails FullMatchDetails
        {
            get { return _fullMatchDetails; }
            set { SetProperty(ref _fullMatchDetails, value); }
        }

        int _detailsMatchTabIndex = 0;
        public int DetailsMatchTabIndex
        {
            get { return _detailsMatchTabIndex; }
            set { SetProperty(ref _detailsMatchTabIndex, value); SetDetailsTabSelected(value); }
        }

        bool _summarySelected = true;
        public bool SummarySelected
        {
            get { return _summarySelected; }
            set { SetProperty(ref _summarySelected, value); }
        }

        bool _commentarySelected = false;
        public bool CommentarySelected
        {
            get { return _commentarySelected; }
            set { SetProperty(ref _commentarySelected, value); }
        }

        bool _firstElevenSelected = false;
        public bool FirstElevenSelected
        {
            get { return _firstElevenSelected; }
            set { SetProperty(ref _firstElevenSelected, value); }
        }

        bool _statisticSelected = false;
        public bool StatisticSelected
        {
            get { return _statisticSelected; }
            set { SetProperty(ref _statisticSelected, value); }
        }


        //----------------------------//
        //         Constructor        //
        //----------------------------//
        public Match()
        {
            TimeService timeService = DependencyService.Get<TimeService>();
            timeService.OnTimerElapsed += OnTimerElapsed;
            LanguageService languageService = DependencyService.Get<LanguageService>();
            languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;
        }

        //----------------------------//
        //       Private Methods      //
        //----------------------------//
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

        private string GetTimeStatusColor()
        {
            switch (StatusID)
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
                    return "LimeGreen";
                case 11:
                case 12:
                case 13:
                    return "Salmon";
                case 1:
                    return "Khaki";
                default:
                    return "Transparent";
            }
        }

        private void ShowLiveStandingsColorBlinking(TeamStanding teamStanding)
        {
            if (teamStanding.LiveColor == "LightGreen")
                teamStanding.LiveColor = "LimeGreen";
            else
                teamStanding.LiveColor = "LightGreen";
        }

        private void SetMatchInfo()
        {
            string matchInfo = string.Empty;
            string matchTime = Time?.ToLocalTime().ToString("HH:mm - ");
            if (Time?.Hour == 0 && Time?.Minute == 0)
            {
                matchTime = string.Empty;
            }
            string matchRound = !string.IsNullOrWhiteSpace(RoundName) ? RoundName + " - " : string.Empty;
            string matchLocation = Location;
            matchInfo = matchTime + matchRound + matchLocation;
            MatchInfo = matchInfo.Trim();
        }

        private void SetDetailsTabSelected(int detailsMatchTabIndex)
        {
            SummarySelected = false;
            FirstElevenSelected = false;
            CommentarySelected = false;
            StatisticSelected = false;

            switch (detailsMatchTabIndex)
            {
                case 0:
                default:
                    SummarySelected = true;
                    break;
                case 1:
                    FirstElevenSelected = true;
                    break;
                case 2:
                    CommentarySelected = true;
                    break;
                case 3:
                    StatisticSelected = true;
                    break;
            }
        }


        //----------------------------//
        //    INotifyPropertyChanged  //
        //----------------------------//
        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
