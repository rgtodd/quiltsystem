//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_FulfillableTransaction : MCommon_Transaction
    {
        public override string Source => MSources.Fulfillable;
        public long FulfillableTransactionId => TransactionId;

        public IList<MFulfillment_FulfillableTransactionItem> Items { get; set; }
    }

    public class MFulfillment_FulfillableTransactionItem
    {
        public long FulfillableTransactionItemId { get; set; }
        public long FulfillableItemId { get; set; }
        public int RequestQuantity { get; set; }
        public int CompleteQuantity { get; set; }
        public int ReturnQuantity { get; set; }
        public int ConsumeQuantity { get; set; }
    }
}
