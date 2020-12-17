//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.InventoryItem
{
    public class InventoryItemStockModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }

        [Display(Name = "Unit Cost")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat, ApplyFormatInEditMode = true)]
        public decimal UnitCost { get; set; }

        [Display(Name = "Stock Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StockDateTime { get; set; }

        [Display(Name = "Original Quantity")]
        public int OriginalQuantity { get; set; }

        [Display(Name = "Current Quantity")]
        public int CurrentQuantity { get; set; }
    }
}