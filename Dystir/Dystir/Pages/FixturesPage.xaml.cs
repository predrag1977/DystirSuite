using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages;

public partial class FixturesPage : ContentPage
{
    private readonly FixturesViewModel _fixturesViewModel;

    public FixturesPage(FixturesViewModel fixturesViewModel)
    {
        _fixturesViewModel = fixturesViewModel;

        InitializeComponent();
        BindingContext = fixturesViewModel;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (sender as CollectionView);
        if (collectionView.SelectedItem is Match selectedMatch)
        {
            selectedMatch.MatchDetails = null;
            selectedMatch.IsLoading = true;

            await Shell.Current.GoToAsync($"{nameof(FixturesPage)}/{nameof(MatchDetailsPage)}?matchID={selectedMatch.MatchID}");

            collectionView.SelectedItem = null;
        }
    }

    async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (_fixturesViewModel.IsLoading == false)
        {
            await _fixturesViewModel.DystirService.LoadDataAsync(true);
        }
    }
}

