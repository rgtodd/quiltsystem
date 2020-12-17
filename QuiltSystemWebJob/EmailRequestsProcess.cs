//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Business.Job;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem
{
    public class EmailRequestsProcess
    {
        private static readonly Semaphore m_semaphore = new Semaphore(1, 1);

        private IOptionsMonitor<ApplicationOptions> Options { get; }
        private ILogger Logger { get; }
        private ICommunicationMicroService CommunicationMicroService { get; }

        public EmailRequestsProcess(
            IOptionsMonitor<ApplicationOptions> options,
            ILogger<EmailRequestsProcess> logger,
            ICommunicationMicroService communicationMicroService)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
        }

        public async Task RunAsync(
#pragma warning disable IDE0060 // Remove unused parameter
            [TimerTrigger(Application.EmailTimerInterval)] TimerInfo myTimer,
            ILogger logger)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            try
            {
                //Logger.Logusing var log = BeginFunction(nameof(EmailRequestsProcess), nameof(RunAsync));

                if (m_semaphore.WaitOne())
                {
                    try
                    {
                        var job = EmailRequestsProcessJob.Create(Options, Logger, CommunicationMicroService);
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
