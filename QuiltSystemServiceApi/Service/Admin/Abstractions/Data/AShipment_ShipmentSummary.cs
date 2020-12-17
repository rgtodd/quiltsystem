//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AShipment_ShipmentSummary
    {
        public long ShipmentId { get; set; }
        public string ShipmentNumber { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
        public string ShipmentStatusName { get; set; }
        public string ShippingVendorId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
    }
}
