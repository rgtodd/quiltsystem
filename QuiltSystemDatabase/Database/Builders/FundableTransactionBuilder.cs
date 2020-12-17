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
    public class FundableTransactionBuilder : ITransactionBuilder<FundableTransaction, FundableTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private DateTime m_utcNow;

        private Fundable m_fundable;
        private FundableTransaction m_fundableTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public FundableTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public FundableTransactionBuilder Begin(long fundableId, DateTime utcNow)
        {
            m_utcNow = utcNow;

            m_fundable = m_ctx.Fundables.Where(r => r.FundableId == fundableId).Single();

            m_fundableTransaction = new FundableTransaction()
            {
                TransactionDateTimeUtc = m_utcNow,
                Fundable = m_fundable
            };
            _ = m_ctx.FundableTransactions.Add(m_fundableTransaction);

            return this;
        }

        public FundableTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_fundableTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public FundableTransactionBuilder AddFundsRequiredIncome(decimal fundsRequiredIncomeDelta)
        {
            if (m_fundableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_fundableTransaction.FundsRequiredIncome != 0)
            {
                throw new InvalidOperationException("Income funds required already specified.");
            }

            if (fundsRequiredIncomeDelta != 0)
            {
                m_fundableTransaction.FundsRequiredIncome = fundsRequiredIncomeDelta;
                m_fundableTransaction.Fundable.FundsRequiredIncome += fundsRequiredIncomeDelta;
                m_fundableTransaction.Fundable.FundsRequiredTotal =
                    m_fundableTransaction.Fundable.FundsRequiredIncome +
                    m_fundableTransaction.Fundable.FundsRequiredSalesTax;

                m_description.Append($"Income funds required updated by {fundsRequiredIncomeDelta:c}.");
            }

            return this;
        }

        public FundableTransactionBuilder AddFundsRequiredSalesTax(decimal fundsRequiredSalesTaxDelta)
        {
            if (m_fundableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_fundableTransaction.FundsRequiredSalesTax != 0)
            {
                throw new InvalidOperationException("Sales tax funds required already specified.");
            }

            if (fundsRequiredSalesTaxDelta != 0)
            {
                m_fundableTransaction.FundsRequiredSalesTax = fundsRequiredSalesTaxDelta;
                m_fundableTransaction.Fundable.FundsRequiredSalesTax += fundsRequiredSalesTaxDelta;
                m_fundableTransaction.Fundable.FundsRequiredTotal =
                    m_fundableTransaction.Fundable.FundsRequiredIncome +
                    m_fundableTransaction.Fundable.FundsRequiredSalesTax;

                m_description.Append($"Sales tax funds required updated by {fundsRequiredSalesTaxDelta:c}.");
            }

            return this;
        }

        public FundableTransactionBuilder AddFundsReceived(decimal fundsReceivedDelta)
        {
            if (m_fundableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            if (m_fundableTransaction.FundsReceived != 0)
            {
                throw new InvalidOperationException("Funding received already specified.");
            }

            if (fundsReceivedDelta != 0)
            {
                m_fundableTransaction.FundsReceived = fundsReceivedDelta;
                m_fundableTransaction.Fundable.FundsReceived += fundsReceivedDelta;

                m_description.Append($"Funds received updated by {fundsReceivedDelta:c}.");
            }

            return this;
        }

        public FundableTransactionBuilder SetFundsRequiredSalesTaxJurisdiction(string salesTaxJurisdiction)
        {
            if (m_fundableTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_fundableTransaction.FundsRequiredSalesTaxJurisdiction != null)
            {
                throw new InvalidOperationException("Sales tax jurisdiction already specified.");
            }

            if (salesTaxJurisdiction != null)
            {
                m_fundableTransaction.FundsRequiredSalesTaxJurisdiction = salesTaxJurisdiction;
                m_fundableTransaction.Fundable.FundsRequiredSalesTaxJurisdiction = salesTaxJurisdiction;

                m_description.Append($"Sales tax jurisdiction set to {salesTaxJurisdiction}.");
            }

            return this;
        }

        public FundableTransactionBuilder Event(string eventTypeCode)
        {
            var dbFundableEvent = new FundableEvent()
            {
                FundableTransaction = m_fundableTransaction,
                EventTypeCode = eventTypeCode,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow,
                EventDateTimeUtc = m_utcNow
            };
            m_fundableTransaction.FundableEvents.Add(dbFundableEvent);

            return this;
        }

        public FundableTransaction Create()
        {
            if (m_fundable != null)
            {
                m_fundable.UpdateDateTimeUtc = m_utcNow;
            }

            m_fundableTransaction.Description = m_description.ToString();

            return m_fundableTransaction;
        }
    }
}
