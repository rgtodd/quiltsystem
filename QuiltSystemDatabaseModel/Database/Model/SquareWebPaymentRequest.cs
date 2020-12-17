//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquareWebPaymentRequest
    {
        public SquareWebPaymentRequest()
        {
            SquarePayloads = new HashSet<SquarePayload>();
        }

        public long SquareWebPaymentRequestId { get; set; }
        public long SquarePaymentId { get; set; }
        public string WebRequestTypeCode { get; set; }
        public string IndempotencyKey { get; set; }
        public string Nonce { get; set; }
        public bool Autocomplete { get; set; }
        public decimal TransactionAmount { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime ProcessingStatusDateTimeUtc { get; set; }
        public string RequestJson { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquarePayment SquarePayment { get; set; }
        public virtual ICollection<SquarePayload> SquarePayloads { get; set; }
    }
}
