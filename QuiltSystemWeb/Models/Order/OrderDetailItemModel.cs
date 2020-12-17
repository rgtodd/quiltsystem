//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderDetailItemModel
    {
        public int OrderItemSequence { get; set; }
        public long OrderItemId { get; set; }
        public string OrderableReference { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "SKU")]
        public string Sku { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Kit Price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Status Date/Time")]
        [DataType(DataType.DateTime)]
        public DateTime StatusDateTime { get; set; }

        [Display(Name = "Components")]
        public IList<OrderItemComponentModel> Components { get; set; }
    }
}