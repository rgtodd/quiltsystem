//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderEditItemModel
    {
        public long OrderItemId { get; set; }

        public string OrderableReference { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "SKU")]
        public string Sku { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Kit Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal KitPrice { get; set; }

        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Component")]
        public IList<OrderItemComponentModel> Components { get; set; }

        public int OriginalQuantity { get; set; }
    }
}