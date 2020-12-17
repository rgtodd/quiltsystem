//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Fundable
    {
        public Fundable()
        {
            FundableTransactions = new HashSet<FundableTransaction>();
        }

        public long FundableId { get; set; }
        public string FundableReference { get; set; }
        public decimal FundsRequiredTotal { get; set; }
        public decimal FundsRequiredIncome { get; set; }
        public decimal FundsRequiredSalesTax { get; set; }
        public string FundsRequiredSalesTaxJurisdiction { get; set; }
        public decimal FundsReceived { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<FundableTransaction> FundableTransactions { get; set; }
    }
}
