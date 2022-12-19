using Dystir.ViewModels;

namespace Dystir.Views;

public partial class StatisticView : ContentView
{
	public StatisticView(MatchDetailsViewModel matchDetailsViewModel)
	{
        InitializeComponent();
        BindingContext = matchDetailsViewModel;
    }
}
