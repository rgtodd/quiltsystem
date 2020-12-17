//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFunding_Fundable
    {
        public long FundableId { get; set; }
        public string FundableReference { get; set; }
        public decimal FundsRequiredTotal { get; set; }
        public decimal FundsRequiredIncome { get; set; }
        public decimal FundsRequiredSalesTax { get; set; }
        public string FundsRequiredSalesTaxJurisdiction { get; set; }
        public decimal FundsReceived { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        public IList<MFunding_FundableTransaction> FundableTransactions { get; set; }
    }
}
