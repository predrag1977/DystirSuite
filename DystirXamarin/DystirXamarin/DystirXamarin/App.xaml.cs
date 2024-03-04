using DystirXamarin.Services;
using DystirXamarin.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DystirXamarin
{
    public partial class App : Application
    {
        //public event Action OnResumeApplication;
        //public void ResumeApplication() => OnResumeApplication?.Invoke();

        public App()
        {
            InitializeComponent();
            SetupServices();
            AppAnalytics();
            MainPage = new NavigationPage(new LogInPage());
        }

        private void SetupServices()
        {
            DependencyService.Register<DataLoaderService>();
            DependencyService.Register<EncryptorService>();
        }

        private void AppAnalytics()
        {
            AppCenter.Start("android=9ab87fe9-92da-45bb-aabf-37022479c19a;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            //OnResumeApplication();
        }
    }
}
