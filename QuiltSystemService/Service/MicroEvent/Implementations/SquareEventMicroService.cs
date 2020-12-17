//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    internal class SquareEventMicroService : MicroEventMicroService, ISquareEventMicroService
    {
        public SquareEventMicroService(
            IApplicationLocale locale,
            ILogger<SquareEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        { }

        public async Task HandleEventAsync(MSquare_Event eventData)
        {
            using var log = BeginFunction(nameof(SquareEventMicroService), nameof(HandleEventAsync), eventData);
            try
            {
                switch (eventData.EventTypeCode)
                {
                    case SquarePaymentEventTypeCodes.Payment:
                    case SquarePaymentEventTypeCodes.Refund:
                        await HandlePaymentEventAsync(eventData).ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentException($"Unknown event type code {eventData.EventTypeCode}.");
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private async Task HandlePaymentEventAsync(MSquare_Event eventData)
        {
            var funderReference = CreateFunderReference.FromSquarePaymentId(eventData.SquarePaymentId);
            var funderId = await FundingMicroService.AllocateFunderAsync(funderReference).ConfigureAwait(false);

            if (TryParseOrderId.FromSquarePaymentReference(eventData.SquarePaymentReference, out var orderId))
            {
                var fundableReference = CreateFundableReference.FromOrderId(orderId);

                await FundingMicroService.SetFundsReceivedAsync(funderId, fundableReference, eventData.PaymentAmount, eventData.UnitOfWork).ConfigureAwait(false);
                await FundingMicroService.SetProcessingFeeAsync(funderId, fundableReference, eventData.ProcessingFeeAmount, eventData.UnitOfWork).ConfigureAwait(false);
                await FundingMicroService.SetFundsRefundedAsync(funderId, fundableReference, eventData.RefundAmount, eventData.UnitOfWork).ConfigureAwait(false);
            }
        }
    }
}
