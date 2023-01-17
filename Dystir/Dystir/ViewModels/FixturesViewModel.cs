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
        private ObservableCollection<Match> AllFixturesMatches = new ObservableCollection<Match>();

        ObservableCollection<MatchGroup> fixturesGroupList = new ObservableCollection<MatchGroup>();
        public ObservableCollection<MatchGroup> FixturesGroupList
        {
            get { return fixturesGroupList; }
            set { fixturesGroupList = value; OnPropertyChanged();  }
        }

        Competition fixturesCompetitionSelected;
        public Competition FixturesCompetitionSelected
        {
            get { return fixturesCompetitionSelected; }
            set { fixturesCompetitionSelected = value; OnPropertyChanged(); _ = SetSelectedCompetition(); }
        }

        ObservableCollection<Competition> fixturesCompetitions = new ObservableCollection<Competition>();
        public ObservableCollection<Competition> FixturesCompetitions
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

            _ = SetFixturesCompetitions();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(200);
            await SetFixturesCompetitions();
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
            await SetFixtures();
        }

        private async Task SetCompetitionsSelection()
        {
            var fixturesCompetitions = FixturesCompetitions;
            foreach (Competition competition in fixturesCompetitions)
            {
                competition.TextColor = competition.CompetitionName == FixturesCompetitionSelected.CompetitionName ? Colors.LimeGreen : Colors.White;
            }
            FixturesCompetitions = new ObservableCollection<Competition>(fixturesCompetitions);
            await Task.CompletedTask;
        }

        private async Task SetFixturesCompetitions()
        {
            var fixturesCompetitions = new ObservableCollection<Competition>();
            var fromDate = DateTime.Now.Date.AddDays(0);
            var toDate = DateTime.Now.Date.AddDays(360);
            var fixturesMatches = DystirService.AllMatches?.Where(x => x.Match.Time > fromDate && x.Match.Time < toDate || (x.Match.StatusID < 12 || x.Match.StatusID == 14));
            var orderedFixturesMatches = fixturesMatches.OrderBy(x => x.Match.MatchTypeID).ThenByDescending(x => x.Match.Time).Select(x => x.Match);
            AllFixturesMatches = new ObservableCollection<Match>(orderedFixturesMatches);

            foreach (IGrouping<string, Match> competitionMatches in AllFixturesMatches?.GroupBy(x => x.MatchTypeName))
            {
                if (!string.IsNullOrEmpty(competitionMatches.Key))
                {
                    Competition competition = new Competition()
                    {
                        CompetitionName = competitionMatches.Key,
                        TextColor = Colors.White
                    };
                    fixturesCompetitions.Add(competition);
                }
            }
            FixturesCompetitions = new ObservableCollection<Competition>(fixturesCompetitions);
            if (FixturesCompetitionSelected == null && FixturesCompetitions.Count > 0)
            {
                FixturesCompetitionSelected = FixturesCompetitions.FirstOrDefault();
            }
            await SetSelectedCompetition();
            await Task.CompletedTask;

            }

        private async Task SetFixtures()
        {
            var fixturesMatches = AllFixturesMatches?.Where(x => x.MatchTypeName == FixturesCompetitionSelected.CompetitionName)
                .OrderBy(x => x.RoundID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchTypeID);
            FixturesGroupList = new ObservableCollection<MatchGroup>(fixturesMatches.GroupBy(x => x.RoundName).Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group))));
            await Task.CompletedTask;
        }
    }
}