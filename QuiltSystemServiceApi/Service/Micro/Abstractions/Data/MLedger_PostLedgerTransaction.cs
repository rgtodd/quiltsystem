//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MLedger_PostLedgerTransaction
    {
        public DateTime PostDateTime { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }
        public IList<MLedger_PostLedgerTransactionEntry> Entries { get; set; }
    }

    public class MLedger_PostLedgerTransactionEntry
    {
        public int LedgerAccountNumber { get; set; }
        public decimal EntryAmount { get; set; }
        public string DebitCreditCode { get; set; }
        public string LedgerReference { get; set; }
        public string SalesTaxJurisdiction { get; set; }
    }
}
