//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AOrder_PostOrderTransactionItem
    {
        [Display(Name = "Order Item ID")]
        public long OrderItemId { get; set; }

        [Display(Name = "Order Item Status")]
        public int? OrderItemStatusTypeCode { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
