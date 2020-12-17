//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;

namespace RichTodd.QuiltSystem.Business.Job
{
    public class TestRequestProcessJob : IJob
    {
        public const string QueueName = "job-testrequest";

        private IConfiguration Configuration { get; }
        private IOptionsMonitor<ApplicationOptions> Options { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private ILogger Logger { get; }
        private ParameterValuess Parameters { get; }

        private TestRequestProcessJob(
            IConfiguration configuration,
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger,
            ParameterValuess parameters)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public static TestRequestProcessJob Create(
            IConfiguration configuration,
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger,
            DateTime utcNow, string command, int count)
        {
            var parameters = new ParameterValuess()
            {
                UtcNow = utcNow,
                Command = command,
                Count = count
            };

            return new TestRequestProcessJob(configuration, options, logger, parameters);
        }

        public static TestRequestProcessJob Create(
            IConfiguration configuration,
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger,
            string message)
        {
            var parameters = ParameterValuess.Parse(message);

            return new TestRequestProcessJob(configuration, options, logger, parameters);
        }

        public async Task ExecuteAsync()
        {
            //Logger.Logusing var log = BeginFunction(nameof(TestRequestProcessJob), nameof(ExecuteAsync));
            try
            {
                if (Options.CurrentValue.DeferJobProcessing)
                {
                    await ProcessDeferred().ConfigureAwait(false);
                }
                else
                {
                    await Process().ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                //Logger.log.LogException(ex);
                throw;
            }
        }

        private async Task Process()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ProcessDeferred()
        {
            await new JobRequest(Configuration, QueueName, Parameters.ToString()).EnqueueAsync().ConfigureAwait(false);
        }

        #region Private Classes

        private class ParameterValuess
        {

            private const char Delimiter = '~';

            public string Command { get; set; }
            public int Count { get; set; }
            public DateTime UtcNow { get; set; }

            public static ParameterValuess Parse(string value)
            {
                var fields = value.Split(new char[] { Delimiter });

                return new ParameterValuess()
                {
                    UtcNow = DateTime.Parse(fields[0]),
                    Command = fields[1],
                    Count = int.Parse(fields[2])
                };
            }

            public override string ToString()
            {
                return string.Format("{0:O}{1}{2}{3}{4}", UtcNow, Delimiter, Command, Delimiter, Count);
            }

        }

        #endregion Private Classes
    }
}