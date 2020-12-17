//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderTransactionItem
    {
        public long OrderTransactionItemId { get; set; }
        public long OrderTransactionId { get; set; }
        public long OrderItemId { get; set; }
        public int OrderQuantity { get; set; }
        public int CancelQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int FulfillmentReturnQuantity { get; set; }
        public int FulfillmentRequiredQuantity { get; set; }
        public int FulfillmentCompleteQuantity { get; set; }

        public virtual OrderItem OrderItem { get; set; }
        public virtual OrderTransaction OrderTransaction { get; set; }
    }
}
