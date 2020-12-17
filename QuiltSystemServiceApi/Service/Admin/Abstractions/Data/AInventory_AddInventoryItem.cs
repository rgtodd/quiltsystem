//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AInventory_AddInventoryItem
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string InventoryItemTypeCode { get; set; }
        public string Collection { get; set; }
        public string Manufacturer { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }
        public IList<string> UnitOfMeasureCodeList { get; set; }
        public string PricingScheduleName { get; set; }
    }
}
