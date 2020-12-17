//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class OrderVcModel
    {
        public long OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ItemSubtotal { get; set; }

        [Display(Name = "Shipping")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Shipping { get; set; }

        [Display(Name = "Discount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Discount { get; set; }

        [Display(Name = "Taxable Subtotal")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal TaxableAmount { get; set; }

        [Display(Name = "Sales Tax Percent")]
        [DisplayFormat(DataFormatString = "{0:p}", ApplyFormatInEditMode = true)]
        public decimal SalesTaxPercent { get; set; }

        [Display(Name = "Sales Tax")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal SalesTax { get; set; }

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [Display(Name = "Status Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StatusDateTime { get; set; }

        [Display(Name = "Submission Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? SubmissionDateTime { get; set; }

        [Display(Name = "Shipping To")]
        public string ShippingName { get; set; }

        [Display(Name = "Shipping Address")]
        public IReadOnlyList<string> ShippingAddressLines { get; set; }

        [Display(Name = "Items")]
        public IList<OrderItemVcModel> Items { get; set; }
    }
}