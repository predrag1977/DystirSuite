using Xamarin.Forms;
using Dystir.Services;

namespace Dystir
{
    public partial class App : Application
    {
        private readonly DystirService _dystirService;

        public App ()
        {
            InitializeComponent();
            ServiceRegistrations();

            MainPage = new AppShell();

            _dystirService = DependencyService.Get<DystirService>();
            _ = _dystirService.LoadDataAsync(true);

            var languageService = DependencyService.Get<LanguageService>();
            languageService.SetLanguage();

        }

        private void ServiceRegistrations()
        {
            DependencyService.Register<LanguageService>();
            DependencyService.Register<TimeService>();
            DependencyService.Register<LiveStandingService>();
            DependencyService.Register<DataLoadService>();
            DependencyService.Register<DystirService>();
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

