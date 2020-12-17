//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableSummaryView
    {
        public long FulfillableId { get; set; }
        public string FulfillableReference { get; set; }
        public string FulfillableName { get; set; }
        public string FulfillableStatusCode { get; set; }
        public DateTime FulfillableStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public int? TotalRequestQuantity { get; set; }
        public int? TotalCompleteQuantity { get; set; }
        public int? TotalReturnQuantity { get; set; }
    }
}
