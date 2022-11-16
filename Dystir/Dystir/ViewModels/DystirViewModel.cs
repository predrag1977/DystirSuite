﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Dystir.Models;
using Dystir.Services;
using System.Reflection;

namespace Dystir.ViewModels
{
    public class DystirViewModel : INotifyPropertyChanged
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        public DystirService DystirService;

        Match selectedMatch;
        public Match SelectedMatch
        {
            get { return selectedMatch; }
            set { selectedMatch = value; OnPropertyChanged(); }
        }

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
        public void SetSponsors()
        {
            Sponsors = DystirService.AllSponsors;
            var sponsors = new ObservableCollection<Sponsor>(Sponsors.OrderBy(a => Guid.NewGuid()));
            PrimarySponsors = new ObservableCollection<Sponsor>(sponsors.Where(x => x.SponsorID < 100).OrderBy(a => Guid.NewGuid()));
            SecondarySponsors = new ObservableCollection<Sponsor>(PrimarySponsors?.Take(2));
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }







        ObservableCollection<Standing> allStandings = new ObservableCollection<Standing>();
        public ObservableCollection<Standing> AllStandings
        {
            get { return allStandings; }
            set { allStandings = value; SetStandings(); }
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

        ObservableCollection<PlayersInRow> selectedCompetitionStatistics = new ObservableCollection<PlayersInRow>();
        public ObservableCollection<PlayersInRow> SelectedCompetitionStatistics
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
            set { standingsCompetitionSelected = value; SetStandings(); }
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


        // COMPETITION STANDINGS
        private void SetStandings()
        {
            var standingsCompetitions = new ObservableCollection<string>();
            foreach (Standing standing in AllStandings)
            {
                standingsCompetitions.Add(standing?.StandingCompetitionName);
            }
            StandingsCompetitions = new ObservableCollection<string>(standingsCompetitions);
            if (string.IsNullOrEmpty(StandingsCompetitionSelected) && StandingsCompetitions.Count > 0)
            {
                StandingsCompetitionSelected = StandingsCompetitions.FirstOrDefault();
            }
            var teamStandings = AllStandings.FirstOrDefault(x => x.StandingCompetitionName == StandingsCompetitionSelected)?.TeamStandings ?? new ObservableCollection<TeamStanding>();
            CompetitionTeamStandings = new ObservableCollection<TeamStanding>(teamStandings);
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
            var selectedCompetitionStatisticsRowsList = new ObservableCollection<PlayersInRow>();
            if (selectedCompetitionStatistics != null)
            {
                foreach (PlayerOfMatch goalPlayer in selectedCompetitionStatistics.GoalPlayers)
                {
                    PlayersInRow statisticRow = new PlayersInRow();
                    statisticRow.FirstPlayer = goalPlayer;
                    goalPlayer.NumberOrder = (selectedCompetitionStatistics.GoalPlayers.IndexOf(goalPlayer) + 1).ToString() + ".";
                    selectedCompetitionStatisticsRowsList.Add(statisticRow); 
                }
                foreach (PlayerOfMatch assistPlayer in selectedCompetitionStatistics.AssistPlayers)
                {
                    int index = selectedCompetitionStatistics.AssistPlayers.IndexOf(assistPlayer);
                    assistPlayer.NumberOrder = (index + 1).ToString() + ".";
                    PlayersInRow statisticRow = new PlayersInRow();
                    if (selectedCompetitionStatisticsRowsList.Count > index)
                    {
                        statisticRow = selectedCompetitionStatisticsRowsList[index];
                        statisticRow.SecondPlayer = assistPlayer;
                    }
                    else
                    {
                        statisticRow.SecondPlayer = assistPlayer;
                        selectedCompetitionStatisticsRowsList.Add(statisticRow);
                    }
                }
            }
            SelectedCompetitionStatistics = selectedCompetitionStatisticsRowsList;
        }
        


        
    }
}
