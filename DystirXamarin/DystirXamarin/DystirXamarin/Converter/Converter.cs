using DystirXamarin.Models;
using DystirXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DystirXamarin.Converter
{
    public class HomeTeamFullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string homeTeamFull = string.Empty;
            if (value != null)
            {
                Match match = value as Match;
                if (match != null)
                {
                    string homeTeamName = match.HomeTeam;
                    string homeCategorie = match.HomeCategoriesName?.ToLower().Trim() == "seniors" ? string.Empty : String.Format(" {0}", match.HomeCategoriesName?.Trim());
                    string homeSquad = match.HomeSquadName?.ToUpper().Trim() == "I" ? string.Empty : String.Format(" {0}", match.HomeSquadName?.Trim());
                    homeTeamFull = homeTeamName + homeSquad + homeCategorie;
                }
            }
            return homeTeamFull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AwayTeamFullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string awayTeamFull = string.Empty;
            if (value != null)
            {
                Match match = value as Match;
                if (match != null)
                {
                    string awayTeamName = match.AwayTeam;
                    string awayCategorie = match.AwayCategoriesName?.ToLower().Trim() == "seniors" ? string.Empty : String.Format(" {0}", match.AwayCategoriesName?.Trim());
                    match.AwayCategoriesName?.Trim();
                    string awaySquad = match.AwaySquadName?.ToUpper().Trim() == "I" ? string.Empty : String.Format(" {0}", match.AwaySquadName?.Trim());
                    awayTeamFull = awayTeamName + awaySquad + awayCategorie;
                }
            }
            return awayTeamFull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategorieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString().ToLower().Trim() == "seniors")
                return string.Empty;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SquadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString().ToUpper().Trim() == "I")
                return string.Empty;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MatchStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval = string.Empty;
            if (value != null)
            {
                Match match = (Match)value;
                int? statusID = match.StatusID;
                statusID = statusID > 0 && statusID < 13 ? 0 : statusID;
                if (statusID == null)
                {
                    retval = match.StatusName;
                }
                else
                {
                    retval = match.Statuses?.FirstOrDefault(x => x.StatusID == statusID)?.StatusName;
                }
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        
    public class MatchStatusNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval = string.Empty;
            if (value != null)
            {
                Match match = (Match)value;
                retval = match.Statuses?.FirstOrDefault(x => x.StatusID == match?.StatusID)?.StatusName;
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? !(bool)value : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CompetitonNameVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retval = false;
            if (value != null && parameter != null)
            {
                ListView listView = parameter as ListView;
                List<Match> matchesList = (listView.ItemsSource as IEnumerable<Match>)?.ToList() ?? new List<Match>();
                Match match = matchesList?.FirstOrDefault(x => x.MatchID == (int)value);
                int matchIndex = matchesList.IndexOf(match);
                Match previousMatch = new Match();
                if (matchIndex > 0)
                {
                    previousMatch = matchesList?[matchIndex - 1];
                }
                if (previousMatch?.MatchTypeID != match?.MatchTypeID || previousMatch?.MatchTypeName != match?.MatchTypeName)
                {
                    retval = true;
                }
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeAndResultMatchVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retval = false;
            if (value != null && (int)value >= 0 && (int)value < 14)
            {
                retval = true;
            }

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LiveMatchVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retval = false;
            if (value != null && (int)value >= 0 && (int)value < 13)
            {
                retval = true;
            }

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color retColor = Color.Red;
            if (value != null && (int)value >= 0 && (int)value < 12)
            {
                retColor = Color.LightGreen;
            }

            return retColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class TotalTimeFromSelectedMatchTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string minutes = "000";
            string seconds = "00";
            Match selectedMatch = (Match)value;
            if (selectedMatch != null)
            {
                DateTime matchStatusTime = selectedMatch.StatusTime != null ? selectedMatch.StatusTime.Value.ToLocalTime() : new DateTime();
                TimeSpan? ts = DateTime.Now - matchStatusTime;
                if (ts.Value.TotalSeconds < 0)
                {
                    selectedMatch.StatusTime = DateTime.Now;
                    ts = new TimeSpan();
                }
                int min = (int)ts.Value.TotalMinutes;
                int sec = ts.Value.Seconds;

                switch (selectedMatch.StatusID)
                {
                    case 2:
                        break;
                    case 4:
                        min += 45;
                        break;
                    case 6:
                        min += 90;
                        break;
                    case 8:
                        min += 105;
                        break;
                    default:
                        min = 0;
                        sec = 0;
                        break;
                }
                minutes = min.ToString();
                seconds = sec.ToString();
                if (min < 100)
                {
                    minutes = "0" + min;
                    if (min < 10)
                    {
                        minutes = "0" + minutes;
                    }
                    if (sec < 10)
                    {
                        seconds = "0" + seconds;
                    }
                }
            }
            return minutes + ":" + seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LiveMatchTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string liveMatchTime = string.Empty;
            if (value != null)
            {
                string eventTotalTime = value.ToString();
                int matchStatusID = 14;
                if (parameter != null)
                {
                    matchStatusID = (int)parameter;
                }
                int minutes = 0;
                int seconds = 0;
                string[] totalTimeArray = eventTotalTime.Split(':');
                if (totalTimeArray != null && totalTimeArray.Length == 2)
                {
                    minutes = int.Parse(totalTimeArray[0]);
                    seconds = int.Parse(totalTimeArray[1]);
                }
                string addtime = "";
                switch (matchStatusID)
                {
                    case 0:
                        return "00:00";
                    case 1:
                        return "00:00";
                    case 2:
                        if (minutes > 45)
                        {
                            addtime = "45+";
                            minutes = minutes - 45;
                        }
                        break;
                    case 3:
                        return "half time";
                    case 4:
                        if (minutes > 90)
                        {
                            addtime = "90+";
                            minutes = minutes - 90;
                        }
                        break;
                    case 5:
                        return "full time";
                    case 6:
                        if (minutes > 105)
                        {
                            addtime = "105+";
                            minutes = minutes - 105;
                        }
                        break;
                    case 7:
                        return "ex time pause";
                    case 8:
                        if (minutes > 120)
                        {
                            addtime = "120+";
                            minutes = minutes - 120;
                        }
                        break;
                    case 9:
                        return "ex time finished";
                    case 10:
                        return "penalties";
                    case 12:
                    case 13:
                        return "finished";
                    default:
                        return "";
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                    min = "0" + minutes;
                if (seconds < 10)
                    sec = "0" + seconds;
                liveMatchTime = addtime + " " + min + ":" + sec;
            }
            return liveMatchTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LineUpsGroupVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                View layout = (View)parameter;
                MatchesViewModel viewModel = layout.BindingContext as MatchesViewModel;
                var allPlayers = value as ObservableCollection<PlayerOfMatch>;
                var playersByTeam = allPlayers?.Where(x => x.TeamName?.ToUpper().Trim() == viewModel?.SelectedLiveMatch?.SelectedTeam?.ToUpper().Trim())?.ToList();

                return playersByTeam;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayingStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 1:
                    return "Starting lineups";
                case 2:
                    return "Substitutions";
                case 0:
                    return "Out of playing";
                default:
                    return "Other players";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayingStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch (value)
            {
                case 1:
                    return Color.DarkGreen;
                case 2:
                    return Color.DarkGoldenrod;
                case 0:
                    return Color.DarkRed;
                default:
                    return Color.DarkGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayingStatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch (value)
            {
                case 1:
                    return Application.Current.Resources["LineUpGreen"];
                case 2:
                    return Application.Current.Resources["LineUpYellow"];
                case 0:
                    return Application.Current.Resources["LineUpRed"];
                default:
                    return Application.Current.Resources["LineUpGray"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StartersTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int retval = 0;
            if (value != null)
            {
                retval = (value as ObservableCollection<PlayerOfMatch>)?.Count(x=>x.PlayingStatus == 1) ?? 0;
            }
            return "Starters: " + retval.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SubsTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int retval = 0;
            if (value != null)
            {
                retval = (value as ObservableCollection<PlayerOfMatch>)?.Count(x => x.PlayingStatus == 2) ?? 0;
            }
            return "Subs: " + retval.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayersListByTeamNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            MatchesViewModel viewModel = value as MatchesViewModel;
            var fullPlayerList = viewModel.SelectedLiveMatch?.PlayersOfMatch?.Where(y => y.TeamName == viewModel?.SelectedLiveMatch.SelectedTeam);
            return new ObservableCollection<IGrouping<int?, PlayerOfMatch>>(fullPlayerList.GroupBy(x => x.PlayingStatus));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.FromHex("#aac1dd");
            String position = value?.ToString();
            String param = parameter?.ToString();
            if (value != null && param != null)
            {
                if (position.ToUpper() == param.ToUpper())
                {
                    color = Color.FromHex("#2f5079");
                }
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MatchPeriodTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string liveMatchTime = string.Empty;
            if (value != null)
            {
                EventOfMatch eventOfMatch = (EventOfMatch)value;
                int totalMinutes = 0;
                string eventMinute = eventOfMatch.EventMinute.TrimStart('0').Replace("'", "");
                try
                {
                    int index = eventMinute.IndexOf('+');
                    if (index > 0)
                    {
                        totalMinutes = int.Parse(eventMinute.Substring(0, index));
                    }
                    else
                    {
                        totalMinutes = int.Parse(eventMinute);
                    }
                }
                catch
                {
                    totalMinutes = 0;
                }

                int minutes = 0;
                int seconds = 0;
                string addtime = "";
                int statusID = 0;
                if (parameter != null)
                {
                    statusID = (int)parameter;
                }
                switch (statusID)
                {
                    case 1:
                        return "00:00";
                    case 2:
                        if (minutes > 45)
                        {
                            addtime = "45+";
                            minutes = minutes - 45;
                        }
                        break;
                    case 3:
                        return "half time";
                    case 4:
                        minutes = minutes + 45;
                        if (minutes > 90)
                        {
                            addtime = "90+";
                            minutes = minutes - 90;
                        }
                        break;
                    case 5:
                        return "full time";
                    case 6:
                        minutes = minutes + 90;
                        if (minutes > 105)
                        {
                            addtime = "105+";
                            minutes = minutes - 105;
                        }
                        break;
                    case 7:
                        return "ex time pause";
                    case 8:
                        minutes = minutes + 105;
                        if (minutes > 120)
                        {
                            addtime = "120+";
                            minutes = minutes - 120;
                        }
                        break;
                    case 9:
                        return "ex time finished";
                    case 10:
                        return "penalties";
                    default:
                        return "finished";
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                    min = "0" + minutes;
                if (seconds < 10)
                    sec = "0" + seconds;
                liveMatchTime = addtime + " " + min + ":" + sec;
            }
            return liveMatchTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ConvertTimeToLocal : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTimeLocal = null;
            if (value != null)
            {
                DateTime dateTime = (DateTime)value;
                dateTimeLocal = dateTime.ToLocalTime();
            }
            return dateTimeLocal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
