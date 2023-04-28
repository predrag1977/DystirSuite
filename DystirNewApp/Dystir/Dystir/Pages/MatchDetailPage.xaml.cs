using System.ComponentModel;
using Xamarin.Forms;
using Dystir.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dystir.Models;
using System.Threading.Tasks;
using Dystir.Services;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dystir.Pages
{
    [QueryProperty(nameof(MatchID), nameof(MatchID))]
    public partial class MatchDetailPage : ContentPage
    {
        private MatchDetailViewModel matchDetailViewModel;
        private DystirService dystirService;

        private string matchID;
        public string MatchID
        {
            get { return matchID; }
            set { matchID = value; }
        }

        public MatchDetailPage()
        {
            dystirService = DependencyService.Get<DystirService>();
            InitializeComponent();
            BindingContext = matchDetailViewModel = new MatchDetailViewModel();
            matchDetailViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            Shell.SetTabBarIsVisible(this, false);
            matchDetailViewModel.MatchID = int.Parse(MatchID);
            matchDetailViewModel.SelectedMatch = dystirService.AllMatches.FirstOrDefault(x => x.Match?.MatchID.ToString() == MatchID).Match;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await matchDetailViewModel.PopulateMatchDetailsTabs();
            await Task.Delay(100);
            await matchDetailViewModel.LoadMatchDetailAsync();
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            Shell.SetTabBarIsVisible(this, true);
            await Shell.Current.GoToAsync("..", true);
        }

        private async void RefreshButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (matchDetailViewModel.IsLoading == false)
            {
                await matchDetailViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}
