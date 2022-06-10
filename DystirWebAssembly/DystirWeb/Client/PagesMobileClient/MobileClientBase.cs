using System;
using System.Collections.Generic;
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
        private TimeService _timeService { get; set; }
        [Inject]
        private HubConnection _hubConnection { get; set; }
        [Inject]
        private LiveStandingService _liveStandingService { get; set; }
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        private SelectedPage _previousPage = SelectedPage.Matches;
        private int _daysFrom = 0;
        private int _daysAfter = 0;

        public string SelectedCompetition { get; set; }
        public List<string> CompetitionsList { get; set; }
        public Matches SelectedMatch = new Matches();
        public string MatchID { get; set; }
        public string daysRange { get; set; }
        public SelectedPage selectedPage = SelectedPage.Matches;
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
            _timeService.OnTimerElapsed += TimerElapsed;
            TimeZoneOffset = int.Parse(await _jsRuntime.InvokeAsync<String>("getTimeZoneOffset"));
            await _dystirWebClientService.StartUpAsync();
        }

        void IDisposable.Dispose()
        {
            _dystirWebClientService.OnFullDataLoaded -= DystirWebClientService_FullDataLoaded;
            _dystirWebClientService.OnConnected -= HubConnection_OnConnected;
            _dystirWebClientService.OnDisconnected -= HubConnection_OnDisconnected;
            _dystirWebClientService.OnRefreshMatchDetails -= DystirWebClientService_OnRefreshMatchDetails;
            _timeService.OnTimerElapsed -= TimerElapsed;
        }

        private async void DystirWebClientService_FullDataLoaded()
        {
            await LoadData(selectedPage);
        }

        private async void DystirWebClientService_OnRefreshMatchDetails(MatchDetails matchDetails)
        {
            if (selectedPage == SelectedPage.MatchDetails)
            {
                MatchUpdate(matchDetails);
            }
            else
            {
                await LoadData(selectedPage);
            }
        }

        private async void HubConnection_OnConnected()
        {
            isLoading = false;
            Refresh();
            await LoadData(selectedPage);
        }

        private async void HubConnection_OnDisconnected()
        {
            isLoading = true;
            Refresh();
            await LoadData(selectedPage);
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                InvokeAsync(() => StateHasChanged());
            }
        }

        public async void DaysOnClick(string daysRange)
        {
            isLoading = true;
            SetSelectedDaysRange(daysRange);
            await LoadData(selectedPage);
        }

        public async void CompetitionsOnClick(string competition)
        {
            isLoading = true;
            SelectedCompetition = competition;
            await LoadResultsMatches();
        }

        public async void BackOnClickAsync()
        {
            selectedPage = _previousPage;
            Refresh();
            await Task.CompletedTask;
        }

        private void Refresh()
        {
            InvokeAsync(() => StateHasChanged());
        }

        public async void ChangePage(SelectedPage selectedPage)
        {
            isLoading = true;
            Refresh();
            await LoadData(selectedPage);
        }

        private async Task LoadData(SelectedPage selectedPage)
        {
            _previousPage = selectedPage != SelectedPage.MatchDetails ? selectedPage : _previousPage;
            this.selectedPage = selectedPage;
            switch (selectedPage)
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
                    break;
                case SelectedPage.Statistics:
                    break;
                case SelectedPage.MatchDetails:
                    await LoadMatchDetails(SelectedMatch);
                    break;
            }
            _ = _jsRuntime.InvokeVoidAsync("onPageResize", "");
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
            isLoading = false;
            Refresh();
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

                if (string.IsNullOrWhiteSpace(SelectedCompetition))
                {
                    SelectedCompetition = CompetitionsList?.FirstOrDefault() ?? "";
                }

                SelectedMatchListGroup = allResultMatchByCompetition.FirstOrDefault(x => x.Key == SelectedCompetition)?
                .GroupBy(x => x.MatchTypeName)?.ToList();
            });
            isLoading = false;
            Refresh();
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

                if (string.IsNullOrWhiteSpace(SelectedCompetition))
                {
                    SelectedCompetition = CompetitionsList?.FirstOrDefault() ?? "";
                }

                SelectedMatchListGroup = allFixturesMatchByCompetition.FirstOrDefault(x => x.Key == SelectedCompetition)?
                .GroupBy(x => x.MatchTypeName)?.ToList();
            });
            isLoading = false;
            Refresh();
        }

        public async Task LoadMatchDetails(Matches match)
        {
            int parseMatchID = int.TryParse(MatchID, out int m) ? int.Parse(MatchID) : 0;
            SelectedMatch = _dystirWebClientService.AllMatches?.FirstOrDefault(x => x.MatchID == parseMatchID);
            MatchesListSameDay = _dystirWebClientService.GetMatchesListSameDay(SelectedMatch);
            Refresh();
            var loadMatchDetailsAsyncTask = _dystirWebClientService.LoadMatchDetailsAsync(parseMatchID);
            var loadLiveStandingAsyncTask = LoadLiveStandingAsync(SelectedMatch);
            await Task.WhenAll(loadMatchDetailsAsyncTask, loadLiveStandingAsyncTask);
            match.FullMatchDetails = loadMatchDetailsAsyncTask.Result;
            isLoading = false;
            Refresh();
        }

        private async void MatchUpdate(MatchDetails matchDetails)
        {
            string matchIDForUpdate = matchDetails?.MatchDetailsID.ToString();
            if (MatchID == matchIDForUpdate)
            {
                await LoadMatchDetails(matchDetails.Match);
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
            this.daysRange = daysRange;
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
            var competititionNamesArray = new string[] { "Betri deildin", "1. deild", "Betri deildin kvinnur", "2. deild" };
            return competititionNamesArray.Any(x => x == match?.MatchTypeName);
        }

        public void OnTabClick(string tabIndex)
        {
            selectedTab = tabIndex;
        }

        public void FooterMenuTabOnClick (SelectedPage selectedPage)
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
