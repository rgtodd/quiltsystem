//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderTransaction
    {
        public OrderTransaction()
        {
            OrderEvents = new HashSet<OrderEvent>();
            OrderTransactionItems = new HashSet<OrderTransactionItem>();
        }

        public long OrderTransactionId { get; set; }
        public string OrderTransactionTypeCode { get; set; }
        public long OrderId { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public decimal ItemSubtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesTax { get; set; }
        public string SalesTaxJurisdiction { get; set; }
        public decimal? SalesTaxRate { get; set; }
        public decimal FundsRequired { get; set; }
        public decimal FundsReceived { get; set; }
        public string OrderStatusCode { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual Order Order { get; set; }
        public virtual OrderStatusType OrderStatusCodeNavigation { get; set; }
        public virtual OrderTransactionType OrderTransactionTypeCodeNavigation { get; set; }
        public virtual ICollection<OrderEvent> OrderEvents { get; set; }
        public virtual ICollection<OrderTransactionItem> OrderTransactionItems { get; set; }
    }
}
