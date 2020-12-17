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
    internal class FundingAdminService : BaseService, IFundingAdminService
    {
        private IFundingMicroService FundingMicroService { get; }

        public FundingAdminService(
            IApplicationRequestServices requestServices,
            ILogger<FundingAdminService> logger,
            IFundingMicroService fundingMicroService)
            : base(requestServices, logger)
        {
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
        }

        public async Task<AFunding_Fundable> GetFundableAsync(long fundableId)
        {
            using var log = BeginFunction(nameof(FundingAdminService), nameof(GetFundableAsync), fundableId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mFundable = await FundingMicroService.GetFundableAsync(fundableId);
                var mFunders = await FundingMicroService.GetFunderSummariesAsync(fundableId, null, null, null);
                var mTransactionSummaries = await FundingMicroService.GetFundableTransactionSummariesAsync(fundableId, null, null);
                var mEventSummaries = await FundingMicroService.GetFundableEventLogSummariesAsync(fundableId, null, null);

                var result = Create.AFunding_Fundable(mFundable, mFunders, mTransactionSummaries, mEventSummaries);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AFunding_Funder> GetFunderAsync(long funderId)
        {
            using var log = BeginFunction(nameof(FundingAdminService), nameof(GetFunderAsync), funderId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mFunder = await FundingMicroService.GetFunderAsync(funderId);
                var mFundables = await FundingMicroService.GetFundableSummariesAsync(funderId, null, null);
                var mTransactionSummaries = await FundingMicroService.GetFunderTransactionSummariesAsync(funderId, null, null);
                var mEventSummaries = await FundingMicroService.GetFunderEventLogSummariesAsync(funderId, null, null);

                var result = Create.AFunding_Funder(mFunder, mFundables, mTransactionSummaries, mEventSummaries);

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
            public static AFunding_Fundable AFunding_Fundable(
                MFunding_Fundable mFundable,
                MFunding_FunderSummaryList mFunders,
                MFunding_FundableTransactionSummaryList mTransactionSummaries,
                MFunding_FundableEventLogSummaryList mEventSummaries)
            {
                return new AFunding_Fundable()
                {
                    MFundable = mFundable,
                    MFunders = mFunders,
                    MTransactions = mTransactionSummaries,
                    MEvents = mEventSummaries
                };
            }

            public static AFunding_Funder AFunding_Funder(
                MFunding_Funder mFunder,
                MFunding_FundableSummaryList mFundables,
                MFunding_FunderTransactionSummaryList mTransactionSummaries,
                MFunding_FunderEventLogSummaryList mEventSummaries)
            {
                return new AFunding_Funder()
                {
                    MFunder = mFunder,
                    MFundables = mFundables,
                    MTransactions = mTransactionSummaries,
                    MEvents = mEventSummaries
                };
            }

        }
    }
}
