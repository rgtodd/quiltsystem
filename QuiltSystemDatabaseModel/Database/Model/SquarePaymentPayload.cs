//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquarePaymentPayload
    {
        public SquarePaymentPayload()
        {
            SquarePaymentTransactions = new HashSet<SquarePaymentTransaction>();
        }

        public long SquarePayloadId { get; set; }
        public string SquarePaymentRecordId { get; set; }
        public int VersionNumber { get; set; }
        public string PaymentPayloadJson { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime ProcessingStatusDateTimeUtc { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquarePayload SquarePayload { get; set; }
        public virtual ICollection<SquarePaymentTransaction> SquarePaymentTransactions { get; set; }
    }
}
