//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_CreateReturnRequest
    {
        public MFulfillment_ReturnRequestTypes ReturnRequestType { get; set; }
        public string ReturnRequestReasonCode { get; set; }
        public string Notes { get; set; }

        public IList<MFulfillment_CreateReturnRequestItem> CreateReturnRequestItems { get; set; }
    }

    public class MFulfillment_CreateReturnRequestItem
    {
        public long FulfillableItemId { get; set; }
        public int Quantity { get; set; }
    }
}
