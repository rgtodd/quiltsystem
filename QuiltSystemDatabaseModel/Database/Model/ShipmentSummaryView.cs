//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentSummaryView
    {
        public long ShipmentId { get; set; }
        public string ShipmentNumber { get; set; }
        public string ShipmentStatusCode { get; set; }
        public DateTime ShipmentStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string ShippingVendorId { get; set; }
        public string TrackingCode { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableReference { get; set; }
        public string FulfillableName { get; set; }
    }
}
