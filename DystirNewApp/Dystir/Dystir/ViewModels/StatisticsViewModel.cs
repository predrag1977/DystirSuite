using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.Services;
using Xamarin.Forms;

namespace Dystir.ViewModels
{
    public class StatisticsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        public Command<Competition> CompetitionTapped { get; }

        ObservableCollection<CompetitionStatistic> allCompetitionStatistics;
        public ObservableCollection<CompetitionStatistic> AllCompetitionStatistics
        {
            get { return allCompetitionStatistics; }
            set { allCompetitionStatistics = value; }
        }

        Competition statisticCompetitionSelected;
        public Competition StatisticCompetitionSelected
        {
            get { return statisticCompetitionSelected; }
            set { statisticCompetitionSelected = value; OnPropertyChanged(); _ = SetStatisticCompetitions(); }
        }

        private ObservableCollection<Competition> statisticCompetitions;
        public ObservableCollection<Competition> StatisticCompetitions
        {
            get { return statisticCompetitions; }
            set { statisticCompetitions = value; OnPropertyChanged(); }
        }

        private CompetitionStatistic competitionStatistic = new CompetitionStatistic();
        public CompetitionStatistic CompetitionStatistic
        {
            get { return competitionStatistic; }
            set { competitionStatistic = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PlayersInLineups> statisticPlayers;
        public ObservableCollection<PlayersInLineups> StatisticPlayers
        {
            get { return statisticPlayers; }
            set { statisticPlayers = value; OnPropertyChanged(); }
        }

        private bool isStatisticHeaderVisible;
        public bool IsStatisticHeaderVisible
        {
            get { return isStatisticHeaderVisible; }
            set { isStatisticHeaderVisible = value; OnPropertyChanged(); }
        }


        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public StatisticsViewModel()
        {
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            TimeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;

            LiveStandingService = DependencyService.Get<LiveStandingService>();

            CompetitionTapped = new Command<Competition>(OnCompetitionSelected);

            _ = SetStatisticCompetitions();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(200);
            AllCompetitionStatistics = await DystirService.GetCompetitionStatistics();
            IsStatisticHeaderVisible = AllCompetitionStatistics != null;
            await SetStatisticCompetitions();
            IsLoading = false;
            await SetSponsors();
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private void OnCompetitionSelected(Competition competition)
        {
            if (competition == null)
                return;

            StatisticCompetitionSelected = competition;
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
            await SetStatistics();
        }

        private async Task SetCompetitionsSelection()
        {
            foreach (Competition competition in StatisticCompetitions)
            {
                competition.TextColor = competition.CompetitionName == StatisticCompetitionSelected.CompetitionName ? Color.LimeGreen : Color.White;
            }
            await Task.CompletedTask;
        }

        private async Task SetStatisticCompetitions()
        {
            if((AllCompetitionStatistics?.Count ?? 0) == 0)
            {
                return;
            }

            var statisticCompetitions = new ObservableCollection<Competition>();
            foreach (CompetitionStatistic standing in AllCompetitionStatistics)
            {
                var competition = new Competition()
                {
                    CompetitionName = standing.CompetitionName,
                    TextColor = Color.FromHex("#A9A9A9")
                };
                statisticCompetitions.Add(competition);
            }

            StatisticCompetitions = new ObservableCollection<Competition>(statisticCompetitions);
            if (StatisticCompetitionSelected == null && StatisticCompetitions.Count > 0)
            {
                StatisticCompetitionSelected = StatisticCompetitions.FirstOrDefault();
            }
            await SetSelectedCompetition();
        }

        private async Task SetStatistics()
        {
            CompetitionStatistic = AllCompetitionStatistics.FirstOrDefault(x => x.CompetitionName == StatisticCompetitionSelected?.CompetitionName);
            await LoadStatisticPlayersAsync();
        }

        private async Task LoadStatisticPlayersAsync()
        {
            await Task.Run(() =>
            {
                var goalPlayers = CompetitionStatistic?.GoalPlayers
                    .OrderByDescending(x => x.Goal)
                    .ThenBy(x => x.TeamName).ToList() ?? new List<PlayerOfMatch>();
                var asistPlayers = CompetitionStatistic?.AssistPlayers
                    .OrderByDescending(x => x.Assist)
                    .ThenBy(x => x.TeamName).ToList() ?? new List<PlayerOfMatch>(); ;

                var statisticPlayers = new ObservableCollection<PlayersInLineups>();

                var biggerPlayerList = goalPlayers.Count >= asistPlayers.Count ? goalPlayers : asistPlayers;
                for (int i = 0; i < biggerPlayerList.Count; i++)
                {
                    var statisticPlayer = new PlayersInLineups()
                    {
                        HomePlayer = goalPlayers.Count() > i ? goalPlayers[i] : new PlayerOfMatch(),
                        AwayPlayer = asistPlayers.Count > i ? asistPlayers[i] : new PlayerOfMatch()
                    };
                    if (!string.IsNullOrEmpty(statisticPlayer.HomePlayer.FirstName))
                    {
                        statisticPlayer.HomePlayer.NumberOrder = $"{i + 1}.";
                    }
                    if (!string.IsNullOrEmpty(statisticPlayer.AwayPlayer.FirstName))
                    {
                        statisticPlayer.AwayPlayer.NumberOrder = $"{i + 1}.";
                    }
                    statisticPlayers.Add(statisticPlayer);
                }

                StatisticPlayers = new ObservableCollection<PlayersInLineups>(statisticPlayers);
            });
        }
    }
}