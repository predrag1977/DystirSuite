using DystirManager.Models;
using DystirManager.ViewModels;
using DystirManager.Views;
using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace DystirManager.Helper
{
    class PlayersView
    {
        internal void PopulatePlayersView(StackLayout firstElevenView, IEnumerable<PlayerOfMatch> playerList)
        {
            firstElevenView.Children.Clear();
            foreach (PlayerOfMatch playerOfMatch in playerList)
            {
                StackLayout numberLayout = new StackLayout()
                {
                    WidthRequest = 30,
                    HeightRequest = 30
                };
                Label numberLabel = new Label()
                {
                    Text = playerOfMatch.Number.ToString(),
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = Application.Current.Resources["BoldFont"].ToString(),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                };
                numberLayout.Children.Add(numberLabel);

                StackLayout nameLayout = new StackLayout();
                Label nameLabel = new Label()
                {
                    Text = string.Format("{0} {1}", playerOfMatch.FirstName, playerOfMatch.LastName).Trim().ToString(),
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                nameLayout.Children.Add(nameLabel);

                StackLayout infoLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };
                PopulatePlayerInfoLayout(playerOfMatch, infoLayout);

                StackLayout playerInfoLayout = new StackLayout();
                playerInfoLayout.Children.Add(nameLayout);
                playerInfoLayout.Children.Add(infoLayout);

                StackLayout mainLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };
                mainLayout.Children.Add(numberLayout);
                mainLayout.Children.Add(playerInfoLayout);

                firstElevenView.Children.Add(mainLayout);
                BoxView boxView = new BoxView()
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.DimGray
                };
                firstElevenView.Children.Add(boxView);
            }
        }

        private void PopulatePlayerInfoLayout(PlayerOfMatch playerOfMatch, StackLayout infoLayout)
        {
            //POSITION
            Label positionLabel = new Label()
            {
                Text = GetPositionText(playerOfMatch.Position),
                TextColor = Color.FromHex("#a6a6a6"),
                FontSize = 10,
                Margin = new Thickness(0, 0, 2, 0)
            };
            infoLayout.Children.Add(positionLabel);

            //GOAL
            if (playerOfMatch.Goal > 0)
            {
                CachedImage goalImage = new CachedImage()
                {
                    Source = @"resource://DystirManager.Images.goal.png",
                    HeightRequest = 10,
                    WidthRequest = 10,
                    Margin = new Thickness(2, 0, 0, 0)
                };
                Label goalLabel = new Label()
                {
                    TextColor = Color.FromHex("#a6a6a6"),
                    Text = playerOfMatch.Goal.ToString(),
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 2, 0)
                };
                infoLayout.Children.Add(goalImage);
                infoLayout.Children.Add(goalLabel);
            }

            //OWNGOAL
            if (playerOfMatch.OwnGoal > 0)
            {
                CachedImage ownGoalImage = new CachedImage()
                {
                    Source = @"resource://DystirManager.Images.owngoal.png",
                    HeightRequest = 10,
                    WidthRequest = 10,
                    Margin = new Thickness(2, 0, 0, 0)
                };
                Label ownGoalLabel = new Label()
                {
                    TextColor = Color.FromHex("#a6a6a6"),
                    Text = playerOfMatch.OwnGoal.ToString(),
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 2, 0)
                };
                infoLayout.Children.Add(ownGoalImage);
                infoLayout.Children.Add(ownGoalLabel);
            }

            //YELLOW CARD
            if (playerOfMatch.YellowCard > 0)
            {
                Grid yellowCardGrid = new Grid()
                {
                    BackgroundColor = Color.Gold,
                    HeightRequest = 8,
                    WidthRequest = 7,
                    Margin = new Thickness(2)
                };
                infoLayout.Children.Add(yellowCardGrid);
                if (playerOfMatch.YellowCard > 1)
                {
                    yellowCardGrid = new Grid()
                    {
                        BackgroundColor = Color.Gold,
                        HeightRequest = 8,
                        WidthRequest = 7,
                        Margin = new Thickness(2)
                    };
                    infoLayout.Children.Add(yellowCardGrid);
                }
            }

            //RED CARD
            if (playerOfMatch.RedCard > 0)
            {
                Grid redCardGrid = new Grid()
                {
                    BackgroundColor = Color.Red,
                    HeightRequest = 8,
                    WidthRequest = 7,
                    Margin = new Thickness(2)
                };
                infoLayout.Children.Add(redCardGrid);
            }

            //SUBIN
            if (playerOfMatch.SubIN > 0)
            {
                Label subInLabel = new Label()
                {
                    Text = char.ConvertFromUtf32(0x25B2).ToString(),
                    TextColor = Color.Green,
                    FontSize = 10,
                    Margin = new Thickness(2, 0, 0, 0)
                };
                infoLayout.Children.Add(subInLabel);
                Label subInText = new Label()
                {
                    Text = playerOfMatch.SubIN.ToString() + "'",
                    TextColor = Color.FromHex("#a6a6a6"),
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 2, 0)
                };
                infoLayout.Children.Add(subInText);
            }

            //SUBOUT
            if (playerOfMatch.SubOUT > 0)
            {
                Label subOutLabel = new Label()
                {
                    Text = char.ConvertFromUtf32(0x25BC).ToString(),
                    TextColor = Color.Red,
                    FontSize = 10,
                    Margin = new Thickness(2, 0, 0, 0)
                };
                infoLayout.Children.Add(subOutLabel);
                Label subOutText = new Label()
                {
                    Text = playerOfMatch.SubOUT.ToString() + "'",
                    TextColor = Color.FromHex("#a6a6a6"),
                    FontSize = 10,
                    Margin = new Thickness(0, 0, 2, 0)
                };
                infoLayout.Children.Add(subOutText);
            }
        }

        private string GetPositionText(string position)
        {
            switch (position?.ToUpper())
            {
                case "GK":
                    return Properties.Resources.GK;
                case "DEF":
                    return Properties.Resources.DEF;
                case "MID":
                    return Properties.Resources.MID;
                case "ATT":
                    return Properties.Resources.ATT;
                default:
                    return "---";
            }
        }
    }

    class MenuView
    {
        internal void PopulateMenuView(Grid menuView, int pageIndex)
        {
            if (menuView.Children != null)
            {
                menuView.Children.Clear();
                menuView.ColumnDefinitions.Clear();
                menuView.RowDefinitions.Clear();
            }

            RowDefinition rd = new RowDefinition
            {
                Height = new GridLength(50)
            };
            menuView.RowDefinitions.Add(rd);
            List<string> menuItemList = new List<string>()
            {
                Properties.Resources.Matches.ToUpper(),
                Properties.Resources.Results.ToUpper(),
                Properties.Resources.Fixtures.ToUpper(),
                //Properties.Resources.Standings.ToUpper(),
                //"IN.FO"
            };

            int i = 0;
            foreach (string item in menuItemList)
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                menuView.ColumnDefinitions.Add(cd);

                DetailMenuItem view = new DetailMenuItem
                {
                    BindingContext = item
                };
                StackLayout detailMenuItemPanel = (StackLayout)view.FindByName("DetailMenuItemPanel");
                detailMenuItemPanel.BackgroundColor = Color.DimGray;
                Label menuTextLabel = (Label)view.FindByName("MenuTextLabel");
                menuTextLabel.FontSize = 9;
                menuTextLabel.TextColor = Color.White;
                if (item == menuItemList[pageIndex])
                {
                    detailMenuItemPanel.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Grid.SetColumn(view, i);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += MenuItemSelected_Tapped;
                tapGestureRecognizer.CommandParameter = menuItemList.IndexOf(item);
                view.GestureRecognizers.Add(tapGestureRecognizer);
                menuView.Children.Add(view);
                i++;
            }
        }

        private void MenuItemSelected_Tapped(object sender, EventArgs e)
        {
            int menuIndex = (int)(e as TappedEventArgs).Parameter;
            (Application.Current as App).NavigateFromMenu(menuIndex);
        }
    }
}
