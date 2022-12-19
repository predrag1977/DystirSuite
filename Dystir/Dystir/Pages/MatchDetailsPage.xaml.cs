using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Dystir.Resources;
using Dystir.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace Dystir.Pages;

[QueryProperty(nameof(MatchID), "matchID")]
public partial class MatchDetailsPage : ContentPage
{
    public int MatchID { get; set; } = 0;

    private MatchDetailsViewModel matchDetailsViewModel;

    public MatchDetailsPage(MatchDetailsViewModel matchDetailsViewModel)
    {
        this.matchDetailsViewModel = matchDetailsViewModel;
        InitializeComponent();
        BindingContext = matchDetailsViewModel;
    }

    protected override void OnAppearing()
    {
        matchDetailsViewModel.ClearMatchDetails();
        matchDetailsViewModel.SelectedMatch = matchDetailsViewModel.DystirService.AllMatches.FirstOrDefault(x => x.MatchID == MatchID);
        _ = matchDetailsViewModel.PopulateMatchDetailsTabs(matchDetailsViewModel.SelectedMatch);
        _ = matchDetailsViewModel.SetDetailsTabSelected(0);
    }

    protected override void OnDisappearing()
    {
        MatchDetailsTabsCarouselView.Position = 0;
    }

    async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if(!matchDetailsViewModel.IsLoadingSelectedMatch)
        {
            //_ = matchDetailsViewModel.LoadMatchDataAsync(MatchID);
        }
        return;
    }

    void Button_Clicked(object sender, System.EventArgs e)
    {

    }

    void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (sender as CollectionView).SelectedItem;
        var position = matchDetailsViewModel.MatchDetailsTabs.IndexOf(selectedItem as MatchDetailsTab);
        if(position >= 0)
        {
            MatchDetailsTabsCarouselView.Position = position;
        }
    }

    void MatchDetailsTabsCarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
    {

    }

    void MatchDetailsTabsCarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (MatchDetailsTabsCarouselView.Position >= 0)
        {
            _ = matchDetailsViewModel.SetDetailsTabSelected(MatchDetailsTabsCarouselView.Position);
        }
    }

    void CollectionView_Loaded(System.Object sender, System.EventArgs e)
    {
        var t = matchDetailsViewModel.MatchDetailsTabs.Count;
        (sender as CollectionView).ItemsLayout = new GridItemsLayout(matchDetailsViewModel.MatchDetailsTabs.Count, ItemsLayoutOrientation.Vertical);
    }
}
