//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquarePaymentTransaction
    {
        public SquarePaymentTransaction()
        {
            SquarePaymentEvents = new HashSet<SquarePaymentEvent>();
        }

        public long SquarePaymentTransactionId { get; set; }
        public long SquarePaymentId { get; set; }
        public long SquarePayloadId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquarePaymentRecordId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquarePaymentPayload SquarePayload { get; set; }
        public virtual SquarePayment SquarePayment { get; set; }
        public virtual ICollection<SquarePaymentEvent> SquarePaymentEvents { get; set; }
    }
}
