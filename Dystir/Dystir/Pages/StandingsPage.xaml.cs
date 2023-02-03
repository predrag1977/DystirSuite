using Dystir.Services;
using Dystir.ViewModels;

namespace Dystir.Pages;

public partial class StandingsPage : ContentPage
{
    private readonly StandingsViewModel standingsViewModel;

    public StandingsPage(DystirService dystirService, LiveStandingService liveStandingService)
	{
        standingsViewModel = new StandingsViewModel(dystirService, liveStandingService);

        InitializeComponent();
		BindingContext = standingsViewModel;
	}

    protected override void OnAppearing()
    {
        _ = standingsViewModel.LoadDataAsync();
    }

    async void RefreshButton_Clicked(object sender, EventArgs e)
    {
        if (standingsViewModel.IsLoading == false)
        {
            await standingsViewModel.DystirService.LoadDataAsync(true);
        }
    }
}
