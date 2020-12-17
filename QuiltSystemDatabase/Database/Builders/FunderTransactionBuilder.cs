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
    public class FunderTransactionBuilder : ITransactionBuilder<FunderTransaction, FunderTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private DateTime m_utcNow;

        private FunderTransaction m_funderTransaction;
        private Funder m_funder;
        private FunderAccount m_funderAccount;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public FunderTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public FunderTransactionBuilder Begin(long funderId, string fundableReference, DateTime utcNow)
        {
            if (fundableReference == null) throw new ArgumentNullException(nameof(fundableReference));

            m_utcNow = utcNow;

            m_funder = m_ctx.Funders.Where(r => r.FunderId == funderId).Single();

            if (fundableReference != null)
            {
                m_funderAccount = m_funder.FunderAccounts.Where(r => r.FundableReference == fundableReference).SingleOrDefault();
                if (m_funderAccount == null)
                {
                    m_funderAccount = new FunderAccount()
                    {
                        FundableReference = fundableReference,
                        UpdateDateTimeUtc = m_utcNow
                    };
                    m_funder.FunderAccounts.Add(m_funderAccount);
                }
            }

            m_funderTransaction = new FunderTransaction()
            {
                TransactionDateTimeUtc = m_utcNow,
                Fund = m_funderAccount,
                FundableReference = fundableReference,
            };
            _ = m_ctx.FunderTransactions.Add(m_funderTransaction);

            return this;
        }

        public FunderTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_funderTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public FunderTransactionBuilder AddFundsReceived(decimal fundsReceivedDelta)
        {
            if (m_funderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_funderTransaction.FundsReceived != 0)
            {
                throw new InvalidOperationException("Funds received already specified.");
            }

            if (fundsReceivedDelta != 0)
            {
                // Assume all funds received are immediately available.
                //
                var fundsAvailableDelta = fundsReceivedDelta;

                m_funderTransaction.FundsReceived = fundsReceivedDelta;
                m_funderTransaction.FundsAvailable = fundsAvailableDelta;
                m_funderAccount.FundsReceived += fundsReceivedDelta;
                m_funderAccount.FundsAvailable += fundsAvailableDelta;
                m_funderAccount.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Funds received updated by {fundsReceivedDelta:c} for {m_funderAccount.FundableReference}.");
                m_description.Append($"Funds available updated by {fundsReceivedDelta:c} for {m_funderAccount.FundableReference}.");
            }

            return this;
        }

        public FunderTransactionBuilder AddFundsAvailable(decimal fundsAvailableDelta)
        {
            if (m_funderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_funderTransaction.FundsAvailable != 0)
            {
                throw new InvalidOperationException("Funds available already specified.");
            }

            if (fundsAvailableDelta != 0)
            {
                m_funderTransaction.FundsAvailable = fundsAvailableDelta;
                m_funderAccount.FundsAvailable += fundsAvailableDelta;
                m_funderAccount.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Funds available updated by {fundsAvailableDelta:c} for {m_funderAccount.FundableReference}.");
            }

            return this;
        }

        public FunderTransactionBuilder AddFundsRefundable(decimal fundsRefundableDelta)
        {
            if (m_funderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_funderTransaction.FundsRefundable != 0)
            {
                throw new InvalidOperationException("Funds refundable already specified.");
            }

            if (fundsRefundableDelta != 0)
            {
                m_funderTransaction.FundsRefundable = fundsRefundableDelta;
                m_funderAccount.FundsRefundable += fundsRefundableDelta;
                m_funderAccount.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Funds refundable updated by {fundsRefundableDelta:c} for {m_funderAccount.FundableReference}.");
            }

            return this;
        }

        public FunderTransactionBuilder AddFundsRefunded(decimal fundsRefundedDelta)
        {
            if (m_funderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_funderTransaction.FundsRefunded != 0)
            {
                throw new InvalidOperationException("Funds refunded already specified.");
            }

            if (fundsRefundedDelta != 0)
            {
                m_funderTransaction.FundsRefunded = fundsRefundedDelta;
                m_funderAccount.FundsRefunded += fundsRefundedDelta;
                m_funderAccount.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Funds refunded updated by {fundsRefundedDelta:c} for {m_funderAccount.FundableReference}.");
            }

            return this;
        }

        public FunderTransactionBuilder AddProcessingFee(decimal processingFeeDelta)
        {
            if (m_funderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_funderTransaction.ProcessingFee != 0)
            {
                throw new InvalidOperationException("Processing fee already specified.");
            }

            if (processingFeeDelta != 0)
            {
                m_funderTransaction.ProcessingFee = processingFeeDelta;
                m_funderAccount.ProcessingFee += processingFeeDelta;
                m_funderAccount.UpdateDateTimeUtc = m_utcNow;

                m_description.Append($"Processing fee updated by {processingFeeDelta:c} for {m_funderAccount.FundableReference}.");
            }

            return this;
        }

        public FunderTransactionBuilder Event(string eventTypeCode)
        {
            var dbFunderEvent = new FunderEvent()
            {
                FunderTransaction = m_funderTransaction,
                EventTypeCode = eventTypeCode,
                EventDateTimeUtc = m_utcNow,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow
            };
            m_funderTransaction.FunderEvents.Add(dbFunderEvent);

            return this;
        }

        public FunderTransaction Create()
        {
            if (m_funder != null)
            {
                m_funder.UpdateDateTimeUtc = m_utcNow;
            }

            m_funderTransaction.Description = m_description.ToString();

            return m_funderTransaction;
        }
    }
}
