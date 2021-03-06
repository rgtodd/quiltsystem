﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderEditModel
    {
        public long OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Items")]
        public IList<OrderEditItemModel> Items { get; set; }

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ItemSubtotal { get; set; }

        [Display(Name = "Discount")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Discount { get; set; }

        [Display(Name = "Shipping")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Shipping { get; set; }

        [Display(Name = "Taxable Subtotal")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal TaxableSubtotal { get; set; }

        [Display(Name = "Sales Tax Percent")]
        [DisplayFormat(DataFormatString = "{0:p}", ApplyFormatInEditMode = true)]
        public decimal SalesTaxPercent { get; set; }

        [Display(Name = "Sales Tax")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal SalesTax { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Total { get; set; }

        [Display(Name = "Submission Date/Time")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? SubmissionDateTime { get; set; }

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [Display(Name = "PayPal Button HTML")]
        public string PayPalButton { get; set; }
    }
}