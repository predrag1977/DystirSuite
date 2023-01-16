using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;
using Dystir.Views;

namespace Dystir.Pages;

public partial class MatchesPage : ContentPage
{
    private readonly MatchesViewModel matchesViewModel;

    public MatchesPage(DystirService dystirService, TimeService timeService)
    {
        this.matchesViewModel = new MatchesViewModel(dystirService, timeService);

        InitializeComponent();
        BindingContext = this.matchesViewModel;

        this.matchesViewModel.LoadDataAsync();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (sender as CollectionView);
        if (collectionView.SelectedItem is Match selectedMatch)
        {
            //await Shell.Current.Navigation.PushAsync(new MatchDetailsPage(selectedMatch, matchesViewModel.DystirService, liveStandingService));

            await Shell.Current.GoToAsync($"{nameof(MatchesPage)}/{nameof(MatchDetailsPage)}");

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
