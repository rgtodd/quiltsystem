//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Cart
{
    public class CartEditItemModel
    {
        public long OrderItemId { get; set; }
        public string OrderableReference { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "SKU")]
        public string Sku { get; set; }

        [Display(Name = "Quantity")]
        [Range(1, 99)]
        [Required]
        public int Quantity { get; set; }

        [Display(Name = "Kit Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal KitPrice { get; set; }

        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Component")]
        public IList<CartItemComponentModel> Components { get; set; }

        public int OriginalQuantity { get; set; }
    }
}