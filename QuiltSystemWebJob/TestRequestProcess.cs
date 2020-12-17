//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Business.Job;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;

namespace RichTodd.QuiltSystem
{
    public class TestRequestProcess
    {
        private static readonly Semaphore m_semaphore = new Semaphore(1, 1);

        private IConfiguration Configuration { get; }
        private IOptionsMonitor<ApplicationOptions> Options { get; }
        private ILogger Logger { get; }

        public TestRequestProcess(
            IConfiguration configuration,
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(TestRequestProcess))]
        public async Task RunAsync([QueueTrigger(TestRequestProcessJob.QueueName, Connection = Application.StorageConnectionName)] string message)
        {
            //var f = BeginFunction(nameof(TestRequestProcess), nameof(RunAsync), message);
            try
            {

                if (m_semaphore.WaitOne())
                {
                    try
                    {
                        var job = TestRequestProcessJob.Create(Configuration, Options, Logger, message);
                        await job.ExecuteAsync();
                    }
                    finally
                    {
                        _ = m_semaphore.Release();
                    }
                }
            }
            catch (Exception)
            {
                //Logger.log.LogException(ex);
                throw;
            }
        }
    }
}
