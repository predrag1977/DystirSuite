using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Dystir.Models
{
    [DataContract]
    public class TeamStanding : INotifyPropertyChanged
    {
        [DataMember]
        public string Team { get; set; }
        [DataMember]
        public int TeamID { get; set; }
        [DataMember]
        public int? MatchesNo { get; set; } = 0;
        [DataMember]
        public int? Points { get; set; } = 0;
        [DataMember]
        public int? GoalScored { get; set; } = 0;
        [DataMember]
        public int? GoalAgainst { get; set; } = 0;
        [DataMember]
        public int? GoalDifference { get; set; } = 0;
        [DataMember]
        public double? PointsProcent { get; set; }
        [DataMember]
        public int Victories { get; set; }
        [DataMember]
        public int Draws { get; set; }
        [DataMember]
        public int Losses { get; set; }
        [DataMember]
        public string CompetitionName { get; set; }
        [DataMember]
        public int Position { get; set; }
        [DataMember]
        public string PositionColor { get; set; }
        [DataMember]
        public bool IsLive { get; set; }

        string _liveColor = "Transparent";
        public string LiveColor
        {
            get { return _liveColor; }
            set { SetProperty(ref _liveColor, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
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