//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ShipmentRequest
    {
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestNumber { get; set; }
        public MFulfillment_ShipmentRequestStatus ShipmentRequestStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public MFulfillment_Address ShipToAddress { get; set; }

        public bool CanCreateShipment { get; set; }
        public bool CanCancel { get; set; }

        public IList<MFulfillment_ShipmentRequestItem> ShipmentRequestItems { get; set; }
    }

    public class MFulfillment_ShipmentRequestItem
    {
        public long ShipmentRequestItemId { get; set; }
        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public int Quantity { get; set; }

        // Convienence properties
        //
        public long FulfillableId { get; set; }
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestNumber { get; set; }
    }
}
