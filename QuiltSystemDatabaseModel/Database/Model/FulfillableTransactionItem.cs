//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableTransactionItem
    {
        public long FulfillableTransactionItemId { get; set; }
        public long FulfillableTransactionId { get; set; }
        public long FulfillableItemId { get; set; }
        public int RequestQuantity { get; set; }
        public int CompleteQuantity { get; set; }
        public int ReturnQuantity { get; set; }
        public int ConsumeQuantity { get; set; }

        public virtual FulfillableItem FulfillableItem { get; set; }
        public virtual FulfillableTransaction FulfillableTransaction { get; set; }
    }
}
