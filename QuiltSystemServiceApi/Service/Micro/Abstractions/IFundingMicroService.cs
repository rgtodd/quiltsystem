//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IFundingMicroService : IEventService
    {
        Task<MFunding_Dashboard> GetDashboardAsync();

        // Funder Methods

        Task<long> AllocateFunderAsync(string funderReference);

        Task<long?> LookupFunderAsync(string funderReference);

        Task<MFunding_Funder> GetFunderAsync(long funderId);

        Task<MFunding_FunderSummaryList> GetFunderSummariesAsync(long? fundableId, bool? hasFundsAvailable, bool? hasFundsRefundable, int? recordCount);

        Task SetFundsReceivedAsync(long funderId, string fundableReference, decimal fundsReceived, string unitOfWork);

        Task SetFundsRefundedAsync(long funderId, string fundableReference, decimal fundsRefunded, string unitOfWork);

        Task SetProcessingFeeAsync(long funderId, string fundableReference, decimal processingFee, string unitOfWork);

        Task<MFunding_FunderTransaction> GetFunderTransactionAsync(long funderTransactionId);

        Task<MFunding_FunderTransactionSummaryList> GetFunderTransactionSummariesAsync(long? funderId, string unitOfWork, string source);

        Task<MFunding_FunderEventLog> GetFunderEventLogAsync(long funderEventId);

        Task<MFunding_FunderEventLogSummaryList> GetFunderEventLogSummariesAsync(long? funderId, string unitOfWork, string source);

        // Fundable Methods

        Task<long> AllocateFundableAsync(string fundableReference);

        Task<long?> LookupFundableAsync(string fundableReference);

        Task<MFunding_Fundable> GetFundableAsync(long fundableId);

        Task<MFunding_FundableSummaryList> GetFundableSummariesAsync(long? funderId, bool? hasFundsRequired, int? recordCount);

        Task SetFundsRequiredAsync(long fundableId, decimal? fundsRequiredIncome, decimal? fundsRequiredSalesTax, string fundsRequiredSalesTaxJurisdiction, string unitOfWork);

        Task<MFunding_FundableTransaction> GetFundableTransactionAsync(long fundableTransactionId);

        Task<MFunding_FundableTransactionSummaryList> GetFundableTransactionSummariesAsync(long? fundableId, string unitOfWork, string source);

        Task<MFunding_FundableEventLog> GetFundableEventLogAsync(long fundableEventId);

        Task<MFunding_FundableEventLogSummaryList> GetFundableEventLogSummariesAsync(long? fundableId, string unitOfWork, string source);
    }
}
