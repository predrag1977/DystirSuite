using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Dystir.Models
{
    public class TeamStanding : INotifyPropertyChanged
    {
        [JsonProperty("Team")]
        public string Team { get; internal set; }
        [JsonProperty("TeamID")]
        public int TeamID { get; internal set; }
        [JsonProperty("MatchesNo")]
        public int? MatchesNo { get; internal set; } = 0;
        [JsonProperty("Points")]
        public int? Points { get; internal set; } = 0;
        [JsonProperty("GoalScored")]
        public int? GoalScored { get; internal set; } = 0;
        [JsonProperty("GoalAgainst")]
        public int? GoalAgainst { get; internal set; } = 0;
        [JsonProperty("GoalDifference")]
        public int? GoalDifference { get; internal set; } = 0;
        [JsonProperty("PointsProcent")]
        public double? PointsProcent { get; internal set; }
        [JsonProperty("Victories")]
        public int Victories { get; internal set; }
        [JsonProperty("Draws")]
        public int Draws { get; internal set; }
        [JsonProperty("Losses")]
        public int Losses { get; internal set; }
        [JsonProperty("CompetitionName")]
        public string CompetitionName { get; internal set; }
        [JsonProperty("Position")]
        public int Position { get; internal set; }
        [JsonProperty("PositionColor")]
        public string PositionColor { get; internal set; }
        [JsonProperty("IsLive")]
        public bool IsLive { get; internal set; }

        string _liveColor = "Transparent";
        public string LiveColor
        {
            get { return _liveColor; }
            set { SetProperty(ref _liveColor, value); }
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