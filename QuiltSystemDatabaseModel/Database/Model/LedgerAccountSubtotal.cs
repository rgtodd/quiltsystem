//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class LedgerAccountSubtotal
    {
        public LedgerAccountSubtotal()
        {
            LedgerTransactionEntries = new HashSet<LedgerTransactionEntry>();
        }

        public long LedgerAccountSubtotalId { get; set; }
        public int LedgerAccountNumber { get; set; }
        public int AccountingYear { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AccountingYear AccountingYearNavigation { get; set; }
        public virtual LedgerAccount LedgerAccountNumberNavigation { get; set; }
        public virtual ICollection<LedgerTransactionEntry> LedgerTransactionEntries { get; set; }
    }
}
