using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;
using Dystir.Views;

namespace Dystir.Pages;

public partial class MatchesPage : ContentPage
{
    private readonly MatchesViewModel matchesViewModel;
    private readonly DystirService dystirService;
    private readonly LiveStandingService liveStandingService;

    public MatchesPage(DystirService dystirService, LiveStandingService liveStandingService, TimeService timeService)
    {
        this.matchesViewModel = new MatchesViewModel(dystirService, timeService);
        this.dystirService = dystirService;
        this.liveStandingService = liveStandingService;

        InitializeComponent();
        BindingContext = this.matchesViewModel;
    }

    protected override void OnAppearing()
    {
        _ = matchesViewModel.LoadDataAsync();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (sender as CollectionView);
        if (collectionView.SelectedItem is Match selectedMatch)
        {
            MatchDetailsPage matchDetailsPage = dystirService.AllMatchesDetailsPages.FirstOrDefault(x => x.MatchID == selectedMatch.MatchID);

            if (matchDetailsPage == null)
            {
                matchDetailsPage = new MatchDetailsPage(dystirService, liveStandingService, selectedMatch);
                dystirService.AllMatchesDetailsPages.Add(matchDetailsPage);
            }

            await App.Current.MainPage.Navigation.PushAsync(matchDetailsPage);

            //await Shell.Current.GoToAsync($"{nameof(ResultsPage)}/{nameof(MatchDetailsPage)}?matchID={selectedMatch.MatchID}");

            collectionView.SelectedItem = null;
        }
    }

    private async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if(matchesViewModel.IsLoading == false)
        {
            await matchesViewModel.DystirService.LoadDataAsync(true);
        }
    }
}
