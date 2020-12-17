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
    public class ReturnTransactionBuilder : ITransactionBuilder<ReturnTransaction, ReturnTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private Return m_return;
        private DateTime m_utcNow;
        private ReturnTransaction m_returnTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public ReturnTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public ReturnTransactionBuilder Begin(long returnId, DateTime utcNow)
        {
            m_return = m_ctx.Returns.Where(r => r.ReturnId == returnId).Single();
            m_utcNow = utcNow;

            m_returnTransaction = new ReturnTransaction()
            {
                Return = m_return,
                TransactionDateTimeUtc = m_utcNow
            };
            _ = m_ctx.ReturnTransactions.Add(m_returnTransaction);

            return this;
        }

        public ReturnTransactionBuilder Begin(Return dbReturn, string description, DateTime utcNow)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            m_return = dbReturn;
            m_utcNow = utcNow;

            m_returnTransaction = new ReturnTransaction()
            {
                Return = m_return,
                TransactionDateTimeUtc = m_utcNow,
                Description = description
            };
            _ = m_ctx.ReturnTransactions.Add(m_returnTransaction);

            return this;
        }

        public ReturnTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_returnTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public ReturnTransactionBuilder SetStatusTypeCode(string returnStatusCode)
        {
            if (m_returnTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            m_returnTransaction.ReturnStatusCode = returnStatusCode;
            m_returnTransaction.Return.ReturnStatusCode = returnStatusCode;
            m_returnTransaction.Return.ReturnStatusDateTimeUtc = m_utcNow;

            m_description.Append($"Status code set to {returnStatusCode}.");

            return this;
        }

        public ReturnTransactionBuilder Event(string eventTypeCode)
        {
            var dbShpimentEvent = new ReturnEvent()
            {
                ReturnTransaction = m_returnTransaction,
                EventTypeCode = eventTypeCode,
                EventDateTimeUtc = m_utcNow,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow
            };
            m_returnTransaction.ReturnEvents.Add(dbShpimentEvent);

            return this;
        }

        public ReturnTransaction Create()
        {
            m_returnTransaction.Description = m_description.ToString();

            return m_returnTransaction;
        }
    }
}
