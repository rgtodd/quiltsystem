//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_FulfillableSummary
    {
        public long FulfillableId { get; set; }
        public string Name { get; set; }
        public string FulfillableReference { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public MFulfillment_FulfillableStatus FulfillableStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public int TotalFulfillmentRequiredQuantity { get; set; }
        public int TotalFulfillmentCompleteQuantity { get; set; }
        public int TotalFulfillmentReturnQuantity { get; set; }
    }
}
