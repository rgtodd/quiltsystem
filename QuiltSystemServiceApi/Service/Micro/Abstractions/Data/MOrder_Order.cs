//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_Order
    {
        // Order
        //
        public long OrderId { get; set; }
        public long OrdererId { get; set; }
        public string OrderNumber { get; set; }
        public decimal ItemSubtotalAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PretaxAmount { get; set; }
        public decimal SalesTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FundsRequired { get; set; }
        public decimal FundsReceived { get; set; }
        public string SalesTaxJurisdiction { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal SalesTaxPercent { get; set; }
        public MOrder_OrderStatus OrderStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime? SubmissionDateTimeUtc { get; set; }
        public DateTime? FulfillmentDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        // Orderer
        //
        public string OrdererReference { get; set; }

        public bool CanCancel { get; set; }
        public bool CanPay { get; set; }
        public bool CanReturn { get; set; }
        public MOrder_OrderShippingAddress ShippingAddress { get; set; }
        public IList<MOrder_OrderItem> OrderItems { get; set; }
    }

    public class MOrder_OrderItem
    {
        // OrderItem
        //
        public long OrderItemId { get; set; }
        public int OrderItemSequence { get; set; }
        public long OrderableId { get; set; }
        public int OrderQuantity { get; set; }
        public int CancelQuantity { get; set; }
        public int FulfillmentReturnQuantity { get; set; }
        public int NetQuantity { get; set; }
        public int FulfillmentRequiredQuantity { get; set; }
        public int FulfillmentCompleteQuantity { get; set; }
        public string ConsumableReference { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        // Orderable
        //
        public string OrderableReference { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }

        public bool CanReturn { get; set; }

        public IList<MOrder_OrderItemComponent> OrderItemComponents { get; set; }
    }

    public class MOrder_OrderItemComponent
    {
        public long OrderableComponentId { get; set; }
        public string OrderableComponentReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
