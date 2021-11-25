using DystirWeb.Server.DystirDB;
using DystirWeb.Server.Hubs;
using DystirWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DystirWeb.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DystirDBContext>(options =>
            options.UseSqlServer("data source=162.250.122.185;initial catalog=dystirDB;persist security info=True;user id=predragmarkovic;password=Nadja11012012;"));

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddSingleton<DystirService>();
            services.AddSingleton<StandingService>();
            services.AddSingleton<StatisticCompetitionsService>();
            services.AddScoped<MatchDetailsService>();
            services.AddScoped<AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<DystirHub>("/dystirhub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
