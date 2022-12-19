﻿using System;
using System.Collections.ObjectModel;
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
        private ObservableCollection<MatchDetails> AllMatchesDetails = new ObservableCollection<MatchDetails>();
        private MatchDetails _matchDetails;

        Match selectedMatch = new Match();
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

        ObservableCollection<PlayersInRow> lineups;
        public ObservableCollection<PlayersInRow> Lineups
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

        ObservableCollection<MatchDetailsTab> matchDetailsTabs = new ObservableCollection<MatchDetailsTab>();
        public ObservableCollection<MatchDetailsTab> MatchDetailsTabs
        {
            get { return matchDetailsTabs; }
            set { matchDetailsTabs = value; OnPropertyChanged(); }
        }

        MatchDetailsTab selectedMatchDetailsTab = new MatchDetailsTab()
        {
            TabName = Resources.Localization.Resources.Summary,
            TextColor = Colors.LimeGreen
        };

        public MatchDetailsTab SelectedMatchDetailsTab
        {
            get { return selectedMatchDetailsTab; }
            set
            {
                selectedMatchDetailsTab = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<ContentView> matchDetailsTabsViews = new ObservableCollection<ContentView>();
        public ObservableCollection<ContentView> MatchDetailsTabsViews
        {
            get { return matchDetailsTabsViews; }
            set { matchDetailsTabsViews = value; OnPropertyChanged(); }
        }

        bool isLoadingSelectedMatch = false;

        public bool IsLoadingSelectedMatch
        {
            get { return isLoadingSelectedMatch; }
            set { isLoadingSelectedMatch = value; OnPropertyChanged(); }
        }

        //**********************//
        //     CONSTRUCTOR      //
        //**********************//
        public MatchDetailsViewModel(DystirService dystirService)
        {
            DystirService = dystirService;
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task SetDetailsTabSelected(int tabIndex)
        {
            await Task.Delay(200);
            foreach (MatchDetailsTab matchDetailsTab in MatchDetailsTabs)
            {
                matchDetailsTab.TextColor = matchDetailsTab.TabIndex == tabIndex ? Colors.LimeGreen : Colors.White;
            }
            await LoadMatchDataAsync();

            switch (tabIndex)
            {
                case 0:
                default:
                    if (Summary == null)
                    {
                        _ = LoadSummaryAsync(_matchDetails);
                    }
                    break;
                case 1:
                    if (Lineups == null)
                    {
                        _ = LoadLineupsAsync(_matchDetails);
                    }
                    break;
                case 2:
                    if (Commentary == null)
                    {
                        _ = LoadCommentaryAsync(_matchDetails);
                    }
                    break;
                case 3:
                    if (MatchStatistics == null)
                    {
                        _ = LoadMatchStatisticsAsync(_matchDetails);
                    }
                    break;
                case 4:
                    if (MatchStatistics == null)
                    {
                        _ = LoadMatchStatisticsAsync(_matchDetails);
                    }
                    break;
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
                matchDetailsTabsViews.Add(new SummaryView(this));
                matchDetailsTabsViews.Add(new LineupsView(this));
                matchDetailsTabsViews.Add(new CommentaryView(this));
                matchDetailsTabsViews.Add(new StatisticView(this));

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
                    matchDetailsTabsViews.Add(new StatisticView(this));
                }
            }
            
            MatchDetailsTabs = new ObservableCollection<MatchDetailsTab>(matchDetailsTabs);
            MatchDetailsTabsViews = new ObservableCollection<ContentView>(matchDetailsTabsViews);
            await Task.CompletedTask;
        }

        public void ClearMatchDetails()
        {
            Summary = null;
            Lineups = null;
            Commentary = null;
            MatchStatistics = null;
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private async Task LoadMatchDataAsync()
        {
            _matchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == SelectedMatch.MatchID);
            if (_matchDetails == null || _matchDetails.IsDataLoaded == false)
            {
                IsLoadingSelectedMatch = true;
                _matchDetails = await DystirService.GetMatchDetailsAsync(SelectedMatch.MatchID);
                _matchDetails.IsDataLoaded = true;
                SelectedMatch = _matchDetails.Match;
                SetMatchesBySelectedDate();
                SetMatchDetails(_matchDetails);
                _ = PopulateMatchDetailsTabs(SelectedMatch);
            }
            IsLoadingSelectedMatch = false;
        }

        private async Task LoadDataAsync(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                var loadLineupsTask = LoadLineupsAsync(matchDetails);
                var loadSummaryTask = LoadSummaryAsync(matchDetails);
                var loadCommentaryTask = LoadCommentaryAsync(matchDetails);
                var loadMatchStatisticsTask = LoadMatchStatisticsAsync(matchDetails);
                await Task.WhenAll(loadLineupsTask, loadSummaryTask, loadCommentaryTask, loadMatchStatisticsTask);
            }
        }

        private void DystirService_OnShowLoading()
        {
            if (SelectedMatch != null)
            {
                IsLoadingSelectedMatch = true;
            }
        }

        private void DystirService_OnMatchDetailsLoaded(Match match)
        {
            if (SelectedMatch?.MatchID == match?.MatchID)
            {
                _matchDetails.IsDataLoaded = false;
                _ = LoadMatchDataAsync();
            }
            else
            {
                SetMatchesBySelectedDate();
            }
        }

        private void DystirService_OnFullDataLoaded()
        {
            _matchDetails.IsDataLoaded = false;
            _ = LoadMatchDataAsync();
        }

        private void SetMatchDetails(MatchDetails matchDetails)
        {
            if (matchDetails != null)
            {
                var foundMatchDetails = AllMatchesDetails.FirstOrDefault(x => x.MatchDetailsID == matchDetails.MatchDetailsID);
                if (foundMatchDetails != null)
                {
                    AllMatchesDetails.Remove(foundMatchDetails);
                }
                AllMatchesDetails.Add(matchDetails);
                //_ = LoadDataAsync(matchDetails);
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
            Summary = new ObservableCollection<SummaryEventOfMatch>(GetSummary(matchDetails));
            await Task.CompletedTask;
        }

        private async Task LoadLineupsAsync(MatchDetails matchDetails)
        {
            var homeTeamLineups = new ObservableCollection<PlayerOfMatch>();
            var awayTeamLineups = new ObservableCollection<PlayerOfMatch>();
            var homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>();
            var awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>();

            var starterPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 1)
                .OrderByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number);
            var substitutionPlayers = matchDetails.PlayersOfMatch.Where(x => x.PlayingStatus == 2)
                .OrderByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number);

            var homeTeamLineupsTask = Task.Run(() =>
            {
                homeTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
            });
            var awayTeamLineupsTask = Task.Run(() =>
            {
                awayTeamLineups = new ObservableCollection<PlayerOfMatch>(starterPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));
            });
            var homeTeamSubtitutionsTask = Task.Run(() =>
            {
                homeTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.HomeTeam));
            });
            var awayTeamSubtitutionsTask = Task.Run(() =>
            {
                awayTeamSubtitutions = new ObservableCollection<PlayerOfMatch>(substitutionPlayers.Where(x => x.TeamName == matchDetails?.Match.AwayTeam));
            });

            await Task.WhenAll(homeTeamLineupsTask, awayTeamLineupsTask, homeTeamSubtitutionsTask, awayTeamSubtitutionsTask);

            Lineups = new ObservableCollection<PlayersInRow>(new Lineups(homeTeamLineups, awayTeamLineups, homeTeamSubtitutions, awayTeamSubtitutions));
        }

        private async Task LoadCommentaryAsync(MatchDetails matchDetails)
        {
            Commentary = new ObservableCollection<SummaryEventOfMatch>(GetCommentary(matchDetails));
            //Commentary = new ObservableCollection<CommentaryEventGroup>(commentary.GroupBy(x => x.EventOfMatch.EventOfMatchID).Select(group => new CommentaryEventGroup(group.Key, new ObservableCollection<SummaryEventOfMatch>(group))));

            await Task.CompletedTask;
        }

        private async Task LoadMatchStatisticsAsync(MatchDetails matchDetails)
        {
            MatchStatistics = new MatchStatistics(matchDetails.EventsOfMatch, matchDetails.Match);
            await Task.CompletedTask;
        }

        private ObservableCollection<PlayerOfMatch> GetLineups(MatchDetails matchDetails, string Team, int playingStatus)
        {
            var lineUps = matchDetails.PlayersOfMatch.Where(x => x.TeamName == Team && x.PlayingStatus == playingStatus).OrderBy(x => x.Number);
            return new ObservableCollection<PlayerOfMatch>(lineUps);
        }

        private ObservableCollection<SummaryEventOfMatch> GetSummary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listSummaryEvents = GetEventOfMatchList(matchDetails, true);
            if (matchDetails.Match?.StatusID < 12)
            {
                listSummaryEvents.Reverse();
            }
            return new ObservableCollection<SummaryEventOfMatch>(listSummaryEvents);
        }

        private ObservableCollection<SummaryEventOfMatch> GetCommentary(MatchDetails matchDetails)
        {
            List<SummaryEventOfMatch> listCommentaryEvents = GetEventOfMatchList(matchDetails, false);
            listCommentaryEvents.Reverse();
            return new ObservableCollection<SummaryEventOfMatch>(listCommentaryEvents);
        }

        private List<SummaryEventOfMatch> GetEventOfMatchList(MatchDetails matchDetails, bool isSummaryList)
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
    }
}

