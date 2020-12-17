//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AShipment_ShipmentRequestSummary
    {
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestNumber { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
        public string ShipmentRequestStatus { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
    }
}
