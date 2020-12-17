//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_Shipment
    {
        public long ShipmentId { get; set; }
        public string ShipmentNumber { get; set; }
        public MFulfillment_Address ShipToAddress { get; set; }
        public MFulfillment_ShipmentStatus ShipmentStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public string ShippingVendorId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime ShipmentDateTimeUtc { get; set; }

        public bool CanEdit { get; set; }
        public bool CanPost { get; set; }
        public bool CanProcess { get; set; }
        public bool CanCancel { get; set; }

        public IList<MFulfillment_ShipmentItem> ShipmentItems { get; set; }
    }

    public class MFulfillment_ShipmentItem
    {
        public long ShipmentItemId { get; set; }
        public long ShipmentRequestItemId { get; set; }
        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public int Quantity { get; set; }

        // Convienence properties
        //
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestNumber { get; set; }
        public long FulfillableId { get; set; }
    }
}
