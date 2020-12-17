//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class CreateFulfillableItemReference
    {
        public static string FromOrderItemId(long orderItemId)
        {
            var reference = $"{ReferencePrefixes.OrderItem}{orderItemId}";

            return reference;
        }

        public static string FromTimestamp(DateTime dateTime)
        {
            var reference = $"{ReferencePrefixes.Timestamp}{dateTime.Ticks}";

            return reference;
        }
    }
}
