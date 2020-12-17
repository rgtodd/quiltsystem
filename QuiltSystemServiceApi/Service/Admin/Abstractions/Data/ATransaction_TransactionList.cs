//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ATransaction_TransactionList
    {
        public MFunding_FunderTransactionSummaryList MFunderTransactions { get; set; }
        public MFunding_FundableTransactionSummaryList MFundableTransactions { get; set; }
        public MFulfillment_FulfillableTransactionSummaryList MFulfillableTransactions { get; set; }
        public MFulfillment_ShipmentRequestTransactionSummaryList MShipmentRequestTransactions { get; set; }
        public MFulfillment_ShipmentTransactionSummaryList MShipmentTransactions { get; set; }
        public MFulfillment_ReturnRequestTransactionSummaryList MReturnRequestTrnsactions { get; set; }
        public MFulfillment_ReturnTransactionSummaryList MReturnTransactions { get; set; }
        public MLedger_LedgerTransactionSummaryList MLedgerTransactions { get; set; }
        public MOrder_OrderTransactionSummaryList MOrderTransactions { get; set; }
        public MSquare_PaymentTransactionSummaryList MSquarePaymentTransactions { get; set; }
        public MSquare_RefundTransactionSummaryList MSquareRefundTransactions { get; set; }
    }
}