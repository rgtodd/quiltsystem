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
    public class ShipmentTransactionBuilder : ITransactionBuilder<ShipmentTransaction, ShipmentTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private Shipment m_shipment;
        private DateTime m_utcNow;
        private ShipmentTransaction m_shipmentTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public ShipmentTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public ShipmentTransactionBuilder Begin(long shipmentId, DateTime utcNow)
        {
            m_shipment = m_ctx.Shipments.Where(r => r.ShipmentId == shipmentId).Single();
            m_utcNow = utcNow;

            m_shipmentTransaction = new ShipmentTransaction()
            {
                Shipment = m_shipment,
                TransactionDateTimeUtc = m_utcNow
            };
            _ = m_ctx.ShipmentTransactions.Add(m_shipmentTransaction);

            return this;
        }

        public ShipmentTransactionBuilder Begin(Shipment dbShipment, string description, DateTime utcNow)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            m_shipment = dbShipment;
            m_utcNow = utcNow;

            m_shipmentTransaction = new ShipmentTransaction()
            {
                Shipment = m_shipment,
                TransactionDateTimeUtc = m_utcNow,
                Description = description
            };
            _ = m_ctx.ShipmentTransactions.Add(m_shipmentTransaction);

            return this;
        }

        public ShipmentTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_shipmentTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public ShipmentTransactionBuilder SetStatusTypeCode(string shipmentStatusCode)
        {
            if (m_shipmentTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            m_shipmentTransaction.ShipmentStatusCode = shipmentStatusCode;
            m_shipmentTransaction.Shipment.ShipmentStatusCode = shipmentStatusCode;
            m_shipmentTransaction.Shipment.ShipmentStatusDateTimeUtc = m_utcNow;

            m_description.Append($"Status code set to {shipmentStatusCode}.");

            return this;
        }

        public ShipmentTransactionBuilder Event(string eventTypeCode)
        {
            var dbShpimentEvent = new ShipmentEvent()
            {
                ShipmentTransaction = m_shipmentTransaction,
                EventTypeCode = eventTypeCode,
                EventDateTimeUtc = m_utcNow,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow
            };
            m_shipmentTransaction.ShipmentEvents.Add(dbShpimentEvent);

            return this;
        }

        public ShipmentTransaction Create()
        {
            m_shipmentTransaction.Description = m_description.ToString();

            return m_shipmentTransaction;
        }
    }
}
