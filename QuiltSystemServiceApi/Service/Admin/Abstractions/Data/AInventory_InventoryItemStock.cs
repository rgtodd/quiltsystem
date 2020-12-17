//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AInventory_InventoryItemStock
    {
        public int CurrentQuantity { get; set; }
        public long InventoryItemStockId { get; set; }
        public int OriginalQuantity { get; set; }
        public DateTime StockDateTimeUtc { get; set; }
        public decimal UnitCost { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}