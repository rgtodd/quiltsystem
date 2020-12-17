//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class TryParseOrderId
    {
        public static bool FromFulfillableReference(string reference, out long orderId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.Order))
            {
                orderId = long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
                return true;
            }
            else
            {
                orderId = default;
                return false;
            }
        }

        public static bool FromFundableReference(string reference, out long orderId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.Order))
            {
                orderId = long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
                return true;
            }
            else
            {
                orderId = default;
                return false;
            }
        }

        public static bool FromSquarePaymentReference(string reference, out long orderId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.Order))
            {
                orderId = long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
                return true;
            }
            else
            {
                orderId = default;
                return false;
            }
        }
    }
}
