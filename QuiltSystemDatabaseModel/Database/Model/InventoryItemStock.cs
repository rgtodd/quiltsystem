//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemStock
    {
        public InventoryItemStock()
        {
            InventoryItemStockTransactionItems = new HashSet<InventoryItemStockTransactionItem>();
        }

        public long InventoryItemStockId { get; set; }
        public long InventoryItemId { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime StockDateTimeUtc { get; set; }
        public int OriginalQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
        public virtual UnitOfMeasure UnitOfMeasureCodeNavigation { get; set; }
        public virtual ICollection<InventoryItemStockTransactionItem> InventoryItemStockTransactionItems { get; set; }
    }
}
