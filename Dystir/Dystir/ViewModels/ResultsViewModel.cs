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

            SetResults();
            SetSponsors();
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
            SetResults();
            SetSponsors();
            IsLoading = false;
        }

        private void DystirService_OnMatchDetailsLoaded(Match match)
        {
            SetResults();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            SetSponsors();
        }

        private void SetResults()
        {
            var toDate = DateTime.Now.Date.AddDays(0);
            var fromDate = new DateTime(toDate.Year - 1, 12, 31);
            var allResultsMatches = DystirService.AllMatches?.OrderBy(x => x.MatchTypeID).ThenByDescending(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            || (x.StatusID == 13 || x.StatusID == 12)) ?? new ObservableCollection<Match>();
            var allResultsGroupList = new ObservableCollection<IGrouping<string, Match>>(allResultsMatches?.GroupBy(x => x.MatchTypeName));

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

            var resultsMatches = allResultsMatches?.Where(x => x.MatchTypeName == ResultsCompetitionSelected).OrderByDescending(x => x.RoundID).ThenByDescending(x => x.Time).ThenBy(x => x.MatchTypeID);
            ResultsGroupList = new ObservableCollection<MatchGroup>(resultsMatches.GroupBy(x => x.RoundName).Select(group => new MatchGroup(group.Key, group.ToList())));

        }
    }
}

