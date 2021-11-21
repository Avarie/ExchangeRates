using System;
using System.Globalization;
using CurrencyAggregator.Models;
using CurrencyAggregator.Repository;
using CurrencyAggregator.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CurrencyAggregator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSession();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddViewLocalization();

            services.AddControllersWithViews();
            services.RegisterImplementations();


            var configurationSection = Configuration.GetSection("ConnectionStrings:CurrencyData");
            services.AddDbContext<CurrencyDbContext>(options => options.UseSqlServer(configurationSection.Value));

            services.AddHangfire(configuration =>
            {
                var options = new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = false,
                    QueuePollInterval = TimeSpan.FromMinutes(5)
                };

                configuration.UseSqlServerStorage(configurationSection.Value, options);
            });

            var enUSCulture = "en-US";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("uk")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: enUSCulture, uiCulture: enUSCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context => 
                {
                    // My custom request culture logic
                    return new ProviderCultureResult("uk");
                }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Data/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // ip detection
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });


            app.UseSession();
            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name: "default",
            //         template: "{controller=Home}/{action=Index}/{id?}");
            // });

            app.UseDeveloperExceptionPage();

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("uk")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("uk"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Data}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] {new LocalRequestsOnlyAuthorizationFilter()}
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute {Attempts = 5});
            HangfireJobScheduler.ScheduleJobs();
        }
    }
}
