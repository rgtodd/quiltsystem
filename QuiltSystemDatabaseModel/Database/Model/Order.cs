//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            OrderTransactions = new HashSet<OrderTransaction>();
            OrdererPendingOrders = new HashSet<OrdererPendingOrder>();
        }

        public long OrderId { get; set; }
        public long OrdererId { get; set; }
        public string OrderNumber { get; set; }
        public decimal ItemSubtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Discount { get; set; }
        public decimal PretaxAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FundsRequired { get; set; }
        public decimal FundsReceived { get; set; }
        public string SalexTaxJurisdiction { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal SalesTaxRate { get; set; }
        public string OrderStatusCode { get; set; }
        public DateTime OrderStatusDateTimeUtc { get; set; }
        public DateTime? SubmissionDateTimeUtc { get; set; }
        public DateTime? FulfillmentDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual OrderStatusType OrderStatusCodeNavigation { get; set; }
        public virtual Orderer Orderer { get; set; }
        public virtual OrderBillingAddress OrderBillingAddress { get; set; }
        public virtual OrderShippingAddress OrderShippingAddress { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderTransaction> OrderTransactions { get; set; }
        public virtual ICollection<OrdererPendingOrder> OrdererPendingOrders { get; set; }
    }
}
