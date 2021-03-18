using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Dystir.Models;
using Dystir.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using Xamarin.Forms.Internals;

namespace Dystir.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public static IDataLoader<Match> GetDataStore()
        {
            return DependencyService.Get<IDataLoader<Match>>() ?? new DataLoader();
        }

        public TimeCounter TimeCounter = new TimeCounter();

        ObservableCollection<Match> _allMatches = new ObservableCollection<Match>();
        public ObservableCollection<Match> AllMatches
        {
            get { return _allMatches; }
            set {  _allMatches = value; SetAllMatches(); }
        }

        public ObservableCollection<MatchDetails> AllMatchesWithDetails { get; set; } = new ObservableCollection<MatchDetails>();

        MatchDetails _selectedMatchDetails = new MatchDetails();
        public MatchDetails SelectedMatchDetails
        {
            get { return _selectedMatchDetails; }
            set { SetProperty(ref _selectedMatchDetails, value); }
        }
    
        ObservableCollection<Standing> _allStandings = new ObservableCollection<Standing>();
        public ObservableCollection<Standing> AllStandings
        {
            get { return _allStandings; }
            set { _allStandings = value; SetStandings(); }
        }

        ObservableCollection<TeamStanding> _competitionTeamStandings = new ObservableCollection<TeamStanding>();
        public ObservableCollection<TeamStanding> CompetitionTeamStandings
        {
            get { return _competitionTeamStandings; }
            set { SetProperty(ref _competitionTeamStandings, value); }
        }

        ObservableCollection<CompetitionStatistic> _allCompetitionStatistics = new ObservableCollection<CompetitionStatistic>();
        public ObservableCollection<CompetitionStatistic> AllCompetitionStatistics
        {
            get { return _allCompetitionStatistics; }
            set { _allCompetitionStatistics = value; SetCompetitionStatistics(); }
        }

        ObservableCollection<PlayersInRow> _selectedCompetitionStatistics = new ObservableCollection<PlayersInRow>();
        public ObservableCollection<PlayersInRow> SelectedCompetitionStatistics
        {
            get { return _selectedCompetitionStatistics; }
            set { SetProperty(ref _selectedCompetitionStatistics, value); }
        }

        CultureInfo languageCode = CultureInfo.GetCultureInfo("fo-FO");
        public CultureInfo LanguageCode
        {
            get { return languageCode; }
            set
            {
                SetProperty(ref languageCode, value);
                AllMatches.ForEach(x => x.LanguageCode = value);
                SetMatchesDays();
                if (SelectedMatch != null) SelectedMatch.LanguageCode = value;
            }
        }

        string pageTitle = string.Empty;
        public string PageTitle
        {
            get { return pageTitle; }
            set { SetProperty(ref pageTitle, value); }
        }

        ObservableCollection<Sponsor> _sponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> Sponsors
        {
            get { return _sponsors; }
            set { SetProperty(ref _sponsors, value); }
        }

        public ObservableCollection<Sponsor> AllMainSponsors { get; set; }

        ObservableCollection<Sponsor> _sponsorsMain = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> SponsorsMain
        {
            get { return _sponsorsMain; }
            set { SetProperty(ref _sponsorsMain, value); }
        }

        ObservableCollection<IGrouping<string, Match>> _matchesGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public ObservableCollection<IGrouping<string, Match>> MatchesGroupList
        {
            get { return _matchesGroupList; }
            set { SetProperty(ref _matchesGroupList, value); }
        }

        ObservableCollection<IGrouping<string, Match>> _resultsGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public ObservableCollection<IGrouping<string, Match>> ResultsGroupList
        {
            get { return _resultsGroupList; }
            set { SetProperty(ref _resultsGroupList, value); }
        }

        ObservableCollection<IGrouping<string, Match>> _fixturesGroupList = new ObservableCollection<IGrouping<string, Match>>();
        public ObservableCollection<IGrouping<string, Match>> FixturesGroupList
        {
            get { return _fixturesGroupList; }
            set { SetProperty(ref _fixturesGroupList, value); }
        }

        DateTime _matchesDaySelected = DateTime.Now.ToLocalTime();
        public DateTime MatchesDaySelected
        {
            get { return _matchesDaySelected; }
            set { SetProperty(ref _matchesDaySelected, value); SetMatches(); }
        }

        ObservableCollection<DateTime> _matchesDays = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> MatchesDays
        {
            get { return _matchesDays; }
            set { SetProperty(ref _matchesDays, value); }
        }

        string _resultsCompetitionSelected;
        public string ResultsCompetitionSelected
        {
            get { return _resultsCompetitionSelected; }
            set { SetProperty(ref _resultsCompetitionSelected, value); SetResults(); }
        }

        ObservableCollection<string> _resultsCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> ResultsCompetitions
        {
            get { return _resultsCompetitions; }
            set { SetProperty(ref _resultsCompetitions, value); }
        }

        string _fixturesCompetitionSelected;
        public string FixturesCompetitionSelected
        {
            get { return _fixturesCompetitionSelected; }
            set { SetProperty(ref _fixturesCompetitionSelected, value); SetFixtures(); }
        }

        ObservableCollection<string> _fixturesCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> FixturesCompetitions
        {
            get { return _fixturesCompetitions; }
            set { SetProperty(ref _fixturesCompetitions, value); }
        }

        string _standingsCompetitionSelected;
        public string StandingsCompetitionSelected
        {
            get { return _standingsCompetitionSelected; }
            set { SetProperty(ref _standingsCompetitionSelected, value); SetStandings(); }
        }

        ObservableCollection<string> _standingsCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> StandingsCompetitions
        {
            get { return _standingsCompetitions; }
            set { SetProperty(ref _standingsCompetitions, value); }
        }

        string _statisticsCompetitionSelected;
        public string StatisticsCompetitionSelected
        {
            get { return _statisticsCompetitionSelected; }
            set { SetProperty(ref _statisticsCompetitionSelected, value); SetCompetitionStatistics(); }
        }

        ObservableCollection<string> _statisticCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> StatisticCompetitions
        {
            get { return _statisticCompetitions; }
            set { SetProperty(ref _statisticCompetitions, value); }
        }

        Match _selectedMatch = new Match();
        public Match SelectedMatch
        {
            get { return _selectedMatch; }
            set { SetProperty(ref _selectedMatch, value); }
        }

        ObservableCollection<MatchDetails> _matchesBySelectedDate = new ObservableCollection<MatchDetails>();
        public ObservableCollection<MatchDetails> MatchesBySelectedDate
        {
            get { return _matchesBySelectedDate; }
            set { SetProperty(ref _matchesBySelectedDate, value); }
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
            set { SetProperty(ref isConnectionError, value); }
        }

        bool isApplicationActive = true;
        public bool IsApplicationActive
        {
            get { return isApplicationActive; }
            set { SetProperty(ref isApplicationActive, value); }
        }

        Exception mainException = null;
        public Exception MainException
        {
            get { return mainException; }
            set { SetProperty(ref mainException, value); }
        }

        // ALL MATCHES
        private void SetAllMatches()
        {
            SetMatches();
            SetResults();
            SetFixtures();
        }

        // MATCHES
        private void SetMatches()
        {
            SetMatchesDays();
            var matches = AllMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).Where(x => x.Time?.Date == MatchesDaySelected.Date);
            var matchesGroupList = new ObservableCollection<IGrouping<string, Match>>(matches?.GroupBy(x => x.MatchTypeName));
            MatchesGroupList = matchesGroupList;
        }

        private void SetMatchesDays()
        {
            var matchesDays = new ObservableCollection<DateTime>();
            for (DateTime date = DateTime.Now.AddDays(-3); date <= DateTime.Now.AddDays(3); date = date.AddDays(1))
            {
                matchesDays.Add(date);
            }
            MatchesDays = new ObservableCollection<DateTime>(matchesDays);
        }


        // RESULTS
        private void SetResults()
        {
            var resultsCompetitions = new ObservableCollection<string>();
            var toDate = DateTime.Now.Date.AddDays(0);
            var fromDate = new DateTime(toDate.Year - 1, 12, 31);
            var resultsMatches = AllMatches?.OrderBy(x => x.MatchTypeID).ThenByDescending(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            || (x.StatusID == 13 || x.StatusID == 12)) ?? new ObservableCollection<Match>();
            foreach (IGrouping<string, Match> competitionMatches in resultsMatches.GroupBy(x => x.MatchTypeName))
            {
                if (!string.IsNullOrEmpty(competitionMatches.Key))
                {
                    resultsCompetitions.Add(competitionMatches.Key);
                }
            }
            ResultsCompetitions = new ObservableCollection<string>(resultsCompetitions);
            if (string.IsNullOrEmpty(ResultsCompetitionSelected) && ResultsCompetitions.Count > 0)
            {
                ResultsCompetitionSelected = ResultsCompetitions.FirstOrDefault();
            }
            var resultsGroupList = resultsMatches?.Where(x => x.MatchTypeName == ResultsCompetitionSelected).GroupBy(x => x.MatchTypeName);
            ResultsGroupList = new ObservableCollection<IGrouping<string, Match>>(resultsGroupList);
        }

        // FIXTURES
        private void SetFixtures()
        {
            var fixturesCompetitions = new ObservableCollection<string>();
            var fromDate = DateTime.Now.Date.AddDays(0);
            var toDate = DateTime.Now.Date.AddDays(360);
            var fixturesMatches = AllMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            && (x.StatusID < 12) || x.StatusID == 14) ?? new ObservableCollection<Match>();
            foreach (IGrouping<string, Match> competitionMatches in fixturesMatches?.GroupBy(x => x.MatchTypeName))
            {
                if (!string.IsNullOrEmpty(competitionMatches.Key))
                {
                    fixturesCompetitions.Add(competitionMatches.Key);
                }
            }
            FixturesCompetitions = new ObservableCollection<string>(fixturesCompetitions);
            if (string.IsNullOrEmpty(FixturesCompetitionSelected) && FixturesCompetitions.Count > 0)
            {
                FixturesCompetitionSelected = FixturesCompetitions.FirstOrDefault();
            }
            var fixturesGroupList = fixturesMatches?.Where(x => x.MatchTypeName == FixturesCompetitionSelected).GroupBy(x => x.MatchTypeName);
            FixturesGroupList = new ObservableCollection<IGrouping<string, Match>>(fixturesGroupList);
        }

        // COMPETITION STANDINGS
        private void SetStandings()
        {
            var standingsCompetitions = new ObservableCollection<string>();
            foreach (Standing standing in AllStandings)
            {
                standingsCompetitions.Add(standing?.StandingCompetitionName);
            }
            StandingsCompetitions = new ObservableCollection<string>(standingsCompetitions);
            if (string.IsNullOrEmpty(StandingsCompetitionSelected) && StandingsCompetitions.Count > 0)
            {
                StandingsCompetitionSelected = StandingsCompetitions.FirstOrDefault();
            }
            var teamStandings = AllStandings.FirstOrDefault(x => x.StandingCompetitionName == StandingsCompetitionSelected)?.TeamStandings ?? Enumerable.Empty<TeamStanding>();
            CompetitionTeamStandings = new ObservableCollection<TeamStanding>(teamStandings);
        }

        // COMPETITION STATISTICS
        private void SetCompetitionStatistics()
        {
            var statisticCompetitions = new ObservableCollection<string>();
            foreach (CompetitionStatistic competitionStatistic in AllCompetitionStatistics)
            {
                statisticCompetitions.Add(competitionStatistic?.CompetitionName);
            }
            StatisticCompetitions = new ObservableCollection<string>(statisticCompetitions);
            if (string.IsNullOrEmpty(StatisticsCompetitionSelected) && StatisticCompetitions.Count > 0)
            {
                StatisticsCompetitionSelected = StatisticCompetitions.FirstOrDefault();
            }
            var selectedCompetitionStatistics = AllCompetitionStatistics.FirstOrDefault(x => x.CompetitionName == StatisticsCompetitionSelected);
            var selectedCompetitionStatisticsRowsList = new ObservableCollection<PlayersInRow>();
            if (selectedCompetitionStatistics != null)
            {
                foreach (PlayerOfMatch goalPlayer in selectedCompetitionStatistics.GoalPlayers)
                {
                    PlayersInRow statisticRow = new PlayersInRow();
                    statisticRow.FirstPlayer = goalPlayer;
                    goalPlayer.NumberOrder = (selectedCompetitionStatistics.GoalPlayers.IndexOf(goalPlayer) + 1).ToString() + ".";
                    selectedCompetitionStatisticsRowsList.Add(statisticRow); 
                }
                foreach (PlayerOfMatch assistPlayer in selectedCompetitionStatistics.AssistPlayers)
                {
                    int index = selectedCompetitionStatistics.AssistPlayers.IndexOf(assistPlayer);
                    assistPlayer.NumberOrder = (index + 1).ToString() + ".";
                    PlayersInRow statisticRow = new PlayersInRow();
                    if (selectedCompetitionStatisticsRowsList.Count > index)
                    {
                        statisticRow = selectedCompetitionStatisticsRowsList[index];
                        statisticRow.SecondPlayer = assistPlayer;
                    }
                    else
                    {
                        statisticRow.SecondPlayer = assistPlayer;
                        selectedCompetitionStatisticsRowsList.Add(statisticRow);
                    }
                }
            }
            SelectedCompetitionStatistics = selectedCompetitionStatisticsRowsList;
        }

        // MATCHES BY SELECTED DATE
        internal void SetMatchesBySelectedDate()
        {
            var matchesBySelectedDate = AllMatchesWithDetails?.Where(x => x?.Match?.Time?.Date == SelectedMatch?.Time?.Date && x?.Match?.StatusID < 13);
            MatchesBySelectedDate = new ObservableCollection<MatchDetails>(matchesBySelectedDate);
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
