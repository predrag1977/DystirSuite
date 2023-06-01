﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        public int MatchID { get; internal set; }
        public Command<MatchDetailsTab> MatchDetailsTabTapped { get; }

        //private readonly BackgroundWorker backgroundWorker;
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

            //backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += DoWork;
            
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadMatchDetailAsync()
        {
            try
            {
                IsLoading = true;
                var reverse = DystirService.AllMatches.Reverse();
                foreach (var matchDetails in reverse)
                {
                    if(matchDetails.MatchDetailsID == MatchID)
                    {
                        if (matchDetails?.IsDataLoaded == false)
                        {
                            MatchDetails = await DystirService.GetMatchDetailsAsync(MatchID);
                        }
                        else
                        {
                            MatchDetails = matchDetails;
                        }
                        break;
                    }
                }
                
                SelectedMatch = MatchDetails.Match;
                await PopulateMatchDetailsTabs();
                //await LoadMatchDetailsData();
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
            //await Task.WhenAll(
            //    LoadSummaryAsync(),
            //    LoadLineupsAsync(),
            //    LoadCommentaryAsync(),
            //    LoadMatchStatisticsAsync(),
            //    LoadLiveStandingAsync());
        }

        //private void DoWork(object sender, DoWorkEventArgs e)
        //{
        //    Application.Current.Dispatcher.BeginInvokeOnMainThread(new Action(async delegate
        //    {
        //        await Task.WhenAll(
        //            LoadSummaryAsync(),
        //            LoadLineupsAsync(),
        //            LoadCommentaryAsync(),
        //            LoadMatchStatisticsAsync(),
        //            LoadLiveStandingAsync());
        //    }));
        //}

        private static ObservableCollection<PlayerOfMatch> GetLineups(MatchDetails matchDetails, string Team, int playingStatus)
        {
            var lineUps = matchDetails.PlayersOfMatch.Where(x => x.TeamName == Team && x.PlayingStatus == playingStatus).OrderBy(x => x.Number);
            return new ObservableCollection<PlayerOfMatch>(lineUps);
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
                _ = LoadMatchDetailsData();
            }
            await SetMatchesBySelectedDate();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            _ = SetSponsors();
        }
    }
}
