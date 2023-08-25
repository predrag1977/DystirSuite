using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dystir.Models;
using System.Linq;
using Dystir.Services;
using Xamarin.Forms;

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
            DystirService.OnFullDataLoaded += DystirService_OnFullDataLoaded;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
            TimeService.OnSponsorsTimerElapsed += TimeService_OnSponsorsTimerElapsed;

            DayTapped = new Command<DayOfMatch>(OnDaySelected);
        }

        //**********************//
        //    PUBLIC METHODS    //
        //**********************//
        public async Task LoadDataAsync()
        {
            await Task.Delay(100);
            if (DystirService.AllMatches != null)
            {
                await SetMatches();
                IsLoading = false;
            }
            await SetSponsors();
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
                .OrderBy(x => GetOrder(x.MatchTypeID))
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

        private int? GetOrder(int? matchTypeId)
        {
            return DystirService.AllCompetitions.FirstOrDefault(x => x.MatchTypeID == matchTypeId)?.OrderID;
        }
    }
}