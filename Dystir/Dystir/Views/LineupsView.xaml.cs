using CommunityToolkit.Maui.Views;
using Dystir.Models;
using Dystir.ViewModels;

namespace Dystir.Views;

public partial class LineupsView : ContentView
{
	public LineupsView(MatchDetailsViewModel matchDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = matchDetailsViewModel;
    }

    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;
        await Shell.Current.CurrentPage.ShowPopupAsync(new PlayerInfoPopupView(playerOfMatch));
    }
}
