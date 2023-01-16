using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Dystir.Views;
using Dystir.Services;

namespace Dystir.Models
{
    public class MatchDetails : INotifyPropertyChanged
    {
        //*****************************//
        //         PROPERTIES          //
        //*****************************//
        public bool IsDataLoaded = false;

        private readonly LanguageService _languageService;
        private readonly DystirService dystirService;
        private readonly LiveStandingService liveStandingService;

        public int MatchDetailsID { get; set; }

        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; }

        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; }

        Match match;
        public Match Match
        {
            get { return match; }
            set { match = value; OnPropertyChanged(); }
        }

        ObservableCollection<MatchDetailsTab> matchDetailsTabs;
        public ObservableCollection<MatchDetailsTab> MatchDetailsTabs
        {
            get { return matchDetailsTabs; }
            set { matchDetailsTabs = value; OnPropertyChanged(); }
        }

        ObservableCollection<ContentView> matchDetailsTabsViews = new ObservableCollection<ContentView>();
        public ObservableCollection<ContentView> MatchDetailsTabsViews
        {
            get { return matchDetailsTabsViews; }
            set { matchDetailsTabsViews = value; OnPropertyChanged(); }
        }

        ObservableCollection<Match> matchesBySelectedDate = new ObservableCollection<Match>();
        public ObservableCollection<Match> MatchesBySelectedDate
        {
            get { return matchesBySelectedDate; }
            set { matchesBySelectedDate = value; OnPropertyChanged(); }
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

        bool isLoadingSelectedMatch = false;
        public bool IsLoadingSelectedMatch
        {
            get { return isLoadingSelectedMatch; }
            set { isLoadingSelectedMatch = value; OnPropertyChanged(); }
        }

        //*****************************//
        //        CONSTRUCTOR          //
        //*****************************//
        public MatchDetails()
        {
        }

        public MatchDetails(DystirService dystirService, LiveStandingService liveStandingService)
        {
            this.dystirService = dystirService;
            this.liveStandingService = liveStandingService;
            _languageService = DependencyService.Get<LanguageService>();

            this.dystirService.OnShowLoading += DystirService_OnShowLoading;
            this.dystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
            this.dystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task Startup()
        {
            await PopulateMatchDetailsTabs(Match);
            await LoadMatchDetailsAsync();
        }

        public void SetDetailsTabSelected(int tabIndex)
        {
            foreach (MatchDetailsTab matchDetailsTab in MatchDetailsTabs ?? new ObservableCollection<MatchDetailsTab>())
            {
                matchDetailsTab.TextColor = matchDetailsTab.TabIndex == tabIndex ? Colors.LimeGreen : Colors.White;
            }
        }

        public async Task PopulateMatchDetailsTabs(Match match)
        {
            var matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
            var matchDetailsTabsViews = new ObservableCollection<ContentView>();

            if (match.MatchTypeID != null)
            {
                matchDetailsTabs.Add(new MatchDetailsTab()
                {
                    TabIndex = matchDetailsTabs.Count,
                    TabName = Resources.Localization.Resources.Summary,
                    TextColor = Colors.LimeGreen
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

                matchDetailsTabsViews.Add(new SummaryView() { BindingContext = this });
                matchDetailsTabsViews.Add(new LineupsView());
                matchDetailsTabsViews.Add(new CommentaryView());
                matchDetailsTabsViews.Add(new StatisticView());

                if (match.MatchTypeID == 1
                    || match.MatchTypeID == 5
                    || match.MatchTypeID == 6
                    || match.MatchTypeID == 15
                    || match.MatchTypeID == 101)
                {
                    matchDetailsTabs.Add(new MatchDetailsTab()
                    {
                        TabIndex = matchDetailsTabs.Count,
                        TabName = Resources.Localization.Resources.StandingsTab
                    });
                    matchDetailsTabsViews.Add(new LiveStandingView());
                }
            }

            MatchDetailsTabs = new ObservableCollection<MatchDetailsTab>(matchDetailsTabs);
            MatchDetailsTabsViews = new ObservableCollection<ContentView>(matchDetailsTabsViews);
            await Task.CompletedTask;
        }


        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private async Task LoadMatchDetailsAsync()
        {
            IsLoadingSelectedMatch = true;
            var matchDetails = dystirService.AllMatches.FirstOrDefault(x => x.Match?.MatchID == Match.MatchID);
            if (IsDataLoaded == false)
            {
                matchDetails = await dystirService.GetMatchDetailsAsync(Match.MatchID);

                IsDataLoaded = true;
                Match = matchDetails.Match;
                MatchDetailsID = matchDetails.MatchDetailsID;
                EventsOfMatch = matchDetails.EventsOfMatch;
                PlayersOfMatch = matchDetails.PlayersOfMatch;

                await PopulateMatchDetailsTabs(Match);
                await PopulateMatchDetails();
                await SetMatchDetails(matchDetails);
            }

            await SetMatchesBySelectedDate();
            IsLoadingSelectedMatch = false;
        }

        private async Task PopulateMatchDetails()
        {
            await Task.WhenAll(LoadSummaryAsync(this), LoadLineupsAsync(this), LoadCommentaryAsync(this), LoadMatchStatisticsAsync(this), LoadLiveStandingAsync(this));
        }

        private async Task SetMatchDetails(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                var foundMatchDetails = dystirService.AllMatches.FirstOrDefault(x => x.Match?.MatchID == Match.MatchID);
                if (foundMatchDetails != null)
                {
                    dystirService.AllMatches.Remove(foundMatchDetails);
                }
                dystirService.AllMatches.Add(matchDetails);
            }
            await Task.CompletedTask;
        }

        private async Task SetMatchesBySelectedDate()
        {
            var matches = dystirService.AllMatches?.Where(x => x.Match.Time?.Date == Match?.Time?.AddSeconds(1).Date)
                .Select(x=>x.Match)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)?.ToList() ?? new List<Match>();

            if (Match != null)
            {
                matches.RemoveAll(x => x.MatchID == MatchDetailsID);
                matches.Insert(0, Match);
                MatchesBySelectedDate = new ObservableCollection<Match>(matches);
            }
            await Task.CompletedTask;
        }

        private async Task LoadSummaryAsync(MatchDetails matchDetails)
        {
            Summary = new ObservableCollection<SummaryEventOfMatch>(GetSummary(matchDetails));
            await Task.CompletedTask;
        }

        private async Task LoadLineupsAsync(MatchDetails matchDetails)
        {
            if (Lineups == null)
            {
                var starterPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 1)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);
                var substitutionPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 2)
                    .OrderByDescending(x => x.Position == "GK")
                    .ThenBy(x => x.Number);

                var homeTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
                var awayTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));
                var homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
                var awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));

                var lineups = new ObservableCollection<PlayersInLineups>();

                var biggerLineups = homeTeamLineups.Count >= awayTeamLineups.Count ? homeTeamLineups : awayTeamLineups;
                for (int i = 0; i < biggerLineups.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamLineups.Count > i ? homeTeamLineups[i] : null,
                        AwayPlayer = awayTeamLineups.Count > i ? awayTeamLineups[i] : null
                    };
                    lineups.Add(playerInLineups);
                }

                var biggerSubstitution = homeTeamSubtitutions.Count >= awayTeamLineups.Count ? homeTeamSubtitutions : awayTeamSubtitutions;
                for (int i = 0; i < biggerSubstitution.Count; i++)
                {
                    var playerInLineups = new PlayersInLineups()
                    {
                        HomePlayer = homeTeamSubtitutions.Count > i ? homeTeamSubtitutions[i] : null,
                        AwayPlayer = awayTeamSubtitutions.Count > i ? awayTeamSubtitutions[i] : null,
                        IsFirstSub = i == 0
                    };
                    lineups.Add(playerInLineups);
                }

                Lineups = new ObservableCollection<PlayersInLineups>(lineups);
            }
            await Task.CompletedTask;
        }

        private async Task LoadCommentaryAsync(MatchDetails matchDetails)
        {
            if (Commentary == null)
            {
                Commentary = new ObservableCollection<SummaryEventOfMatch>(GetCommentary(matchDetails));
            }
            await Task.CompletedTask;
        }

        private async Task LoadMatchStatisticsAsync(MatchDetails matchDetails)
        {
            if (MatchStatistics == null)
            {
                MatchStatistics = new MatchStatistics(matchDetails.EventsOfMatch, matchDetails.Match);
            }
            await Task.CompletedTask;
        }

        private async Task LoadLiveStandingAsync(MatchDetails matchDetails)
        {
            if (LiveStanding == null)
            {
                LiveStanding = liveStandingService.GetStanding(matchDetails.Match);
            }
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
                            PlayerOfMatch assistPlayerOfMatch = matchDetails.PlayersOfMatch?.First(x => x.PlayerOfMatchID == nextEvent.MainPlayerOfMatchID);
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
                    summaryEventOfMatch.ShowMinutes = false;
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

        //**********************//
        //        EVENTS        //
        //**********************//
        private void DystirService_OnShowLoading()
        {
            _ = LoadMatchDetailsAsync();
        }

        private async void DystirService_OnMatchDetailsLoaded(MatchDetails matchDetails)
        {
            if (MatchDetailsID == matchDetails?.Match?.MatchID)
            {
                await SetMatchDetails(matchDetails);
            }
            await SetMatchesBySelectedDate();
        }

        private void DystirService_OnFullDataLoaded()
        {
            _ = LoadMatchDetailsAsync();
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}