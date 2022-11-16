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
}
