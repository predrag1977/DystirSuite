using System;
using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.Services;

namespace Dystir.ViewModels
{
    public class MatchDetailsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//
        ObservableCollection<Match> matchesBySelectedDate = new ObservableCollection<Match>();
        public ObservableCollection<Match> MatchesBySelectedDate
        {
            get { return matchesBySelectedDate; }
            set { matchesBySelectedDate = value; OnPropertyChanged(); }
        }

        public Command SelectedMatchChangedCommand => new Command(SelectedMatchChanged);

        //**********************//
        //     CONSTRUCTOR      //
        //**********************//
        public MatchDetailsViewModel(DystirService dystirService)
        {
            DystirService = dystirService;
            DystirService.OnShowLoading += DystirService_OnShowLoading;
            DystirService.OnMatchDetailsLoaded += DystirService_OnMatchDetailsLoaded;
        }

        //**********************//
        //    PRIVATE METHODS   //
        //**********************//
        private void DystirService_OnShowLoading()
        {
            if (SelectedMatch != null)
            {
                SelectedMatch.IsLoading = true;
            }
        }

        private void DystirService_OnMatchDetailsLoaded(Match match)
        {
            if (SelectedMatch?.MatchID == match?.MatchID)
            {
                SelectedMatch = match;
            }
            //SetMatchesBySelectedDate();
        }

        private void SetMatchesBySelectedDate()
        {
            var matches = DystirService.AllMatches?.Where(x => x.Time?.Date == SelectedMatch?.Time?.AddSeconds(1).Date)
                .OrderBy(x => x.MatchTypeID)
                .ThenBy(x => x.Time)?.ToList() ?? new List<Match>();

            if (SelectedMatch != null)
            {
                matches.RemoveAll(x => x.MatchID == SelectedMatch.MatchID);
                matches.Insert(0, SelectedMatch);
                MatchesBySelectedDate = new ObservableCollection<Match>(matches);
            }
        }

        private async void SelectedMatchChanged(object sender)
        {
            var match = sender as Match;
            if (match != null)
            {
                SelectedMatch = match;
                //await _dystirService?.LoadMatchDetailsAsync(match);
            }
            await Task.CompletedTask;
        }
    }
}

