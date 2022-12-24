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
    private readonly LiveStandingService liveStandingService;
    private readonly DystirService dystirService;

    public MatchDetailsPage(DystirService dystirService, LiveStandingService liveStandingService)
    {
        this.liveStandingService = liveStandingService;
        this.dystirService = dystirService;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        matchDetailsViewModel = dystirService.AllMatchesDetailViewModels.FirstOrDefault(x => x.SelectedMatch.MatchID == MatchID);
        if(matchDetailsViewModel == null)
        {
            matchDetailsViewModel = new MatchDetailsViewModel(MatchID, dystirService, liveStandingService);
        }
        BindingContext = matchDetailsViewModel;
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

    async void Button_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.CurrentPage.ShowPopupAsync(new MatchesPopupView(matchDetailsViewModel));
    }

    void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (sender as CollectionView).SelectedItem;
        var position = matchDetailsViewModel.MatchDetailsTabs.IndexOf(selectedItem as MatchDetailsTab);
        MatchDetailsTabsCarouselView.Position = position;
        _ = ChangeMatchDetailsView(position);
    }

    void MatchDetailsTabsCarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
    {
        _ = ChangeMatchDetailsView(MatchDetailsTabsCarouselView.Position);
    }

    private async Task ChangeMatchDetailsView(int position)
    {
        matchDetailsViewModel.SetDetailsTabSelected(position);
        matchDetailsViewModel.IsLoadingSelectedMatch = true;
        await Task.Delay(500);
        matchDetailsViewModel.MatchDetailsTabsViews[position].BindingContext = matchDetailsViewModel;

        matchDetailsViewModel.IsLoadingSelectedMatch = false;
    }

    void CollectionView_Loaded(System.Object sender, System.EventArgs e)
    {
        (sender as CollectionView).ItemsLayout = new GridItemsLayout(matchDetailsViewModel.MatchDetailsTabs.Count, ItemsLayoutOrientation.Vertical);
    }
}
