//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemStockTransactionItem
    {
        public long InventoryItemStockTransactionItemId { get; set; }
        public long InventoryItemStockTransactionId { get; set; }
        public long InventoryItemStockId { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }

        public virtual InventoryItemStock InventoryItemStock { get; set; }
        public virtual InventoryItemStockTransaction InventoryItemStockTransaction { get; set; }
    }
}
