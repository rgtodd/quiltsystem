//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReturnRequestSummaryView
    {
        public long ReturnRequestId { get; set; }
        public string ReturnRequestNumber { get; set; }
        public string ReturnRequestStatusCode { get; set; }
        public DateTime ReturnRequestStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public long FulfillableId { get; set; }
        public string FulfillableReference { get; set; }
        public string FulfillableName { get; set; }
    }
}
