using Dystir.Models;
using Dystir.Pages;
using Dystir.Services;
using Dystir.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui;

namespace Dystir;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
            fonts.AddFont("Nunito-Semibold.ttf", "NunitoSemibold");
        }).UseMauiCommunityToolkit();
        builder.Services.AddSingleton<DataLoadService>();
        builder.Services.AddSingleton<TimeService>();
        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddSingleton<MatchesViewModel>();
        builder.Services.AddSingleton<ResultsViewModel>();
        builder.Services.AddSingleton<FixturesViewModel>();
        builder.Services.AddSingleton<DystirService>();
        builder.Services.AddSingleton<LiveStandingService>();
        builder.Services.AddSingleton<MatchesPage>();
        builder.Services.AddSingleton<ResultsPage>();
        builder.Services.AddSingleton<FixturesPage>();
        builder.Services.AddSingleton<MatchDetailsPage>();
        return builder.Build();
    }
}