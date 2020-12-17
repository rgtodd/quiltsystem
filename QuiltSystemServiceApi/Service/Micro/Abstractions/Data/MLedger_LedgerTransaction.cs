//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MLedger_LedgerTransaction : MCommon_Transaction
    {
        public override string Source => MSources.LedgerTransaction;
        public long LedgerTransactionId => TransactionId;

        public DateTime PostDateTime { get; set; }
        public IList<MLedger_LedgerTransactionEntry> Entries { get; set; }
    }

    public class MLedger_LedgerTransactionEntry
    {
        public long LedgerTransactionEntryId { get; set; }
        public int LedgerAccountNumber { get; set; }
        public string LedgerAccountName { get; set; }
        public decimal EntryAmount { get; set; }
        public string DebitCreditCode { get; set; }
        public string LedgerReference { get; set; }
        public string SalesTaxJurisdiction { get; set; }
    }
}
