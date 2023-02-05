using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Dystir.Resources;
using Dystir.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace Dystir.Pages;

//[QueryProperty(nameof(MatchID), "matchID")]
public partial class MatchDetailsPage : ContentPage
{
    public int MatchID = 0;
    private readonly MatchDetails _matchDetails;

    public MatchDetailsPage(DystirService dystirService, LiveStandingService liveStandingService, Match match)
    {
        MatchID = match.MatchID;
        _matchDetails = new MatchDetails(dystirService, liveStandingService);
        _matchDetails.Match = match;

        InitializeComponent();
        BindingContext = _matchDetails;

        _matchDetails.IsLoadingSelectedMatch = true;
        _ = _matchDetails.Startup();
    }

    async void BackButton_Clicked(object sender, EventArgs e)
    {
        await App.Current.MainPage.Navigation.PopAsync();
    }

    void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if(!_matchDetails.IsLoadingSelectedMatch)
        {
            _matchDetails.IsDataLoaded = false;
            _ = _matchDetails.Startup();
        }
        return;
    }

    async void Button_Clicked(object sender, System.EventArgs e)
    {
        await App.Current.MainPage.ShowPopupAsync(new MatchesPopupView(_matchDetails));
    }

    void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (sender as CollectionView).SelectedItem;
        var position = _matchDetails.MatchDetailsTabs.IndexOf(selectedItem as MatchDetailsTab);
        MatchDetailsTabsCarouselView.Position = position;
        _ = ChangeMatchDetailsView(position);
    }

    void MatchDetailsTabsCarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
    {
        //_ = ChangeMatchDetailsView(MatchDetailsTabsCarouselView.Position);
    }

    private async Task ChangeMatchDetailsView(int position)
    {
        _matchDetails.SetDetailsTabSelected(position);
        _matchDetails.IsLoadingSelectedMatch = _matchDetails.MatchDetailsTabsViews[position].BindingContext == null;
        await Task.Delay(200);
        _matchDetails.MatchDetailsTabsViews[position].BindingContext = _matchDetails;
        _matchDetails.IsLoadingSelectedMatch = false;
    }

    void CollectionView_Loaded(System.Object sender, System.EventArgs e)
    {
        (sender as CollectionView).ItemsLayout = new GridItemsLayout(_matchDetails?.MatchDetailsTabs?.Count ?? 1, ItemsLayoutOrientation.Vertical);
    }
}
