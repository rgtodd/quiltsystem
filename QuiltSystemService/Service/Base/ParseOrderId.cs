﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class ParseOrderId
    {
        public static long FromFulfillableReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.Order))
            {
                throw new ArgumentException($"Reference {reference} is not an order.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
        }

        public static long FromFundableReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.Order))
            {
                throw new ArgumentException($"Reference {reference} is not an order.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
        }

        public static long FromSquarePaymentReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.Order))
            {
                throw new ArgumentException($"Reference {reference} is not an order.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
        }

        public static long FromTopicReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.Order))
            {
                throw new ArgumentException($"Reference {reference} is not an order.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.Order.Length));
        }
    }
}
