//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_FulfillableEvent
    {
        public long FulfillableEventId { get; set; }
        public MFulfillment_FulfillmentEventTypes EventType { get; set; }
        public string UnitOfWork { get; set; }

        public IList<MFulfillment_FulfillableEventItem> FulfillableEventItems { get; set; }
    }

    public class MFulfillment_FulfillableEventItem
    {
        public long FulfillableItemId { get; set; }
        public string FulfillmentItemReference { get; set; }
        public int FulfillmentReturnQuantity { get; set; }
        public int FulfillmentRequiredQuantity { get; set; }
        public int FulfillmentCompleteQuantity { get; set; }
    }
}
