using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DystirXamarin.Models
{
    public class PlayerOfMatch : INotifyPropertyChanged
    {
        [JsonProperty("PlayerOfMatchID")]
        public int PlayerOfMatchID { get; set; }
        
        string _firstName;
        [JsonProperty("FirstName")]
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        string _lastName;
        [JsonProperty("Lastname")]
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        [JsonProperty("TeamName")]
        public string TeamName { get; internal set; }

        int? _number;
        [JsonProperty("Number")]
        public int? Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        int? _playingStatus;
        [JsonProperty("PlayingStatus")]
        public int? PlayingStatus {
            get { return _playingStatus; }
            set { SetProperty(ref _playingStatus, value); }
        }

        string _position;
        [JsonProperty("Position")]
        public string Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        [JsonProperty("PlayerID")]
        public int? PlayerID { get; set; }

        [JsonProperty("MatchID")]
        public int? MatchID { get; set; }

        [JsonProperty("TeamID")]
        public int? TeamID { get; set; }

        [JsonProperty("MatchTypeID")]
        public int? MatchTypeID { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; internal set; }

        [JsonProperty("Goal")]
        public int? Goal { get; set; }

        [JsonProperty("YellowCard")]
        public int? YellowCard { get; set; }

        [JsonProperty("RedCard")]
        public int? RedCard { get; set; }

        [JsonProperty("SubIN")]
        public int? SubIN { get; set; }

        [JsonProperty("SubOUT")]
        public int? SubOUT { get; set; }

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