using System;
using DystirManager.Services;
using DystirManager.Views;
using System.Globalization;
using System.Threading;
using DystirManager.ViewModels;
using DystirManager.Models;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Device = Xamarin.Forms.Device;

namespace DystirManager
{
    public partial class App : Application
    {
        private MatchesViewModel _viewModel = new MatchesViewModel();
        private HubConnection hubConnection;
        private bool _isBusy;

        public App()
        {
            InitializeComponent();
            AppAnalytics();
            SetLanguage();
            DependencyService.Register<DataLoader>();
            Login();
        }

        private void AppAnalytics()
        {
            AppCenter.Start("android=9ab87fe9-92da-45bb-aabf-37022479c19a;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));
        }

        private void SetLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            _viewModel.LanguageCode = CultureInfo.GetCultureInfo("en-US");
        }

        private void Login()
        {
            MainPage = new NavigationPage(new LogInPage(_viewModel, true));
        }

        internal void Logout()
        {
            Current.Properties.Remove("username");
            Current.Properties.Remove("password");
            Current.MainPage = new NavigationPage(new LogInPage(_viewModel, false));
            Login();
        }

        public void StartApplication()
        {
            MainPage = new NavigationPage(new MatchesPage(_viewModel));
            StartLoadIng();
        }

        private async void StartLoadIng()
        {
            hubConnection = new HubConnectionBuilder().WithUrl("https://www.dystir.fo/dystirHub").Build();
            hubConnection.Closed += Connection_Closed;
            _viewModel.IsLoading = true;
            await MakeConnectAsync();
        }

        private async Task Connection_Closed(Exception arg)
        {
            await Reconnect();
        }

        private async Task MakeConnectAsync()
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            bool connected = await RefreshPageData();
            if (connected)
            {
                connected = await TryHubConnectAsync();
            }
            _isBusy = false;

            _viewModel.IsDisconnected = _viewModel.IsConnectionError = !connected;
            if (!connected)
            {
                await Reconnect();
            }
        }

        public async Task<bool> RefreshPageData()
        {
            bool connected = await _viewModel.GetPageData();
            Device.BeginInvokeOnMainThread(() =>
            {
                PopulatePageView(true);
            });
            _viewModel.IsLoading = false;
            return connected;
        }

        internal async Task Reconnect()
        {
            await Task.Delay(1000);
            if (_viewModel.IsApplicationActive)
            {
                await MakeConnectAsync();
            }
            else
            {
                await Reconnect();
            }
        }

        private async Task<bool> TryHubConnectAsync()
        {
            bool success = true;
            try
            {
                await hubConnection.StartAsync();
                _ = hubConnection.On<string, string>("ReceiveMessage", (a, b) =>
                  {
                      Device.BeginInvokeOnMainThread(() =>
                      {
                          //_ = RefreshMatchData(a, b);
                      });
                  });
                _ = hubConnection.On<string, string>("ReceiveMatchDetails", (action, matchDetailsJson) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var matchDetails = JsonConvert.DeserializeObject<MatchDetails>(matchDetailsJson);
                        RefreshMatchData(action, matchDetails);
                    });
                });
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Error: {ex.Message}");
                await Task.Delay(1000);
            }
            return success;
        }

        private async Task RefreshMatchData(string action, string message)
        {
            try
            {
                string[] textArray = message.Split(';');
                string matchID = textArray[0];
                Match match = await _viewModel.GetMatchByIDAsync(matchID);
                if (match != null)
                {
                    Match findMatch = _viewModel.AllMatches?.FirstOrDefault(x => x.MatchID == match?.MatchID);
                    if (findMatch != null)
                    {
                        int findMatchIndex = _viewModel.AllMatches.IndexOf(findMatch);
                        _viewModel.AllMatches?.Insert(findMatchIndex, match);
                        _viewModel.AllMatches?.Remove(findMatch);
                    }
                    else
                    {
                        _viewModel.AllMatches?.Add(match);
                    }

                    if (match.StatusID != findMatch?.StatusID)
                    {
                        await _viewModel.GetStandingsAsync();
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PopulatePageView(_viewModel.SelectedLiveMatch.MatchID == match?.MatchID);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void RefreshMatchData(string action, MatchDetails matchDetails)
        {
            try
            {
                Match match = matchDetails?.Match;
                if (match != null)
                {
                    Match findMatch = _viewModel.AllMatches?.FirstOrDefault(x => x.MatchID == match.MatchID);
                    if (findMatch != null)
                    {
                        int findMatchIndex = _viewModel.AllMatches.IndexOf(findMatch);
                        _viewModel.AllMatches?.Insert(findMatchIndex, match);
                        _viewModel.AllMatches?.Remove(findMatch);
                    }
                    else
                    {
                        _viewModel.AllMatches?.Add(match);
                    }

                    //if (match.StatusID != findMatch?.StatusID)
                    //{
                    //    await _viewModel.GetStandingsAsync();
                    //}
                    bool isSelectedMatch = _viewModel.SelectedLiveMatch.MatchID == match.MatchID;
                    if (isSelectedMatch)
                    {
                        Match selectedMatch = matchDetails.Match;
                        selectedMatch.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(matchDetails.PlayersOfMatch);
                        selectedMatch.EventsOfMatch = new ObservableCollection<EventOfMatch>(matchDetails.EventsOfMatch);
                        _viewModel.SelectedLiveMatch = selectedMatch;
                        _viewModel.SelectedLiveMatch.LiveTime = selectedMatch.LiveTime;
                        _viewModel.TimeCounter.MatchesTime(_viewModel, new ObservableCollection<Match>());
                        _viewModel.SetMatchAdditionalDetails();
                        _viewModel.SelectedLiveMatch.IsLoadingSelectedMatch = false;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PopulatePageView(false);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void PopulatePageView(bool refreshSelectedMatch)
        {
            var navigationPage = Current.MainPage as NavigationPage;
            if (navigationPage?.RootPage is MatchesPage)
            {
                var currentPage = navigationPage?.RootPage as MatchesPage;
                currentPage.PopulateView(_viewModel);
            }
            else if (navigationPage?.RootPage is ResultsPage)
            {
                var currentPage = navigationPage?.RootPage as ResultsPage;
                currentPage.PopulateView(_viewModel);
            }
            else if (navigationPage?.RootPage is FixturesPage)
            {
                var currentPage = navigationPage?.RootPage as FixturesPage;
                currentPage.PopulateView(_viewModel);
            }
            if (navigationPage?.CurrentPage is SelectedMatchDetailsView)
            {
                var currentPage = navigationPage?.CurrentPage as SelectedMatchDetailsView;
                currentPage.ReloadSelectedMatches(_viewModel, refreshSelectedMatch);
            }
        }

        public void NavigateFromMenu(int pageIndex)
        {
            switch (pageIndex)
            {
                case 0:
                    MainPage = new NavigationPage(new MatchesPage(_viewModel));
                    break;
                case 1:
                    MainPage = new NavigationPage(new ResultsPage(_viewModel));
                    break;
                case 2:
                    MainPage = new NavigationPage(new FixturesPage(_viewModel));
                    break;
            }
        }

        public void ChangeLanguage()
        {
            if (_viewModel.LanguageCode == CultureInfo.GetCultureInfo("fo-FO"))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                _viewModel.LanguageCode = CultureInfo.GetCultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fo-FO");
                _viewModel.LanguageCode = CultureInfo.GetCultureInfo("fo-FO");
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            _viewModel.IsApplicationActive = false;
        }

        protected override void OnResume()
        {
            // Handle when your app resume
            _viewModel.IsApplicationActive = true;
        }
    }
}
