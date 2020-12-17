//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnRequestSummary
    {
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public MFulfillment_ReturnRequestStatus ReturnRequestStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
    }
}
