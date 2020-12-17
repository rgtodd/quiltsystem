//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class EventAdminService : BaseService, IEventAdminService
    {
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IFundingMicroService FundingMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }

        public EventAdminService(
            IApplicationRequestServices requestServices,
            ILogger<EventAdminService> logger,
            IFulfillmentMicroService fulfillmentMicroService,
            IFundingMicroService fundingMicroService,
            IOrderMicroService orderMicroService,
            ISquareMicroService squareMicroService)
            : base(requestServices, logger)
        {
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
        }

        public async Task<AEvent_EventLogList> GetEventLogsAsync(string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(EventAdminService), nameof(GetEventLogsAsync), unitOfWork);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var eventLogs = new AEvent_EventLogList()
                {
                    MFunderEventLogs = await FundingMicroService.GetFunderEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MFundableEventLogs = await FundingMicroService.GetFundableEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MFulfillableEventLogs = await FulfillmentMicroService.GetFulfillableEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MShipmentRequestEventLogs = await FulfillmentMicroService.GetShipmentRequestEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MShipmentEventLogs = await FulfillmentMicroService.GetShipmentEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MReturnRequestTrnsactions = await FulfillmentMicroService.GetReturnRequestEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MReturnEventLogs = await FulfillmentMicroService.GetReturnEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MOrderEventLogs = await OrderMicroService.GetOrderEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MSquarePaymentEventLogs = await SquareMicroService.GetPaymentEventLogSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MSquareRefundEventLogs = await SquareMicroService.GetRefundEventLogSummariesAsync(null, null, unitOfWork, source).ConfigureAwait(false)
                };

                var result = eventLogs;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }
    }
}
