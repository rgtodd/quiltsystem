//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Base
{
    internal class LedgerServiceAccountTransactionBuilder
    {
        private ILedgerMicroService LedgerMicroService { get; }

        private MLedger_PostLedgerTransaction m_transaction;

        public LedgerServiceAccountTransactionBuilder(ILedgerMicroService ledgerMicroService)
        {
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException();
        }

        public LedgerServiceAccountTransactionBuilder Begin(string description, DateTime postDateTime)
        {
            if (m_transaction != null) throw new InvalidOperationException("Begin has already been called.");

            m_transaction = new MLedger_PostLedgerTransaction()
            {
                Description = description,
                PostDateTime = postDateTime,
                Entries = new List<MLedger_PostLedgerTransactionEntry>()
            };

            return this;
        }

        public LedgerServiceAccountTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            if (m_transaction == null) throw new InvalidOperationException("Begin has not been called.");

            m_transaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public LedgerServiceAccountTransactionBuilder Credit(int ledgerAccountNumber, decimal amount, string ledgerReference = null, string salesTaxJurisdiction = null)
        {
            if (m_transaction == null) throw new InvalidOperationException("Begin has not been called.");

            if (amount != 0m)
            {
                m_transaction.Entries.Add(
                    new MLedger_PostLedgerTransactionEntry()
                    {
                        LedgerAccountNumber = ledgerAccountNumber,
                        EntryAmount = amount,
                        DebitCreditCode = LedgerAccountCodes.Credit,
                        LedgerReference = ledgerReference,
                        SalesTaxJurisdiction = salesTaxJurisdiction
                    });
            }

            return this;
        }

        public LedgerServiceAccountTransactionBuilder Debit(int ledgerAccountNumber, decimal amount, string ledgerReference = null, string salesTaxJurisdiction = null)
        {
            if (m_transaction == null) throw new InvalidOperationException("Begin has not been called.");

            if (amount != 0m)
            {
                m_transaction.Entries.Add(
                    new MLedger_PostLedgerTransactionEntry()
                    {
                        LedgerAccountNumber = ledgerAccountNumber,
                        EntryAmount = amount,
                        DebitCreditCode = LedgerAccountCodes.Debit,
                        LedgerReference = ledgerReference,
                        SalesTaxJurisdiction = salesTaxJurisdiction
                    }); ;
            }

            return this;
        }

        public async Task<long> CreateAsync()
        {
            var ledgeAccountTransactionId = await LedgerMicroService.PostLedgerAccountTransactionAsync(m_transaction);

            return ledgeAccountTransactionId;
        }
    }
}