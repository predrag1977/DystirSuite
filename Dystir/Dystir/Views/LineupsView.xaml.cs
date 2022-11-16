using CommunityToolkit.Maui.Views;
using Dystir.Models;

namespace Dystir.Views;

public partial class LineupsView : ContentView
{
	public LineupsView()
	{
		InitializeComponent();
	}

    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;
        await Shell.Current.CurrentPage.ShowPopupAsync(new PlayerInfoPopupView(playerOfMatch));
    }
}
