using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Dystir.Models;
using Dystir.Services;

namespace Dystir.ViewModels
{

    public class StandingsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        private ObservableCollection<Standing> AllStandings;

        private Competition selectedCompetition;
        public Competition SelectedCompetition
        {
            get { return selectedCompetition; }
            set {
                selectedCompetition = value;
                OnPropertyChanged();
                SetStandingCompetitions();
            }
        }

        private Standing standing;
        public Standing Standing
        {
            get { return standing; }
            set { standing = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Competition> standingCompetitions;
        public ObservableCollection<Competition> StandingCompetitions
        {
            get { return standingCompetitions; }
            set { standingCompetitions = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public StandingsViewModel(DystirService dystirService, LiveStandingService liveStandingService)
        {
            DystirService = dystirService;
            LiveStandingService = liveStandingService;
            IsLoading = true;
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(100);
            
            AllStandings = new ObservableCollection<Standing>();
            var competitions = DystirService.AllCompetitions.Where(x => x.CompetitionID == 1).OrderBy(x => x.OrderID);
            foreach (MatchCompetition competition in competitions)
            {
                var standing = LiveStandingService.GetStanding(competition.MatchTypeName);
                AllStandings.Add(standing);
            }

            if (SelectedCompetition == null)
            {
                SelectedCompetition = new Competition()
                {
                    CompetitionName = AllStandings.FirstOrDefault().StandingCompetitionName,
                    TextColor = Colors.LimeGreen
                };
            }
            IsLoading = false;
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private async void SetStandingCompetitions()
        {
            if(AllStandings.Count == 0)
            {
                return;
            }

            await Task.Delay(200);
            IsLoading = true;
            

            var competitions = new List<Competition>();
            foreach (Standing standing in AllStandings)
            {
                competitions.Add(new Competition()
                {
                    CompetitionName = standing.StandingCompetitionName,
                    TextColor = standing.StandingCompetitionName == SelectedCompetition.CompetitionName ? Colors.LimeGreen : Colors.White
                });
            }

            StandingCompetitions = new ObservableCollection<Competition>(competitions);
            Standing = AllStandings.FirstOrDefault(x => x.StandingCompetitionName == SelectedCompetition.CompetitionName);

            IsLoading = false;
        }
    }
}