//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Base
{
    public static class CreateTopicReference
    {
        public static string FromUserId(string userId)
        {
            var reference = $"{ReferencePrefixes.User}{userId}";

            return reference;
        }

        public static string FromOrderId(long orderId)
        {
            var reference = $"{ReferencePrefixes.Order}{orderId}";

            return reference;
        }

        public static string FromReturnId(long returnId)
        {
            var reference = $"{ReferencePrefixes.Return}{returnId}";

            return reference;
        }

        public static string FromReturnRequestId(long returnRequestId)
        {
            var reference = $"{ReferencePrefixes.ReturnRequest}{returnRequestId}";

            return reference;
        }

        public static string FromShipmentId(long shipmentId)
        {
            var reference = $"{ReferencePrefixes.Shipment}{shipmentId}";

            return reference;
        }

        public static string FromShipmentRequestId(long shipmentRequestId)
        {
            var reference = $"{ReferencePrefixes.ShipmentRequest}{shipmentRequestId}";

            return reference;
        }

        public static string FromTimestamp(DateTime dateTime)
        {
            var reference = $"{ReferencePrefixes.Timestamp}{dateTime.Ticks}";

            return reference;
        }
    }
}
