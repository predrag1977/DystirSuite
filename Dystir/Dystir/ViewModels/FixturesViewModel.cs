using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dystir.Services;
using Dystir.Models;

namespace Dystir.ViewModels
{
	public class FixturesViewModel : DystirViewModel
	{
        //**********************//
        //      PROPERTIES      //
        //**********************//
        ObservableCollection<MatchGroup> fixturesGroupList = new ObservableCollection<MatchGroup>();
        public ObservableCollection<MatchGroup> FixturesGroupList
        {
            get { return fixturesGroupList; }
            set { fixturesGroupList = value; OnPropertyChanged();  }
        }

        string fixturesCompetitionSelected;
        public string FixturesCompetitionSelected
        {
            get { return fixturesCompetitionSelected; }
            set { fixturesCompetitionSelected = value; OnPropertyChanged(); SetFixtures(); }
        }

        ObservableCollection<string> fixturesCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> FixturesCompetitions
        {
            get { return fixturesCompetitions; }
            set { fixturesCompetitions = value; OnPropertyChanged();  }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public FixturesViewModel(DystirService dystirService, TimeService timeService)
        {
            DystirService = dystirService;
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            timeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;
            timeService.StartSponsorsTime();

            SetFixtures();
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
            SetFixtures();
            SetSponsors();
            IsLoading = false;
        }

        private void DystirService_OnMatchDetailsLoaded(Match match)
        {
            SetFixtures();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            SetSponsors();
        }

        private void SetFixtures()
        {
            var fixturesCompetitions = new ObservableCollection<string>();
            var fromDate = DateTime.Now.Date.AddDays(0);
            var toDate = DateTime.Now.Date.AddDays(360);
            var fixturesMatches = DystirService.AllMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).Where(x => x.Time > fromDate && x.Time < toDate
            && (x.StatusID < 12) || x.StatusID == 14) ?? new ObservableCollection<Match>();
            foreach (IGrouping<string, Match> competitionMatches in fixturesMatches?.GroupBy(x => x.MatchTypeName))
            {
                if (!string.IsNullOrEmpty(competitionMatches.Key))
                {
                    fixturesCompetitions.Add(competitionMatches.Key);
                }
            }
            FixturesCompetitions = new ObservableCollection<string>(fixturesCompetitions);
            if (string.IsNullOrEmpty(FixturesCompetitionSelected) && FixturesCompetitions.Count > 0)
            {
                FixturesCompetitionSelected = FixturesCompetitions.FirstOrDefault();
            }
            var resultsGroupList = new ObservableCollection<IGrouping<string, Match>>(fixturesMatches?.GroupBy(x => x.MatchTypeName));
            FixturesGroupList = new ObservableCollection<MatchGroup>(resultsGroupList.Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group))));
        }
    }
}