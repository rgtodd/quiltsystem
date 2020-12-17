//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquarePayload
    {
        public long SquarePayloadId { get; set; }
        public long? SquareWebPaymentRequestId { get; set; }
        public string PayloadTypeCode { get; set; }
        public string PayloadJson { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime ProcessingStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquareWebPaymentRequest SquareWebPaymentRequest { get; set; }
        public virtual SquarePaymentPayload SquarePaymentPayload { get; set; }
        public virtual SquareRefundPayload SquareRefundPayload { get; set; }
    }
}
