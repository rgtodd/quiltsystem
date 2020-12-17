//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    internal class FundingEventMicroService : MicroEventMicroService, IFundingEventMicroService
    {
        public FundingEventMicroService(
            IApplicationLocale locale,
            ILogger<FundingEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        {
        }

        public async Task HandleFundableEventAsync(MFunding_FundableEvent eventData)
        {
            using var log = BeginFunction(nameof(FundingEventMicroService), nameof(HandleFundableEventAsync), eventData);
            try
            {
                using var ctx = CreateQuiltContext();

                if (eventData.FundsReceived != 0)
                {
                    if (TryParseOrderId.FromFundableReference(eventData.FundableReference, out var orderId))
                    {
                        _ = await OrderMicroService.SetFundsReceivedAsync(orderId, eventData.FundsReceived, eventData.UnitOfWork).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task HandleFunderEventAsync(MFunding_FunderEvent eventData)
        {
            using var log = BeginFunction(nameof(FundingEventMicroService), nameof(HandleFunderEventAsync), eventData);
            try
            {
                using var ctx = CreateQuiltContext();

                await Task.CompletedTask.ConfigureAwait(false);

                if (eventData.FundsAvailable != 0)
                {
                    if (TryParseUserId.FromFunderReference(eventData.FunderReference, out var funderUserId))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }
    }
}
