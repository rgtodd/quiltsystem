//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItem
    {
        public InventoryItem()
        {
            InventoryItemStocks = new HashSet<InventoryItemStock>();
            InventoryItemTags = new HashSet<InventoryItemTag>();
            InventoryItemTransactions = new HashSet<InventoryItemTransaction>();
            InventoryItemUnits = new HashSet<InventoryItemUnit>();
        }

        public long InventoryItemId { get; set; }
        public string InventoryItemTypeCode { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public long PricingScheduleId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual InventoryItemType InventoryItemTypeCodeNavigation { get; set; }
        public virtual PricingSchedule PricingSchedule { get; set; }
        public virtual ICollection<InventoryItemStock> InventoryItemStocks { get; set; }
        public virtual ICollection<InventoryItemTag> InventoryItemTags { get; set; }
        public virtual ICollection<InventoryItemTransaction> InventoryItemTransactions { get; set; }
        public virtual ICollection<InventoryItemUnit> InventoryItemUnits { get; set; }
    }
}
