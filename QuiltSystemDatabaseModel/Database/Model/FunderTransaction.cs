//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FunderTransaction
    {
        public FunderTransaction()
        {
            FunderEvents = new HashSet<FunderEvent>();
        }

        public long FunderTransactionId { get; set; }
        public long FunderId { get; set; }
        public string FundableReference { get; set; }
        public decimal FundsReceived { get; set; }
        public decimal FundsAvailable { get; set; }
        public decimal FundsRefunded { get; set; }
        public decimal FundsRefundable { get; set; }
        public decimal ProcessingFee { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual FunderAccount Fund { get; set; }
        public virtual ICollection<FunderEvent> FunderEvents { get; set; }
    }
}
