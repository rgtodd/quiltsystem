//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FundableTransaction
    {
        public FundableTransaction()
        {
            FundableEvents = new HashSet<FundableEvent>();
        }

        public long FundableTransactionId { get; set; }
        public long FundableId { get; set; }
        public decimal FundsRequiredIncome { get; set; }
        public decimal FundsRequiredSalesTax { get; set; }
        public string FundsRequiredSalesTaxJurisdiction { get; set; }
        public decimal FundsReceived { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual Fundable Fundable { get; set; }
        public virtual ICollection<FundableEvent> FundableEvents { get; set; }
    }
}
