//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrderEvent
    {
        public long OrderEventId { get; set; }
        public long OrderTransactionId { get; set; }
        public string EventTypeCode { get; set; }
        public DateTime EventDateTimeUtc { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime ProcessingStatusDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual OrderTransaction OrderTransaction { get; set; }
    }
}
