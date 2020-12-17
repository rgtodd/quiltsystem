//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Business.NodeFactories;
using RichTodd.QuiltSystem.Design.Component.Standard;
using RichTodd.QuiltSystem.Service.Ajax.Extensions;
using RichTodd.QuiltSystem.Service.Core;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Extensions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Extensions;
using RichTodd.QuiltSystem.Service.Micro.Extensions;
using RichTodd.QuiltSystem.Service.MicroEvent.Extensions;
using RichTodd.QuiltSystem.Service.User.Extensions;

namespace RichTodd.QuiltSystem.Test
{
    public static class Setup
    {
        public static IConfiguration LoadConfiguration(TestContext testContext)
        {
            return new ConfigurationBuilder()
                .SetBasePath(testContext.DeploymentDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("*SECRET*")
                .AddEnvironmentVariables()
                .Build();
        }

        public static ServiceProvider ConfigureServices(IConfiguration configuration, bool mockEvents)
        {
            IServiceCollection services = new ServiceCollection();

            _ = services
                .AddSingleton(configuration)
                .Configure<ApplicationOptions>(configuration.GetSection(ConfigurationSectionNames.Application))
                .Configure<DatabaseOptions>(configuration.GetSection(ConfigurationSectionNames.Application))
                .AddSingleton<IConfigureOptions<ApplicationOptions>, ApplicationOptionsConfig>()
                .AddSingleton<IConfigureOptions<DatabaseOptions>, DatabaseOptionsConfig>()
                .AddLogging(builder => builder.AddConsole().AddDebug().AddConfiguration(configuration.GetSection("Logging")))

                .AddCoreServices(options => options.UseStaticUserId(BuiltInUsers.AdminUser))

                .AddDbContext<IdentityDbContext>((serviceProvider, options) =>
                    {
                        var contextFactory = serviceProvider.GetService<IQuiltContextFactory>();
                        _ = options.UseSqlServer(contextFactory.ConnectionString);
                    })
                .AddIdentityCore<IdentityUser>(options =>
                    {
                        options.Password.RequireUppercase = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequiredUniqueChars = 0;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDbContext>().Services

                .AddDatabaseServices()
                .AddUserServices()
                .AddMicroServices()
                .AddAjaxServices();

            _ = mockEvents
                ? services.AddMockMicroEventServices()
                : services.AddMicroEventServices();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public static void ConfigureFactories(IServiceProvider services)
        {
            var quiltContextFactory = services.GetRequiredService<IQuiltContextFactory>();

            var nodeFactory = new CompositeNodeFactory();
            nodeFactory.Add(new DatabaseNodeFactory(quiltContextFactory));
            nodeFactory.Add(new BuiltInQuiltLayoutNodeFactory());

            BlockComponent.Configure(nodeFactory);
            LayoutComponent.Configure(nodeFactory);
        }
    }
}
