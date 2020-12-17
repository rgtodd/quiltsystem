//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_FundableTransaction : MCommon_Transaction
    {
        public override string Source => MSources.Fundable;
        public long FundableTransactionId => TransactionId;
        public long FundableId => EntityId;

        public decimal FundsRequiredIncome { get; set; }
        public decimal FundsRequiredSalesTax { get; set; }
        public string FundsRequiredSalesTaxJurisdiction { get; set; }
        public decimal FundsReceived { get; set; }
    }
}
