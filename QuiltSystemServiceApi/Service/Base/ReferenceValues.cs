//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    public class ReferenceValues
    {
        public long? OrderId { get; set; }
        public long? ReturnId { get; set; }
        public long? ReturnRequestId { get; set; }
        public long? ShipmentId { get; set; }
        public long? ShipmentRequestId { get; set; }
        public long? SquareCustomerId { get; set; }
        public long? SquarePaymentId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            return
                OrderId != null ? $"Order {OrderId}"
                : ReturnId != null ? $"Return {ReturnId}"
                : ReturnRequestId != null ? $"Return Request {ReturnRequestId}"
                : ShipmentId != null ? $"Shipment {ShipmentId}"
                : ShipmentRequestId != null ? $"Shipment Request {ShipmentRequestId}"
                : SquareCustomerId != null ? $"Square Customer {SquareCustomerId}"
                : SquarePaymentId != null ? $"Square Payment {SquarePaymentId}"
                : Timestamp != null ? $"Timestamp {Timestamp}"
                : UserId != null ? $"User ID {UserId}"
                : string.Empty;
        }
    }
}
