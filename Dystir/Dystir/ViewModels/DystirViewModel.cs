using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Dystir.Models;
using Dystir.Services;
using System.Reflection;
using Dystir.Views;

namespace Dystir.ViewModels
{
    public class DystirViewModel : INotifyPropertyChanged
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        public DystirService DystirService;
        public LiveStandingService LiveStandingService;

        ObservableCollection<Sponsor> sponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> Sponsors
        {
            get { return sponsors; }
            set { sponsors = value; }
        }

        ObservableCollection<Sponsor> primarySponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> PrimarySponsors
        {
            get { return primarySponsors; }
            set { primarySponsors = value; OnPropertyChanged(); }
        }

        ObservableCollection<Sponsor> secondarySponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> SecondarySponsors
        {
            get { return secondarySponsors; }
            set { secondarySponsors = value; OnPropertyChanged(); }
        }

        bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        string pageTitle = string.Empty;
        public string PageTitle
        {
            get { return pageTitle; }
            set { pageTitle = value; OnPropertyChanged(); }
        }

        //**************************//
        //      PRIVATE METHODS     //
        //**************************//
        public async Task SetSponsors()
        {
            Sponsors = DystirService.AllSponsors;
            var sponsors = new ObservableCollection<Sponsor>(Sponsors.OrderBy(a => Guid.NewGuid()));
            PrimarySponsors = new ObservableCollection<Sponsor>(sponsors.Where(x => x.SponsorID < 100).OrderBy(a => Guid.NewGuid()));
            SecondarySponsors = new ObservableCollection<Sponsor>(PrimarySponsors?.Take(2));
            await Task.CompletedTask;
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }





        

        ObservableCollection<TeamStanding> competitionTeamStandings = new ObservableCollection<TeamStanding>();
        public ObservableCollection<TeamStanding> CompetitionTeamStandings
        {
            get { return competitionTeamStandings; }
            set { competitionTeamStandings = value; }
        }

        ObservableCollection<CompetitionStatistic> allCompetitionStatistics = new ObservableCollection<CompetitionStatistic>();
        public ObservableCollection<CompetitionStatistic> AllCompetitionStatistics
        {
            get { return allCompetitionStatistics; }
            set { allCompetitionStatistics = value; SetCompetitionStatistics(); }
        }

        ObservableCollection<PlayersInLineups> selectedCompetitionStatistics = new ObservableCollection<PlayersInLineups>();
        public ObservableCollection<PlayersInLineups> SelectedCompetitionStatistics
        {
            get { return selectedCompetitionStatistics; }
            set { selectedCompetitionStatistics = value; }
        }

        CultureInfo languageCode = CultureInfo.GetCultureInfo("fo-FO");
        public CultureInfo LanguageCode
        {
            get { return languageCode; }
            set
            {
                 languageCode = value;
                //foreach(Match matches in AllMatches)
                //{
                //    matches.LanguageCode = value;
                //}
                //SetMatchesDays();
                //if (SelectedMatch != null) SelectedMatch.LanguageCode = value;
            }
        }

        




        string standingsCompetitionSelected;
        public string StandingsCompetitionSelected
        {
            get { return standingsCompetitionSelected; }
            set { standingsCompetitionSelected = value; }
        }

        ObservableCollection<string> standingsCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> StandingsCompetitions
        {
            get { return standingsCompetitions; }
            set { standingsCompetitions = value; }
        }

        string statisticsCompetitionSelected;
        public string StatisticsCompetitionSelected
        {
            get { return statisticsCompetitionSelected; }
            set { statisticsCompetitionSelected = value; SetCompetitionStatistics(); }
        }

        ObservableCollection<string> statisticCompetitions = new ObservableCollection<string>();
        public ObservableCollection<string> StatisticCompetitions
        {
            get { return statisticCompetitions; }
            set { statisticCompetitions = value; }
        }

        bool isDisconnected = false;
        public bool IsDisconnected
        {
            get { return isDisconnected; }
            set { isDisconnected = value; }
        }

        bool isConnectionError = false;
        public bool IsConnectionError
        {
            get { return isConnectionError; }
            set { isConnectionError = value; }
        }

        bool isApplicationActive = true;
        public bool IsApplicationActive
        {
            get { return isApplicationActive; }
            set { isApplicationActive = value; }
        }

        Exception mainException = null;
        public Exception MainException
        {
            get { return mainException; }
            set { mainException = value; }
        }



        // COMPETITION STATISTICS
        private void SetCompetitionStatistics()
        {
            var statisticCompetitions = new ObservableCollection<string>();
            foreach (CompetitionStatistic competitionStatistic in AllCompetitionStatistics)
            {
                statisticCompetitions.Add(competitionStatistic?.CompetitionName);
            }
            StatisticCompetitions = new ObservableCollection<string>(statisticCompetitions);
            if (string.IsNullOrEmpty(StatisticsCompetitionSelected) && StatisticCompetitions.Count > 0)
            {
                StatisticsCompetitionSelected = StatisticCompetitions.FirstOrDefault();
            }
            var selectedCompetitionStatistics = AllCompetitionStatistics.FirstOrDefault(x => x.CompetitionName == StatisticsCompetitionSelected);
            var selectedCompetitionStatisticsRowsList = new ObservableCollection<PlayersInLineups>();
            if (selectedCompetitionStatistics != null)
            {
                foreach (PlayerOfMatch goalPlayer in selectedCompetitionStatistics.GoalPlayers)
                {
                    PlayersInLineups statisticRow = new PlayersInLineups();
                    //statisticRow.FirstPlayer = goalPlayer;
                    goalPlayer.NumberOrder = (selectedCompetitionStatistics.GoalPlayers.IndexOf(goalPlayer) + 1).ToString() + ".";
                    selectedCompetitionStatisticsRowsList.Add(statisticRow); 
                }
                foreach (PlayerOfMatch assistPlayer in selectedCompetitionStatistics.AssistPlayers)
                {
                    int index = selectedCompetitionStatistics.AssistPlayers.IndexOf(assistPlayer);
                    assistPlayer.NumberOrder = (index + 1).ToString() + ".";
                    PlayersInLineups statisticRow = new PlayersInLineups();
                    if (selectedCompetitionStatisticsRowsList.Count > index)
                    {
                        statisticRow = selectedCompetitionStatisticsRowsList[index];
                        //statisticRow.SecondPlayer = assistPlayer;
                    }
                    else
                    {
                        //statisticRow.SecondPlayer = assistPlayer;
                        selectedCompetitionStatisticsRowsList.Add(statisticRow);
                    }
                }
            }
            SelectedCompetitionStatistics = selectedCompetitionStatisticsRowsList;
        }
        


        
    }
}
