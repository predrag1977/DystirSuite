using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.Services;
using Dystir.Views;
using Xamarin.Forms;
using Match = Dystir.Models.Match;

namespace Dystir.ViewModels
{
    public class MatchDetailViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        public Command<MatchDetailsTab> MatchDetailsTabTapped { get; }
        public int MatchID { get; set; }

        MatchDetails matchDetails;
        public MatchDetails MatchDetails
        {
            get { return matchDetails; }
            set { matchDetails = value; OnPropertyChanged(); }
        }

        MatchDetailsTab selectedMatchDetailsTab;
        public MatchDetailsTab SelectedMatchDetailsTab
        {
            get { return selectedMatchDetailsTab; }
            set { selectedMatchDetailsTab = value; SetDetailsTabSelected(); }
        }

        ObservableCollection<MatchDetailsTab> matchDetailsTabs;
        public ObservableCollection<MatchDetailsTab> MatchDetailsTabs
        {
            get { return matchDetailsTabs; }
            set { matchDetailsTabs = value; OnPropertyChanged(); }
        }

        ObservableCollection<SummaryEventOfMatch> summary;
        public ObservableCollection<SummaryEventOfMatch> Summary
        {
            get { return summary; }
            set { summary = value; OnPropertyChanged(); }
        }

        ObservableCollection<PlayersInLineups> lineups;
        public ObservableCollection<PlayersInLineups> Lineups
        {
            get { return lineups; }
            set { lineups = value; OnPropertyChanged(); }
        }

        ObservableCollection<SummaryEventOfMatch> commentary;
        public ObservableCollection<SummaryEventOfMatch> Commentary
        {
            get { return commentary; }
            set { commentary = value; OnPropertyChanged(); }
        }

        MatchStatistics matchStatistics;
        public MatchStatistics MatchStatistics
        {
            get { return matchStatistics; }
            set { matchStatistics = value; OnPropertyChanged(); }
        }

        Standing liveStanding;
        public Standing LiveStanding
        {
            get { return liveStanding; }
            set { liveStanding = value; OnPropertyChanged(); }
        }

        ObservableCollection<Match> matchesBySelectedDate = new ObservableCollection<Match>();
        public ObservableCollection<Match> MatchesBySelectedDate
        {
            get { return matchesBySelectedDate; }
            set { matchesBySelectedDate = value; OnPropertyChanged(); }
        }

        bool summaryIsVisible;
        public bool SummaryIsVisible
        {
            get { return summaryIsVisible; }
            set { summaryIsVisible = value; OnPropertyChanged(); }
        }

        bool lineupsIsVisible;
        public bool LineupsIsVisible
        {
            get { return lineupsIsVisible; }
            set { lineupsIsVisible = value; OnPropertyChanged(); }
        }

        bool commentaryIsVisible;
        public bool CommentaryIsVisible
        {
            get { return commentaryIsVisible; }
            set { commentaryIsVisible = value; OnPropertyChanged(); }
        }

        bool statisticsIsVisible;
        public bool StatisticsIsVisible
        {
            get { return statisticsIsVisible; }
            set { statisticsIsVisible = value; OnPropertyChanged(); }
        }

        bool liveStandingsIsVisible;
        public bool LiveStandingsIsVisible
        {
            get { return liveStandingsIsVisible; }
            set { liveStandingsIsVisible = value; OnPropertyChanged(); }
        }

        //**********************//
        //     CONSTRUCTOR      //
        //**********************//
        public MatchDetailViewModel()
        {
            DystirService = DependencyService.Get<DystirService>();
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            var timeService = DependencyService.Get<TimeService>();
            timeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;
            timeService.StartSponsorsTime();

            MatchDetailsTabTapped = new Command<MatchDetailsTab>(OnMatchDetailsTabTapped);
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadMatchDetailAsync()
        {
            try
            {
                IsLoading = true;
                MatchDetails = DystirService.AllMatches.FirstOrDefault(x => x.MatchDetailsID == MatchID);
                if (MatchDetails?.IsDataLoaded == false)
                {
                    MatchDetails = await DystirService.GetMatchDetailsAsync(MatchID);
                }
                SelectedMatch = MatchDetails.Match;
                await PopulateMatchDetailsTabs();
                await LoadMatchDetailsData();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        public async Task PopulateMatchDetailsTabs()
        {
            var matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
            var matchDetailsTabsViews = new ObservableCollection<ContentView>();
            var matchTypeID = SelectedMatch.MatchTypeID;

            if (matchTypeID != null)
            {
                matchDetailsTabs.Add(new MatchDetailsTab()
                {
                    TabIndex = matchDetailsTabs.Count,
                    TabName = Resources.Localization.Resources.Summary,
                    TextColor = Color.LimeGreen
                });
                matchDetailsTabs.Add(new MatchDetailsTab()
                {
                    TabIndex = matchDetailsTabs.Count,
                    TabName = Resources.Localization.Resources.FirstEleven
                });
                matchDetailsTabs.Add(new MatchDetailsTab()
                {
                    TabIndex = matchDetailsTabs.Count,
                    TabName = Resources.Localization.Resources.Commentary
                });
                matchDetailsTabs.Add(new MatchDetailsTab()
                {
                    TabIndex = matchDetailsTabs.Count,
                    TabName = Resources.Localization.Resources.Statistics
                });

                if (matchTypeID == 1
                    || matchTypeID == 5
                    || matchTypeID == 6
                    || matchTypeID == 15
                    || matchTypeID == 101)
                {
                    matchDetailsTabs.Add(new MatchDetailsTab()
                    {
                        TabIndex = matchDetailsTabs.Count,
                        TabName = Resources.Localization.Resources.StandingsTab
                    });
                }
            }

            MatchDetailsTabs = new ObservableCollection<MatchDetailsTab>(matchDetailsTabs);
            if(SelectedMatchDetailsTab == null)
            {
                SelectedMatchDetailsTab = MatchDetailsTabs.FirstOrDefault();
            }
            await Task.CompletedTask;
        }

        public async Task LoadMatchDetailsData()
        {
            await Task.WhenAll(LoadSummaryAsync(), LoadLineupsAsync(), LoadCommentaryAsync(), LoadMatchStatisticsAsync(), LoadLiveStandingAsync());
        }

        private async Task LoadSummaryAsync()
        {
            Summary = new ObservableCollection<SummaryEventOfMatch>(GetSummary(MatchDetails));
            await Task.CompletedTask;
        }

        private async Task LoadLineupsAsync()
        {
            var starterPlayers = MatchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 1)
                .OrderByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number);
            var substitutionPlayers = MatchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 2)
                .OrderByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number);

            var homeTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == MatchDetails?.Match.HomeTeam));
            var awayTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == MatchDetails?.Match.AwayTeam));
            var homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == MatchDetails?.Match.HomeTeam));
            var awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == MatchDetails?.Match.AwayTeam));

            var lineups = new ObservableCollection<PlayersInLineups>();

            var biggerLineups = homeTeamLineups.Count >= awayTeamLineups.Count ? homeTeamLineups : awayTeamLineups;
            for (int i = 0; i < biggerLineups.Count; i++)
            {
                var playerInLineups = new PlayersInLineups()
                {
                    HomePlayer = homeTeamLineups.Count > i ? homeTeamLineups[i] : new PlayerOfMatch(),
                    AwayPlayer = awayTeamLineups.Count > i ? awayTeamLineups[i] : new PlayerOfMatch()
                };
                lineups.Add(playerInLineups);
            }

            var biggerSubstitution = homeTeamSubtitutions.Count >= awayTeamLineups.Count ? homeTeamSubtitutions : awayTeamSubtitutions;
            for (int i = 0; i < biggerSubstitution.Count; i++)
            {
                var playerInLineups = new PlayersInLineups()
                {
                    HomePlayer = homeTeamSubtitutions.Count > i ? homeTeamSubtitutions[i] : new PlayerOfMatch(),
                    AwayPlayer = awayTeamSubtitutions.Count > i ? awayTeamSubtitutions[i] : new PlayerOfMatch(),
                    IsFirstSub = i == 0
                };
                lineups.Add(playerInLineups);
            }

            Lineups = new ObservableCollection<PlayersInLineups>(lineups);
            await Task.CompletedTask;
        }

        private async Task LoadCommentaryAsync()
        {
            Commentary = new ObservableCollection<SummaryEventOfMatch>(GetCommentary(MatchDetails));
            await Task.CompletedTask;
        }

        private async Task LoadMatchStatisticsAsync()
        {
            MatchStatistics = new MatchStatistics(MatchDetails.EventsOfMatch, MatchDetails.Match);
            await Task.CompletedTask;
        }

        private async Task LoadLiveStandingAsync()
        {
            LiveStanding = DependencyService.Get<LiveStandingService>().GetStanding(MatchDetails?.Match?.MatchTypeName);
            await Task.CompletedTask;
        }

        private static ObservableCollection<PlayerOfMatch> GetLineups(MatchDetails matchDetails, string Team, int playingStatus)
        {
            var lineUps = matchDetails.PlayersOfMatch.Where(x => x.TeamName == Team && x.PlayingStatus == playingStatus).OrderBy(x => x.Number);
            return new ObservableCollection<PlayerOfMatch>(lineUps);
        }

        private static ObservableCollection<SummaryEventOfMatch> GetSummary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetEventOfMatchList(matchDetails, true);
            if (matchDetails.Match?.StatusID < 12)
            {
                listSummaryEvents.Reverse();
            }
            return new ObservableCollection<SummaryEventOfMatch>(listSummaryEvents);
        }

        private static ObservableCollection<SummaryEventOfMatch> GetCommentary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listCommentaryEvents = GetEventOfMatchList(matchDetails, false);
            listCommentaryEvents.Reverse();
            return new ObservableCollection<SummaryEventOfMatch>(listCommentaryEvents);
        }

        private static List<SummaryEventOfMatch> GetEventOfMatchList(MatchDetails matchDetails, bool isSummaryList)
        {
            List<SummaryEventOfMatch> eventOfMatchesList = new List<SummaryEventOfMatch>();
            Match selectedMatch = matchDetails.Match;
            var homeTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.HomeTeam.Trim());
            var awayTeamPlayers = matchDetails.PlayersOfMatch?.Where(x => x.TeamName.Trim() == selectedMatch.AwayTeam.Trim());
            var eventsList = isSummaryList ? matchDetails.EventsOfMatch?
                .Where(x => x.EventName == "GOAL"
                || x.EventName == "OWNGOAL"
                || x.EventName == "PENALTYSCORED"
                || x.EventName == "PENALTYMISSED"
                || x.EventName == "YELLOW"
                || x.EventName == "RED"
                || x.EventName == "SUBSTITUTION"
                || x.EventName == "PLAYEROFTHEMATCH"
                || x.EventName == "ASSIST").ToList() : matchDetails.EventsOfMatch.ToList();

            int homeScore = 0;
            int awayScore = 0;
            int homeTeamPenaltiesScore = 0;
            int awayTeamPenaltiesScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventOfMatch>())
            {
                SummaryEventOfMatch summaryEventOfMatch = new SummaryEventOfMatch(eventOfMatch, selectedMatch); ;
                PlayerOfMatch mainPlayerOfMatch = matchDetails.PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.MainPlayerOfMatchID);
                string mainPlayerFullName = (mainPlayerOfMatch?.FirstName?.Trim() + " " + mainPlayerOfMatch?.LastName?.Trim())?.Trim();
                PlayerOfMatch secondPlayerOfMatch = matchDetails.PlayersOfMatch.FirstOrDefault(x => x.PlayerOfMatchID == eventOfMatch.SecondPlayerOfMatchID);
                string secondPlayerFullName = (secondPlayerOfMatch?.FirstName?.Trim() + " " + secondPlayerOfMatch?.LastName?.Trim())?.Trim();
                if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                {
                    summaryEventOfMatch.IsHomeTeamEvent = true;
                    summaryEventOfMatch.HomeMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.HomeSecondPlayer = secondPlayerFullName;
                }
                else
                {
                    summaryEventOfMatch.IsAwayTeamEvent = true;
                    summaryEventOfMatch.AwayMainPlayer = mainPlayerFullName;
                    summaryEventOfMatch.AwaySecondPlayer = secondPlayerFullName;
                }
                summaryEventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeTeam);
                summaryEventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwayTeam);
                summaryEventOfMatch.IsGoal = eventOfMatch.EventName == "GOAL"
                    || eventOfMatch.EventName == "OWNGOAL"
                    || eventOfMatch.EventName == "PENALTYSCORED";
                if (summaryEventOfMatch.IsGoal)
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                    {
                        if (eventOfMatch.EventPeriodID != 10)
                        {
                            homeScore += 1;
                        }
                        else
                        {
                            homeTeamPenaltiesScore += 1;
                        }
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                    {
                        if (eventOfMatch.EventPeriodID != 10)
                        {
                            awayScore += 1;
                        }
                        else
                        {
                            awayTeamPenaltiesScore += 1;
                        }
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;
                summaryEventOfMatch.HomeTeamPenaltiesScore = homeTeamPenaltiesScore;
                summaryEventOfMatch.AwayTeamPenaltiesScore = awayTeamPenaltiesScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count)
                    {
                        EventOfMatch nextEvent = eventsList[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            PlayerOfMatch assistPlayerOfMatch = matchDetails.PlayersOfMatch?.FirstOrDefault(x => x.PlayerOfMatchID == nextEvent.MainPlayerOfMatchID);
                            string assistPlayerFullName = (assistPlayerOfMatch?.FirstName?.Trim() + " " + assistPlayerOfMatch?.LastName?.Trim())?.Trim();
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.HomeSecondPlayer = assistPlayerFullName;
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.AwaySecondPlayer = assistPlayerFullName;
                            }

                        }
                    }
                }
                else if (eventOfMatch.EventName == "ASSIST" && isSummaryList)
                {
                    continue;
                }
                else if (eventOfMatch.EventName == "PLAYEROFTHEMATCH")
                {
                    summaryEventOfMatch.TextColorOfEventMinute = Color.DarkOrange;
                    summaryEventOfMatch.EventMinute = Resources.Localization.Resources.PlayerOfTheMatch;
                }
                else if (eventOfMatch.EventName == "SUBSTITUTION")
                {
                    var homeMainPlayer = summaryEventOfMatch.HomeMainPlayer;
                    var awayMainPlayer = summaryEventOfMatch.AwayMainPlayer;
                    summaryEventOfMatch.HomeMainPlayer = summaryEventOfMatch.HomeSecondPlayer;
                    summaryEventOfMatch.AwayMainPlayer = summaryEventOfMatch.AwaySecondPlayer;
                    summaryEventOfMatch.HomeSecondPlayer = homeMainPlayer;
                    summaryEventOfMatch.AwaySecondPlayer = awayMainPlayer;
                }
                eventOfMatchesList.Add(summaryEventOfMatch);
            }

            return eventOfMatchesList.ToList();
        }

        private async Task SetMatchesBySelectedDate()
        {
            var matches = DystirService.AllMatches?.Where(x => x.Match.Time?.Date == SelectedMatch?.Time?.AddSeconds(1).Date)
                .Select(x => x.Match)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)?.ToList() ?? new List<Match>();

            if (SelectedMatch != null)
            {
                matches.RemoveAll(x => x.MatchID == MatchDetails.MatchDetailsID);
                matches.Insert(0, SelectedMatch);
                MatchesBySelectedDate = new ObservableCollection<Match>(matches);
            }
            await Task.CompletedTask;
        }

        private void SetDetailsTabSelected()
        {
            foreach (MatchDetailsTab matchDetailsTab in MatchDetailsTabs ?? new ObservableCollection<MatchDetailsTab>())
            {
                matchDetailsTab.TextColor = matchDetailsTab == SelectedMatchDetailsTab ? Color.LimeGreen : Color.White;
            }
            int positionIndex = MatchDetailsTabs?.IndexOf(SelectedMatchDetailsTab) ?? 0;
            SummaryIsVisible = LineupsIsVisible = CommentaryIsVisible = StatisticsIsVisible = LiveStandingsIsVisible = false;
            switch (positionIndex)
            {
                case 0:
                    SummaryIsVisible = true;
                    break;
                case 1:
                    LineupsIsVisible = true;
                    break;
                case 2:
                    CommentaryIsVisible = true;
                    break;
                case 3:
                    StatisticsIsVisible = true;
                    break;
                case 4:
                    LiveStandingsIsVisible = true;
                    break;
            }
        }

        //**********************//
        //        EVENTS        //
        //**********************//
        private void OnMatchDetailsTabTapped(MatchDetailsTab matchDetailsTab)
        {
            if (matchDetailsTab == null)
                return;

            SelectedMatchDetailsTab = matchDetailsTab;
        }

        private void DystirService_OnShowLoading()
        {
            IsLoading = true;
        }

        private void DystirService_OnFullDataLoaded()
        {
            _ = LoadMatchDetailAsync();
        }

        private async void DystirService_OnMatchDetailsLoaded(MatchDetails matchDetails)
        {
            if (MatchDetails.MatchDetailsID == matchDetails?.MatchDetailsID)
            {
                SelectedMatch = matchDetails.Match;
                MatchDetails = matchDetails;
                await LoadMatchDetailsData();
            }
            await SetMatchesBySelectedDate();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            _ = SetSponsors();
        }
    }
}

