using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using DystirXamarin.ViewModels;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineUpsPage : ContentPage
    {
        public MatchesViewModel _viewModel;
        private string _teamName;

        public LineUpsPage(MatchesViewModel viewModel, string teamName)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _teamName = teamName;
            Title = teamName;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.SelectedLiveMatch.SelectedTeam = _teamName;
        }

        private async void SetPlayerStatus(PlayerOfMatch player, int playingStatus)
        {
            int? previousPlayingStatus = player.PlayingStatus;

            if (previousPlayingStatus == playingStatus)
            {
                return;
            }

            player.PlayingStatus = playingStatus;

            if (previousPlayingStatus < 3)
            {
                if (await _viewModel.UpdatePlayerOfMatch(player))
                {
                    ModifyPlayersOfMatch(player);
                }
                else
                {
                    player.PlayingStatus = previousPlayingStatus;
                }
            }
            else
            {
                if (await _viewModel.AddPlayerOfMatch(player))
                {
                    await _viewModel.GetPlayersOfMatchAsync(_viewModel.SelectedLiveMatch);
                }
                else
                {
                    player.PlayingStatus = previousPlayingStatus;
                }
            }
        }

        private void ModifyPlayersOfMatch(PlayerOfMatch player)
        {
            var playersList = _viewModel?.SelectedLiveMatch?.PlayersOfMatch?.ToList();
            playersList.Add(player);
            playersList.Remove(playersList.Find(x => x.PlayerID == player.PlayerID));
            _viewModel.SelectedLiveMatch.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(playersList);
            PlayingListView.SelectedItem = player;
        }

        private void StartingPlaying_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch player = (PlayerOfMatch)(e as TappedEventArgs).Parameter;
            if (player != null)
            {
                int playingStatus = 1;
                SetPlayerStatus(player, playingStatus);
            }
        }

        private void Substitutions_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch player = (PlayerOfMatch)(e as TappedEventArgs).Parameter;
            if (player != null)
            {
                int playingStatus = 2;
                SetPlayerStatus(player, playingStatus);
            }
        }

        private void OutOfPlaying_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch player = (PlayerOfMatch)(e as TappedEventArgs).Parameter;
            if (player != null)
            {
                int playingStatus = 0;
                SetPlayerStatus(player, playingStatus);
            }
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        private async void SelectedPlayer_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch selectedPlayer = (e as TappedEventArgs).Parameter as PlayerOfMatch;
            await Navigation.PushAsync(new SelectedPlayerPage(selectedPlayer, _viewModel, PlayingListView), false);
        }

        private async void NewPlayer_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch player = new PlayerOfMatch();
            await Navigation.PushAsync(new SelectedPlayerPage(player, _viewModel, PlayingListView), false);
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Navigation?.NavigationStack?.LastOrDefault()?.GetType() != typeof(SelectedPlayerPage))
            {
                await _viewModel.UpdateMatchAsync(_viewModel.SelectedLiveMatch, false);
            }
        }

        private void PlayingListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //PlayingListView.ScrollTo(e.SelectedItem, ScrollToPosition.MakeVisible, true);
        }

        async void PlayingListView_Refreshing(object sender, EventArgs e)
        {
            PlayingListView.IsRefreshing = true;
            await _viewModel.GetPlayersOfMatchAsync(_viewModel.SelectedLiveMatch);
            PlayingListView.IsRefreshing = false;
        }
    }
}