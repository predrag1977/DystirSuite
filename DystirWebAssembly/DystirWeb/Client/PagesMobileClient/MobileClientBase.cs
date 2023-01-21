using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace DystirWeb.Client.PagesMobileClient
{
    public class MobileClientBase : ComponentBase, IDisposable
    {
        [Inject]
        private DystirWebClientService _dystirWebClientService { get; set; }
        [Inject]
        private HubConnection _hubConnection { get; set; }
        [Inject]
        private LiveStandingService _liveStandingService { get; set; }
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        private SelectedPage _previousPage = SelectedPage.Matches;
        private int _matchid = 0;
        private int _daysFrom = 0;
        private int _daysAfter = 0;
        private string _selectedCompetition;
        private bool _loadStandingsFromServer = true;
        private bool _loadStatisticsFromServer = true;

        public string SelectedResultsCompetition { get; set; }
        public string SelectedFixturesCompetition { get; set; }
        
        public List<string> CompetitionsList { get; set; }
        public Matches SelectedMatch = new Matches();
        public string DaysRange { get; set; }
        public StandingsModelView StandingsView { get; set; }
        public FullStatisticModelView FullStatistics { get; private set; }

        public SelectedPage SelectedPage = SelectedPage.Matches;
        public List<Matches> MatchesListSameDay = new List<Matches>();
        public Standing standing = new Standing();
        public string selectedTab = "0";
        public List<IGrouping<string, Matches>> SelectedMatchListGroup;
        public int TimeZoneOffset = 0;
        public bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            _dystirWebClientService.OnFullDataLoaded += DystirWebClientService_FullDataLoaded;
            _dystirWebClientService.OnConnected += HubConnection_OnConnected;
            _dystirWebClientService.OnDisconnected += HubConnection_OnDisconnected;
            _dystirWebClientService.OnRefreshMatchDetails += DystirWebClientService_OnRefreshMatchDetails;
            TimeZoneOffset = int.Parse(await _jsRuntime.InvokeAsync<String>("getTimeZoneOffset"));
            await _dystirWebClientService.StartUpAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (SelectedPage == SelectedPage.MatchDetails)
            {
                await _jsRuntime.InvokeVoidAsync("setMatchDetailsMobileClientHeight", "");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("setMainContainerMobileClientHeight", "");
            }
        }

        void IDisposable.Dispose()
        {
            _dystirWebClientService.OnFullDataLoaded -= DystirWebClientService_FullDataLoaded;
            _dystirWebClientService.OnConnected -= HubConnection_OnConnected;
            _dystirWebClientService.OnDisconnected -= HubConnection_OnDisconnected;
            _dystirWebClientService.OnRefreshMatchDetails -= DystirWebClientService_OnRefreshMatchDetails;
        }

        private async void DystirWebClientService_FullDataLoaded()
        {
            _loadStandingsFromServer = true;
            _loadStatisticsFromServer = true;
            await LoadData();
        }

        private async void DystirWebClientService_OnRefreshMatchDetails(MatchDetails matchDetails)
        {
            _loadStandingsFromServer = true;
            _loadStatisticsFromServer = true;
            if (SelectedPage == SelectedPage.MatchDetails)
            {
                MatchUpdate(matchDetails);
            }
            else
            {
                await LoadData();
            }
        }

        private async void HubConnection_OnConnected()
        {
            isLoading = false;
            Refresh();
            _loadStandingsFromServer = true;
            _loadStatisticsFromServer = true;
            await LoadData();
        }

        private async void HubConnection_OnDisconnected()
        {
            isLoading = true;
            Refresh();
            await LoadData();
        }

        public async void DaysOnClick(string daysRange)
        {
            isLoading = true;
            SetSelectedDaysRange(daysRange);
            await LoadData();
        }

        public async void CompetitionsOnClick(string competition)
        {
            isLoading = true;
            _selectedCompetition = competition;
            await LoadData();
        }

        public async void BackOnClickAsync()
        {
            SelectedPage = _previousPage;
            Refresh();
            await _jsRuntime.InvokeVoidAsync("setMainContainerMobileClientHeight", "");
        }

        public void Refresh()
        {
            InvokeAsync(() => StateHasChanged());
        }

        public async void ChangePage(SelectedPage selectedPage)
        {
            SelectedPage = selectedPage;
            isLoading = true;
            Refresh();
            await LoadData();
        }

        private async Task LoadData()
        {
            _previousPage = SelectedPage != SelectedPage.MatchDetails ? SelectedPage : _previousPage;
            switch (SelectedPage)
            {
                case SelectedPage.Matches:
                default:
                    await LoadMatches();
                    break;
                case SelectedPage.Results:
                    await LoadResultsMatches();
                    break;
                case SelectedPage.Fixtures:
                    await LoadFixturesMatches();
                    break;
                case SelectedPage.Standings:
                    await LoadStandings();
                    break;
                case SelectedPage.Statistics:
                    await LoadStatistics();
                    break;
                case SelectedPage.MatchDetails:
                    await LoadMatchDetails();
                    break;
            }
            isLoading = false;
            Refresh();
            if(SelectedPage == SelectedPage.MatchDetails)
            {
                await _jsRuntime.InvokeVoidAsync("setMatchDetailsMobileClientHeight", "");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("setMainContainerMobileClientHeight", "");
            }
        }

        private async Task LoadMatches()
        {
            await Task.Run(() =>
            {
                var fromDate = DateTime.Now.Date.AddDays(_daysFrom);
                var toDate = fromDate.AddDays(_daysAfter);
                SelectedMatchListGroup = _dystirWebClientService.AllMatches?.OrderBy(x => GetOrder(x.MatchTypeID)).ThenBy(x => x.Time).ThenBy(x => x.MatchID)
                .Where(x => x.Time.Value.AddMinutes(-TimeZoneOffset).Date >= fromDate
                && x.Time.Value.AddMinutes(-TimeZoneOffset).Date <= toDate).GroupBy(x => x.MatchTypeName)?.ToList();
            });
        }

        private async Task LoadResultsMatches()
        {
            await Task.Run(() =>
            {
                var allResultMatchByCompetition = _dystirWebClientService.AllMatches?
                .Where(x => (x.StatusID == 12 || x.StatusID == 13)
                && x.MatchTypeID != 3 && x.MatchTypeID != 4 && x.MatchTypeID != 100 && x.MatchTypeID != 103)
                .OrderBy(x => GetOrder(x.MatchTypeID))
                .ThenByDescending(x => x.Time).ThenByDescending(x => x.RoundID).ThenBy(x => x.MatchID)
                .GroupBy(x => x.MatchTypeName)?.ToList();

                if (allResultMatchByCompetition == null) return;

                CompetitionsList = new List<string>();
                foreach (var matchGroup in allResultMatchByCompetition)
                {
                    CompetitionsList.Add(matchGroup.Key);
                }

                SelectedResultsCompetition = _selectedCompetition;
                if (string.IsNullOrWhiteSpace(SelectedResultsCompetition))
                {
                    SelectedResultsCompetition = CompetitionsList?.FirstOrDefault() ?? "";
                }

                SelectedMatchListGroup = allResultMatchByCompetition.FirstOrDefault(x => x.Key == SelectedResultsCompetition)?
                .GroupBy(x => x.MatchTypeName)?.ToList();
            });
        }

        private async Task LoadFixturesMatches()
        {
            await Task.Run(() =>
            {
                var allFixturesMatchByCompetition = _dystirWebClientService.AllMatches?.Where(x => x.StatusID < 12 || x.StatusID == 14)
                .OrderBy(x => x.MatchTypeID).OrderBy(x => GetOrder(x.MatchTypeID))
                .ThenBy(x => x.RoundID).ThenBy(x => x.Time).ThenBy(x => x.MatchID)
                .GroupBy(x => x.MatchTypeName)?.ToList() ?? new List<IGrouping<string, Matches>>();

                if (allFixturesMatchByCompetition == null) return;

                CompetitionsList = new List<string>();
                foreach (var matchGroup in allFixturesMatchByCompetition)
                {
                    CompetitionsList.Add(matchGroup.Key);
                }

                SelectedFixturesCompetition = _selectedCompetition;
                if (string.IsNullOrWhiteSpace(SelectedFixturesCompetition))
                {
                    SelectedFixturesCompetition = CompetitionsList?.FirstOrDefault() ?? "";
                }

                SelectedMatchListGroup = allFixturesMatchByCompetition.FirstOrDefault(x => x.Key == SelectedFixturesCompetition)?
                .GroupBy(x => x.MatchTypeName)?.ToList();
            });
        }

        private async Task LoadStandings()
        {
            var standings = _loadStandingsFromServer ? await _dystirWebClientService.GetStandings() : _dystirWebClientService.Standings;
            _loadStandingsFromServer = false;

            List<string> competitionsList = GetStandingsCompetitionsList(standings);
            var selectedStandingsCompetition = competitionsList.Contains(_selectedCompetition) ? _selectedCompetition : competitionsList.FirstOrDefault() ?? "";

            Standing standing = standings?.FirstOrDefault(x => x.StandingCompetitionName == selectedStandingsCompetition);
            StandingsView = new StandingsModelView()
            {
                SelectedCompetition = selectedStandingsCompetition,
                CompetitionsList = GetStandingsCompetitionsList(standings),
                Standing = standing
            };
        }

        public async Task LoadStatistics()
        {
            var statisticCompetitions = _loadStatisticsFromServer ? await _dystirWebClientService.GetCompetitionStatistics() : _dystirWebClientService.CompetitionStatistics;
            _loadStatisticsFromServer = false;

            List<string> competitionsList = GetStatisticsCompetitionsList(statisticCompetitions);
            var selectedStatisticsCompetition = competitionsList.Contains(_selectedCompetition) ? _selectedCompetition : competitionsList.FirstOrDefault() ?? "";

            var competitionStatistic = statisticCompetitions?.FirstOrDefault(x => x.CompetitionName == selectedStatisticsCompetition);

            FullStatistics = new FullStatisticModelView()
            {
                SelectedCompetition = selectedStatisticsCompetition,
                CompetitionsList = GetStatisticsCompetitionsList(statisticCompetitions),
                CompetitionStatistic = competitionStatistic
            };
        }

        private List<string> GetStandingsCompetitionsList(ObservableCollection<Standing> standings)
        {
            List<string> competitionsList = new List<string>();
            foreach (var standing in standings ?? new ObservableCollection<Standing>())
            {
                competitionsList.Add(standing.StandingCompetitionName);
            }
            return competitionsList;
        }

        private List<string> GetStatisticsCompetitionsList(ObservableCollection<CompetitionStatistic> statisticCompetitions)
        {
            List<string> competitionsList = new List<string>();
            foreach (var statistic in statisticCompetitions ?? new ObservableCollection<CompetitionStatistic>())
            {
                competitionsList.Add(statistic.CompetitionName);
            }
            return competitionsList;
        }

        public async Task LoadMatchDetails()
        {
            _matchid = SelectedMatch.MatchID;
            MatchesListSameDay = _dystirWebClientService.GetMatchesListSameDay(SelectedMatch);
            var loadMatchDetailsAsyncTask = _dystirWebClientService.LoadMatchDetailsAsync(_matchid);
            var loadLiveStandingAsyncTask = LoadLiveStandingAsync(SelectedMatch);
            await Task.WhenAll(loadMatchDetailsAsyncTask, loadLiveStandingAsyncTask);
            SelectedMatch.FullMatchDetails = loadMatchDetailsAsyncTask.Result;
        }

        private async void MatchUpdate(MatchDetails matchDetails)
        {
            if (_matchid == matchDetails?.MatchDetailsID)
            {
                SelectedMatch = matchDetails.Match;
                await LoadMatchDetails();
            }
            MatchesListSameDay = _dystirWebClientService.GetMatchesListSameDay(SelectedMatch);
            await LoadLiveStandingAsync(SelectedMatch);
            Refresh();
        }

        private async Task LoadLiveStandingAsync(Matches selectedMatch)
        {
            standing = _liveStandingService.GetStanding(selectedMatch);
            await Task.CompletedTask;
        }

        private void SetSelectedDaysRange(string daysRange)
        {
            DateTime dateToday = DateTime.UtcNow.AddMinutes(-TimeZoneOffset);
            CultureInfo cultureInfo = new CultureInfo("fo-FO");
            int offset = cultureInfo.DateTimeFormat.FirstDayOfWeek - dateToday.DayOfWeek;
            switch (daysRange?.ToLower())
            {
                case "lastweek":
                    _daysFrom = offset - 7;
                    _daysAfter = 6;
                    break;
                case "yesterday":
                    _daysFrom = -1;
                    _daysAfter = 0;
                    break;
                case "today":
                default:
                    _daysFrom = 0;
                    _daysAfter = 0;
                    break;
                case "tomorrow":
                    _daysFrom = 1;
                    _daysAfter = 0;
                    break;
                case "nextweek":
                    _daysFrom = offset;
                    _daysAfter = 6;
                    break;
            }
            this.DaysRange = daysRange;
        }

        private object GetOrder(int? matchTypeId)
        {
            switch (matchTypeId)
            {
                case 101:
                    return 6;
                case 6:
                    return 101;
                default:
                    return matchTypeId;
            }
        }

        public bool ShowLiveStandings(Matches match)
        {
            var competititionNamesArray = _dystirWebClientService.AllCompetitions.Where(x => x.CompetitionID > 0).Select(x => x.MatchTypeName);
            return competititionNamesArray.Any(x => x == match?.MatchTypeName);
        }

        public void OnTabClick(string tabIndex)
        {
            selectedTab = tabIndex;
        }

        public void FooterMenuTabOnClick(SelectedPage selectedPage)
        {
            ChangePage(selectedPage);
        }
    }

    public enum SelectedPage
    {
        Matches,
        Results,
        Fixtures,
        Standings,
        Statistics,
        MatchDetails
    }
}
