using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dystir.Services;
using Dystir.Models;
using System.Runtime.CompilerServices;

namespace Dystir.ViewModels
{
    public class ResultsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        private ObservableCollection<Match> AllResultsMatches = new ObservableCollection<Match>();

        ObservableCollection<MatchGroup> resultsGroupList = new ObservableCollection<MatchGroup>();
        public ObservableCollection<MatchGroup> ResultsGroupList
        {
            get { return resultsGroupList; }
            set { resultsGroupList = value; OnPropertyChanged(); }
        }

        Competition resultsCompetitionSelected;
        public Competition ResultsCompetitionSelected
        {
            get { return resultsCompetitionSelected; }
            set { resultsCompetitionSelected = value; OnPropertyChanged(); _ = SetSelectedCompetition(); }
        }

        ObservableCollection<Competition> resultsCompetitions = new ObservableCollection<Competition>();
        public ObservableCollection<Competition> ResultsCompetitions
        {
            get { return resultsCompetitions; }
            set { resultsCompetitions = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public ResultsViewModel(DystirService dystirService, TimeService timeService)
        {
            DystirService = dystirService;
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            timeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;
            timeService.StartSponsorsTime();

            _ = SetResultCompetitions();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(200);
            await SetResultCompetitions();
            await SetSponsors();
            IsLoading = false;
        }

        //**********************//
        //   PRIVATE METHODS    //
        //**********************//
        private void DystirService_OnShowLoading()
        {
            IsLoading = true;
        }

        private void DystirService_OnFullDataLoaded()
        {
            _ = LoadDataAsync();
        }

        private void DystirService_OnMatchDetailsLoaded(MatchDetails matchDetails)
        {
            _ = LoadDataAsync();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            _ = SetSponsors();
        }

        private async Task SetSelectedCompetition()
        {
            await SetCompetitionsSelection();
            await SetResults();
        }

        private async Task SetCompetitionsSelection()
        {
            var resultsCompetitions = ResultsCompetitions;
            foreach (Competition competition in resultsCompetitions)
            {
                competition.TextColor = competition.CompetitionName == ResultsCompetitionSelected.CompetitionName ? Colors.LimeGreen : Colors.White;
            }
            ResultsCompetitions = new ObservableCollection<Competition>(resultsCompetitions);
            await Task.CompletedTask;
        }

        private async Task SetResultCompetitions()
        {
            var toDate = DateTime.Now.Date.AddDays(0);
            var fromDate = new DateTime(toDate.Year - 2, 12, 31);

            var resultsMatches = DystirService.AllMatches?.Where(x => x.Match.Time > fromDate && x.Match.Time < toDate || (x.Match.StatusID == 13 || x.Match.StatusID == 12));
            var orderedResultsMatches = resultsMatches.OrderBy(x => x.Match.MatchTypeID).ThenByDescending(x => x.Match.Time).Select(x=>x.Match);
            AllResultsMatches = new ObservableCollection<Match>(orderedResultsMatches);

            var allResultsGroupList = new ObservableCollection<IGrouping<string, Match>>(AllResultsMatches?.GroupBy(x => x.MatchTypeName));
            var resultsCompetitions = new ObservableCollection<Competition>();
            foreach (IGrouping<string, Match> competitionMatches in allResultsGroupList ?? new ObservableCollection<IGrouping<string, Match>>())
            {
                if (!string.IsNullOrEmpty(competitionMatches.Key))
                {
                    Competition competition = new Competition()
                    {
                        CompetitionName = competitionMatches.Key,
                        TextColor = Colors.White
                    };
                    resultsCompetitions.Add(competition);
                }
            }

            ResultsCompetitions = new ObservableCollection<Competition>(resultsCompetitions);
            if (ResultsCompetitionSelected == null && ResultsCompetitions.Count > 0)
            {
                ResultsCompetitionSelected = ResultsCompetitions.FirstOrDefault();
            }
            await SetSelectedCompetition();
            await Task.CompletedTask;
        }

        private async Task SetResults()
        {
            var resultsMatches = AllResultsMatches?.Where(x => x.MatchTypeName == ResultsCompetitionSelected.CompetitionName)
                .OrderByDescending(x => x.RoundID)
                .ThenByDescending(x => x.Time)
                .ThenBy(x => x.MatchTypeID);
            ResultsGroupList = new ObservableCollection<MatchGroup>(resultsMatches.GroupBy(x => x.RoundName).Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group))));
            await Task.CompletedTask;
        }
    }
}

