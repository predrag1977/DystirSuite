using Dystir.ViewModels;

namespace Dystir.Views;

public partial class CommentaryView : ContentView
{
	public CommentaryView(MatchDetailsViewModel matchDetailsViewModel)
	{
        InitializeComponent();
        BindingContext = matchDetailsViewModel;
    }
}
