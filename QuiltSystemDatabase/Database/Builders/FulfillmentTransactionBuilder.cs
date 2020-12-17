//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class FulfillmentTransactionBuilder : ITransactionBuilder<FulfillableTransaction, FulfillmentTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private DateTime m_utcNow;
        private FulfillableTransaction m_fulfillableTransaction;
        private Fulfillable m_fulfillable;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public FulfillmentTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public FulfillmentTransactionBuilder Begin(DateTime utcNow)
        {
            m_utcNow = utcNow;

            m_fulfillableTransaction = new FulfillableTransaction()
            {
                TransactionDateTimeUtc = m_utcNow
            };
            _ = m_ctx.FulfillableTransactions.Add(m_fulfillableTransaction);

            return this;
        }

        public FulfillmentTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_fulfillableTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public FulfillmentTransactionBuilder SetRequestQuantity(long fulfillableItemId, int requestQuantity)
        {
            if (m_fulfillableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbFulfillableTransactionItem = GetFulfillableTransactionItem(fulfillableItemId);
            if (dbFulfillableTransactionItem.RequestQuantity != 0)
            {
                throw new InvalidOperationException("Request quantity already specified.");
            }

            if (requestQuantity != 0)
            {
                dbFulfillableTransactionItem.RequestQuantity = requestQuantity;
                dbFulfillableTransactionItem.FulfillableItem.RequestQuantity += requestQuantity;
                dbFulfillableTransactionItem.FulfillableItem.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Fulfillable item {fulfillableItemId} request quantity updated by {requestQuantity}.");
            }

            return this;
        }

        public FulfillmentTransactionBuilder SetCompleteQuantity(long fulfillableItemId, int completeQuantity)
        {
            if (m_fulfillableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbFulfillableTransactionItem = GetFulfillableTransactionItem(fulfillableItemId);
            if (dbFulfillableTransactionItem.CompleteQuantity != 0)
            {
                throw new InvalidOperationException("Request complete already specified.");
            }

            if (completeQuantity != 0)
            {
                dbFulfillableTransactionItem.CompleteQuantity = completeQuantity;
                dbFulfillableTransactionItem.FulfillableItem.CompleteQuantity += completeQuantity;
                dbFulfillableTransactionItem.FulfillableItem.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Fulfillable item {fulfillableItemId} complete quantity updated by {completeQuantity}.");
            }

            return this;
        }

        public FulfillmentTransactionBuilder SetReturnQuantity(long fulfillableItemId, int returnQuantity)
        {
            if (m_fulfillableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbFulfillableTransactionItem = GetFulfillableTransactionItem(fulfillableItemId);
            if (dbFulfillableTransactionItem.ReturnQuantity != 0)
            {
                throw new InvalidOperationException("Request return already specified.");
            }

            if (returnQuantity != 0)
            {
                dbFulfillableTransactionItem.ReturnQuantity = returnQuantity;
                dbFulfillableTransactionItem.FulfillableItem.ReturnQuantity += returnQuantity;
                dbFulfillableTransactionItem.FulfillableItem.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Fulfillable item {fulfillableItemId} return quantity updated by {returnQuantity}.");
            }

            return this;
        }

        public FulfillmentTransactionBuilder Event(string eventTypeCode)
        {
            var dbFulfillableEvent = new FulfillableEvent()
            {
                FulfillableTransaction = m_fulfillableTransaction,
                EventTypeCode = eventTypeCode,
                EventDateTimeUtc = m_utcNow,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow
            };
            m_fulfillableTransaction.FulfillableEvents.Add(dbFulfillableEvent);

            return this;
        }

        public FulfillableTransaction Create()
        {
            // Update the status of any affected fulfillables.
            //
            var fulfillableStatus = FulfillableStatusCodes.Closed; // Assume all items are closed.
            foreach (var dbFulfillableItem in m_fulfillable.FulfillableItems)
            {
                if (dbFulfillableItem.CompleteQuantity != dbFulfillableItem.RequestQuantity)
                {
                    fulfillableStatus = FulfillableStatusCodes.Open;
                    break;
                }
            }

            if (m_fulfillable.FulfillableStatusCode != fulfillableStatus)
            {
                m_fulfillable.FulfillableStatusCode = fulfillableStatus;
                m_fulfillable.FulfillableStatusDateTimeUtc = m_utcNow;
                m_description.Append($"Status set to {fulfillableStatus}.");
            }

            m_fulfillableTransaction.Description = m_description.ToString();

            return m_fulfillableTransaction;
        }

        private FulfillableTransactionItem GetFulfillableTransactionItem(long fulfillableItemId)
        {
            // Load the fulfillable.
            //
            if (m_fulfillable == null)
            {
                m_fulfillable = m_ctx.Fulfillables.Where(r => r.FulfillableItems.Any(r => r.FulfillableItemId == fulfillableItemId)).Include(r => r.FulfillableItems).First();
            }

            // Find the specified fulfillable item.
            //
            var dbFulfillableItem = m_fulfillable.FulfillableItems.Where(r => r.FulfillableItemId == fulfillableItemId).First();

            // Retrieve or create a corresponding transaction item for the fulfillable item.
            //
            var dbFulfillableTransactionItem = m_fulfillableTransaction.FulfillableTransactionItems.Where(r => r.FulfillableItemId == fulfillableItemId).SingleOrDefault();
            if (dbFulfillableTransactionItem == null)
            {
                dbFulfillableTransactionItem = new FulfillableTransactionItem()
                {
                    FulfillableTransaction = m_fulfillableTransaction,
                    FulfillableItem = dbFulfillableItem,
                    RequestQuantity = 0,
                    ReturnQuantity = 0,
                    CompleteQuantity = 0
                };
                m_fulfillableTransaction.FulfillableTransactionItems.Add(dbFulfillableTransactionItem);
            }

            return dbFulfillableTransactionItem;
        }
    }
}
