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
    internal class SquareAdminService : BaseService, ISquareAdminService
    {
        private IFundingMicroService FundingMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public SquareAdminService(
            IApplicationRequestServices requestServices,
            ILogger<SquareAdminService> logger,
            IFundingMicroService fundingMicroService,
            ISquareMicroService squareMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task<ASquare_Customer> GetCustomerAsync(long squareCustomerId)
        {
            using var log = BeginFunction(nameof(SquareAdminService), nameof(GetCustomerAsync), squareCustomerId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mCustomer = await SquareMicroService.GetSquareCustomerAsync(squareCustomerId);

                var result = new ASquare_Customer()
                {
                    MCustomer = mCustomer
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

        public async Task<ASquare_CustomerSummaryList> GetCustomerSummariesAsync(long? squareCustomerId, int? recordCount)
        {
            using var log = BeginFunction(nameof(SquareAdminService), nameof(GetCustomerSummariesAsync), squareCustomerId, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mCustomerSummaryList = await SquareMicroService.GetSquareCustomerSummariesAsync(squareCustomerId, recordCount);

                var result = new ASquare_CustomerSummaryList()
                {
                    MSummaries = mCustomerSummaryList.Summaries
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

        public async Task<ASquare_Payment> GetPaymentAsync(long squarePaymentId)
        {
            using var log = BeginFunction(nameof(SquareAdminService), nameof(GetPaymentAsync), squarePaymentId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mPayment = await SquareMicroService.GetSquarePaymentAsync(squarePaymentId);
                var result = await GetPaymentDetails(mPayment);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<ASquare_Payment> GetPaymentByRefundAsync(long squareRefundId)
        {
            using var log = BeginFunction(nameof(SquareAdminService), nameof(GetPaymentByRefundAsync), squareRefundId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mPayment = await SquareMicroService.GetSquarePaymentByRefundAsync(squareRefundId);
                var result = await GetPaymentDetails(mPayment);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<ASquare_PaymentSummaryList> GetPaymentSummariesAsync(long? squareCustomerId, DateTime? paymentDateUtc, int? recordCount)
        {
            using var log = BeginFunction(nameof(SquareAdminService), nameof(GetPaymentSummariesAsync), squareCustomerId, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mPaymentSummaryList = await SquareMicroService.GetSquarePaymentSummariesAsync(squareCustomerId, paymentDateUtc, recordCount);

                var result = new ASquare_PaymentSummaryList()
                {
                    MSummaries = mPaymentSummaryList.Summaries
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

        private async Task<ASquare_Payment> GetPaymentDetails(MSquare_Payment mPayment)
        {
            var mPaymentTransactions = await SquareMicroService.GetPaymentTransactionSummariesAsync(mPayment.SquarePaymentId, null, null);
            var mPaymentEvents = await SquareMicroService.GetPaymentEventLogSummariesAsync(mPayment.SquarePaymentId, null, null);
            var mRefundTransactions = await SquareMicroService.GetRefundTransactionSummariesAsync(null, mPayment.SquarePaymentId, null, null);
            var mRefundEvents = await SquareMicroService.GetRefundEventLogSummariesAsync(null, mPayment.SquarePaymentId, null, null);

            var mUser = TryParseUserId.FromSquareCustomerReference(mPayment.SquareCustomerReference, out string userId)
                ? await UserMicroService.GetUserAsync(userId).ConfigureAwait(false)
                : null;

            var funderReference = CreateFunderReference.FromSquarePaymentId(mPayment.SquarePaymentId);
            var funderId = await FundingMicroService.LookupFunderAsync(funderReference);

            var result = new ASquare_Payment()
            {
                MPayment = mPayment,
                MPaymentTransactions = mPaymentTransactions,
                MPaymentEvents = mPaymentEvents,
                MRefundTransactions = mRefundTransactions,
                MRefundEvents = mRefundEvents,
                MUser = mUser,
                FunderId = funderId
            };
            return result;
        }
    }
}
