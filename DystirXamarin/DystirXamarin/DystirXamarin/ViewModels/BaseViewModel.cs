using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DystirXamarin.Models;
using System.Collections.ObjectModel;

namespace DystirXamarin.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        ObservableCollection<Match> _allMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> AllMatches
        {
            get { return _allMatches; }
            set { SetProperty(ref _allMatches, value); }
        }

        ObservableCollection<Match> _matches = new ObservableCollection<Match>();
        public ObservableCollection<Match> Matches
        {
            get { return _matches; }
            set { SetProperty(ref _matches, value); }
        }

        ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        public ObservableCollection<Team> Teams
        {
            get { return _teams; }
            set { SetProperty(ref _teams, value); }
        }

        ObservableCollection<Manager> _managers = new ObservableCollection<Manager>();
        public ObservableCollection<Manager> Managers
        {
            get { return _managers; }
            set { SetProperty(ref _managers, value); }
        }

        ObservableCollection<Categorie> _categories = new ObservableCollection<Categorie>();
        public ObservableCollection<Categorie> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        ObservableCollection<MatchType> _matchTypes = new ObservableCollection<MatchType>();
        public ObservableCollection<MatchType> MatchTypes
        {
            get { return _matchTypes; }
            set { SetProperty(ref _matchTypes, value); }
        }

        ObservableCollection<Squad> _squads = new ObservableCollection<Squad>();
        public ObservableCollection<Squad> Squads
        {
            get { return _squads; }
            set { SetProperty(ref _squads, value); }
        }

        ObservableCollection<Administrator> _administrators = new ObservableCollection<Administrator>();
        public ObservableCollection<Administrator> Administrators
        {
            get { return _administrators; }
            set { SetProperty(ref _administrators, value); }
        }

        ObservableCollection<Status> _statuses = new ObservableCollection<Status>();
        public ObservableCollection<Status> Statuses
        {
            get { return _statuses; }
            set { SetProperty(ref _statuses, value); }
        }

        ObservableCollection<Round> _rounds = new ObservableCollection<Round>();
        public ObservableCollection<Round> Rounds
        {
            get { return _rounds; }
            set { SetProperty(ref _rounds, value); }
        }

        bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        Exception mainException = null;
        public Exception MainException
        {
            get { return mainException; }
            set { SetProperty(ref mainException, value); }
        }

        Match selectedLiveMatch = new Match();
        public Match SelectedLiveMatch
        {
            get { return selectedLiveMatch; }
            set { SetProperty(ref selectedLiveMatch, value); }
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
