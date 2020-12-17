//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_AllocateFulfillable
    {
        public string FulfillableReference { get; set; }
        public string Name { get; set; }
        public MCommon_Address ShippingAddress { get; set; }

        public IList<MFulfillment_AllocateFulfillableItem> FulfillableItems { get; set; }
    }

    public class MFulfillment_AllocateFulfillableItem
    {
        public string FulfillableItemReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }

        public IList<MFulfillment_AllocateFulfillableItemComponent> FulfillableItemComponents { get; set; }
    }

    public class MFulfillment_AllocateFulfillableItemComponent
    {
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
    }
}
