//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableItem
    {
        public FulfillableItem()
        {
            FulfillableItemComponents = new HashSet<FulfillableItemComponent>();
            FulfillableTransactionItems = new HashSet<FulfillableTransactionItem>();
            ReturnRequestItems = new HashSet<ReturnRequestItem>();
            ShipmentRequestItems = new HashSet<ShipmentRequestItem>();
        }

        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public long FulfillableId { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int RequestQuantity { get; set; }
        public int CompleteQuantity { get; set; }
        public int ReturnQuantity { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Fulfillable Fulfillable { get; set; }
        public virtual ICollection<FulfillableItemComponent> FulfillableItemComponents { get; set; }
        public virtual ICollection<FulfillableTransactionItem> FulfillableTransactionItems { get; set; }
        public virtual ICollection<ReturnRequestItem> ReturnRequestItems { get; set; }
        public virtual ICollection<ShipmentRequestItem> ShipmentRequestItems { get; set; }
    }
}
