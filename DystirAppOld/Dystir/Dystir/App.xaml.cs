using System;
using Xamarin.Forms;
using Dystir.Services;
using Dystir.Views;
using System.Globalization;
using System.Threading;
using Dystir.ViewModels;
using Dystir.Models;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Device = Xamarin.Forms.Device;
using Newtonsoft.Json;
using Plugin.LatestVersion;
using Dystir.Services.DystirHubService;
using Dystir.Pages;

namespace Dystir
{
    public partial class App : Application
    {
        private DystirViewModel dystirViewModel;
        private bool _isBusy;
        private DystirService _dystirService;

        public App()
        {
            InitializeComponent();
            ServiceRegistrations();
            
            dystirViewModel = DependencyService.Get<DystirViewModel>();

            AppAnalytics();
            SetLanguage();

            MainPage = new NavigationPage(new DystirTabbedPage())
            {
                BindingContext = dystirViewModel,
                BarTextColor = Color.White
            };

            LoadData();
            //StartLoading();
            //TimeOfMatches();
            //StartSponsors();
            //CheckLatestVersion();
        }

        private void LoadData()
        {
            _dystirService = DependencyService.Get<DystirService>();
            _ = _dystirService.StartUpAsync();
        }

        private void ServiceRegistrations()
        {
            DependencyService.Register<DystirViewModel>();
            DependencyService.Register<DataLoader>();
            DependencyService.Register<TimeService>();
            DependencyService.Register<DystirService>();
        }

        private async void CheckLatestVersion()
        {
            var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();

            if (!isLatest)
            {
                bool result = await MainPage.DisplayAlert("New Version", "There is a new version of this app.\nPlease, update.", "OK", "Cancel");
                if(result)
                {
                    await CrossLatestVersion.Current.OpenAppInStore();
                }
            }
        }

        private async void AppAnalytics()
        {
            await Task.Run(() =>
            {
                AppCenter.Start("ios=3e8bf986-c3f6-49be-8d24-e75c9d9b0a69;" +
                  "uwp={Your UWP App secret here};" +
                  "android=f8d197dc-a359-4e1c-8afe-f9487281f374;",
                  typeof(Analytics), typeof(Crashes));
            });
        }

        private async void StartLoading()
        {
            Connectivity.ConnectivityChanged += Connectivity_Changed;
            dystirViewModel.IsLoading = true;
            await ReloadAsync(LoadDataType.MainData);
        }

        public async Task ReloadAsync(LoadDataType loadDataType)
        {
            await Task.Delay(300);
            if (_isBusy && loadDataType == LoadDataType.FullData)
            {
                return;
            }
            _isBusy = true;
            dystirViewModel.IsDisconnected = true;
            bool connected = false;

            var refreshDataTask = RefreshData(loadDataType);
            //var tryHubConnectTask = TryHubConnectAsync();
            bool[] result = await Task.WhenAll(refreshDataTask/*, tryHubConnectTask*/);
            connected = result.All(x => x == true);

            dystirViewModel.IsDisconnected = !connected;
            dystirViewModel.IsConnectionError = !connected && dystirViewModel.IsApplicationActive;
            _isBusy = false;
            if (!connected)
            {
                await ReloadAsync(LoadDataType.FullData);
            }
        }

        private async Task<bool> RefreshData(LoadDataType loadDataType)
        {
            bool success = false;
            switch (loadDataType)
            {
                case LoadDataType.FullData:
                    //_viewModel.AllMatchesWithDetails.ToList().ForEach(x => x.IsDataLoaded = false);
                    var refrRefreshSelectedMatchDataTask = RefreshSelectedMatchData();
                    var refreshFullDataTask = RefreshFullData();
                    bool[] result = await Task.WhenAll(refrRefreshSelectedMatchDataTask, refreshFullDataTask);
                    success = result.All(x => x == true);
                    break;
                case LoadDataType.MainData:
                    success = await RefreshFullData();
                    break;
                case LoadDataType.MatchDataOnly:
                    success = await RefreshSelectedMatchData();
                    break;
            }
            return success;
        }

        public async Task<bool> RefreshFullData()
        {
            bool connected = await dystirViewModel.GetFullDataAsync();
            dystirViewModel.IsLoading = false;
            return connected;
        }

        public async Task<bool> RefreshSelectedMatchData()
        {
            bool success = true;
            if (dystirViewModel?.SelectedMatch?.MatchID > 0)
            {
                success = await dystirViewModel.GetSelectedLiveMatch(dystirViewModel.SelectedMatch);
                dystirViewModel.IsLoadingSelectedMatch = false;
            }
            return success;
        }

        private async Task Connection_Closed(Exception arg)
        {
            await ReloadAsync(LoadDataType.FullData);
        }

        

        //private void UpdateMatch(string matchID, MatchDetails matchDetails)
        //{
        //    try
        //    {
        //        Match match = matchDetails?.Match;
        //        if (match != null)
        //        {
        //            var allmatches = new ObservableCollection<Match>(_viewModel.AllMatches);
        //            var allMatchesWithDetails = new ObservableCollection<MatchDetails>(_viewModel.AllMatchesWithDetails);
        //            Match findMatch = allmatches?.FirstOrDefault(x => x.MatchID == match?.MatchID);
        //            if (findMatch != null)
        //            {
        //                // UPDATE MATCH
        //                allmatches.Remove(findMatch);
        //                allmatches.Add(match);
        //                _viewModel.AllMatches = new ObservableCollection<Match>(allmatches);

        //                // REMOVE MATCH
        //                if (match.StatusID > 14)
        //                {
        //                    allmatches?.Remove(findMatch);
        //                    _viewModel.AllMatches = new ObservableCollection<Match>(allmatches);
        //                }

        //                MatchDetails findMatchDetails = _viewModel.AllMatchesWithDetails.FirstOrDefault(x => x?.Match?.MatchID == findMatch?.MatchID);
        //                if (findMatchDetails != null)
        //                {
        //                    // UPDATE MATCH DETAILS
        //                    findMatchDetails.Match = match;
        //                    findMatchDetails.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(matchDetails.PlayersOfMatch);
        //                    findMatchDetails.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
        //                    findMatchDetails.IsDataLoaded = true;

        //                    // REMOVE MATCH DETAILS
        //                    if (match.StatusID > 14)
        //                    {
        //                        allMatchesWithDetails.Remove(findMatchDetails);
        //                        _viewModel.AllMatchesWithDetails = new ObservableCollection<MatchDetails>(allMatchesWithDetails);
        //                        _viewModel.SetMatchesBySelectedDate();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // ADD MATCH
        //                allmatches?.Add(match);
        //                _viewModel.AllMatches = new ObservableCollection<Match>(allmatches);

        //                // ADD MATCH DETAILS
        //                allMatchesWithDetails.Add(matchDetails);
        //                _viewModel.AllMatchesWithDetails = new ObservableCollection<MatchDetails>(allMatchesWithDetails);
        //                _viewModel.SetMatchesBySelectedDate();
        //            }

        //            if (match.StatusID!= findMatch.StatusID || match.HomeTeamScore != findMatch.HomeTeamScore || match.AwayTeamScore != findMatch.AwayTeamScore)
        //            {
        //                _ = _viewModel.GetStandingsAsync();
        //                _ = _viewModel.GetCompetitionStatisticsAsync();
        //            }
        //            _viewModel.TimeCounter.MatchesTime(_viewModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = ReloadAsync(LoadDataType.FullData);
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //}

        internal void SetLanguage()
        {
            string languageCode = Current.Properties.FirstOrDefault(x => x.Key == "languageCode").Value?.ToString();
            if (string.IsNullOrWhiteSpace(languageCode)) 
            {
                languageCode = "fo-FO";
            }
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
        }

        private void StartSponsors()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(10000), () =>
            {
                if (dystirViewModel.Sponsors != null && dystirViewModel.Sponsors.Count > 0)
                {
                    var tempList = new ObservableCollection<Sponsor>(dystirViewModel.Sponsors);
                    var item = tempList.FirstOrDefault();
                    tempList.Remove(item);
                    tempList.Add(item);
                    dystirViewModel.Sponsors = new ObservableCollection<Sponsor>(tempList);
                }
                if (dystirViewModel.AllMainSponsors != null && dystirViewModel.AllMainSponsors.Count > 0)
                {
                    var tempList = new ObservableCollection<Sponsor>(dystirViewModel.AllMainSponsors);
                    var item = tempList.FirstOrDefault();
                    tempList.Remove(item);
                    tempList.Add(item);
                    dystirViewModel.AllMainSponsors = new ObservableCollection<Sponsor>(tempList);
                    dystirViewModel.SponsorsMain = new ObservableCollection<Sponsor>(tempList.Take(2));
                }
                return true;
            });
        }

        private async void Connectivity_Changed(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;
            if (access == NetworkAccess.Internet)
            {
                //"Local and internet access";
            }
            else if (access == NetworkAccess.ConstrainedInternet)
            {
                //"Limited internet access";
            }
            else if (access == NetworkAccess.Local)
            {
                //"Local network access only";
            }
            else if (access == NetworkAccess.None)
            {
                await ReloadAsync(LoadDataType.FullData);
                //"No connectivity is available";

            }
            else if (access == NetworkAccess.Unknown)
            {
                //"Unable to determine internet connectivity";
            }
            if (profiles.Contains(ConnectionProfile.WiFi))
            {
                //profiles.FirstOrDefault().ToString();
            }
            else
            {
                //profiles.FirstOrDefault().ToString();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            dystirViewModel.IsApplicationActive = false;
        }

        protected override void OnResume()
        {
            dystirViewModel.IsApplicationActive = true;
        }
    }
}
