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
            //var matchDetailsPage = _fixturesViewModel.AllMatchDetailsPage.FirstOrDefault(x => x.MatchID == selectedMatch.MatchID);
            //if (matchDetailsPage == null)
            //{
            //    matchDetailsPage = new MatchDetailsPage(new MatchDetailsViewModel(_fixturesViewModel.DystirService)
            //    {
            //        SelectedMatch = selectedMatch
            //    });
            //    _fixturesViewModel.AllMatchDetailsPage.Add(matchDetailsPage);
            //}

            //await Shell.Current.Navigation.PushAsync(matchDetailsPage);

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

