//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Business.Job
{
    public class EmailRequestsProcessJob : IJob
    {
        private IOptionsMonitor<ApplicationOptions> Options { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private ILogger Logger { get; }
        private ICommunicationMicroService CommunicationMicroService { get; }

        private EmailRequestsProcessJob(
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger,
            ICommunicationMicroService emailService)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CommunicationMicroService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public static EmailRequestsProcessJob Create(
            IOptionsMonitor<ApplicationOptions> options,
            ILogger logger,
            ICommunicationMicroService emailService)
        {
            return new EmailRequestsProcessJob(options, logger, emailService);
        }

        public async Task ExecuteAsync()
        {
            //Logger.Logusing var log = BeginFunction(nameof(EmailRequestsProcessJob), nameof(ExecuteAsync));
            try
            {
                if (Options.CurrentValue.DeferJobProcessing)
                {
                    throw new NotSupportedException();
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
            await CommunicationMicroService.TransmitPendingEmailsAsync().ConfigureAwait(false);
        }
    }
}
