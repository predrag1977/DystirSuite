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
        //_matchDetailsViewModel.ClearMatchDetails();
        matchDetailsViewModel.SelectedMatch = matchDetailsViewModel.DystirService.AllMatches.FirstOrDefault(x => x.MatchID == MatchID);

        _ = LoadingData();
    }

    private async Task LoadingData()
    {
        await Task.Delay(200);
        await matchDetailsViewModel.LoadMatchDataAsync(MatchID);
    }

    async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if(!matchDetailsViewModel.IsLoadingSelectedMatch)
        {
            _ = matchDetailsViewModel.LoadMatchDataAsync(MatchID);
        }
        return;
    }

    void Button_Clicked(object sender, System.EventArgs e)
    {

    }
}
