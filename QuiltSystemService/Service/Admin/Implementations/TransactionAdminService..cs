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
    internal class TransactionAdminService : BaseService, ITransactionAdminService
    {
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IFundingMicroService FundingMicroService { get; }
        private ILedgerMicroService LedgerMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }

        public TransactionAdminService(
            IApplicationRequestServices requestServices,
            ILogger<TransactionAdminService> logger,
            IFulfillmentMicroService fulfillmentMicroService,
            IFundingMicroService fundingMicroService,
            ILedgerMicroService ledgerMicroService,
            IOrderMicroService orderMicroService,
            ISquareMicroService squareMicroService)
            : base(requestServices, logger)
        {
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
        }

        public async Task<ATransaction_TransactionList> GetTransactionsAsync(string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(TransactionAdminService), nameof(GetTransactionsAsync), unitOfWork);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var transactions = new ATransaction_TransactionList()
                {
                    MFunderTransactions = await FundingMicroService.GetFunderTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MFundableTransactions = await FundingMicroService.GetFundableTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MFulfillableTransactions = await FulfillmentMicroService.GetFulfillableTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MShipmentRequestTransactions = await FulfillmentMicroService.GetShipmentRequestTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MShipmentTransactions = await FulfillmentMicroService.GetShipmentTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MReturnRequestTrnsactions = await FulfillmentMicroService.GetReturnRequestTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MReturnTransactions = await FulfillmentMicroService.GetReturnTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MLedgerTransactions = await LedgerMicroService.GetLedgerTransactionSummariesAsync(unitOfWork, source).ConfigureAwait(false),
                    MOrderTransactions = await OrderMicroService.GetOrderTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MSquarePaymentTransactions = await SquareMicroService.GetPaymentTransactionSummariesAsync(null, unitOfWork, source).ConfigureAwait(false),
                    MSquareRefundTransactions = await SquareMicroService.GetRefundTransactionSummariesAsync(null, null, unitOfWork, source).ConfigureAwait(false)
                };

                var result = transactions;

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
