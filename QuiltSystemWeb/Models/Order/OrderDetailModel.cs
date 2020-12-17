//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderDetailModel
    {
        public bool Collapsable { get; set; }

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

        [Display(Name = "Status Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StatusDateTime { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? SubmissionDateTime { get; set; }

        [Display(Name = "Shipping To")]
        public string ShippingName { get; set; }

        [Display(Name = "Shipping Address")]
        public string[] ShippingAddressLines { get; set; }

        [Display(Name = "Items")]
        public IList<OrderDetailItemModel> PendingItems { get; set; }

        [Display(Name = "Shipments")]
        public IList<OrderDetailShipmentModel> Shipments { get; set; }

        [Display(Name = "Returns")]
        public IList<OrderDetailReturnModel> Returns { get; set; }

        public bool CanCancel { get; set; }

        public bool CanPay { get; set; }

        public bool CanReturn { get; set; }
    }
}