using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DystirXamarin.Models
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
        public Nullable<System.DateTime> Time { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("StatusID")]
        public Nullable<int> StatusID { get; set; }

        [JsonProperty("HomeTeamScore")]
        public Nullable<int> HomeTeamScore { get; set; }

        [JsonProperty("AwayTeamScore")]
        public Nullable<int> AwayTeamScore { get; set; }

        [JsonProperty("MatchTypeName")]
        public string MatchTypeName { get; set; }

        [JsonProperty("HomeTeamID")]
        public Nullable<int> HomeTeamID { get; set; }

        [JsonProperty("AwayTeamID")]
        public Nullable<int> AwayTeamID { get; set; }

        [JsonProperty("StatusName")]
        public string StatusName { get; set; }

        [JsonProperty("MatchActivation")]
        public Nullable<int> MatchActivation { get; set; }

        [JsonProperty("StatusTime")]
        public Nullable<System.DateTime> StatusTime { get; set; }

        [JsonProperty("MatchTypeID")]
        public Nullable<int> MatchTypeID { get; set; }

        [JsonProperty("TeamAdminID")]
        public Nullable<int> TeamAdminID { get; set; }

        [JsonProperty("RoundID")]
        public int? RoundID { get; set; }

        [JsonProperty("RoundName")]
        public string RoundName { get; set; }

        [JsonProperty("ExtraMinutes")]
        public int ExtraMinutes { get; set; }

        [JsonProperty("ExtraSeconds")]
        public int ExtraSeconds { get; set; }

        private string homeTeamLogo;
        [JsonProperty("HomeTeamLogo")]
        public string HomeTeamLogo
        {
            get { return homeTeamLogo; }
            set { homeTeamLogo = "https://www.dystir.fo/team_logos/" + value; }
        }

        private string awayTeamLogo;
        [JsonProperty("AwayTeamLogo")]
        public string AwayTeamLogo
        {
            get { return awayTeamLogo; }
            set { awayTeamLogo = "https://www.dystir.fo/team_logos/" + value; }
        }

        string _selectedTeam;
        public string SelectedTeam
        {
            get { return _selectedTeam; }
            set { SetProperty(ref _selectedTeam, value); CreatePlayersOfTeam(value, SearchingTextPlayersOfTeam); }
        }

        string _searchingTextPlayersOfTeam;
        public string SearchingTextPlayersOfTeam
        {
            get { return _searchingTextPlayersOfTeam; }
            set { SetProperty(ref _searchingTextPlayersOfTeam, value); CreatePlayersOfTeam(SelectedTeam, value); }
        }

        public ObservableCollection<Team> Teams { get; set; }
        public ObservableCollection<Categorie> Categories { get; set; }
        public ObservableCollection<MatchType> MatchTypes { get; set; }
        public ObservableCollection<Squad> Squads { get; internal set; }
        public ObservableCollection<Status> Statuses { get; internal set; }
        public ObservableCollection<Round> Rounds { get; internal set; }

        ObservableCollection<EventOfMatch> _eventsOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> EventsOfMatch
        {
            get { return _eventsOfMatch; }
            set { SetProperty(ref _eventsOfMatch, SortEventOfMatch(value)); }
        }

        ObservableCollection<PlayerOfMatch> _playersOfMatch = new ObservableCollection<PlayerOfMatch>();
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch
        {
            get { return _playersOfMatch; }
            set { SetProperty(ref _playersOfMatch, SortingPlayersOfMatch(value)); }
        }

        ObservableCollection<PlayerOfMatch> _playersOfTeam = new ObservableCollection<PlayerOfMatch>();
        public ObservableCollection<PlayerOfMatch> PlayersOfTeam
        {
            get { return _playersOfTeam; }
            set { SetProperty(ref _playersOfTeam, value); }
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

            PlayersOfTeam = new ObservableCollection<PlayerOfMatch>(playersOfMatchSorted?.Where(p => p.TeamName == SelectedTeam
            && p.FirstName?.StartsWith((SearchingTextPlayersOfTeam ?? ""), StringComparison.OrdinalIgnoreCase) == true));
            return new ObservableCollection<PlayerOfMatch>(playersOfMatchSorted);
        }

        private ObservableCollection<EventOfMatch> SortEventOfMatch(ObservableCollection<EventOfMatch> eventOfMatches)
        {
            ObservableCollection<EventOfMatch> sortEvents = new ObservableCollection<EventOfMatch>();
            foreach (EventOfMatch eventOfMatch in eventOfMatches)
            {
                eventOfMatch.HomeTeamVisible = eventOfMatch.EventTeam?.Trim().ToUpper() == HomeTeam?.Trim().ToUpper();
                eventOfMatch.AwayTeamVisible = eventOfMatch.EventTeam?.Trim().ToUpper() == AwayTeam?.Trim().ToUpper();
                EventOfMatch findEvent = eventOfMatches.FirstOrDefault(x => x.EventOfMatchID == eventOfMatch.SecondPlayerOfMatchID);
                if (findEvent != null)
                {
                    eventOfMatch.AdditionalText = findEvent.EventName;
                }
                if (eventOfMatch.EventName != "BIGCHANCE")
                {
                    sortEvents.Add(eventOfMatch);
                }
                switch(eventOfMatch.EventName)
                {
                    case "GOAL":
                    case "OWNGOAL":
                    case "PENALTYSCORED":
                        eventOfMatch.EventBackgroundColor = Application.Current.Resources["LineUpGreen"];
                        break;
                    case "YELLOW":
                        eventOfMatch.EventBackgroundColor = Application.Current.Resources["LineUpYellow"];
                        break;
                    case "RED":
                        eventOfMatch.EventBackgroundColor = Application.Current.Resources["LineUpRed"];
                        break;
                    default:
                        eventOfMatch.EventBackgroundColor = Application.Current.Resources["LightBackgroundColor"];
                        break;
                }
                switch (eventOfMatch.EventName)
                {
                    case "GOAL":
                    case "OWNGOAL":
                    case "PENALTYSCORED":
                    case "PENALTYMISSED":
                    case "YELLOW":
                    case "RED":
                    case "SUBSTITUTION":
                        eventOfMatch.EventIconSource = (eventOfMatch.EventName + ".svg").ToLower();
                        eventOfMatch.EventIconSize = 20;
                        eventOfMatch.ShowEventIcon = true;
                        break;
                    default:
                        eventOfMatch.EventIconSource = "noimage.svg";
                        eventOfMatch.EventIconSize = 0;
                        eventOfMatch.ShowEventIcon = false;
                        break;
                }
                
            }
            return new ObservableCollection<EventOfMatch>(sortEvents.Reverse());
        }

        private void CreatePlayersOfTeam(string selectedTeam, string searchingTextPlayersOfTeam)
        {
            PlayersOfTeam = new ObservableCollection<PlayerOfMatch>(PlayersOfMatch?.Where(p => p.TeamName == selectedTeam
            && p.FirstName?.StartsWith((searchingTextPlayersOfTeam ?? ""), StringComparison.OrdinalIgnoreCase) == true));
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

        public static implicit operator Match(ObservableCollection<PlayerOfMatch> v)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class PlayersGrouping : IGrouping<int?, PlayerOfMatch>
    {
        public int? Key { get; set; }
        public List<PlayerOfMatch> Items { get; set; }

        public PlayersGrouping()
        {
        }

        public PlayersGrouping(PlayersGrouping grouping)
        {
            if (grouping == null)
                throw new ArgumentNullException("grouping");
            Key = grouping.Key;
            Items = grouping.Items.ToList();
        }

        public IEnumerator<PlayerOfMatch> GetEnumerator()
        {
            if (Items == null)
            {
                Items = new List<PlayerOfMatch>();
            }
            return Items?.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator() ; }
        
    }
}
