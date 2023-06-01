using DystirWeb.DystirDB;
using DystirWeb.Hubs;
using DystirWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<DystirHub>("/dystirhub");
app.MapFallbackToFile("index.html");

app.Run();

