//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_AllocateFulfillableResponse
    {
        public string FulfillableReference { get; set; }
        public long FulfillableId { get; set; }

        public IList<MFulfillment_AllocateFulfillableItemResponse> FulfillableItemResponses { get; set; }
    }

    public class MFulfillment_AllocateFulfillableItemResponse
    {
        public string FulfillableItemReference { get; set; }
        public long FulfillableItemId { get; set; }
    }
}
