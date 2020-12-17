//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    public static class ParseReferenceValues
    {
        public static ReferenceValues From(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.Order))
            {
                return new ReferenceValues()
                {
                    OrderId = long.Parse(reference.Substring(ReferencePrefixes.Order.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.Return))
            {
                return new ReferenceValues()
                {
                    ReturnId = long.Parse(reference.Substring(ReferencePrefixes.Return.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.ReturnRequest))
            {
                return new ReferenceValues()
                {
                    ReturnRequestId = long.Parse(reference.Substring(ReferencePrefixes.ReturnRequest.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.Shipment))
            {
                return new ReferenceValues()
                {
                    ShipmentId = long.Parse(reference.Substring(ReferencePrefixes.Shipment.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.ShipmentRequest))
            {
                return new ReferenceValues()
                {
                    ShipmentRequestId = long.Parse(reference.Substring(ReferencePrefixes.ShipmentRequest.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.SquareCustomer))
            {
                return new ReferenceValues()
                {
                    SquareCustomerId = long.Parse(reference.Substring(ReferencePrefixes.SquareCustomer.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.SquarePayment))
            {
                return new ReferenceValues()
                {
                    SquarePaymentId = long.Parse(reference.Substring(ReferencePrefixes.SquarePayment.Length))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.Timestamp))
            {
                return new ReferenceValues()
                {
                    Timestamp = new DateTime(long.Parse(reference.Substring(ReferencePrefixes.Timestamp.Length)))
                };
            }

            if (reference.StartsWith(ReferencePrefixes.User))
            {
                return new ReferenceValues()
                {
                    UserId = reference.Substring(ReferencePrefixes.User.Length)
                };
            }

            throw new ArgumentException($"Unknown reference {reference}.", nameof(reference));
        }
    }
}
