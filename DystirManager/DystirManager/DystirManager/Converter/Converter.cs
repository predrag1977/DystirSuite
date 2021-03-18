using DystirManager.Helper;
using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xamarin.Forms;

namespace DystirManager.Converter
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

    public class ColorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color retColor = Color.DarkRed;
            if (value != null && (int)value >= 0 && (int)value < 12)
            {
                retColor = Color.DarkGreen;
            }

            return retColor;
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
            string matchInfoText = string.Empty;
            if (value != null)
            {
                Match selectedMatch = (Match)value;
                string matchHour = selectedMatch.Time != null ? selectedMatch.Time?.ToLocalTime().ToString("HH:mm - ") : string.Empty;
                string matchCompetition = !string.IsNullOrWhiteSpace(selectedMatch.MatchTypeName) ? selectedMatch.MatchTypeName + " - " : string.Empty;
                string matchRound = !string.IsNullOrWhiteSpace(selectedMatch.RoundName) ? selectedMatch.RoundName + " - " : string.Empty;
                string matchLocation = selectedMatch.Location;
                matchInfoText = matchHour + matchCompetition + matchRound + matchLocation;
            }
            return matchInfoText;
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
                    matchTime = Properties.Resources.Today;
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
            if (value != null)
            {
                Match selectedMatch = value as Match;
                if (parameter.ToString() == "home")
                {
                    return selectedMatch.PlayersOfMatch?.Where(x => x.PlayingStatus == 1 && x.TeamName.ToUpper().Trim() == selectedMatch?.HomeTeam.ToUpper().Trim());
                }
                if (parameter.ToString() == "away")
                {
                    return selectedMatch?.PlayersOfMatch?.Where(x => x.PlayingStatus == 1 && x.TeamName.ToUpper().Trim() == selectedMatch?.AwayTeam.ToUpper().Trim());
                }
            }
            return null;
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
            if (value != null)
            {
                Match selectedMatch = (value as Match);
                if (parameter.ToString() == "home")
                {
                    return selectedMatch.PlayersOfMatch?.Where(x => x.PlayingStatus == 2 && x.TeamName.ToUpper().Trim() == selectedMatch?.HomeTeam.ToUpper().Trim());
                }
                if (parameter.ToString() == "away")
                {
                    return selectedMatch?.PlayersOfMatch?.Where(x => x.PlayingStatus == 2 && x.TeamName.ToUpper().Trim() == selectedMatch?.AwayTeam.ToUpper().Trim());
                }
            }
            return null;
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
            bool retval = value != null ? !(bool)value : false;
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
            return Properties.Resources.ResourceManager.GetString((parameter ?? "").ToString(), culture);
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

    public class AdministratorFullnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Administrator administratorLoggedIn = value as Administrator;
                return administratorLoggedIn.AdministratorFirstName + " " + administratorLoggedIn.AdministratorLastName;
            }
            return "N/A";
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
                statusID = statusID > 0 && statusID <= 12 ? 0 : statusID;
                statusID = statusID == null ? 14 : statusID;
                retval = match.Statuses?.FirstOrDefault(x => x.StatusID == statusID)?.StatusName;
            }
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LivePeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval = string.Empty;
            if (value != null)
            {
                Match match = (Match)value;
                int? statusID = match.StatusID;
                retval = match.Statuses?.FirstOrDefault(x => x.StatusID == statusID)?.StatusName;
            }
            return retval;
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

    public class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.DimGray;
            string position = value?.ToString();
            string param = parameter?.ToString();
            if (value != null && param != null)
            {
                if (position.ToUpper() == param.ToUpper())
                {
                    color = Color.Black;
                }
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MainPlayerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is EventOfMatch)
            {
                EventOfMatch eventOfMatch = (EventOfMatch)value;
                if (eventOfMatch?.EventName?.ToUpper() == "CORNER"
                    || eventOfMatch?.EventName?.ToUpper() == "PENALTY"
                    || eventOfMatch?.EventName?.ToUpper() == "DOMINATION"
                    || eventOfMatch?.EventName?.ToUpper() == "COMMENTARY")
                {
                    return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SecondPlayerVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is EventOfMatch)
            {
                EventOfMatch eventOfMatch = (EventOfMatch)value;
                if (eventOfMatch?.EventName?.ToUpper() == "SUBSTITUTION")
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MatchResultVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is EventOfMatch)
            {
                EventOfMatch eventOfMatch = (EventOfMatch)value;
                if (eventOfMatch?.EventName?.ToUpper() == "GOAL"
                    || eventOfMatch?.EventName?.ToUpper() == "OWNGOAL"
                    || eventOfMatch?.EventName?.ToUpper() == "PENALTYSCORED")
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
