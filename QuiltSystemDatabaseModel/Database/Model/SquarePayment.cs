//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class SquarePayment
    {
        public SquarePayment()
        {
            SquarePaymentTransactions = new HashSet<SquarePaymentTransaction>();
            SquareRefunds = new HashSet<SquareRefund>();
            SquareWebPaymentRequests = new HashSet<SquareWebPaymentRequest>();
        }

        public long SquarePaymentId { get; set; }
        public string SquarePaymentReference { get; set; }
        public long SquareCustomerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquarePaymentRecordId { get; set; }
        public int? VersionNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual SquareCustomer SquareCustomer { get; set; }
        public virtual ICollection<SquarePaymentTransaction> SquarePaymentTransactions { get; set; }
        public virtual ICollection<SquareRefund> SquareRefunds { get; set; }
        public virtual ICollection<SquareWebPaymentRequest> SquareWebPaymentRequests { get; set; }
    }
}
