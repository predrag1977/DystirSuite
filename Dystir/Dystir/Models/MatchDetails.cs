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
        //*****************************//
        //         PROPERTIES          //
        //*****************************//
        [DataMember]
        public Match Match { get; set; }
        [DataMember]
        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; }
        [DataMember]
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; }
        [DataMember]
        public int MatchDetailsID { get; set; }

        ObservableCollection<SummaryEventOfMatch> summary;
        public ObservableCollection<SummaryEventOfMatch> Summary
        {
            get { return summary; }
            set { SetProperty(ref summary, value); }
        }

        ObservableCollection<SummaryEventOfMatch> commentary;
        public ObservableCollection<SummaryEventOfMatch> Commentary
        {
            get { return commentary; }
            set { SetProperty(ref commentary, value); }
        }

        public Statistic Statistics { get; set; }

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

        public ObservableCollection<PlayerOfMatch> HomeTeamLineups { get; set; }
        public ObservableCollection<PlayerOfMatch> AwayTeamLineups { get; set; }

        //*****************************//
        //    INotifyPropertyChanged   //
        //*****************************//
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