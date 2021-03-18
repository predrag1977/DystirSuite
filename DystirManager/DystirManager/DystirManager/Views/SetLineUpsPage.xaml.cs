using DystirManager.Helper;
using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetLineUpsPage : ContentPage
    {
        private readonly MatchesViewModel _viewModel;
        private int _teamIndex = 0;
        private PlayerOfMatch _selectedPlayer;
        private bool _isPressed = false;
        private Label _numberLabel;

        public SetLineUpsPage(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = viewModel.SelectedLiveMatch;
            AddDetailsMenuItemView();
            ShowSelectedDetailsTab();
        }

        private void AddDetailsMenuItemView()
        {
            if (DetailsMenuItemView.Children != null)
            {
                DetailsMenuItemView.Children.Clear();
                DetailsMenuItemView.ColumnDefinitions.Clear();
                DetailsMenuItemView.RowDefinitions.Clear();
            }

            RowDefinition rd = new RowDefinition
            {
                Height = new GridLength(30)
            };
            DetailsMenuItemView.RowDefinitions.Add(rd);
            List<string> menuItemList = new List<string>()
            {
                _viewModel.SelectedLiveMatch.HomeTeam,
                _viewModel.SelectedLiveMatch.AwayTeam
            };

            int i = 0;
            foreach (string item in menuItemList)
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                DetailsMenuItemView.ColumnDefinitions.Add(cd);
                DetailMenuItem view = new DetailMenuItem
                {
                    BindingContext = item
                };
                Label menuTextLabel = (Label)view.FindByName("MenuTextLabel");
                menuTextLabel.FontSize = 10;
                StackLayout detailMenuItemPanel = (StackLayout)view.FindByName("DetailMenuItemPanel");
                detailMenuItemPanel.BackgroundColor = Color.DimGray;
                menuTextLabel.TextColor = Color.White;
                if (_teamIndex < menuItemList.Count && item == menuItemList[_teamIndex])
                {
                    detailMenuItemPanel.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Grid.SetColumn(view, i);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += TeamSelected_Tapped;
                tapGestureRecognizer.CommandParameter = menuItemList.IndexOf(item);
                view.GestureRecognizers.Add(tapGestureRecognizer);
                DetailsMenuItemView.Children.Add(view);
                i++;
            }
        }

        private void ShowSelectedDetailsTab()
        {
            HomePlayersListView.IsVisible = _teamIndex == 0;
            AwayPlayersListView.IsVisible = _teamIndex == 1;
        }

        private void SelectedPlayer_Tapped(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.IsSelected = (e as TappedEventArgs).Parameter as PlayerOfMatch == _selectedPlayer;
            }
            _selectedPlayer = (e as TappedEventArgs).Parameter as PlayerOfMatch;
            _selectedPlayer.IsSelected = true;
            var view = sender as View;
            ViewCell parentViewCell = view.Parent as ViewCell;
            parentViewCell.ForceUpdateSize();
        }

        private void OK_Player_Tapped(object sender, EventArgs e)
        {
            _selectedPlayer.IsSelected = false;
            ModifyPlayersOfMatch(_selectedPlayer);
            _selectedPlayer = null;
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
            if (_teamIndex == 0)
            {
                HomePlayersListView.SelectedItem = player;
            }
            else
            {
                AwayPlayersListView.SelectedItem = player;
            }
        }

        private void TeamSelected_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = int.Parse((e as TappedEventArgs).Parameter.ToString());
            if (selectedIndex != _teamIndex)
            {
                _teamIndex = selectedIndex;
                AddDetailsMenuItemView();
                ShowSelectedDetailsTab();
            }
        }

        private void GK_Position_Tapped(object sender, EventArgs e)
        {
            SetPosition(e as TappedEventArgs, "GK");
        }

        private void DEF_Position_Tapped(object sender, EventArgs e)
        {
            SetPosition(e as TappedEventArgs, "DEF");
        }

        private void MID_Position_Tapped(object sender, EventArgs e)
        {
            SetPosition(e as TappedEventArgs, "MID");
        }

        private void ATT_Position_Tapped(object sender, EventArgs e)
        {
            SetPosition(e as TappedEventArgs, "ATT");
        }

        private void SetPosition(TappedEventArgs tappedEventArgs, string position)
        {
            _selectedPlayer = tappedEventArgs.Parameter as PlayerOfMatch;
            if (_selectedPlayer.Position != position)
            {
                _selectedPlayer.Position = position;
            }
            else
            {
                _selectedPlayer.Position = null;
            }
        }

        private void NumberUp_Tapped(object sender, EventArgs e)
        {
            View numberParentLayout = (sender as Button).Parent as View;
            _numberLabel = numberParentLayout.FindByName("NumberLabel") as Label;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), IncreaseNumber);
            _isPressed = true;
        }

        private void NumberDown_Tapped(object sender, EventArgs e)
        {
            View numberParentLayout = (sender as Button).Parent as View;
            _numberLabel = numberParentLayout.FindByName("NumberLabel") as Label;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), DecreaseNumber);
            _isPressed = true;
        }

        private bool IncreaseNumber()
        {
            _selectedPlayer.Number = _selectedPlayer.Number.HasValue ? _selectedPlayer.Number + 1 : 0;
            _numberLabel.Text = _selectedPlayer.Number?.ToString();
            return _isPressed;
        }

        private bool DecreaseNumber()
        {
            _selectedPlayer.Number = _selectedPlayer.Number.HasValue ? _selectedPlayer.Number - 1 : 0;
            _selectedPlayer.Number = _selectedPlayer.Number > 0 ? _selectedPlayer.Number : null;
            _numberLabel.Text = _selectedPlayer.Number?.ToString();
            return _isPressed;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            _isPressed = false;
        }

        private void AddNewPlayer_Tapped(object sender, EventArgs e)
        {
            //_viewModel.SelectedLiveMatch = new Match()
            //{
            //    Time = DateTime.UtcNow.Date
            //};
            //await Navigation.PushAsync(new UpdateAndNewMatchPage(_viewModel, TypePages.NewPage), true);
        }

        private async void OKAndBack_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


        protected async override void OnDisappearing()
        {
            await _viewModel.SelectedLiveMatch.UpdateMatch();
            base.OnDisappearing();
        }
    }
}