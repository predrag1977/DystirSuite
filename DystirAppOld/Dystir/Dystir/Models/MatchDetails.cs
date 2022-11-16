using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Dystir.Models
{
    [DataContract]
    public class MatchDetails : INotifyPropertyChanged
    {
        [DataMember]
        public Match Match { get; set; }
        [DataMember]
        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; } = new ObservableCollection<EventOfMatch>();
        [DataMember]
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; } = new ObservableCollection<PlayerOfMatch>();
        [DataMember]
        public int MatchDetailsID { get; set; }

        bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            set { SetProperty(ref _isDataLoaded, value); }
        }

        int _detailsMatchTabIndex = -1;
        public int DetailsMatchTabIndex
        {
            get { return _detailsMatchTabIndex; }
            set { SetProperty(ref _detailsMatchTabIndex, value); }
        }

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
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


        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
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