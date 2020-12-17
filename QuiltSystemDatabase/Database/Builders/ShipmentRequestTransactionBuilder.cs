//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class ShipmentRequestTransactionBuilder : ITransactionBuilder<ShipmentRequestTransaction, ShipmentRequestTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private ShipmentRequest m_shipmentRequest;
        private DateTime m_utcNow;
        private ShipmentRequestTransaction m_shipmentRequestTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public ShipmentRequestTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public ShipmentRequestTransactionBuilder Begin(long shipmentRequestId, DateTime utcNow)
        {
            m_shipmentRequest = m_ctx.ShipmentRequests.Where(r => r.ShipmentRequestId == shipmentRequestId).Single();
            m_utcNow = utcNow;

            m_shipmentRequestTransaction = new ShipmentRequestTransaction()
            {
                ShipmentRequest = m_shipmentRequest,
                TransactionDateTimeUtc = m_utcNow
            };
            _ = m_ctx.ShipmentRequestTransactions.Add(m_shipmentRequestTransaction);

            return this;
        }

        public ShipmentRequestTransactionBuilder Begin(ShipmentRequest dbShipmentRequest, string description, DateTime utcNow)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            m_shipmentRequest = dbShipmentRequest;
            m_utcNow = utcNow;

            m_shipmentRequestTransaction = new ShipmentRequestTransaction()
            {
                ShipmentRequest = m_shipmentRequest,
                TransactionDateTimeUtc = m_utcNow,
                Description = description
            };
            _ = m_ctx.ShipmentRequestTransactions.Add(m_shipmentRequestTransaction);

            return this;
        }

        public ShipmentRequestTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_shipmentRequestTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public ShipmentRequestTransactionBuilder SetStatusTypeCode(string shipmentRequestStatusCode)
        {
            if (m_shipmentRequestTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            m_shipmentRequestTransaction.ShipmentRequestStatusCode = shipmentRequestStatusCode;
            m_shipmentRequestTransaction.ShipmentRequest.ShipmentRequestStatusCode = shipmentRequestStatusCode;
            m_shipmentRequestTransaction.ShipmentRequest.ShipmentRequestStatusDateTimeUtc = m_utcNow;

            m_description.Append($"Status code set to {shipmentRequestStatusCode}.");

            return this;
        }

        public ShipmentRequestTransactionBuilder Event(string eventTypeCode)
        {
            var dbShpimentRequestEvent = new ShipmentRequestEvent()
            {
                ShipmentRequestTransaction = m_shipmentRequestTransaction,
                EventTypeCode = eventTypeCode,
                EventDateTimeUtc = m_utcNow,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow
            };
            m_shipmentRequestTransaction.ShipmentRequestEvents.Add(dbShpimentRequestEvent);

            return this;
        }

        public ShipmentRequestTransaction Create()
        {
            m_shipmentRequestTransaction.Description = m_description.ToString();

            return m_shipmentRequestTransaction;
        }
    }
}
