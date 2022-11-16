using Dystir.Models;
using System.Globalization;

namespace Dystir.Converter
{
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

    public class ColorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color retColor = Colors.DarkRed;
            if (value != null && (int)value >= 0 && (int)value < 12)
            {
                retColor = Colors.DarkGreen;
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

    public class LineUpsGroupVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value != null && parameter != null)
            //{
            //    View layout = (View)parameter;
            //    MatchesViewModel viewModel = layout.BindingContext as MatchesViewModel;
            //    var allPlayers = value as ObservableCollection<PlayerOfMatch>;
            //    var playersByTeam = allPlayers?.Where(x => x.TeamName?.ToUpper().Trim() == viewModel?.SelectedLiveMatch?.Match?.SelectedTeam?.ToUpper().Trim())?.ToList();

            //    return playersByTeam;
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HomeTeamFullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string homeTeamFull = string.Empty;
            if (value != null && value is Match match)
            {
                homeTeamFull = string.Format("{0} {1} {2}", match.HomeTeam, match.HomeSquadName, match.HomeCategoriesName).Trim();
            }
            return homeTeamFull.Trim();
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
            if (value != null && value is Match match)
            {
                awayTeamFull = string.Format("{0} {1} {2}", match.AwayTeam, match.AwaySquadName, match.AwayCategoriesName).Trim();
            }
            return awayTeamFull.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ToLocalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string localTime = string.Empty;
            if (value != null)
            {
                DateTime matchTime = (DateTime)value;
                localTime = matchTime.ToLocalTime().ToString(parameter.ToString(), Thread.CurrentThread.CurrentUICulture);
            }
            return localTime.ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FullMatchInfoTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string matchInfo = string.Empty;
            if (value != null && value is Match)
            {
                Match match = (Match)value;
                string matchDate = match.Time?.ToLocalTime().ToString("dd.MM.yyyy. - ");
                string matchTime = match.Time?.ToLocalTime().ToString("HH:mm - ");
                if (match.Time?.Hour == 0 && match.Time?.Minute == 0)
                {
                    matchTime = string.Empty;
                }
                string matchRound = !string.IsNullOrWhiteSpace(match.RoundName) ? match.RoundName + " - " : string.Empty;
                string matchLocation = match.Location;
                matchInfo = matchDate + matchTime + matchRound + matchLocation;
            }
            return matchInfo.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MatchInfoTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string matchInfo = string.Empty;
            if (value != null && value is Match)
            {
                Match match = (Match)value;
                string matchTime = match.Time?.ToLocalTime().ToString("HH:mm - ");
                if (match.Time?.Hour == 0 && match.Time?.Minute == 0)
                {
                    matchTime = string.Empty;
                }
                string matchRound = !string.IsNullOrWhiteSpace(match.RoundName) ? match.RoundName + " - " : string.Empty;
                string matchLocation = match.Location;
                matchInfo = matchTime + matchRound + matchLocation;
            }
            return matchInfo.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateToDayTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string matchTime = string.Empty;
            if (value != null)
            {
                DateTime selectedDateTime = (DateTime)value;
                matchTime = selectedDateTime.ToLocalTime().ToString("dd.MM.");
                if (selectedDateTime.ToLocalTime().Date == DateTime.Now.Date)
                {
                    matchTime = Resources.Localization.Resources.Today;
                }
            }
            return matchTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fullName = string.Empty;
            if (value != null)
            {
                PlayerOfMatch player = (value as PlayerOfMatch);
                fullName = player?.FirstName + " " + player?.LastName;
            }
            return fullName.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyNumberAndPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                value = "--";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GoalToVisibleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retVal = false;
            if (value != null)
            {
                string eventName = value.ToString().ToUpper();
                if (eventName == "GOAL" || eventName == "OWNGOAL" || eventName == "PENALTYSCORED")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayersInfoValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retVal = false;
            if (value != null)
            {
                if (int.Parse(value.ToString()) > int.Parse(parameter.ToString()))
                {
                    retVal = true;
                }
            }
            return retVal;
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

    public class SelectedMatchToFirstElevenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value != null)
            //{
            //    Match selectedMatch = value as Match;
            //    if (parameter.ToString() == "home")
            //    {
            //        return selectedMatch.PlayersOfMatch?.Where(x => x.PlayingStatus == 1 && x.TeamName.ToUpper().Trim() == selectedMatch?.HomeTeam.ToUpper().Trim());
            //    }
            //    if (parameter.ToString() == "away")
            //    {
            //        return selectedMatch?.PlayersOfMatch?.Where(x => x.PlayingStatus == 1 && x.TeamName.ToUpper().Trim() == selectedMatch?.AwayTeam.ToUpper().Trim());
            //    }
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedMatchToSubstitutionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value != null)
            //{
            //    Match selectedMatch = (value as Match);
            //    if (parameter.ToString() == "home")
            //    {
            //        return selectedMatch.PlayersOfMatch?.Where(x => x.PlayingStatus == 2 && x.TeamName.ToUpper().Trim() == selectedMatch?.HomeTeam.ToUpper().Trim());
            //    }
            //    if (parameter.ToString() == "away")
            //    {
            //        return selectedMatch?.PlayersOfMatch?.Where(x => x.PlayingStatus == 2 && x.TeamName.ToUpper().Trim() == selectedMatch?.AwayTeam.ToUpper().Trim());
            //    }
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatisticToLenghtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new GridLength((int)value, GridUnitType.Star);
            }
            return new GridLength(1, GridUnitType.Star);
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
            bool retval = value != null ? !(bool)value : true;
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Resources.Localization.Resources.ResourceManager.GetString((parameter ?? "").ToString(), culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayersPositionLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (value as PlayerOfMatch).PlayerOfMatchID > 0)
            {
                string position = (value as PlayerOfMatch).Position;
                if (!string.IsNullOrWhiteSpace(position))
                {
                    return Resources.Localization.Resources.ResourceManager.GetString((position ?? "").ToString(), culture);
                }
                else
                {
                    return "---";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LanguageToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return culture != CultureInfo.GetCultureInfo((parameter ?? "").ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemsCountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TwoItemsTovisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && int.Parse(value.ToString()) > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseItemsCountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (int)value < 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MatchResultDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string score = "-";
            if(value != null)
            {
                Match match = value as Match;
                int? statusID = match.StatusID;
                if(statusID > 1 && statusID < 14)
                {
                    if(parameter != null && parameter?.ToString() == "home")
                    {
                        score = match.HomeTeamScore?.ToString();
                    }
                    else
                    {
                        score = match.AwayTeamScore?.ToString();
                    }
                }
            }
            return score;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
