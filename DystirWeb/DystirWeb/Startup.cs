using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DystirWeb.Services;
using Microsoft.EntityFrameworkCore;
using DystirWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Blazored.Localisation;

namespace DystirWeb
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
            //services.AddDbContext<DystirDBContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DystirDatabase")));
            services.AddDbContextPool<DystirDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DystirDatabase")));

            services.AddMvc(o =>
            {
                o.Filters.Add(new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.None });
                o.EnableEndpointRouting = false;
            });
            services.AddBlazoredLocalisation();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR();
            services.AddSingleton<TimeService>();
            services.AddSingleton<StandingService>();
            services.AddSingleton<StatisticCompetitionsService>();
            services.AddTransient<DystirService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<DystirHub>("/dystirhub");
            });
        }
    }
}
