using System.Collections.ObjectModel;
using Dystir.Models;
using Dystir.ViewModels;
using CommunityToolkit.Maui.Views;

namespace Dystir.Views;

public partial class MatchesPopupView : Popup
{
    public MatchesPopupView(MatchDetailsViewModel matchDetailsViewModel)
    {
        InitializeComponent();
        BindingContext = matchDetailsViewModel;
    }
}
