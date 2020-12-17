//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquareRefundTransaction
    {
        public SquareRefundTransaction()
        {
            SquareRefundEvents = new HashSet<SquareRefundEvent>();
        }

        public long SquareRefundTransactionId { get; set; }
        public long SquareRefundId { get; set; }
        public long SquarePayloadId { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquareRefundRecordId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquareRefundPayload SquarePayload { get; set; }
        public virtual SquareRefund SquareRefund { get; set; }
        public virtual ICollection<SquareRefundEvent> SquareRefundEvents { get; set; }
    }
}
