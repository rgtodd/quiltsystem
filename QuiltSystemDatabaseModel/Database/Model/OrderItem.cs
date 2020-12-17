//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderItem
    {
        public OrderItem()
        {
            OrderTransactionItems = new HashSet<OrderTransactionItem>();
        }

        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public int OrderItemSequence { get; set; }
        public long OrderableId { get; set; }
        public int OrderQuantity { get; set; }
        public int CancelQuantity { get; set; }
        public int FulfillmentReturnQuantity { get; set; }
        public int NetQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int FulfillmentRequiredQuantity { get; set; }
        public int FulfillmentCompleteQuantity { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Order Order { get; set; }
        public virtual Orderable Orderable { get; set; }
        public virtual ICollection<OrderTransactionItem> OrderTransactionItems { get; set; }
    }
}
