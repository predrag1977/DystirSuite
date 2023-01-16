using Dystir.Models;
using Dystir.ViewModels;

namespace Dystir.Views;

public partial class SummaryView : ContentView
{
    public SummaryView()
	{
        InitializeComponent();
    }

    void ContentView_BindingContextChanged(System.Object sender, System.EventArgs e)
    {
        var t = BindingContext;
    }
}
