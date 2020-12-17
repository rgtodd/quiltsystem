//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class TryParseOrderItemId
    {
        public static bool FromFulfillableItemReference(string reference, out long orderItemId)
        {
            if (string.IsNullOrEmpty(reference)) throw new ArgumentNullException(nameof(reference));

            if (reference.StartsWith(ReferencePrefixes.OrderItem))
            {
                orderItemId = long.Parse(reference.Substring(ReferencePrefixes.OrderItem.Length));
                return true;
            }
            else
            {
                orderItemId = default;
                return false;
            }
        }
    }
}
