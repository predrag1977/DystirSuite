using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages;

public partial class ResultsPage : ContentPage
{
    private readonly ResultsViewModel resultsViewModel;
    private readonly DystirService dystirService;
    private readonly LiveStandingService liveStandingService;

    public ResultsPage(DystirService dystirService, LiveStandingService liveStandingService, TimeService timeService)
    {
        resultsViewModel = new ResultsViewModel(dystirService, timeService);
        this.dystirService = dystirService;
        this.liveStandingService = liveStandingService;

        resultsViewModel.IsLoading = true;
        InitializeComponent();
        BindingContext = resultsViewModel;
    }

    protected override void OnAppearing()
    {
        _ = resultsViewModel.LoadDataAsync();
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

    async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (resultsViewModel.IsLoading == false)
        {
            await resultsViewModel.DystirService.LoadDataAsync(true);
        }
    }
}
