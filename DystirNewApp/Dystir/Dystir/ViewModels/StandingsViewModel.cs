using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.Services;
using Xamarin.Forms;

namespace Dystir.ViewModels
{
    public class StandingsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        public Command<Competition> CompetitionTapped { get; }
        private ObservableCollection<Standing> AllStandings = new ObservableCollection<Standing>();

        Competition standingsCompetitionSelected;
        public Competition StandingsCompetitionSelected
        {
            get { return standingsCompetitionSelected; }
            set { standingsCompetitionSelected = value; OnPropertyChanged(); _ = SetStandingCompetitions(); }
        }

        private ObservableCollection<Competition> standingCompetitions;
        public ObservableCollection<Competition> StandingCompetitions
        {
            get { return standingCompetitions; }
            set { standingCompetitions = value; OnPropertyChanged(); }
        }

        private Standing standing = new Standing() { IsHeaderVisible = false};
        public Standing Standing
        {
            get { return standing; }
            set { standing = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public StandingsViewModel()
        {
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            TimeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;

            LiveStandingService = DependencyService.Get<LiveStandingService>();

            CompetitionTapped = new Command<Competition>(OnCompetitionSelected);

            _ = SetStandingCompetitions();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(200);
            if (DystirService.AllMatches != null)
            {
                var competitions = DystirService.AllCompetitions.Where(x => x.CompetitionID > 0).OrderBy(x => x.OrderID);
                AllStandings = new ObservableCollection<Standing>();
                foreach (MatchCompetition competition in competitions)
                {
                    var standing = LiveStandingService.GetStanding(competition.MatchTypeName);
                    AllStandings.Add(standing);
                }
                await SetStandingCompetitions();
                IsLoading = false;
            }
            await SetSponsors();
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private void OnCompetitionSelected(Competition competition)
        {
            if (competition == null)
                return;

            StandingsCompetitionSelected = competition;
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
            await SetStandings();
        }

        private async Task SetCompetitionsSelection()
        {
            var standingCompetitions = StandingCompetitions;
            foreach (Competition competition in standingCompetitions)
            {
                competition.TextColor = competition.CompetitionName == StandingsCompetitionSelected.CompetitionName ? Color.LimeGreen : Color.White;
            }
            await Task.CompletedTask;
        }

        private async Task SetStandingCompetitions()
        {
            if(AllStandings.Count == 0)
            {
                return;
            }

            var standingsCompetitions = new ObservableCollection<Competition>();
            foreach (Standing standing in AllStandings)
            {
                Competition competition = new Competition()
                {
                    CompetitionName = standing.StandingCompetitionName,
                    TextColor = Color.FromHex("#A9A9A9")
                };
                standingsCompetitions.Add(competition);
            }

            StandingCompetitions = new ObservableCollection<Competition>(standingsCompetitions);
            if (StandingsCompetitionSelected == null && StandingCompetitions.Count > 0)
            {
                StandingsCompetitionSelected = StandingCompetitions.FirstOrDefault();
            }
            await SetSelectedCompetition();
        }

        private async Task SetStandings()
        {
            Standing = AllStandings.FirstOrDefault(x => x.StandingCompetitionName == StandingsCompetitionSelected?.CompetitionName);
            await Task.CompletedTask;
        }
    }
}