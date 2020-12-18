//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Business.NodeFactories;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Service.Core;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Extensions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Extensions;
using RichTodd.QuiltSystem.Service.Micro.Extensions;
using RichTodd.QuiltSystem.Service.MicroEvent.Extensions;

namespace RichTodd.QuiltSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);

            _ = hostBuilder.ConfigureWebJobs((context, builder) =>
                {
                    // Hack required until the WebJobs team gets its act together.
                    //
                    var configProviders = (context.Configuration as ConfigurationRoot).Providers as List<IConfigurationProvider>;
                    if (configProviders.Count(c => c.GetType() == typeof(JsonConfigurationProvider) && (c as JsonConfigurationProvider).Source.Path == "appsettings.json") > 1)
                    {
                        _ = configProviders.Remove(configProviders.Last(c => c.GetType() == typeof(JsonConfigurationProvider) && (c as JsonConfigurationProvider).Source.Path == "appsettings.json"));
                    }
                    if (configProviders.Count(c => c.GetType() == typeof(EnvironmentVariablesConfigurationProvider)) > 1)
                    {
                        _ = configProviders.Remove(configProviders.Last(c => c.GetType() == typeof(EnvironmentVariablesConfigurationProvider)));
                    }

                    _ = builder.AddTimers();
                    _ = builder.AddAzureStorageCoreServices();
                    _ = builder.AddAzureStorage();
                });

            _ = hostBuilder.ConfigureLogging(logging =>
              {
                  // Add logging support to Azure BLOB's and the servers file system.  
                  // This logging is only active when App Service Logs are enabled through the Azure portal.
                  //
                  _ = logging.AddAzureWebAppDiagnostics();

                  // Enable logging data to be transmitted to Application Insights.
                  //
                  //_ = logging.AddApplicationInsights();
              });

            _ = hostBuilder.ConfigureServices(services =>
             {
                 // Configure the logging associated with AddAzureWebAppDiagnostics above.
                 //
                 _ = services.Configure<AzureFileLoggerOptions>(options => { });
                 _ = services.Configure<AzureBlobLoggerOptions>(options => { });
             });

            _ = hostBuilder.ConfigureServices((context, services) =>
                {
                    // Register application configuration.
                    //
                    _ = services
                        .Configure<ApplicationOptions>(context.Configuration.GetSection(ConfigurationSectionNames.Application))
                        .Configure<DatabaseOptions>(context.Configuration.GetSection(ConfigurationSectionNames.Application))
                        .AddSingleton<IConfigureOptions<ApplicationOptions>, ApplicationOptionsConfig>()
                        .AddSingleton<IConfigureOptions<DatabaseOptions>, DatabaseOptionsConfig>();

                    _ = services
                        .AddCoreServices(options => options.UseStaticUserId(BuiltInUsers.AdminUser))
                        .AddDatabaseServices()
                        .AddMicroServices(options => options.ExcludeUserManagement())
                        .AddMicroEventServices();

                    //_ = services.AddApplicationInsightsTelemetryWorkerService();
                });

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
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred.");
                    throw;
                }
            }

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
