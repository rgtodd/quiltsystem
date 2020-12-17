//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_OrderSummary
    {
        // Order
        //
        public long OrderId { get; set; }
        public long OrdererId { get; set; }
        public string OrderNumber { get; set; }
        public MOrder_OrderStatus OrderStatus { get; set; }
        public DateTime OrderDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public decimal ItemSubtotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public string SalesTaxJurisdiction { get; set; }
        public decimal SalesTaxPercent { get; set; }
        public decimal SalesTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Orderer
        //
        public string OrdererReference { get; set; }
    }
}
