//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using RichTodd.QuiltSystem.Database.Builders;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal static class CreateUnitOfWork
    {
        public static UnitOfWork CartSubmission(long orderId)
        {
            var reference = $"SUBMIT-CART={orderId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork SquarePayment(long transactionId)
        {
            var reference = $"SQUARE-PAYMENT={transactionId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork SquareWebhook(long transactionId)
        {
            var reference = $"SQUARE-WEBHOOK={transactionId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork ProcessShipment(long shipmentId)
        {
            var reference = $"PROCESS-SHIPMENT={shipmentId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork ProcessReturn(long returnId)
        {
            var reference = $"PROCESS-RETURN={returnId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork PostReturn(long returnId)
        {
            var reference = $"POST-RETURN={returnId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork PostShipment(long shipmentId)
        {
            var reference = $"POST-SHIPMENT={shipmentId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork CancelShipment(long shipmentId)
        {
            var reference = $"CANCEL-SHIPMENT={shipmentId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork CancelReturn(long returnId)
        {
            var reference = $"CANCEL-RETURN={returnId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork CancelShipmentRequest(long shipmentRequestId)
        {
            var reference = $"CANCEL-SHIPMENT-REQUEST={shipmentRequestId}";

            return new UnitOfWork(reference);
        }

        public static UnitOfWork Timestamp(DateTime dateTime)
        {
            var reference = $"TIME={dateTime:yyyyMMdd:HHmmss:fff}";

            return new UnitOfWork(reference);
        }
    }
}
