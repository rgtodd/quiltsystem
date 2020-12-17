//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AReturn_ReturnRequestSummary
    {
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableReference { get; set; }
        public string ReturnRequestStatus { get; set; }
        public DateTime ReturnRequestDateTimeUtc { get; set; }
        public DateTime ReturnRequestStatusDateTimeUtc { get; set; }
    }
}
