//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ASquare_Payment
    {
        public MSquare_Payment MPayment { get; set; }
        public MSquare_PaymentTransactionSummaryList MPaymentTransactions { get; set; }
        public MSquare_PaymentEventLogSummaryList MPaymentEvents { get; set; }
        public MSquare_RefundTransactionSummaryList MRefundTransactions { get; set; }
        public MSquare_RefundEventLogSummaryList MRefundEvents { get; set; }
        public MUser_User MUser { get; set; }

        public long? FunderId { get; set; }
    }
}
