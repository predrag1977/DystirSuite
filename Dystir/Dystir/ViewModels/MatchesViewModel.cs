using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dystir.Models;
using System.Linq;
using System.Reflection;
using Dystir.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dystir.ViewModels
{
    public class MatchesViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        ObservableCollection<MatchGroup> matchesGroupList = new ObservableCollection<MatchGroup>();
        public ObservableCollection<MatchGroup> MatchesGroupList
        {
            get { return matchesGroupList; }
            set { matchesGroupList = value; OnPropertyChanged(); }
        }

        DayOfMatch matchesDaySelected = new DayOfMatch() { Date = DateTime.Now.AddDays(0).Date };
        public DayOfMatch MatchesDaySelected
        {
            get { return matchesDaySelected; }
            set { matchesDaySelected = value; OnPropertyChanged(); SetMatches(); }
        }

        ObservableCollection<DayOfMatch> matchesDays = new ObservableCollection<DayOfMatch>();
        public ObservableCollection<DayOfMatch> MatchesDays
        {
            get { return matchesDays; }
            set { matchesDays = value; OnPropertyChanged();}
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public MatchesViewModel(DystirService dystirService, TimeService timeService)
        {
            DystirService = dystirService;
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
            
            timeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;
            timeService.StartSponsorsTime();

            IsLoading = true;
            SetMatchesDays();
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async void LoadDataAsync()
        {
            await Task.Delay(100);
            SetMatches();
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
            SetMatches();
            SetSponsors();
            IsLoading = false;
        }

        private void DystirService_OnMatchDetailsLoaded(MatchDetails matchDetails)
        {
            SetMatches();
        }

        private void TimeService_OnSponsorsTimerElapsed()
        {
            SetSponsors();
        }

        private void SetMatches()
        {
            SetMatchesDays();
            var matches = DystirService.AllMatches?.OrderBy(x => x.MatchTypeID).ThenBy(x => x.Time).Where(x => x.Time?.Date == MatchesDaySelected.Date.Date);
            MatchesGroupList = new ObservableCollection<MatchGroup>(matches.GroupBy(x => x.MatchTypeName).Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group))));
        }

        private void SetMatchesDays()
        {
            var matchesDays = new ObservableCollection<DayOfMatch>();
            for (DateTime date = DateTime.Now.AddDays(-3); date <= DateTime.Now.AddDays(3); date = date.AddDays(1))
            {
                var dayOfMatch = new DayOfMatch()
                {
                    Date = date.Date,
                    DateText = date.ToString("dd.MM"),
                    DayText = Resources.Localization.Resources.ResourceManager.GetString(date.DayOfWeek.ToString())?.Substring(0, 3).ToUpper(),
                    TextColor = date.Date == MatchesDaySelected.Date ? Colors.LimeGreen : Colors.White
                };
                matchesDays.Add(dayOfMatch);
            }
            MatchesDays = new ObservableCollection<DayOfMatch>(matchesDays);
        }


        


        //    public async Task<bool> GetFullDataAsync()
        //    {
        //        bool success = true;
        //        try
        //        {
        //            var task1 = GetMatchesAsync();
        //            var task2 = GetStandingsAsync();
        //            var task3 = GetSponsorsAsync();
        //            var task4 = GetCompetitionStatisticsAsync();
        //            await Task.WhenAll(task1, task2, task3, task4);
        //        }
        //        catch (Exception ex)
        //        {
        //            success = false;
        //            Console.WriteLine($"Error: {ex.Message}");
        //        }
        //        IsLoading = false;
        //        return success;
        //    }

        //    public async Task GetMatchesAsync()
        //    {
        //        AllMatches = await GetDataStore().GetMatchesAsync();
        //        SetAllMatchesWithDetails();
        //    }

        //    internal async Task<bool> GetSelectedLiveMatch(Match selectedMatch)
        //    {
        //        bool success;
        //        try
        //        {
        //            await GetMatchDetailsAsync(selectedMatch);
        //            success = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            success = false;
        //            Console.WriteLine($"Error: {ex.Message}");
        //        }
        //        IsLoadingSelectedMatch = false;
        //        return success;
        //    }

        //    private async Task GetMatchDetailsAsync(Match selectedMatch)
        //    {
        //        MatchDetails matchDetails = await GetDataStore().GetMatchDetailsAsync(selectedMatch?.MatchID);
        //        if (matchDetails != null)
        //        {
        //            MatchDetails selectedMatchDetails = AllMatches?.FirstOrDefault(x => x.MatchID == selectedMatch?.MatchID).MatchDetails;
        //            selectedMatchDetails.DetailsMatchTabIndex = 0;
        //            selectedMatchDetails.Match = matchDetails.Match;
        //            selectedMatchDetails.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(matchDetails.PlayersOfMatch);
        //            selectedMatchDetails.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
        //            selectedMatchDetails.IsDataLoaded = true;
        //        }
        //    }

        //    public async Task GetStandingsAsync()
        //    {
        //        AllStandings = await GetDataStore().GetStandingsAsync();
        //    }

        //    public async Task GetCompetitionStatisticsAsync()
        //    {
        //        AllCompetitionStatistics = await GetDataStore().GetStatisticsAsync();
        //    }

        //    public async Task GetSponsorsAsync()
        //    {
        //        var sponsors = await GetDataStore().GetSponsorsAsync();
        //        Sponsors = new ObservableCollection<Sponsor>(sponsors.OrderBy(a => Guid.NewGuid()));
        //        AllMainSponsors = new ObservableCollection<Sponsor>(sponsors.Where(x => x.SponsorID < 100).OrderBy(a => Guid.NewGuid()));
        //        SponsorsMain = new ObservableCollection<Sponsor>(AllMainSponsors?.Take(2));
        //    }

        //    internal async Task<Match> GetMatchByIDAsync(string matchID)
        //    {
        //        return await GetDataStore().GetMatchAsync(matchID);
        //    }

        //    internal void CopyProperties(Match target, Match source)
        //    {
        //        foreach (PropertyInfo property in typeof(Match).GetProperties().Where(p => p.CanWrite))
        //        {
        //            property.SetValue(target, property.GetValue(source, null), null);
        //        }
        //    }

        //    // ALL MATCHES WITH DETAILS
        //    public void SetAllMatchesWithDetails()
        //    {
        //        var allMatchesWithDetails = new ObservableCollection<MatchDetails>();
        //        foreach (Match match in AllMatches)
        //        {
        //            MatchDetails matchDetails = AllMatches.FirstOrDefault(x => x.MatchID == match.MatchID).MatchDetails;
        //            if (matchDetails != null)
        //            {
        //                matchDetails.Match = match;
        //            }
        //            else
        //            {
        //                matchDetails = new MatchDetails()
        //                {
        //                    Match = match,
        //                    PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(),
        //                    EventsOfMatch = new ObservableCollection<EventOfMatch>()
        //                };
        //            }
        //            allMatchesWithDetails.Add(matchDetails);
        //        }
        //        //AllMatchesWithDetails = new ObservableCollection<MatchDetails>(allMatchesWithDetails);
        //    }
        //}

        //public class Statistic
        //{
        //    public TeamStatistic HomeTeamStatistic { get; internal set; } = new TeamStatistic();
        //    public TeamStatistic AwayTeamStatistic { get; internal set; } = new TeamStatistic();
        //}

        //public class TeamStatistic
        //{
        //    public int Goal { get; internal set; } = 0;
        //    public int YellowCard { get; internal set; } = 0;
        //    public int RedCard { get; internal set; } = 0;
        //    public int Corner { get; internal set; } = 0;
        //    public int GoalProcent { get; internal set; } = 100;
        //    public int YellowCardProcent { get; internal set; } = 100;
        //    public int RedCardProcent { get; internal set; } = 100;
        //    public int CornerProcent { get; internal set; } = 100;
        //    public int OnTarget { get; internal set; } = 0;
        //    public int OnTargetProcent { get; internal set; } = 100;
        //    public int OffTarget { get; internal set; } = 0;
        //    public int OffTargetProcent { get; internal set; } = 100;
        //    public int BlockedShot { get; internal set; } = 0;
        //    public int BlockedShotProcent { get; internal set; } = 100;
        //    public int BigChance { get; internal set; } = 0;
        //    public int BigChanceProcent { get; internal set; } = 100;
        //}
    }
}