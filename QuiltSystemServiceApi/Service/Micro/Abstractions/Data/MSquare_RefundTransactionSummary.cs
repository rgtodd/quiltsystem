//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_RefundTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.SquareRefund;
        public long SquareRefundTransactionId => TransactionId;
        public long SquareRefundId => EntityId;

        public string SquareRefundRecordId { get; set; }
        public int VersionNumber { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
    }
}
