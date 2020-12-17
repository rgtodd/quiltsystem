//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentRequestSummaryView
    {
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestNumber { get; set; }
        public string ShipmentRequestStatusCode { get; set; }
        public DateTime ShipmentRequestStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
    }
}
