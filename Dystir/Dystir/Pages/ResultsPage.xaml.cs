﻿using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages;

public partial class ResultsPage : ContentPage
{
    private readonly ResultsViewModel _resultsViewModel;

    public ResultsPage(ResultsViewModel resultsViewModel)
    {
        _resultsViewModel = resultsViewModel;

        InitializeComponent();
        BindingContext = _resultsViewModel;

        _resultsViewModel.LoadDataAsync();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (sender as CollectionView);
        if (collectionView.SelectedItem is Match selectedMatch)
        {
            //await Shell.Current.Navigation.PushAsync(new MatchDetailsPage(selectedMatch));

            await Shell.Current.GoToAsync($"{nameof(ResultsPage)}/{nameof(MatchDetailsPage)}?matchID={selectedMatch.MatchID}");

            collectionView.SelectedItem = null;
        }
    }

    async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (_resultsViewModel.IsLoading == false)
        {
            await _resultsViewModel.DystirService.LoadDataAsync(true);
        }
    }
}
