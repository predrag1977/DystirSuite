using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.ViewModels;
using Xamarin.Forms;

namespace Dystir.Helper
{
    public class Summary
    {
        internal void PopulateSummaryView(Grid parentLayout, EventOfMatch summaryEventOfMatch)
        {
            if (summaryEventOfMatch != null)
            {
                StackLayout mainLayout = new StackLayout();
                View summaryItemView = summaryEventOfMatch.HomeTeamVisible ? GetHomeTeamSummaryItemView(summaryEventOfMatch) : GetAwayTeamSummaryItemView(summaryEventOfMatch);
                BoxView separateLine = new BoxView()
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.DimGray
                };
                mainLayout.Children.Add(summaryItemView);
                mainLayout.Children.Add(separateLine);
                parentLayout.Children.Add(mainLayout);
            }
        }

        private View GetHomeTeamSummaryItemView(EventOfMatch summaryEventOfMatch)
        {
            StackLayout homeSummaryItemView = new StackLayout()
            {
                Spacing = 5,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                HeightRequest = 40,
                Padding = new Thickness(3, 0, 3, 3)
            };

            AddSummaryMinute(summaryEventOfMatch, homeSummaryItemView);
            AddSummaryEventName(summaryEventOfMatch, homeSummaryItemView);
            AddSummaryScore(summaryEventOfMatch, homeSummaryItemView);
            AddPlayerName(homeSummaryItemView, summaryEventOfMatch);
            return homeSummaryItemView;
        }

        private View GetAwayTeamSummaryItemView(EventOfMatch summaryEventOfMatch)
        {
            StackLayout awaySummaryItemView = new StackLayout()
            {
                Spacing = 5,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                HeightRequest = 40,
                Padding = new Thickness(3, 0, 3, 3)
            };

            AddPlayerName(awaySummaryItemView, summaryEventOfMatch);
            AddSummaryScore(summaryEventOfMatch, awaySummaryItemView);
            AddSummaryEventName(summaryEventOfMatch, awaySummaryItemView);
            AddSummaryMinute(summaryEventOfMatch, awaySummaryItemView);
            return awaySummaryItemView;
        }

        private void AddPlayerName(StackLayout summaryItemView, EventOfMatch summaryEventOfMatch)
        {
            string mainPlayer = summaryEventOfMatch.HomeMainPlayer;
            string secondPlayer = summaryEventOfMatch.HomeSecondPlayer;
            if (summaryEventOfMatch.AwayTeamVisible)
            {
                mainPlayer = summaryEventOfMatch.AwayMainPlayer;
                secondPlayer = summaryEventOfMatch.AwaySecondPlayer;
            }
            StackLayout playersLayout = new StackLayout()
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.Center
            };
            Label mainPlayersLabel = new Label()
            {
                Text = mainPlayer,
                TextColor = summaryEventOfMatch.EventName.ToUpper() == "SUBSTITUTION" ? Color.DarkRed : Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = summaryEventOfMatch.HomeTeamVisible ? TextAlignment.Start : TextAlignment.End
            };
            playersLayout.Children.Add(mainPlayersLabel);
            if (!string.IsNullOrWhiteSpace(secondPlayer))
            {
                Label secondPlayersLabel = new Label()
                {
                    Text = secondPlayer,
                    TextColor = summaryEventOfMatch.EventName.ToUpper() == "SUBSTITUTION" ? Color.Green : Color.FromHex("#a6a6a6"),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = summaryEventOfMatch.HomeTeamVisible ? TextAlignment.Start : TextAlignment.End
                };
                playersLayout.Children.Add(secondPlayersLabel);
            }
            summaryItemView.Children.Add(playersLayout);
        }

        public void AddSummaryScore(EventOfMatch summaryEventOfMatch, StackLayout summaryItemView)
        {
            switch (summaryEventOfMatch.EventName.ToUpper())
            {
                case "GOAL":
                case "OWNGOAL":
                case "PENALTYSCORED":
                    Label goalScoreLabel = new Label()
                    {
                        Text = summaryEventOfMatch.HomeTeamScore + ":" + summaryEventOfMatch.AwayTeamScore,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = Application.Current.Resources["BoldFont"].ToString(),
                    };
                    summaryItemView.Children.Add(goalScoreLabel);
                    break;
            }
        }

        public void AddSummaryMinute(EventOfMatch summaryEventOfMatch, StackLayout summaryItemView)
        {
            Label minuteLabel = new Label()
            {
                Text = summaryEventOfMatch.EventMinute,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Khaki,
                FontFamily = Application.Current.Resources["NormalFont"].ToString(),
            };
            summaryItemView.Children.Add(minuteLabel);
        }

        public void AddSummaryEventName(EventOfMatch summaryEventOfMatch, StackLayout summaryItemView)
        {
            switch (summaryEventOfMatch.EventName.ToUpper())
            {
                case "GOAL":
                case "OWNGOAL":
                case "PENALTYSCORED":
                    StackLayout goalNameLayout = new StackLayout()
                    {
                        BackgroundColor = Color.DarkGreen,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(5)
                    };
                    Label goalNameLabel = new Label()
                    {
                        Text = GetEventNameText(summaryEventOfMatch.EventName.ToUpper()),
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = Application.Current.Resources["BoldFont"].ToString(),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    goalNameLayout.Children.Add(goalNameLabel);
                    summaryItemView.Children.Add(goalNameLayout);
                    break;
                case "ASSIST":
                    StackLayout assistNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label assistNameLabel = new Label()
                    {
                        Text = Properties.Resources.Assist,
                        TextColor = Color.LightBlue,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    assistNameLayout.Children.Add(assistNameLabel);
                    summaryItemView.Children.Add(assistNameLayout);
                    break;
                case "YELLOW":
                    Grid yellowCardGrid = new Grid()
                    {
                        BackgroundColor = Color.Gold,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 9,
                        HeightRequest = 12
                    };
                    summaryItemView.Children.Add(yellowCardGrid);
                    break;
                case "RED":
                    Grid redCardGrid = new Grid()
                    {
                        BackgroundColor = Color.Red,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 9,
                        HeightRequest = 12
                    };
                    summaryItemView.Children.Add(redCardGrid);
                    break;
                case "SUBSTITUTION":
                    Label subINLabel = new Label()
                    {
                        Text = string.Format("{0} ", char.ConvertFromUtf32(0x25B2)).ToString(),
                        TextColor = Color.Green,
                        FontAttributes = FontAttributes.Bold,
                        FontFamily = Application.Current.Resources["BoldFont"].ToString(),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, -3, 0),
                        FontSize = 12
                    };
                    summaryItemView.Children.Add(subINLabel);
                    Label subOutLabel = new Label()
                    {
                        Text = string.Format("{0} ", char.ConvertFromUtf32(0x25BC)).ToString(),
                        TextColor = Color.Red,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(-3, 0, 0, 0),
                        FontSize = 12
                    };
                    summaryItemView.Children.Add(subOutLabel);
                    break;
                case "BIGCHANCE":
                    StackLayout bigChanceNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label bigChanceNameLabel = new Label()
                    {
                        Text = Properties.Resources.BigChance,
                        TextColor = Color.LightBlue,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    bigChanceNameLayout.Children.Add(bigChanceNameLabel);
                    summaryItemView.Children.Add(bigChanceNameLayout);
                    break;
                case "PENALTY":
                    StackLayout penaltyNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label penaltyNameLabel = new Label()
                    {
                        Text = Properties.Resources.Penalty,
                        TextColor = Color.LightBlue,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    penaltyNameLayout.Children.Add(penaltyNameLabel);
                    summaryItemView.Children.Add(penaltyNameLayout);
                    break;
                case "PENALTYMISSED":
                    StackLayout penaltyMissedNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label penaltyMissedNameLabel = new Label()
                    {
                        Text = Properties.Resources.PenaltyMissed,
                        TextColor = Color.Red,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    penaltyMissedNameLayout.Children.Add(penaltyMissedNameLabel);
                    summaryItemView.Children.Add(penaltyMissedNameLayout);
                    break;
                case "CORNER":
                    StackLayout cornerNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label cornerNameLabel = new Label()
                    {
                        Text = Properties.Resources.Corner,
                        TextColor = Color.FromHex("#a6a6a6"),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    cornerNameLayout.Children.Add(cornerNameLabel);
                    summaryItemView.Children.Add(cornerNameLayout);
                    break;
                case "ONTARGET":
                    StackLayout onTargetNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label onTargetNameLabel = new Label()
                    {
                        Text = Properties.Resources.OnTarget,
                        TextColor = Color.FromHex("#a6a6a6"),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    onTargetNameLayout.Children.Add(onTargetNameLabel);
                    summaryItemView.Children.Add(onTargetNameLayout);
                    break;
                case "OFFTARGET":
                    StackLayout offTargetNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label offTargetNameLabel = new Label()
                    {
                        Text = Properties.Resources.OffTarget,
                        TextColor = Color.FromHex("#a6a6a6"),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    offTargetNameLayout.Children.Add(offTargetNameLabel);
                    summaryItemView.Children.Add(offTargetNameLayout);
                    break;
                case "BLOCKEDSHOT":
                    StackLayout blockedShotNameLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };
                    Label blockedShotNameLabel = new Label()
                    {
                        Text = Properties.Resources.BlockedShot,
                        TextColor = Color.FromHex("#a6a6a6"),
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    blockedShotNameLayout.Children.Add(blockedShotNameLabel);
                    summaryItemView.Children.Add(blockedShotNameLayout);
                    break;
            }
        }

        private string GetEventNameText(string summaryEventName)
        {
            switch (summaryEventName)
            {
                case "GOAL":
                default:
                    return Properties.Resources.Goal;
                case "OWNGOAL":
                    return Properties.Resources.OwnGoal;
                case "PENALTYSCORED":
                    return Properties.Resources.PenaltyScored;
            }
        }

        internal List<EventOfMatch> GetSummaryEventsList(MatchDetails selectedMatch)
        {
            List<EventOfMatch> summaryEventOfMatchesList = new List<EventOfMatch>();
            var eventsList = selectedMatch?.EventsOfMatch?.ToList();
            int homeScore = 0;
            int awayScore = 0;
            foreach (var eventOfMatch in eventsList ?? new List<EventOfMatch>())
            {
                EventOfMatch summaryEventOfMatch = new EventOfMatch()
                {
                    HomeTeamScore = 0,
                    AwayTeamScore = 0,
                    HomeTeam = eventOfMatch?.EventTeam == selectedMatch?.Match?.HomeTeam ? selectedMatch?.Match?.HomeTeam : string.Empty,
                    AwayTeam = eventOfMatch?.EventTeam == selectedMatch?.Match?.AwayTeam ? selectedMatch?.Match?.AwayTeam : string.Empty,
                    EventName = eventOfMatch?.EventName,
                    EventMinute = eventOfMatch?.EventMinute,
                    HomeMainPlayer = GetEventPlayer(selectedMatch, eventOfMatch?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.HomeTeam?.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null),
                    HomeSecondPlayer = GetEventPlayer(selectedMatch, eventOfMatch?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.HomeTeam?.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null),
                    AwayMainPlayer = GetEventPlayer(selectedMatch, eventOfMatch?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.AwayTeam?.ToUpper().Trim() ? eventOfMatch?.MainPlayerOfMatchID : null),
                    AwaySecondPlayer = GetEventPlayer(selectedMatch, eventOfMatch?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.AwayTeam?.ToUpper().Trim() ? eventOfMatch?.SecondPlayerOfMatchID : null)
                };
                summaryEventOfMatch.HomeTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.HomeTeam);
                summaryEventOfMatch.AwayTeamVisible = !string.IsNullOrEmpty(summaryEventOfMatch.AwayTeam);
                if (IsGoal(eventOfMatch))
                {
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.HomeTeam?.ToUpper().Trim())
                    {
                        homeScore += 1;
                    }
                    if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.AwayTeam.ToUpper().Trim())
                    {
                        awayScore += 1;
                    }
                }
                summaryEventOfMatch.HomeTeamScore = homeScore;
                summaryEventOfMatch.AwayTeamScore = awayScore;

                if (eventOfMatch.EventName == "GOAL")
                {
                    int eventIndex = eventsList.IndexOf(eventOfMatch);
                    if (eventIndex + 1 < eventsList.Count)
                    {
                        EventOfMatch nextEvent = eventsList[eventIndex + 1];
                        if (nextEvent.EventName == "ASSIST")
                        {
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.HomeTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.HomeSecondPlayer = GetEventPlayer(selectedMatch, nextEvent?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.HomeTeam.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                            if (eventOfMatch.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.AwayTeam.ToUpper().Trim())
                            {
                                summaryEventOfMatch.AwaySecondPlayer = GetEventPlayer(selectedMatch, nextEvent?.EventTeam.ToUpper().Trim() == selectedMatch?.Match?.AwayTeam.ToUpper().Trim() ? nextEvent?.MainPlayerOfMatchID : null);
                            }
                        }
                    }
                }
                summaryEventOfMatchesList.Add(summaryEventOfMatch);
            }

            return summaryEventOfMatchesList.ToList();
        }

        bool IsGoal(EventOfMatch matchEvent)
        {
            return matchEvent.EventName == "GOAL"
                || matchEvent.EventName == "OWNGOAL"
                || matchEvent.EventName == "PENALTYSCORED";
        }

        private string GetEventPlayer(MatchDetails selectedMatch, int? playerID)
        {
            string fullName = string.Empty;
            if (playerID != null && playerID > 0)
            {
                PlayerOfMatch player = selectedMatch?.PlayersOfMatch?.FirstOrDefault(x => x.PlayerOfMatchID == playerID);
                fullName = player?.FirstName + " " + player?.LastName;
            }
            return fullName.Trim();
        }
    }

    public class Commentary
    {
        readonly Summary summary = new Summary();
        internal void PopulateCommentaryView(Grid commentaryView, EventOfMatch summaryEventOfMatch)
        {
            if (summaryEventOfMatch != null)
            {
                StackLayout mainLayout = new StackLayout();
                View summaryItemView = summaryEventOfMatch.HomeTeamVisible ? GetHomeTeamCommentaryItemView(summaryEventOfMatch) : GetAwayTeamCommentaryItemView(summaryEventOfMatch);
                Label commentaryLabel = new Label()
                {
                    Text = summaryEventOfMatch.EventText,
                    MaxLines = 8,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                mainLayout.Children.Add(summaryItemView);
                mainLayout.Children.Add(commentaryLabel);
                BoxView separateLine = new BoxView()
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.DimGray,
                    Margin = new Thickness(0, 5, 0, 3)
                };
                mainLayout.Children.Add(separateLine);
                commentaryView.Children.Add(mainLayout);
            }
        }

        private View GetHomeTeamCommentaryItemView(EventOfMatch summaryEventOfMatch)
        {
            Grid commentaryItemView = new Grid()
            {
                RowSpacing = 0,
                ColumnSpacing = 3,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(0, 0, 0, 3)
            };
            ColumnDefinition cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Auto)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);

            StackLayout minuteCommentryItemView = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Grid.SetColumn(minuteCommentryItemView, 1);
            StackLayout eventCommentryItemView = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };
            Grid.SetColumn(eventCommentryItemView, 0);

            summary.AddSummaryScore(summaryEventOfMatch, eventCommentryItemView);
            summary.AddSummaryEventName(summaryEventOfMatch, eventCommentryItemView);
            commentaryItemView.Children.Add(eventCommentryItemView);

            summary.AddSummaryMinute(summaryEventOfMatch, minuteCommentryItemView);
            commentaryItemView.Children.Add(minuteCommentryItemView);
            return commentaryItemView;
        }

        private View GetAwayTeamCommentaryItemView(EventOfMatch summaryEventOfMatch)
        {
            Grid commentaryItemView = new Grid()
            {
                RowSpacing = 0,
                ColumnSpacing = 3,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(0, 0, 0, 3)
            };
            ColumnDefinition cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Auto)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            commentaryItemView.ColumnDefinitions.Add(cd);

            StackLayout minuteCommentryItemView = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Grid.SetColumn(minuteCommentryItemView, 1);
            StackLayout eventCommentryItemView = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            Grid.SetColumn(eventCommentryItemView, 2);

            summary.AddSummaryMinute(summaryEventOfMatch, minuteCommentryItemView);
            commentaryItemView.Children.Add(minuteCommentryItemView);

            summary.AddSummaryEventName(summaryEventOfMatch, eventCommentryItemView);
            summary.AddSummaryScore(summaryEventOfMatch, eventCommentryItemView);
            commentaryItemView.Children.Add(eventCommentryItemView);

            return commentaryItemView;
        }
    }
}
