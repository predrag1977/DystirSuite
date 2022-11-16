using Dystir.Services;
using Dystir.ViewModels;

namespace Dystir;

public partial class App : Application
{
    private readonly DystirService _dystirService;

    public App(DystirService dystirService)
    {
        InitializeComponent();

        ServiceRegistrations();

        _dystirService = dystirService;
        //AppAnalytics();
        SetLanguage();

        MainPage = new AppShell();

        _ = _dystirService.LoadDataAsync(true);

        // CheckLatestVersion();
        //AppAnalytics();
    }

    private void ServiceRegistrations()
    {
        DependencyService.Register<LanguageService>();
        DependencyService.Register<TimeService>();
    }

    private void SetLanguage()
    {
        string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        //toggle lang
        if (lang == "fo")
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fo-FO");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fo-FO");
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }
    }

}

