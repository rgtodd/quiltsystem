//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_CreateShipment
    {
        public DateTime ShipmentDateTimeUtc { get; set; }
        public string TrackingCode { get; set; }
        public string ShippingVendorId { get; set; }
        public IList<MFulfillment_CreateShipmentItem> CreateShipmentItems { get; set; }
    }

    public class MFulfillment_CreateShipmentItem
    {
        public long ShipmentRequestItemId { get; set; }
        public int Quantity { get; set; }
    }
}
