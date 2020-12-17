//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;

using RichTodd.QuiltSystem.Business.NodeFactories;
using RichTodd.QuiltSystem.Business.Report;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            _ = hostBuilder.ConfigureLogging(logging =>
                {
                    // Add logging support to Azure BLOB's and the servers file system.  
                    // This logging is only active when App Service Logs are enabled through the Azure portal.
                    //
                    _ = logging.AddAzureWebAppDiagnostics();

                    // Enable logging data to be transmitted to Application Insights.
                    //
                    _ = logging.AddApplicationInsights();
                });
            _ = hostBuilder.ConfigureServices(services =>
                {
                    // Configure the logging associated with AddAzureWebAppDiagnostics above.
                    //
                    _ = services.Configure<AzureFileLoggerOptions>(options => { });
                    _ = services.Configure<AzureBlobLoggerOptions>(options => { });
                });
            _ = hostBuilder.ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());

            var host = hostBuilder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var quiltContextFactory = services.GetRequiredService<IQuiltContextFactory>();

                    var nodeFactory = new CompositeNodeFactory();
                    nodeFactory.Add(new DatabaseNodeFactory(quiltContextFactory));
                    nodeFactory.Add(new BuiltInQuiltLayoutNodeFactory());

                    BlockComponent.Configure(nodeFactory);
                    LayoutComponent.Configure(nodeFactory);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(ex, "An error occurred.");
                    throw;
                }
            }

            GlobalFontSettings.FontResolver = new ReportFontResolver();
            GlobalFontSettings.DefaultFontEncoding = PdfFontEncoding.WinAnsi;

            host.Run();
        }
    }
}
