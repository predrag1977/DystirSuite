using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class Match : INotifyPropertyChanged
    {
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

        [JsonProperty("StatusTime")]
        public DateTime? StatusTime { get; set; }

        [JsonProperty("MatchTypeID")]
        public Nullable<int> MatchTypeID { get; set; }

        [JsonProperty("TeamAdminID")]
        public int? TeamAdminID { get; set; }

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }

        string _liveTime = "";
        public string LiveTime
        {
            get { return _liveTime; }
            set { SetProperty(ref _liveTime, value); }
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

        #region INotifyPropertyChanged
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
