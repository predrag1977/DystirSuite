using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DystirManager.Helper;
using DystirManager.Models;
using DystirManager.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DystirManager.Models
{
    public class Match : INotifyPropertyChanged
    {
        public IDataLoader<Match> GetDataStore()
        {
            return DependencyService.Get<IDataLoader<Match>>() ?? new DataLoader();
        }

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

        string _liveTime = "00:00";
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

        public string SelectedTeam { get; internal set; }

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

        ObservableCollection<EventOfMatch> _eventsOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> EventsOfMatch
        {
            get { return _eventsOfMatch; }
            set
            {
                SetProperty(ref _eventsOfMatch, value);
                CommentaryOfMatch = new ObservableCollection<EventOfMatch>(GetFullEventsOfMatch(value).Reverse());
            }
        }

        ObservableCollection<EventOfMatch> _commentaryOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> CommentaryOfMatch
        {
            get { return _commentaryOfMatch; }
            set
            {
                SetProperty(ref _commentaryOfMatch, value);
                SummaryEventsOfMatch = new ObservableCollection<EventOfMatch>(GetSummaryEventsOfMatch(value));
            }
        }

        ObservableCollection<EventOfMatch> _summaryEventsOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> SummaryEventsOfMatch
        {
            get { return _summaryEventsOfMatch; }
            set { SetProperty(ref _summaryEventsOfMatch, value); }
        }

        bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set { SetProperty(ref _isEditable, value); }
        }

        bool isConnectionError = false;
        public bool IsConnectionError
        {
            get { return isConnectionError; }
            set { SetProperty(ref isConnectionError, value); }
        }

        ObservableCollection<PlayerOfMatch> _playersOfMatch = new ObservableCollection<PlayerOfMatch>();
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch
        {
            get { return _playersOfMatch; }
            set { SetProperty(ref _playersOfMatch, SortingPlayersOfMatch(value)); }
        }

        ObservableCollection<PlayerOfMatch> _homePlayersOfMatch = new ObservableCollection<PlayerOfMatch>();
        public ObservableCollection<PlayerOfMatch> HomePlayersOfMatch
        {
            get { return _homePlayersOfMatch; }
            set { SetProperty(ref _homePlayersOfMatch, value); }
        }

        ObservableCollection<PlayerOfMatch> _awayPlayersOfMatch = new ObservableCollection<PlayerOfMatch>();
        public ObservableCollection<PlayerOfMatch> AwayPlayersOfMatch
        {
            get { return _awayPlayersOfMatch; }
            set { SetProperty(ref _awayPlayersOfMatch, value); }
        }

        bool _isLoadingSelectedMatch = false;
        public bool IsLoadingSelectedMatch
        {
            get { return _isLoadingSelectedMatch; }
            set { SetProperty(ref _isLoadingSelectedMatch, value); }
        }

        private ObservableCollection<PlayerOfMatch> SortingPlayersOfMatch(ObservableCollection<PlayerOfMatch> playersOfMatch)
        {
            var playersOfMatchSorted = playersOfMatch?
                .OrderBy(x => x.PlayingStatus == 3)
                .ThenBy(x => x.PlayingStatus == 0)
                .ThenBy(x => x.PlayingStatus == 2)
                .ThenBy(x => x.PlayingStatus == 1)
                .ThenByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number == null)
                .ThenBy(x => x.Number)
                .ThenBy(x => x.Position == null)
                .ThenBy(x => x.Position == "ATT")
                .ThenBy(x => x.Position == "MID")
                .ThenBy(x => x.Position == "DEF")
                .ThenBy(x => x.Position == "GK")
                .ThenBy(x => x.FirstName)
                .ThenBy(x => x.LastName);

            var sortedPlayersOfMatch = new ObservableCollection<PlayerOfMatch>(playersOfMatchSorted);
            var homePlayers = sortedPlayersOfMatch.Where(x => x.TeamName == HomeTeam);
            var awayPlayers = sortedPlayersOfMatch.Where(x => x.TeamName == AwayTeam);
            HomePlayersOfMatch = new ObservableCollection<PlayerOfMatch>(homePlayers);
            AwayPlayersOfMatch = new ObservableCollection<PlayerOfMatch>(awayPlayers);

            return sortedPlayersOfMatch;
        }

        internal IEnumerable<EventOfMatch> GetFullEventsOfMatch(IEnumerable<EventOfMatch> eventsList)
        {
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? Enumerable.Empty<EventOfMatch>())
            {
                eventOfMatch.HomeTeamScore = 0;
                eventOfMatch.AwayTeamScore = 0;
                eventOfMatch.HomeTeam = eventOfMatch?.EventTeam == HomeTeam ? HomeTeam : string.Empty;
                eventOfMatch.AwayTeam = eventOfMatch?.EventTeam == AwayTeam ? AwayTeam : string.Empty;
                eventOfMatch.HomeMainPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == HomeTeam.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null);
                    eventOfMatch.HomeSecondPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == HomeTeam.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null);
                    eventOfMatch.AwayMainPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == AwayTeam.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null);
                    eventOfMatch.AwaySecondPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == AwayTeam.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null);


                eventOfMatch.HomeTeamVisible = eventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(eventOfMatch.HomeTeam);
                eventOfMatch.AwayTeamVisible = eventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(eventOfMatch.AwayTeam);

                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == HomeTeam.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                eventOfMatch.HomeTeamScore = homeScore;
                eventOfMatch.AwayTeamScore = awayScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.ToList().IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count())
                    {
                        EventOfMatch nextEvent = eventsList.ToList()[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == HomeTeam.ToUpper().Trim())
                            {
                                eventOfMatch.HomeSecondPlayer = GetEventPlayer(nextEvent?.EventTeam.ToUpper().Trim() == HomeTeam.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == AwayTeam.ToUpper().Trim())
                            {
                                eventOfMatch.AwaySecondPlayer = GetEventPlayer(nextEvent?.EventTeam.ToUpper().Trim() == AwayTeam.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                        }
                    }
                }
            }

            return eventsList;
        }

        private string GetEventPlayer(int? playerID)
        {
            string fullName = string.Empty;
            if (playerID != null && playerID > 0)
            {
                PlayerOfMatch player = PlayersOfMatch?.FirstOrDefault(x => x.PlayerOfMatchID == playerID);
                fullName = player?.FirstName + " " + player?.LastName;
            }
            return fullName.Trim();
        }

        bool IsGoal(EventOfMatch matchEvent)
        {
            return matchEvent.EventName == "GOAL"
                || matchEvent.EventName == "OWNGOAL"
                || matchEvent.EventName == "PENALTYSCORED";
        }

        private IEnumerable<EventOfMatch> GetSummaryEventsOfMatch(ObservableCollection<EventOfMatch> eventsList)
        {
            var summaryEventsOfMatch = eventsList.Where(x =>
                 x.EventName == "GOAL"
                 || x.EventName == "OWNGOAL"
                 || x.EventName == "PENALTYSCORED"
                 || x.EventName == "PENALTYMISSED"
                 || x.EventName == "YELLOW"
                 || x.EventName == "RED"
                 || x.EventName == "BIGCHANCE"
                 || x.EventName == "SUBSTITUTION"
                 ).ToList();
            if (StatusID < 12)
            {
                summaryEventsOfMatch.Reverse();
            }
            return summaryEventsOfMatch;
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

        internal async Task<bool> UpdateMatch()
        {
            try
            {
                await GetDataStore().UpdateMatchAsync(this);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        internal async Task<bool> AddMatch()
        {
            try
            {
                await GetDataStore().AddMatchAsync(this);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        internal async Task<bool> DeleteMatch()
        {
            try
            {
                await GetDataStore().DeleteMatchAsync(this);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
