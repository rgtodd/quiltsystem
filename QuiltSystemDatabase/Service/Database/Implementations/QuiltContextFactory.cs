//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Database.Implementations
{
    public class QuiltContextFactory : IQuiltContextFactory
    {
        private IConfiguration Configuration { get; }
        private ILoggerFactory LoggerFactory { get; }
        private IOptionsMonitor<DatabaseOptions> OptionsMonitor { get; }
        private DatabaseOptions OptionsInstance { get; }

        public QuiltContextFactory(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<DatabaseOptions> optionsMonitor)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        private QuiltContextFactory(
            IConfiguration configuration,
            DatabaseOptions optionsInstance)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            OptionsInstance = optionsInstance ?? throw new ArgumentNullException(nameof(optionsInstance));
        }

        public static QuiltContextFactory Create(
            IConfiguration configuration,
            DatabaseOptions optionsInstance)
        {
            return new QuiltContextFactory(configuration, optionsInstance);
        }

        private DatabaseOptions Options
        {
            get
            {
                return OptionsMonitor != null
                    ? OptionsMonitor.CurrentValue
                    : OptionsInstance;
            }
        }

        public string ConnectionString
        {
            get
            {
                return Options.UseLocalDatabase
                    ? Options.LocalConnectionString
                    : Configuration.GetConnectionString("QuiltAGoGo"); // HACK
            }
        }

        public QuiltContext Create()
        {
            var options = new DbContextOptionsBuilder<QuiltContext>()
                .UseLoggerFactory(Options.DatabaseLoggingEnabled ? LoggerFactory : null)
                .UseLazyLoadingProxies()
                .UseSqlServer(ConnectionString)
                .Options;

            var ctx = new QuiltContext(options);

            return ctx;
        }

        public SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(ConnectionString);

            return conn;
        }
    }
}
