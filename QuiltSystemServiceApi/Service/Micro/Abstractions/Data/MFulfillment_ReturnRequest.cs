//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnRequest
    {
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
        public MFulfillment_ReturnRequestTypes ReturnRequestType { get; set; }
        public MFulfillment_ReturnRequestStatus ReturnRequestStatus { get; set; }
        public string ReturnRequestReasonCode { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }

        public bool CanEdit { get; set; }
        public bool CanCreateReturn { get; set; }

        public IList<MFulfillment_ReturnRequestItem> ReturnRequestItems { get; set; }
    }

    public class MFulfillment_ReturnRequestItem
    {
        public long ReturnRequestItemId { get; set; }
        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public int Quantity { get; set; }

        // Convienence properties
        //
        public long FulfillableId { get; set; }
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
    }
}
