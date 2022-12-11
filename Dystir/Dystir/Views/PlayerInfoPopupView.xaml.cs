using CommunityToolkit.Maui.Views;
using Dystir.Models;

namespace Dystir.Views;

public partial class PlayerInfoPopupView : Popup
{
    public PlayerInfoPopupView(PlayerOfMatch playerOfMatch)
    {
        InitializeComponent();
        BindingContext = playerOfMatch;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Close();
    }
}
