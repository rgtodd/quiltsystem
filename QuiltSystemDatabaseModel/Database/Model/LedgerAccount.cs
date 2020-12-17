//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class LedgerAccount
    {
        public LedgerAccount()
        {
            LedgerAccountSubtotals = new HashSet<LedgerAccountSubtotal>();
            LedgerTransactionEntries = new HashSet<LedgerTransactionEntry>();
        }

        public int LedgerAccountNumber { get; set; }
        public string Name { get; set; }
        public string DebitCreditCode { get; set; }

        public virtual ICollection<LedgerAccountSubtotal> LedgerAccountSubtotals { get; set; }
        public virtual ICollection<LedgerTransactionEntry> LedgerTransactionEntries { get; set; }
    }
}
