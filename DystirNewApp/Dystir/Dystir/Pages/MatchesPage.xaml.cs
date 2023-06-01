using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Match = Dystir.Models.Match;

namespace Dystir.Pages
{
    public partial class MatchesPage : ContentPage
    {
        private readonly MatchesViewModel matchesViewModel;

        public MatchesPage()
        {
            InitializeComponent();
            BindingContext = matchesViewModel = new MatchesViewModel();
        }

        protected override void OnAppearing()
        {
            _ = matchesViewModel.LoadDataAsync();
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (matchesViewModel.IsLoading == false)
            {
                await matchesViewModel.DystirService.LoadDataAsync(true);
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            //var match = (e as TappedEventArgs).Parameter as Match;
            //matchesViewModel.SelectedMatch = match;
            //var matchDetailPage = matchesViewModel.DystirService.ListMatchDetailPages.FirstOrDefault(x => x.MatchID == match.MatchID);
            //if (matchDetailPage == null)
            //{
            //    matchDetailPage = new MatchDetailPage(match.MatchID);
            //    matchesViewModel.DystirService.ListMatchDetailPages.Add(matchDetailPage);
            //}

            //// This will push the MatchDetailPage onto the navigation stack
            ////await Shell.Current.GoToAsync($"{nameof(MatchDetailPage)}?MatchID={match.MatchID}");
            ////await Navigation.PushAsync(matchDetailPage);
            //_ = Navigation.PushAsync(matchDetailPage);
            //await Task.CompletedTask;
        }
    }
}
