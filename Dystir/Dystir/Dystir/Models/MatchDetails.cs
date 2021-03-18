using Dystir.Helper;
using Dystir.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dystir.Models
{
    public class MatchDetails : INotifyPropertyChanged
    {
        Match _match = new Match();
        [JsonProperty("Match")]
        public Match Match
        {
            get { return _match; }
            set { SetProperty(ref _match, value);}
        }

        ObservableCollection<EventOfMatch> _eventsOfMatch = new ObservableCollection<EventOfMatch>();
        [JsonProperty("EventsOfMatch")]
        public ObservableCollection<EventOfMatch> EventsOfMatch
        {
            get { return _eventsOfMatch; }
            set
            { 
                _eventsOfMatch =  value; 
                CommentaryOfMatch = new ObservableCollection<EventOfMatch>(GetFullEventsOfMatch(value).Reverse());
            }
        }

        ObservableCollection<PlayerOfMatch> _playersOfMatch = new ObservableCollection<PlayerOfMatch>();
        [JsonProperty("PlayersOfMatch")]
        public ObservableCollection<PlayerOfMatch> PlayersOfMatch
        {
            get { return _playersOfMatch; }
            set 
            {
                _playersOfMatch = value; 
                PlayersListByTeamAndStatus(value); 
            }
        }

        ObservableCollection<IGrouping<string, PlayersInRow>> _playersInRowGroupList = new ObservableCollection<IGrouping<string, PlayersInRow>>();
        public ObservableCollection<IGrouping<string, PlayersInRow>> PlayersInRowGroupList
        {
            get { return _playersInRowGroupList; }
            set { SetProperty(ref _playersInRowGroupList, value); }
        }

        ObservableCollection<EventOfMatch> _commentaryOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> CommentaryOfMatch
        {
            get { return _commentaryOfMatch; }
            set
            {
                SetProperty(ref _commentaryOfMatch, value);
                SummaryEventsOfMatch = GetSummaryEventsOfMatch(value);
            }
        }

        ObservableCollection<EventOfMatch> _summaryEventsOfMatch = new ObservableCollection<EventOfMatch>();
        public ObservableCollection<EventOfMatch> SummaryEventsOfMatch
        {
            get { return _summaryEventsOfMatch; }
            set 
            { 
                SetProperty(ref _summaryEventsOfMatch, value);
                MatchStatistic = GetStatistics();
            }
        }

        Statistic _matchStatistic = new Statistic();
        public Statistic MatchStatistic
        {
            get { return _matchStatistic; }
            set { SetProperty(ref _matchStatistic, value); }
        }

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

        private IEnumerable<EventOfMatch> GetFullEventsOfMatch(IEnumerable<EventOfMatch> eventsList)
        {
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? Enumerable.Empty<EventOfMatch>())
            {
                eventOfMatch.HomeTeamScore = 0;
                eventOfMatch.AwayTeamScore = 0;
                eventOfMatch.HomeTeam = eventOfMatch?.EventTeam == Match?.HomeTeam ? Match?.HomeTeam : string.Empty;
                eventOfMatch.AwayTeam = eventOfMatch?.EventTeam == Match?.AwayTeam ? Match?.AwayTeam : string.Empty;
                eventOfMatch.HomeMainPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == Match?.HomeTeam?.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null);
                eventOfMatch.HomeSecondPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == Match?.HomeTeam?.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null);
                eventOfMatch.AwayMainPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == Match?.AwayTeam?.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null);
                eventOfMatch.AwaySecondPlayer = GetEventPlayer(eventOfMatch?.EventTeam.ToUpper().Trim() == Match?.AwayTeam?.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null);


                eventOfMatch.HomeTeamVisible = eventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(eventOfMatch.HomeTeam);
                eventOfMatch.AwayTeamVisible = eventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(eventOfMatch.AwayTeam);

                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == Match?.HomeTeam?.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == Match?.AwayTeam?.ToUpper().Trim())
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
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == Match?.HomeTeam?.ToUpper().Trim())
                            {
                                eventOfMatch.HomeSecondPlayer = GetEventPlayer(nextEvent?.EventTeam.ToUpper().Trim() == Match?.HomeTeam?.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == Match?.AwayTeam?.ToUpper().Trim())
                            {
                                eventOfMatch.AwaySecondPlayer = GetEventPlayer(nextEvent?.EventTeam.ToUpper().Trim() == Match?.AwayTeam?.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                        }
                    }
                }

                eventOfMatch.ShowResult = eventOfMatch?.EventName?.ToUpper() == "GOAL"
                        || eventOfMatch?.EventName?.ToUpper() == "OWNGOAL"
                        || eventOfMatch?.EventName?.ToUpper() == "PENALTYSCORED";

                eventOfMatch.ShowSecondPlayer = eventOfMatch?.EventName?.ToUpper() == "SUBSTITUTION";
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

        private ObservableCollection<EventOfMatch> GetSummaryEventsOfMatch(ObservableCollection<EventOfMatch> eventsList)
        {
            var summaryEventsOfMatch = eventsList.Where(x =>
                 x.EventName == "GOAL"
                 || x.EventName == "OWNGOAL"
                 || x.EventName == "PENALTYSCORED"
                 || x.EventName == "PENALTYMISSED"
                 || x.EventName == "YELLOW"
                 || x.EventName == "RED"
                 || x.EventName == "BIGCHANCE"
                 //|| x.EventName == "SUBSTITUTION"
                 ).ToList();
            if (Match?.StatusID < 13)
            {
                summaryEventsOfMatch.Reverse();
            }
            return new ObservableCollection<EventOfMatch>(summaryEventsOfMatch);
        }

        private void PlayersListByTeamAndStatus(ObservableCollection<PlayerOfMatch> playersOfMatch)
        {
            foreach (PlayerOfMatch player in playersOfMatch ?? new ObservableCollection<PlayerOfMatch>())
            {
                player.GoalVisible = player.Goal > 0;
                player.OwnGoalVisible = player.OwnGoal > 0;
                player.YellowCardVisible = player.YellowCard > 0;
                player.SecondYellowCardVisible = player.YellowCard > 1;
                player.RedCardVisible = player.RedCard > 0;
                player.SubInVisible = player.SubIn > 0;
                player.SubOutVisible = player.SubOut > 0;
            }
            
            var homeFirstEleven = new ObservableCollection<PlayerOfMatch>(playersOfMatch.Where(x => x.TeamName?.Trim() == Match?.HomeTeam?.Trim() && x.PlayingStatus == 1));
            var awayFirstEleven = new ObservableCollection<PlayerOfMatch>(playersOfMatch.Where(x => x.TeamName?.Trim() == Match?.AwayTeam?.Trim() && x.PlayingStatus == 1));
            var homeSubbstitutions = new ObservableCollection<PlayerOfMatch>(playersOfMatch.Where(x => x.TeamName?.Trim() == Match?.HomeTeam?.Trim() && x.PlayingStatus == 2));
            var awaySubbstitutions = new ObservableCollection<PlayerOfMatch>(playersOfMatch.Where(x => x.TeamName?.Trim() == Match?.AwayTeam?.Trim() && x.PlayingStatus == 2));

            var playersInRowList = new ObservableCollection<PlayersInRow>(GetPlayerInRowList(homeFirstEleven, awayFirstEleven, homeSubbstitutions, awaySubbstitutions));

            PlayersInRowGroupList = new ObservableCollection<IGrouping<string, PlayersInRow>>(playersInRowList?.GroupBy(x => x.PlayingStatus?.ToString()));
        }

        private ObservableCollection<PlayersInRow> GetPlayerInRowList(ObservableCollection<PlayerOfMatch> homeFirstEleven, ObservableCollection<PlayerOfMatch> awayFirstEleven, ObservableCollection<PlayerOfMatch> homeSubbstitutions, ObservableCollection<PlayerOfMatch> awaySubbstitutions)
        {
            ObservableCollection<PlayersInRow> playerInRowList = new ObservableCollection<PlayersInRow>();
            var biggerList = homeFirstEleven.Count >= awayFirstEleven.Count ? homeFirstEleven : awayFirstEleven;
            var smallerList = homeFirstEleven.Count < awayFirstEleven.Count ? homeFirstEleven : awayFirstEleven;

            for (int index = 0; index < biggerList.Count; index++)
            {
                playerInRowList.Add(CreatePlayerInRow(biggerList, smallerList, index));
            }

            biggerList = homeSubbstitutions.Count >= awaySubbstitutions.Count ? homeSubbstitutions : awaySubbstitutions;
            smallerList = homeSubbstitutions.Count < awaySubbstitutions.Count ? homeSubbstitutions : awaySubbstitutions;

            for (int index = 0; index < biggerList.Count; index++)
            {
                playerInRowList.Add(CreatePlayerInRow(biggerList, smallerList, index));
            }

            return playerInRowList;
        }

        private PlayersInRow CreatePlayerInRow(ObservableCollection<PlayerOfMatch> biggerList, ObservableCollection<PlayerOfMatch> smallerList, int index)
        {
            PlayerOfMatch playerOfMatch = biggerList[index];
            PlayersInRow playersInRow = new PlayersInRow();
            playersInRow.PlayingStatus = playerOfMatch.PlayingStatus;
            SetPlayerInRowByPlayingStatus(playersInRow, playerOfMatch);
            if (index < smallerList.Count)
            {
                playerOfMatch = smallerList[index];
                SetPlayerInRowByPlayingStatus(playersInRow, playerOfMatch);
            }
            return playersInRow;
        }

        private void SetPlayerInRowByPlayingStatus(PlayersInRow playersInRow, PlayerOfMatch playerOfMatch)
        {
            if (playerOfMatch.TeamName?.Trim() == Match?.HomeTeam?.Trim())
            {
                playersInRow.FirstPlayer = playerOfMatch;
            }
            if (playerOfMatch.TeamName?.Trim() == Match?.AwayTeam?.Trim())
            {
                playersInRow.SecondPlayer = playerOfMatch;
            };
        }

        private Statistic GetStatistics()
        {
            Statistic statistic = new Statistic();
            List<EventOfMatch> eventsDataList = EventsOfMatch.ToList();
            foreach (EventOfMatch matchEvent in eventsDataList)
            {
                if (matchEvent?.EventTeam?.ToLower().Trim() == Match?.HomeTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.HomeTeamStatistic);
                }
                else if (matchEvent?.EventTeam?.ToLower().Trim() == Match?.AwayTeam?.ToLower().Trim())
                {
                    AddTeamStatistic(matchEvent, statistic.AwayTeamStatistic);
                }
            }

            if (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal != 0)
            {
                statistic.HomeTeamStatistic.GoalProcent = statistic.HomeTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
                statistic.AwayTeamStatistic.GoalProcent = statistic.AwayTeamStatistic.Goal * 100 / (statistic.HomeTeamStatistic.Goal + statistic.AwayTeamStatistic.Goal);
            }
            if (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard != 0)
            {
                statistic.HomeTeamStatistic.YellowCardProcent = statistic.HomeTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
                statistic.AwayTeamStatistic.YellowCardProcent = statistic.AwayTeamStatistic.YellowCard * 100 / (statistic.HomeTeamStatistic.YellowCard + statistic.AwayTeamStatistic.YellowCard);
            }
            if (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard != 0)
            {
                statistic.HomeTeamStatistic.RedCardProcent = statistic.HomeTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
                statistic.AwayTeamStatistic.RedCardProcent = statistic.AwayTeamStatistic.RedCard * 100 / (statistic.HomeTeamStatistic.RedCard + statistic.AwayTeamStatistic.RedCard);
            }
            if (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner != 0)
            {
                statistic.HomeTeamStatistic.CornerProcent = statistic.HomeTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
                statistic.AwayTeamStatistic.CornerProcent = statistic.AwayTeamStatistic.Corner * 100 / (statistic.HomeTeamStatistic.Corner + statistic.AwayTeamStatistic.Corner);
            }
            if (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget != 0)
            {
                statistic.HomeTeamStatistic.OnTargetProcent = statistic.HomeTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
                statistic.AwayTeamStatistic.OnTargetProcent = statistic.AwayTeamStatistic.OnTarget * 100 / (statistic.HomeTeamStatistic.OnTarget + statistic.AwayTeamStatistic.OnTarget);
            }
            if (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget != 0)
            {
                statistic.HomeTeamStatistic.OffTargetProcent = statistic.HomeTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
                statistic.AwayTeamStatistic.OffTargetProcent = statistic.AwayTeamStatistic.OffTarget * 100 / (statistic.HomeTeamStatistic.OffTarget + statistic.AwayTeamStatistic.OffTarget);
            }
            if (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot != 0)
            {
                statistic.HomeTeamStatistic.BlockedShotProcent = statistic.HomeTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
                statistic.AwayTeamStatistic.BlockedShotProcent = statistic.AwayTeamStatistic.BlockedShot * 100 / (statistic.HomeTeamStatistic.BlockedShot + statistic.AwayTeamStatistic.BlockedShot);
            }
            if (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance != 0)
            {
                statistic.HomeTeamStatistic.BigChanceProcent = statistic.HomeTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
                statistic.AwayTeamStatistic.BigChanceProcent = statistic.AwayTeamStatistic.BigChance * 100 / (statistic.HomeTeamStatistic.BigChance + statistic.AwayTeamStatistic.BigChance);
            }

            return statistic;
        }

        private void AddTeamStatistic(EventOfMatch matchEvent, TeamStatistic teamStatistic)
        {
            switch (matchEvent?.EventName?.ToLower())
            {
                case "goal":
                case "penaltyscored":
                    teamStatistic.Goal += 1;
                    teamStatistic.OnTarget += 1;
                    break;
                case "owngoal":
                    teamStatistic.Goal += 1;
                    break;
                case "yellow":
                    teamStatistic.YellowCard += 1;
                    break;
                case "red":
                    teamStatistic.RedCard += 1;
                    break;
                case "corner":
                    teamStatistic.Corner += 1;
                    break;
                case "ontarget":
                    teamStatistic.OnTarget += 1;
                    break;
                case "offtarget":
                    teamStatistic.OffTarget += 1;
                    break;
                case "blockedshot":
                    teamStatistic.BlockedShot += 1;
                    break;
                case "bigchance":
                    teamStatistic.BigChance += 1;
                    break;
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
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