//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_Payment
    {
        public long SquarePaymentId { get; set; }
        public string SquarePaymentReference { get; set; }
        public long SquareCustomerId { get; set; }
        public string SquareCustomerReference { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public string SquarePaymentRecordId { get; set; }
        public int? VersionNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public IList<MSquare_WebPaymentRequest> WebPaymentRequests { get; set; }
        public IList<MSquare_Refund> Refunds { get; set; }
    }

    public class MSquare_WebPaymentRequest
    {
        public long SquareWebPaymentRequestId { get; set; }
        public string WebRequestTypeCode { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
    }
}
