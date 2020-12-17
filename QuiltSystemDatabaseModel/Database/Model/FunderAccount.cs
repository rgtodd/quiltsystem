//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FunderAccount
    {
        public FunderAccount()
        {
            FunderTransactions = new HashSet<FunderTransaction>();
        }

        public long FunderId { get; set; }
        public string FundableReference { get; set; }
        public decimal FundsReceived { get; set; }
        public decimal FundsAvailable { get; set; }
        public decimal FundsRefunded { get; set; }
        public decimal FundsRefundable { get; set; }
        public decimal ProcessingFee { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Funder Funder { get; set; }
        public virtual ICollection<FunderTransaction> FunderTransactions { get; set; }
    }
}
