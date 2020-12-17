//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AInventory_InventoryItem
    {

        public string Collection { get; set; }
        public ACommon_Color Color { get; set; }
        public long Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public string Sku { get; set; }
        public IList<AInventory_InventoryItemStock> InventoryItemStocks { get; set; }
        public string Type { get; set; }

    }
}