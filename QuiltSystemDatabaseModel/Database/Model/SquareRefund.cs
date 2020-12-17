//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquareRefund
    {
        public SquareRefund()
        {
            SquareRefundTransactions = new HashSet<SquareRefundTransaction>();
        }

        public long SquareRefundId { get; set; }
        public long SquarePaymentId { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquareRefundRecordId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquarePayment SquarePayment { get; set; }
        public virtual ICollection<SquareRefundTransaction> SquareRefundTransactions { get; set; }
    }
}
