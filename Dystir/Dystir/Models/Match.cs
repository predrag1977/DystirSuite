using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Dystir.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Dystir.Models;
using System.Linq;
using System.Reflection;

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

        [JsonProperty("MatchTypeID")]
        public int? MatchTypeID { get; set;}

        [JsonProperty("TeamAdminID")]
        int? TeamAdminID { get; set; }

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        private DateTime? statusTime;
        [JsonProperty("StatusTime")]
        public DateTime? StatusTime
        {
            get { return statusTime; }
            set { statusTime  = value; SetMatchTime(); }
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

        string matchTime = string.Empty;
        public string MatchTime
        {
            get { return matchTime; }
            set { matchTime = value; OnPropertyChanged(); }
        }

        CultureInfo _languageCode;
        public CultureInfo LanguageCode
        {
            get { return _languageCode; }
            set { _languageCode = value; OnPropertyChanged();}
        }

        Color statusColor = Colors.Transparent;
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

        MatchDetails _matchDetails;
        public MatchDetails MatchDetails
        {
            get { return _matchDetails; }
            set { _matchDetails = value; OnPropertyChanged(); }
        }

        bool _summarySelected = true;
        public bool SummarySelected
        {
            get { return _summarySelected; }
            set { _summarySelected = value; OnPropertyChanged();}
        }

        bool _firstElevenSelected = false;
        public bool FirstElevenSelected
        {
            get { return _firstElevenSelected; }
            set { _firstElevenSelected = value; OnPropertyChanged();}
        }

        bool _statisticSelected = false;
        public bool StatisticSelected
        {
            get { return _statisticSelected; }
            set { _statisticSelected = value; OnPropertyChanged();}
        }

        bool _commentarySelected = false;
        public bool CommentarySelected
        {
            get { return _commentarySelected; }
            set { _commentarySelected = value; OnPropertyChanged();}
        }

        bool liveStandingsSelected = false;
        public bool LiveStandingsSelected
        {
            get { return liveStandingsSelected; }
            set { liveStandingsSelected = value; OnPropertyChanged();}
        }

        MatchDetailsTab selectedMatchDetailsTab = new MatchDetailsTab()
        {
            TabName = Resources.Localization.Resources.Summary,
            TextColor = Colors.LimeGreen
        };
        public MatchDetailsTab SelectedMatchDetailsTab
        {
            get { return selectedMatchDetailsTab; }
            set { selectedMatchDetailsTab = value; SetDetailsTabSelected(value); OnPropertyChanged();}
        }

        ObservableCollection<MatchDetailsTab> matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
        public ObservableCollection<MatchDetailsTab> MatchDetailsTabs
        {
            get { return matchDetailsTabs; }
            set { matchDetailsTabs = value; OnPropertyChanged();}
        }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged();}
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public Match()
        {
            TimeService timeService = DependencyService.Get<TimeService>();
            timeService.OnTimerElapsed += OnTimerElapsed;
            LanguageService languageService = DependencyService.Get<LanguageService>();
            //languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;
            SetMatchTime();
            SetMatchDetailsTabs();
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
                    return Colors.Khaki;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return Colors.LimeGreen;
                case 11:
                case 12:
                case 13:
                    return Colors.Salmon;
                default:
                    return Colors.Transparent;
            }
        }

        private void ShowLiveStandingsColorBlinking(TeamStanding teamStanding)
        {
            if (teamStanding.LiveColor == "LightGreen")
                teamStanding.LiveColor = "LimeGreen";
            else
                teamStanding.LiveColor = "LightGreen";
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
            string matchInfo = string.Empty;
            string matchTime = Time?.ToLocalTime().ToString("dd.MM. HH:mm - ");
            if (Time?.Hour == 0 && Time?.Minute == 0)
            {
                matchTime = string.Empty;
            }
            string matchRound = !string.IsNullOrWhiteSpace(RoundName) ? RoundName + " - " : string.Empty;
            string matchLocation = Location;
            matchInfo = matchTime + matchRound + matchLocation;
            MatchInfo = matchInfo.Trim();
        }

        private void SetMatchDetailsTabs()
        {
            var matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
            matchDetailsTabs.Add(new MatchDetailsTab()
            {
                TabIndex = matchDetailsTabs.Count,
                TabName = Resources.Localization.Resources.Summary,
                TextColor = Colors.LimeGreen
            });
            matchDetailsTabs.Add(new MatchDetailsTab()
            {
                TabIndex = matchDetailsTabs.Count,
                TabName = Resources.Localization.Resources.FirstEleven
            });
            matchDetailsTabs.Add(new MatchDetailsTab()
            {
                TabIndex = matchDetailsTabs.Count,
                TabName = Resources.Localization.Resources.Commentary
            });
            matchDetailsTabs.Add(new MatchDetailsTab()
            {
                TabIndex = matchDetailsTabs.Count,
                TabName = Resources.Localization.Resources.Statistics
            });
            matchDetailsTabs.Add(new MatchDetailsTab()
            {
                TabIndex = matchDetailsTabs.Count,
                TabName = Resources.Localization.Resources.StandingsTab
            });
            MatchDetailsTabs = new ObservableCollection<MatchDetailsTab>(matchDetailsTabs);
        }

        private void SetDetailsTabSelected(MatchDetailsTab selectedMatchDetailsMatchTab)
        {
            SummarySelected = selectedMatchDetailsMatchTab.TabIndex == 0;
            FirstElevenSelected = selectedMatchDetailsMatchTab.TabIndex == 1;
            CommentarySelected = selectedMatchDetailsMatchTab.TabIndex == 2;
            StatisticSelected = selectedMatchDetailsMatchTab.TabIndex == 3;
            LiveStandingsSelected = selectedMatchDetailsMatchTab.TabIndex == 4;

            foreach (MatchDetailsTab matchDetailsTab in MatchDetailsTabs)
            {
                matchDetailsTab.TextColor = matchDetailsTab.TabIndex == selectedMatchDetailsMatchTab.TabIndex ? Colors.LimeGreen : Colors.White;
            }
        }

        //**************************//
        //  INotifyPropertyChanged  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
