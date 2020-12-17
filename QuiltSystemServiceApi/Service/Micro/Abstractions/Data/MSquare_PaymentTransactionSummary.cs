//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_PaymentTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.SquarePayment;
        public long SquarePaymentTransactionId => TransactionId;
        public long SquarePaymentId => EntityId;

        public string SquarePaymentRecordId { get; set; }
        public int VersionNumber { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
    }
}
