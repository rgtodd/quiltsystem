//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MSquare_Event
    {
        public string EventTypeCode { get; set; }
        public long SquareCustomerId { get; set; }
        public string SquareCustomerReference { get; set; }
        public long SquarePaymentId { get; set; }
        public string SquarePaymentReference { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal ProcessingFeeAmount { get; set; }
        public decimal NetPaymentAmount { get; set; }
        public long? SquarePaymentTransactionId { get; set; }
        public long? SquareRefundTransactionId { get; set; }
        public string UnitOfWork { get; set; }
    }
}
