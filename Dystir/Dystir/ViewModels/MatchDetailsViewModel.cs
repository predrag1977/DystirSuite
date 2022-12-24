using System;
using System.Collections.ObjectModel;
using System.IO;
using Dystir.Models;
using Dystir.Services;
using Dystir.Views;

namespace Dystir.ViewModels
{
    public class MatchDetailsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        Match selectedMatch;
        public Match SelectedMatch
        {
            get { return selectedMatch; }
            set { selectedMatch = value; OnPropertyChanged(); }
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

        MatchDetailsTab selectedMatchDetailsTab = new MatchDetailsTab()
        {
            TabName = Resources.Localization.Resources.Summary,
            TextColor = Colors.LimeGreen
        };
        public MatchDetailsTab SelectedMatchDetailsTab
        {
            get { return selectedMatchDetailsTab; }
            set { selectedMatchDetailsTab = value; }
        }

        ObservableCollection<MatchDetailsTab> matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
        public ObservableCollection<MatchDetailsTab> MatchDetailsTabs
        {
            get { return matchDetailsTabs; }
            set { matchDetailsTabs = value; OnPropertyChanged(); }
        }

        ObservableCollection<ContentView> matchDetailsTabsViews;
        public ObservableCollection<ContentView> MatchDetailsTabsViews
        {
            get { return matchDetailsTabsViews; }
            set { matchDetailsTabsViews = value; OnPropertyChanged(); }
        }

        //**********************//
        //     CONSTRUCTOR      //
        //**********************//
        public MatchDetailsViewModel(int matchID, DystirService dystirService, LiveStandingService liveStandingService)
        {
            DystirService = dystirService;
            LiveStandingService = liveStandingService;
            dystirService.OnShowLoading += DystirService_OnShowLoading;
            dystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
            dystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;

            SelectedMatch = dystirService.AllMatches.FirstOrDefault(x => x.MatchID == matchID);
            
            _ = LoadMatchDetailsAsync();
            _ = PopulateMatchDetailsTabs();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public void SetDetailsTabSelected(int tabIndex)
        {
            foreach (MatchDetailsTab matchDetailsTab in MatchDetailsTabs)
            {
                matchDetailsTab.TextColor = matchDetailsTab.TabIndex == tabIndex ? Colors.LimeGreen : Colors.White;
            }
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private async Task LoadMatchDetailsAsync()
        {
            IsLoadingSelectedMatch = true;
            await Task.Delay(200);
            var matchDetails = DystirService.AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == SelectedMatch.MatchID);
            if (matchDetails == null || matchDetails.IsDataLoaded == false)
            {
                matchDetails = await DystirService.GetMatchDetailsAsync(SelectedMatch.MatchID);
                matchDetails.IsDataLoaded = true;
                SetMatchDetails(matchDetails);
            }
            await PopulateMatchDetails(matchDetails);
            SetMatchesBySelectedDate();
            DystirService.AllMatchesDetailViewModels.Add(this);
            await Task.CompletedTask;
        }

        private async Task PopulateMatchDetailsTabs()
        {
            var matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
            var matchDetailsTabsViews = new ObservableCollection<ContentView>();

            if (SelectedMatch.MatchTypeID != null)
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

                matchDetailsTabsViews.Add(new SummaryView() { BindingContext = this});
                matchDetailsTabsViews.Add(new LineupsView());
                matchDetailsTabsViews.Add(new CommentaryView());
                matchDetailsTabsViews.Add(new StatisticView());

                if (SelectedMatch.MatchTypeID == 1
                    || SelectedMatch.MatchTypeID == 5
                    || SelectedMatch.MatchTypeID == 6
                    || SelectedMatch.MatchTypeID == 15
                    || SelectedMatch.MatchTypeID == 101)
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

        private async Task PopulateMatchDetails(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                await Task.WhenAll(LoadSummaryAsync(matchDetails), LoadLineupsAsync(matchDetails), LoadCommentaryAsync(matchDetails), LoadMatchStatisticsAsync(matchDetails), LoadLiveStandingAsync(matchDetails));
            }
            IsLoadingSelectedMatch = false;
        }

        private void SetMatchDetails(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                var foundMatchDetails = DystirService.AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == SelectedMatch.MatchID);
                if (foundMatchDetails != null)
                {
                    DystirService.AllMatchesDetails.Remove(foundMatchDetails);
                }
                DystirService.AllMatchesDetails.Add(matchDetails);
            }
        }

        private void SetMatchesBySelectedDate()
        {
            var matches = DystirService.AllMatches?.Where(x => x.Time?.Date == SelectedMatch?.Time?.AddSeconds(1).Date)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)?.ToList() ?? new List<Match>();

            if (SelectedMatch != null)
            {
                matches.RemoveAll(x => x.MatchID == SelectedMatch.MatchID);
                matches.Insert(0, SelectedMatch);
                MatchesBySelectedDate = new ObservableCollection<Match>(matches);
            }
        }

        private async Task LoadSummaryAsync(MatchDetails matchDetails)
        {
            if (Summary == null)
            {
                Summary = new ObservableCollection<SummaryEventOfMatch>(GetSummary(matchDetails));
            }
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
                LiveStanding = LiveStandingService.GetStanding(matchDetails.Match);
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

        private void DystirService_OnMatchDetailsLoaded(MatchDetails matchDetails)
        {
            if (SelectedMatch?.MatchID == matchDetails?.Match?.MatchID)
            {
                SetMatchDetails(matchDetails);
                _ = PopulateMatchDetails(matchDetails);
            }
            SetMatchesBySelectedDate();
        }

        private void DystirService_OnFullDataLoaded()
        {
            _ = LoadMatchDetailsAsync();
        }
    }
}

