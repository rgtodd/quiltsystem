//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_PaymentSummary
    {
        public long SquarePaymentId { get; set; }
        public string SquarePaymentReference { get; set; }
        public long SquareCustomerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquarePaymentRecordId { get; set; }
        public int? VersionNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
