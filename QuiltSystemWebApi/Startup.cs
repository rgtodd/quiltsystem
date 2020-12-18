//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Extensions;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Extensions;
using RichTodd.QuiltSystem.Service.Micro.Extensions;
using RichTodd.QuiltSystem.Service.MicroEvent.Extensions;

namespace RichTodd.QuiltSystem.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Define services used by depedency injection.
        //
        public void ConfigureServices(IServiceCollection services)
        {
            // Register application configuration.
            //
            _ = services
                .Configure<ApplicationOptions>(Configuration.GetSection(ConfigurationSectionNames.Application))
                .Configure<DatabaseOptions>(Configuration.GetSection(ConfigurationSectionNames.Application))
                .AddSingleton<IConfigureOptions<ApplicationOptions>, ApplicationOptionsConfig>()
                .AddSingleton<IConfigureOptions<DatabaseOptions>, DatabaseOptionsConfig>();

            // Register ASP.NET Core MVC services.
            //
            _ = services.AddControllersWithViews();
            _ = services.AddRazorPages();

            // Register application services.
            //
            _ = services
                .AddCoreWebServices(options => options.UseHttpContext())
                .AddDatabaseServices()
                .AddMicroServices()
                .AddMicroEventServices();

            // Register Application Insights services.
            //
            //_ = services
            //    .AddApplicationInsightsTelemetry()
            //    .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, options) => module.EnableSqlCommandTextInstrumentation = true);
        }

        // Configure the application request pipeline.
        //
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
                //_ = app.UseDatabaseErrorPage();
            }
            else
            {
                _ = app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }
            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();

            _ = app.UseRouting();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                _ = endpoints.MapRazorPages();
            });
        }
    }
}
