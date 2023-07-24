using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dystir.Models;
using System.Linq;
using System.Reflection;
using Dystir.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Dystir.Views;
using Dystir.Pages;

namespace Dystir.ViewModels
{
    public class MatchesViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        public Command<DayOfMatch> DayTapped { get; }
        ObservableCollection<MatchGroup> matchesGroupList = new ObservableCollection<MatchGroup>();
        public ObservableCollection<MatchGroup> MatchesGroupList
        {
            get { return matchesGroupList; }
            set { matchesGroupList = value; OnPropertyChanged(); }
        }

        DayOfMatch matchesDaySelected = new DayOfMatch()
        {
            Date = DateTime.Now.AddDays(0).Date
        };
        public DayOfMatch MatchesDaySelected
        {
            get { return matchesDaySelected; }
            set { matchesDaySelected = value; OnPropertyChanged(); _ = SetMatches(); }
        }

        ObservableCollection<DayOfMatch> matchesDays = new ObservableCollection<DayOfMatch>();
        public ObservableCollection<DayOfMatch> MatchesDays
        {
            get { return matchesDays; }
            set { matchesDays = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public MatchesViewModel()
        {
            DystirService = DependencyService.Get<DystirService>();
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;

            var timeService = DependencyService.Get<TimeService>();
            timeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;

            DayTapped = new Command<DayOfMatch>(OnDaySelected);

            IsLoading = true;
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(100);
            await SetMatches();
            await SetSponsors();
            IsLoading = false;
        }

        //**********************//
        //   PRIVATE METHODS    //
        //**********************//
        private async void OnDaySelected(DayOfMatch dayOfMatch)
        {
            if (dayOfMatch != null)
            {
                MatchesDaySelected = dayOfMatch;
            }
            await Task.CompletedTask;
        }

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

        private async Task SetMatches()
        {
            await SetMatchesDays();
            var matches = DystirService.AllMatches?
                .Where(x => IsDaysRange(x.Match.Time?.Date, MatchesDaySelected.Date))
                .Select(x => x.Match)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)
                .ThenBy(x => x.MatchID);
            var matchGroups = matches.GroupBy(x => x.MatchTypeName).Select(group => new MatchGroup(group.Key, new ObservableCollection<Match>(group)));
            MatchesGroupList = new ObservableCollection<MatchGroup>(matchGroups);
            await Task.CompletedTask;
        }

        private async Task SetMatchesDays()
        {
            var matchesDays = new ObservableCollection<DayOfMatch>();
            int[] dayRangeArray = new int[] { -10, -1, 0, 1, 10 };
            foreach (int day in dayRangeArray)
            {
                var date = DateTime.Now.AddDays(day).ToLocalTime().Date;
                var dayOfMatch = new DayOfMatch()
                {
                    Date = DateTime.Now.AddDays(day).ToLocalTime().Date,
                    DateText = Math.Abs(day) > 1 ? string.Format("{0} {1}", day > 0 ? "+" : "-", Math.Abs(day)) : date.ToString("dd.MM"),
                    DayText = Math.Abs(day) > 1 ? Resources.Localization.Resources.Days : Resources.Localization.Resources.ResourceManager.GetString(date.DayOfWeek.ToString())?.Substring(0, 3).ToUpper(),
                    TextColor = date.Date == MatchesDaySelected.Date ? Color.LimeGreen : Color.White
                };
                matchesDays.Add(dayOfMatch);
            }
            MatchesDays = new ObservableCollection<DayOfMatch>(matchesDays);
            await Task.CompletedTask;
        }

        private bool IsDaysRange(DateTime? matchDate, DateTime selectedDate)
        {
            DateTime dateNow = DateTime.Now.ToLocalTime().Date;
            if (dateNow == selectedDate)
            {
                return dateNow == matchDate;
            }
            else if (Math.Abs((dateNow - selectedDate).TotalDays) == 1)
            {
                return matchDate == selectedDate;
            }
            else
            {
                return dateNow < selectedDate ? matchDate >= dateNow && matchDate <= selectedDate : matchDate <= dateNow && matchDate >= selectedDate;
            }
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