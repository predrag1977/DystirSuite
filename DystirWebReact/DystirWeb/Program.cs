using DystirWeb.DystirDB;
using DystirWeb.Hubs;
using DystirWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddDbContextPool<DystirDBContext>(options =>
            options.UseSqlServer("data source=162.250.122.185;initial catalog=dystirDB;persist security info=True;user id=predragmarkovic;password=Nadja11012012;TrustServerCertificate=True;"));

services.AddControllersWithViews();
services.AddSignalR();
services.AddSingleton<DystirService>();
services.AddSingleton<StandingService>();
services.AddSingleton<StatisticCompetitionsService>();
services.AddScoped<MatchDetailsService>();
services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapHub<DystirHub>("/dystirhub");
app.MapFallbackToFile("index.html");

app.Run();

