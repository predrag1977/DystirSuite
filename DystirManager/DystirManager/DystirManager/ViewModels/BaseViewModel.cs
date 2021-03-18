using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using DystirManager.Models;
using DystirManager.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;

namespace DystirManager.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataLoader<Match> GetDataStore()
        {
            return DependencyService.Get<IDataLoader<Match>>() ?? new DataLoader();
        }

        CultureInfo _languageCode;
        public CultureInfo LanguageCode
        {
            get { return _languageCode; }
            set { SetProperty(ref _languageCode, value); }
        }

        string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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

        ObservableCollection<Match> _resultsMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> ResultsMatches
        {
            get { return _resultsMatches; }
            set { SetProperty(ref _resultsMatches, value); }
        }

        ObservableCollection<Match> _fixturesMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> FixturesMatches
        {
            get { return _fixturesMatches; }
            set { SetProperty(ref _fixturesMatches, value); }
        }

        ObservableCollection<Standing> _standings = new ObservableCollection<Standing>();
        public ObservableCollection<Standing> Standings
        {
            get { return _standings; }
            set { SetProperty(ref _standings, value); }
        }

        IEnumerable<IGrouping<string, Match>> _matchesGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public IEnumerable<IGrouping<string, Match>> MatchesGroupList
        {
            get { return _matchesGroupList; }
            set { SetProperty(ref _matchesGroupList, value); }
        }

        IEnumerable<IGrouping<string, Match>> _resultsGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public IEnumerable<IGrouping<string, Match>> ResultsGroupList
        {
            get { return _resultsGroupList; }
            set { SetProperty(ref _resultsGroupList, value); }
        }

        IEnumerable<IGrouping<string, Match>> _fixturesGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public IEnumerable<IGrouping<string, Match>> FixturesGroupList
        {
            get { return _fixturesGroupList; }
            set { SetProperty(ref _fixturesGroupList, value); }
        }

        IEnumerable<IGrouping<string, TeamStanding>> _teamStandingsGroupList = new ObservableCollection<IGrouping<string, TeamStanding>>();
        public IEnumerable<IGrouping<string, TeamStanding>> TeamStandingsGroupList
        {
            get { return _teamStandingsGroupList; }
            set { SetProperty(ref _teamStandingsGroupList, value); }
        }

        ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        public ObservableCollection<Team> Teams
        {
            get { return _teams; }
            set { SetProperty(ref _teams, value); }
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

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        bool isLoadingSelectedMatch = false;
        public bool IsLoadingSelectedMatch
        {
            get { return isLoadingSelectedMatch; }
            set { SetProperty(ref isLoadingSelectedMatch, value); }
        }

        bool isDisconnected = false;
        public bool IsDisconnected
        {
            get { return isDisconnected; }
            set { SetProperty(ref isDisconnected, value); }
        }

        bool isConnectionError = false;
        public bool IsConnectionError
        {
            get { return isConnectionError; }
            set { SelectedLiveMatch.IsConnectionError = isConnectionError; SetProperty(ref isConnectionError, value); }
        }

        bool isApplicationActive = true;
        public bool IsApplicationActive
        {
            get {  return isApplicationActive; }
            set { SetProperty(ref isApplicationActive, value); }
        }

        Administrator administratorLoggedIn;
        public Administrator AdministratorLoggedIn
        {
            get { return administratorLoggedIn; }
            set { SetProperty(ref administratorLoggedIn, value); }
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

        public TimeCounter TimeCounter = new TimeCounter();

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
