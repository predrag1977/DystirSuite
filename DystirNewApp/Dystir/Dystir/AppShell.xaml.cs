using Dystir.Pages;
using Dystir.Services;
using Xamarin.Forms;

namespace Dystir
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MatchDetailPage), typeof(MatchDetailPage));
            Routing.RegisterRoute(nameof(NewsPage), typeof(NewsPage));
            LanguageService languageService = DependencyService.Get<LanguageService>();
            languageService.OnLanguageChanged += LanguageServiceOnLanguageChanged;
        }

        private void LanguageServiceOnLanguageChanged()
        {
            TabBar.Items[0].Title = Dystir.Resources.Localization.Resources.Matches;
            TabBar.Items[1].Title = Dystir.Resources.Localization.Resources.Results;
            TabBar.Items[2].Title = Dystir.Resources.Localization.Resources.Fixtures;
            TabBar.Items[3].Title = Dystir.Resources.Localization.Resources.Standings;
            TabBar.Items[4].Title = Dystir.Resources.Localization.Resources.StatisticsTab;
        }
    }
}

