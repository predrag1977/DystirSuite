using Xamarin.Forms;
using Dystir.Services;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Dystir
{
    public partial class App : Application
    {
        private readonly DystirService _dystirService;

        public App ()
        {
            InitializeComponent();
            ServiceRegistrations();
            AppAnalytics();

            MainPage = new AppShell();

            _dystirService = DependencyService.Get<DystirService>();
            _ = _dystirService.LoadDataAsync(true);

            var languageService = DependencyService.Get<LanguageService>();
            languageService.SetLanguage();

            var timeService = DependencyService.Get<TimeService>();
            timeService.StartSponsorsTime();
        }

        private void ServiceRegistrations()
        {
            DependencyService.Register<AnalyticsService>();
            DependencyService.Register<LanguageService>();
            DependencyService.Register<TimeService>();
            DependencyService.Register<LiveStandingService>();
            DependencyService.Register<DataLoadService>();
            DependencyService.Register<DystirService>();
        }

        private void AppAnalytics()
        {
            var analyticsService = DependencyService.Get<AnalyticsService>();
            analyticsService.StartAnalytics();
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

