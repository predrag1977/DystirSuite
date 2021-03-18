using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using System.Collections.Generic;
using DystirXamarin.ViewModels;
using System.Threading.Tasks;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedPlayerPage : ContentPage
    {
        private bool _isPressed = false;
        private PlayerOfMatch _playerOfMatch;

        public Label MatchTimeLabel { get; private set; }
        public DateTime TotalTime { get; private set; }
        
        public MatchesViewModel _viewModel { get; private set; }
        public ListView PlayingListView { get; private set; }

        public SelectedPlayerPage(PlayerOfMatch playerOfMatch, MatchesViewModel viewModel, ListView playingListView)
        {
            InitializeComponent();
            _playerOfMatch = playerOfMatch;
            _viewModel = viewModel;
            PlayingListView = playingListView;
            Title = _playerOfMatch?.FirstName + " " + _playerOfMatch?.LastName;
            BindingContext = _playerOfMatch;
        }

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(Number.Text, out int x))
            {
                _playerOfMatch.Number = Convert.ToInt32(Number.Text);
            }
            else
            {
                _playerOfMatch.Number = null;
                Number.Text = null;
            }
        }

        private void NumberUp_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseNumber);
            _isPressed = true;
        }

        private void NumberDown_Tapped(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseNumber);
            _isPressed = true;
        }

        private bool IncreaseNumber()
        {
            _playerOfMatch.Number = _playerOfMatch.Number.HasValue ? _playerOfMatch.Number + 1 : 0;
            Number.Text = _playerOfMatch.Number?.ToString();
            return _isPressed;
        }

        private bool DecreaseNumber()
        {
            _playerOfMatch.Number = _playerOfMatch.Number.HasValue ? _playerOfMatch.Number - 1 : 0;
            _playerOfMatch.Number = _playerOfMatch.Number > 0 ? _playerOfMatch.Number : null;
            Number.Text = _playerOfMatch.Number?.ToString();
            return _isPressed;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isPressed = false;
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(false);
            var playersList = _viewModel?.SelectedLiveMatch?.PlayersOfMatch?.ToList();
            if (_playerOfMatch.PlayingStatus < 3)
            {
                PlayerOfMatch player = CopyPlayerOfMatch();
                player.FirstName = FirstNameEntry.Text;
                player.LastName = LastNameEntry.Text;
                if (int.TryParse(Number.Text, out int x))
                {
                    player.Number = Convert.ToInt32(Number.Text);
                }
                playersList.Add(player);
                playersList.Remove(_playerOfMatch);
                await _viewModel.UpdatePlayerOfMatch(player);
                _viewModel.SelectedLiveMatch.PlayersOfMatch = new ObservableCollection<PlayerOfMatch>(playersList);
                PlayingListView.SelectedItem = player;
            }
            else
            {
                SetNewPlayerOfMatchValues();
                if (await _viewModel.AddPlayerOfMatch(_playerOfMatch))
                {
                    await _viewModel.GetPlayersOfMatchAsync(_viewModel.SelectedLiveMatch);
                }
                PlayingListView.SelectedItem = _playerOfMatch;
            }
        }

        private PlayerOfMatch CopyPlayerOfMatch()
        {
            return new PlayerOfMatch()
            {
                PlayerID = _playerOfMatch.PlayerID,
                PlayerOfMatchID = _playerOfMatch.PlayerOfMatchID,
                PlayingStatus = _playerOfMatch.PlayingStatus,
                Position = _playerOfMatch.Position,
                TeamName = _playerOfMatch.TeamName,
                MatchID = _playerOfMatch.MatchID,
                MatchTypeID = _playerOfMatch.MatchTypeID,
                MatchTypeName = _playerOfMatch.MatchTypeName
            };
        }

        private void SetNewPlayerOfMatchValues()
        {
            if (!string.IsNullOrWhiteSpace(FirstNameEntry.Text) || !string.IsNullOrWhiteSpace(LastNameEntry.Text) || !string.IsNullOrWhiteSpace(Number.Text))
            {
                _playerOfMatch.FirstName = FirstNameEntry.Text;
                _playerOfMatch.LastName = LastNameEntry.Text;
                _playerOfMatch.PlayingStatus = 0;

                if (int.TryParse(Number.Text, out int x))
                {
                    _playerOfMatch.Number = Convert.ToInt32(Number.Text);
                }

                _playerOfMatch.TeamName = _viewModel.SelectedLiveMatch.SelectedTeam;
                _playerOfMatch.MatchID = _viewModel.SelectedLiveMatch.MatchID;
                _playerOfMatch.MatchTypeID = _viewModel.SelectedLiveMatch.MatchTypeID;
                _playerOfMatch.MatchTypeName = _viewModel.SelectedLiveMatch.MatchTypeName;
            }
        }

        private void Position_Tapped(object sender, EventArgs e)
        {

            string position = (e as TappedEventArgs).Parameter.ToString();
            if(_playerOfMatch.Position != position)
            {
                _playerOfMatch.Position = position;
            }
            else
            {
                _playerOfMatch.Position = null;
            }
        }
    }
}