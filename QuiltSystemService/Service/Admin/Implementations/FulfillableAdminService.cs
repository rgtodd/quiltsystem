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
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class FulfillableAdminService : BaseService, IFulfillableAdminService
    {
        private IFulfillmentMicroService FulfillmentMicroService { get; }

        public FulfillableAdminService(
            IApplicationRequestServices requestServices,
            ILogger<FulfillableAdminService> logger,
            IFulfillmentMicroService fulfillmentMicroService)
            : base(requestServices, logger)
        {
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
        }

        public async Task<AFulfillable_Fulfillable> GetFulfillableAsync(long fulfillableId)
        {
            using var log = BeginFunction(nameof(FulfillableAdminService), nameof(GetFulfillableAsync), fulfillableId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mFulfillable = await FulfillmentMicroService.GetFulfillableAsync(fulfillableId);
                var mTransactionSummaries = await FulfillmentMicroService.GetFulfillableTransactionSummariesAsync(fulfillableId, null, null);
                var mEventSummaries = await FulfillmentMicroService.GetFulfillableEventLogSummariesAsync(fulfillableId, null, null);

                var allowEdit = await SecurityPolicy.AllowEditFulfillment();

                var result = Create.AFulfillable_Fulfillable(mFulfillable, mTransactionSummaries, mEventSummaries, allowEdit);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AFulfillable_FulfillableSummaryList> GetFulfillableSummariesAsync(MFulfillment_FulfillableStatus fulfillableStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillableAdminService), nameof(GetFulfillableSummariesAsync), fulfillableStatus, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mFulfillableSummaryList = await FulfillmentMicroService.GetFulfillableSummariesAsync(fulfillableStatus, recordCount);

                var result = new AFulfillable_FulfillableSummaryList()
                {
                    MSummaries = mFulfillableSummaryList.Summaries
                };

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static AFulfillable_Fulfillable AFulfillable_Fulfillable(MFulfillment_Fulfillable mFulfillable, MFulfillment_FulfillableTransactionSummaryList mTransactionSummaries, MFulfillment_FulfillableEventLogSummaryList mEventSummaries, bool allowEdit)
            {
                return new AFulfillable_Fulfillable()
                {
                    MFulfillable = mFulfillable,
                    MTransactions = mTransactionSummaries,
                    MEvents = mEventSummaries,
                    AllowEdit = allowEdit
                };
            }

        }
    }
}
