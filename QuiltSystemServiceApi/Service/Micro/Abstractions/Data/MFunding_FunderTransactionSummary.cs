//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_FunderTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.Funder;
        public long FunderTransactionId => TransactionId;
        public long FunderId => EntityId;

        public string FundableReference { get; set; }
        public decimal FundsReceived { get; set; }
        public decimal FundsAvailable { get; set; }
        public decimal FundsRefunded { get; set; }
        public decimal FundsRefundable { get; set; }
        public decimal ProcessingFee { get; set; }
    }
}
