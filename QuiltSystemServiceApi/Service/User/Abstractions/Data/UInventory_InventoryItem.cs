//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UInventory_InventoryItem
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Collection { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public MCommon_Color Color { get; set; }
    }
}
