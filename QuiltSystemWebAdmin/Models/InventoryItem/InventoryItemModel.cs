//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.InventoryItem
{
    public class InventoryItemModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }

        [Display(Name = "SKU")]
        public string Sku { get; set; }

        public string Name { get; set; }

        public string Collection { get; set; }

        public string Manufacturer { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Reserved Quantity")]
        public int ReservedQuantity { get; set; }

        [Display(Name = "Web Color")]
        public string WebColor { get; set; }

        public IReadOnlyList<InventoryItemStockModel> Stocks { get; set; }
    }
}