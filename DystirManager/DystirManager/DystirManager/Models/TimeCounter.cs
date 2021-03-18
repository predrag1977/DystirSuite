using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DystirManager.ViewModels;
using Xamarin.Forms;

namespace DystirManager.Models
{
    public class TimeCounter
    {
        internal void MatchesTime(MatchesViewModel viewModel, ObservableCollection<Match> matchesList)
        {
            viewModel.SelectedLiveMatch.LiveTime = !viewModel.IsDisconnected || viewModel.SelectedLiveMatch.StatusID > 11 ? MatchPeriod(viewModel.SelectedLiveMatch) : "....";
            foreach (Match match in matchesList)
            {
                match.LiveTime = !viewModel.IsDisconnected || match.StatusID > 11 ? MatchPeriod(match) : "....";
                match.LanguageCode = viewModel.LanguageCode;
            }
            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                viewModel.SelectedLiveMatch.LiveTime = !viewModel.IsDisconnected || viewModel.SelectedLiveMatch.StatusID > 11 ? MatchPeriod(viewModel.SelectedLiveMatch) : "....";
                foreach (Match match in matchesList)
                {
                    match.LiveTime = !viewModel.IsDisconnected || match.StatusID > 11 ? MatchPeriod(match) : "....";
                    match.LanguageCode = viewModel.LanguageCode;
                }
                return true;
            });
        }

        internal string MatchPeriod(Match match)
        {
            try
            {
                if (match.StatusTime == null)
                {
                    return "00:00";
                } 
                double totalSeconds = (double)(DateTime.Now.ToLocalTime() - match.StatusTime?.ToLocalTime())?.TotalSeconds;
                int minutes = (int)Math.Floor(totalSeconds / 60);
                int seconds = (int)totalSeconds - minutes * 60;
                int? matchStatus = match.StatusID;
                var addtime = "";
                switch (matchStatus)
                {
                    case 1:
                        return "00:00";
                    case 2:
                        if (minutes > 45)
                        {
                            addtime = "45+";
                            minutes -= 45;
                        }
                        break;
                    case 3:
                        return Properties.Resources.HalfTime;
                    case 4:
                        minutes += 45;
                        if (minutes > 90)
                        {
                            addtime = "90+";
                            minutes -= 90;
                        }
                        break;
                    case 5:
                        return Properties.Resources.FullTime;
                    case 6:
                        minutes += 90;
                        if (minutes > 105)
                        {
                            addtime = "105+";
                            minutes -= 105;
                        }
                        break;
                    case 7:
                        return Properties.Resources.ExtraTimePause;
                    case 8:
                        minutes += 105;
                        if (minutes > 120)
                        {
                            addtime = "120+";
                            minutes -= 120;
                        }
                        break;
                    case 9:
                        return Properties.Resources.ExtraTimeFinished;
                    case 10:
                        return Properties.Resources.Penalties;
                    default:
                        return Properties.Resources.Finished;
                }
                string min = minutes.ToString();
                string sec = seconds.ToString(); ;
                if (minutes < 10)
                    min = "0" + min;
                if (seconds < 10)
                    sec = "0" + sec;
                return addtime + " " + min + ":" + sec;
            }
            catch
            {
                return "00:00";
            }
        }
    }
}
