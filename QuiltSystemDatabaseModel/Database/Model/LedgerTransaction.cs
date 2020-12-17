//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class LedgerTransaction
    {
        public LedgerTransaction()
        {
            InventoryItemStockTransactions = new HashSet<InventoryItemStockTransaction>();
            LedgerTransactionEntries = new HashSet<LedgerTransactionEntry>();
        }

        public long LedgerTransactionId { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime PostDateTime { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual ICollection<InventoryItemStockTransaction> InventoryItemStockTransactions { get; set; }
        public virtual ICollection<LedgerTransactionEntry> LedgerTransactionEntries { get; set; }
    }
}
