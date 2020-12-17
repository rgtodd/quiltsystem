//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    public static class ParseOrderItemId
    {
        public static long FromFulfillableItemReference(string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (!reference.StartsWith(ReferencePrefixes.OrderItem))
            {
                throw new ArgumentException($"Reference {reference} is not an order item.");
            }

            return long.Parse(reference.Substring(ReferencePrefixes.OrderItem.Length));
        }
    }
}
