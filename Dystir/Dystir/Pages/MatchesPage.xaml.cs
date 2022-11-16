using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages;

public partial class MatchesPage : ContentPage
{
    private readonly MatchesViewModel _matchesViewModel;

    public MatchesPage(MatchesViewModel matchesViewModel)
    {
        _matchesViewModel = matchesViewModel;

        InitializeComponent();
        BindingContext = matchesViewModel;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (sender as CollectionView);
        if (collectionView.SelectedItem is Match selectedMatch)
        {
            selectedMatch.MatchDetails = null;
            selectedMatch.IsLoading = true;

            await Shell.Current.GoToAsync($"{nameof(MatchesPage)}/{nameof(MatchDetailsPage)}?matchID={selectedMatch.MatchID}");

            collectionView.SelectedItem = null;
        }
    }

    private async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if(_matchesViewModel.IsLoading == false)
        {
            await _matchesViewModel.DystirService.LoadDataAsync(true);
        }
    }
}
