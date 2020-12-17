//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AReturn_ReturnSummary
    {
        public long ReturnId { get; set; }
        public string ReturnNumber { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
        public string ReturnStatusName { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public int ShippingVendorId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime ReturnDateTimeUtc { get; set; }
    }
}
