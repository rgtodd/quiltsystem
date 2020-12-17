//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemStockTransaction
    {
        public InventoryItemStockTransaction()
        {
            InventoryItemStockTransactionItems = new HashSet<InventoryItemStockTransactionItem>();
        }

        public long InventoryItemStockTransactionId { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public long? OrderLedgerAccountTransactionId { get; set; }
        public long? LedgerAccountTransactionId { get; set; }

        public virtual LedgerTransaction LedgerAccountTransaction { get; set; }
        public virtual ICollection<InventoryItemStockTransactionItem> InventoryItemStockTransactionItems { get; set; }
    }
}
