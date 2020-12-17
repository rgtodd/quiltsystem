//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Return
{
    public class ReturnRequestEditItemModel
    {
        public long? OrderReturnRequestItemId { get; set; }

        [Display(Name = "Return Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Maximum Quantity")]
        public int MaximumQuantity { get; set; }

        public ReturnRequestOrderItemModel OrderItem { get; set; }
    }
}