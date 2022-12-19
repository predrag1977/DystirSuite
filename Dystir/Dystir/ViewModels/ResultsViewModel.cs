using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dystir.Services;
using Dystir.Models;

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

        string resultsCompetitionSelected;
        public string ResultsCompetitionSelected
        {
            get { return resultsCompetitionSelected; }
            set { resultsCompetitionSelected = value; OnPropertyChanged(); SetResults(); }
        }

        ObservableCollection<string> resultsCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> ResultsCompetitions
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

            SetResultCompetitions();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async void LoadDataAsync()
        {
            await Task.Delay(100);
            SetResultCompetitions();
            SetResults();
            SetSponsors();
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
            LoadDataAsync();
        }

        private void DystirService_OnMatchDetailsLoaded(Match match)
        {
            LoadDataAsync();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            SetSponsors();
        }

        private void SetResultCompetitions()
        {
            var toDate = DateTime.Now.Date.AddDays(0);
            var fromDate = new DateTime(toDate.Year - 1, 12, 31);

            var resultsMatches = DystirService.AllMatches?.Where(x => x.Time > fromDate && x.Time < toDate || (x.StatusID == 13 || x.StatusID == 12));
            var orderedResultsMatches = resultsMatches.OrderBy(x => x.MatchTypeID).ThenByDescending(x => x.Time);
            AllResultsMatches = new ObservableCollection<Match>(orderedResultsMatches);

            var allResultsGroupList = new ObservableCollection<IGrouping<string, Match>>(AllResultsMatches?.GroupBy(x => x.MatchTypeName));
            var resultsCompetitions = new ObservableCollection<string>();
            foreach (IGrouping<string, Match> competitionMatches in allResultsGroupList ?? new ObservableCollection<IGrouping<string, Match>>())
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
        }

        private void SetResults()
        {
            var resultsMatches = AllResultsMatches?.Where(x => x.MatchTypeName == ResultsCompetitionSelected)
                .OrderByDescending(x => x.RoundID)
                .ThenByDescending(x => x.Time)
                .ThenBy(x => x.MatchTypeID);
            ResultsGroupList = new ObservableCollection<MatchGroup>(resultsMatches.GroupBy(x => x.RoundName).Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group))));
        }
    }
}

