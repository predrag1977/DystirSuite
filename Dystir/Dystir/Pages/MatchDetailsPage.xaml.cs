using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Dystir.Resources;
using Dystir.Views;
using CommunityToolkit.Maui.Views;

namespace Dystir.Pages;

[QueryProperty(nameof(MatchID), "matchID")]
public partial class MatchDetailsPage : ContentPage
{
    public int MatchID
    {
        set { GetSelectedMatch(value); }
    }

    private void GetSelectedMatch(int matchID)
    {
        var selectedMatch = _matchDetailsViewModel.DystirService.AllMatches.FirstOrDefault(x => x.MatchID == matchID);
        _matchDetailsViewModel.SelectedMatch = selectedMatch;
    }

    private readonly MatchDetailsViewModel _matchDetailsViewModel;

    public MatchDetailsPage(MatchDetailsViewModel matchDetailsViewModel)
    {
        _matchDetailsViewModel = matchDetailsViewModel;

        InitializeComponent();
        BindingContext = _matchDetailsViewModel;
    }

    protected override void OnAppearing()
    {
        _ = LoadMatchDetailsData(_matchDetailsViewModel.SelectedMatch);
    }

    async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        await _matchDetailsViewModel.DystirService.RefreshSelectedMatchAsync(_matchDetailsViewModel.SelectedMatch);
    }

    async Task LoadMatchDetailsData(Match selectedMatch)
    {
        await Task.Delay(100);
        await _matchDetailsViewModel.DystirService.LoadMatchDetailsAsync(_matchDetailsViewModel.SelectedMatch);
        _matchDetailsViewModel.SelectedMatch.IsLoading = false;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {

    }
}
