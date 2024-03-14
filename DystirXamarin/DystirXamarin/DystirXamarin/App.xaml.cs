using System.Collections.Generic;
using DystirXamarin.Services;
using DystirXamarin.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DystirXamarin
{
    public partial class App : Application
    {
        public string DeviceToken = "";

        public IDictionary<string, object> NotificationData;

        //public event Action OnResumeApplication;
        //public void ResumeApplication() => OnResumeApplication?.Invoke();

        public App(IDictionary<string, object> notificationData = null)
        {
            InitializeComponent();
            SetupServices();
            AppAnalytics();
            MainPage = new NavigationPage(new LogInPage());

            NotificationData = notificationData;

            // Token event
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");

                DeviceToken = p.Token;
            };
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
