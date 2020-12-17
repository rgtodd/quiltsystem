//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemTransaction
    {
        public long InventoryItemTransactionId { get; set; }
        public long InventoryItemId { get; set; }
        public string InventoryItemTransactionTypeCode { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public long? VendorId { get; set; }
        public int OrderTransactionId { get; set; }
        public string AspNetUserId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public virtual InventoryItemTransactionType InventoryItemTransactionTypeCodeNavigation { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
