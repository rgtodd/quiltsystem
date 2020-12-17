//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Database;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Implementations;

namespace RichTodd.QuiltSystem.Test
{
    [TestClass]
    internal static class AssemblyTest
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var createNewDatabase = CreateNewDatabaseEnabled(context);
            if (createNewDatabase)
            {
                var configuration = Setup.LoadConfiguration(context);

                CreateNewDatabase(configuration);

                var serviceProvider = Setup.ConfigureServices(configuration);

                CreateEntities(serviceProvider);
            }
        }

        private static void CreateNewDatabase(IConfiguration configuration)
        {
            var options = new DatabaseOptions();
            configuration.GetSection(ConfigurationSectionNames.Application).Bind(options);

            var databaseName = string.Format("Quilt-{0:yyyy-MM-dd-HH-mm-ss}", DateTime.Now);
            options.LocalDatabaseName = databaseName;

            var fileName = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\SQL\\{options.LocalDatabaseName}.mdf";
            options.LocalConnectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={options.LocalDatabaseName};AttachDbFilename={fileName};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            var quiltContextFactory = QuiltContextFactory.Create(configuration, options);

            ModelSetup.CreateDatabase(quiltContextFactory);
        }

        private static void CreateEntities(ServiceProvider serviceProvider)
        {
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            IQuiltContextFactory quiltContextFactory = serviceProvider.GetService<IQuiltContextFactory>();
            UserManager<IdentityUser> userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            var task = Task.Run(async () =>
            {
                await ModelSetup.CreateStandardEntitiesAsync(
                    configuration,
                    quiltContextFactory,
                    userManager,
                    roleManager);

                await ModelTestSetup.CreateTestEntitiesAsync(
                    configuration,
                    quiltContextFactory,
                    DateTime.UtcNow,
                    DateTime.Now);
            });
            task.Wait();
        }

        private static bool CreateNewDatabaseEnabled(TestContext context)
        {
            return bool.TryParse(context.Properties["createNewDatabase"].ToString(), out var result) && result;
        }
    }
}