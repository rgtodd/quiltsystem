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
    public class ReturnRequestTransactionBuilder : ITransactionBuilder<ReturnRequestTransaction, ReturnRequestTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private ReturnRequest m_returnRequest;
        private DateTime m_utcNow;
        private ReturnRequestTransaction m_returnRequestTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public ReturnRequestTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public ReturnRequestTransactionBuilder Begin(long returnRequestId, DateTime utcNow)
        {
            m_returnRequest = m_ctx.ReturnRequests.Where(r => r.ReturnRequestId == returnRequestId).Single();
            m_utcNow = utcNow;

            m_returnRequestTransaction = new ReturnRequestTransaction()
            {
                ReturnRequest = m_returnRequest,
                TransactionDateTimeUtc = m_utcNow
            };
            _ = m_ctx.ReturnRequestTransactions.Add(m_returnRequestTransaction);

            return this;
        }

        public ReturnRequestTransactionBuilder Begin(ReturnRequest dbReturnRequest, string description, DateTime utcNow)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            m_returnRequest = dbReturnRequest;
            m_utcNow = utcNow;

            m_returnRequestTransaction = new ReturnRequestTransaction()
            {
                ReturnRequest = m_returnRequest,
                TransactionDateTimeUtc = m_utcNow,
                Description = description
            };
            _ = m_ctx.ReturnRequestTransactions.Add(m_returnRequestTransaction);

            return this;
        }

        public ReturnRequestTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_returnRequestTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public ReturnRequestTransactionBuilder SetStatusTypeCode(string returnRequestStatusCode)
        {
            if (m_returnRequestTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            m_returnRequestTransaction.ReturnRequestStatusCode = returnRequestStatusCode;
            m_returnRequestTransaction.ReturnRequest.ReturnRequestStatusCode = returnRequestStatusCode;
            m_returnRequestTransaction.ReturnRequest.ReturnRequestStatusDateTimeUtc = m_utcNow;

            m_description.Append($"Status code set to {returnRequestStatusCode}.");

            return this;
        }

        public ReturnRequestTransactionBuilder Event(string eventTypeCode)
        {
            var dbShpimentRequestEvent = new ReturnRequestEvent()
            {
                ReturnRequestTransaction = m_returnRequestTransaction,
                EventTypeCode = eventTypeCode,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                EventDateTimeUtc = m_utcNow
            };
            m_returnRequestTransaction.ReturnRequestEvents.Add(dbShpimentRequestEvent);

            return this;
        }

        public ReturnRequestTransaction Create()
        {
            m_returnRequestTransaction.Description = m_description.ToString();

            return m_returnRequestTransaction;
        }
    }
}
