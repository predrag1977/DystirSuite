using Dystir.Pages;

namespace Dystir;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MatchesPage), typeof(MatchesPage));
        Routing.RegisterRoute($"{nameof(MatchesPage)}/{nameof(MatchDetailsPage)}", typeof(MatchDetailsPage));
        Routing.RegisterRoute($"{nameof(ResultsPage)}/{nameof(MatchDetailsPage)}", typeof(MatchDetailsPage));
        Routing.RegisterRoute($"{nameof(FixturesPage)}/{nameof(MatchDetailsPage)}", typeof(MatchDetailsPage));
    }
}
