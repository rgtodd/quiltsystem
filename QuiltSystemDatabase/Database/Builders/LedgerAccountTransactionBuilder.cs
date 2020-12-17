//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class LedgerAccountTransactionBuilder
    {
        private readonly QuiltContext m_ctx;

        private DateTime m_postDateTime;
        private DateTime m_utcNow;
        private LedgerTransaction m_ledgerTransaction;

        private decimal m_creditAmount;
        private decimal m_debitAmount;

        public LedgerAccountTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx;
        }

        public LedgerAccountTransactionBuilder Begin(string description, DateTime postDateTime, DateTime utcNow)
        {
            if (m_ledgerTransaction != null) throw new InvalidOperationException("Begin has already been called.");

            m_postDateTime = postDateTime;
            m_utcNow = utcNow;

            m_ledgerTransaction = new LedgerTransaction()
            {
                TransactionDateTimeUtc = m_utcNow,
                PostDateTime = m_postDateTime,
                Description = description
            };
            _ = m_ctx.LedgerTransactions.Add(m_ledgerTransaction);

            return this;
        }

        public LedgerAccountTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            if (m_ledgerTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            m_ledgerTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public LedgerAccountTransactionBuilder Credit(int ledgerAccountNumber, decimal amount, string ledgerReference = null, string salesTaxJurisdiction = null)
        {
            if (m_ledgerTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            return amount > 0
                ? CreditCore(ledgerAccountNumber, amount, ledgerReference, salesTaxJurisdiction)
                : DebitCore(ledgerAccountNumber, -amount, ledgerReference, salesTaxJurisdiction);
        }

        public LedgerAccountTransactionBuilder Debit(int ledgerAccountNumber, decimal amount, string ledgerReference = null, string salesTaxJurisdiction = null)
        {
            if (m_ledgerTransaction == null) throw new InvalidOperationException("Begin has not been called.");

            return amount > 0
                ? DebitCore(ledgerAccountNumber, amount, ledgerReference, salesTaxJurisdiction)
                : CreditCore(ledgerAccountNumber, -amount, ledgerReference, salesTaxJurisdiction);
        }

        public LedgerTransaction Create()
        {
            if (m_debitAmount != m_creditAmount)
            {
                throw new InvalidOperationException(string.Format("Debit ({0}) / credit ({1}) amount mismatch.", m_debitAmount, m_creditAmount));
            }

            m_ledgerTransaction.TransactionAmount = m_debitAmount;

            return m_ledgerTransaction;
        }

        private LedgerAccountTransactionBuilder CreditCore(int ledgerAccountNumber, decimal amount, string ledgerReference, string salesTaxJurisdiction)
        {
            var dbLedgerAccount = m_ctx.LedgerAccount(ledgerAccountNumber);
            var dbLedgerAccountSubtotal = GetLedgerAccountSubtotal(ledgerAccountNumber);

            var dbLedgerTransactionEntry = new LedgerTransactionEntry()
            {
                LedgerTransaction = m_ledgerTransaction,
                LedgerAccountNumber = ledgerAccountNumber,
                LedgerAccountSubtotal = dbLedgerAccountSubtotal,
                TransactionEntryAmount = amount,
                LedgerReference = ledgerReference,
                SalesTaxJurisdiction = salesTaxJurisdiction,
                DebitCreditCode = LedgerAccountCodes.Credit
            };
            _ = m_ctx.LedgerTransactionEntries.Add(dbLedgerTransactionEntry);

            var ledgerAmount = dbLedgerAccount.DebitCreditCode == LedgerAccountCodes.Credit ? amount : -amount;

            dbLedgerAccountSubtotal.Balance += ledgerAmount;
            dbLedgerAccountSubtotal.UpdateDateTimeUtc = m_utcNow;

            m_creditAmount += amount;

            return this;
        }

        private LedgerAccountTransactionBuilder DebitCore(int ledgerAccountNumber, decimal amount, string ledgerReference, string salesTaxJurisdiction)
        {
            var dbLedgerAccount = m_ctx.LedgerAccount(ledgerAccountNumber);
            var dbLedgerAccountSubtotal = GetLedgerAccountSubtotal(ledgerAccountNumber);

            var dbLedgerTransactionEntry = new LedgerTransactionEntry()
            {
                LedgerTransaction = m_ledgerTransaction,
                LedgerAccountNumber = ledgerAccountNumber,
                LedgerAccountSubtotal = dbLedgerAccountSubtotal,
                TransactionEntryAmount = amount,
                LedgerReference = ledgerReference,
                SalesTaxJurisdiction = salesTaxJurisdiction,
                DebitCreditCode = LedgerAccountCodes.Debit
            };
            _ = m_ctx.LedgerTransactionEntries.Add(dbLedgerTransactionEntry);

            var ledgerAmount = dbLedgerAccount.DebitCreditCode == LedgerAccountCodes.Debit ? amount : -amount;

            dbLedgerAccountSubtotal.Balance += ledgerAmount;
            dbLedgerAccountSubtotal.UpdateDateTimeUtc = m_utcNow;

            m_debitAmount += amount;

            return this;
        }

        private LedgerAccountSubtotal GetLedgerAccountSubtotal(int ledgerAccountNumber)
        {
            var dbLedgerAccountSubtotal = m_ctx.LedgerAccountSubtotals.Where(r => r.LedgerAccountNumber == ledgerAccountNumber && r.AccountingYear == m_postDateTime.Year).SingleOrDefault();
            if (dbLedgerAccountSubtotal == null)
            {
                dbLedgerAccountSubtotal = new LedgerAccountSubtotal()
                {
                    LedgerAccountNumber = ledgerAccountNumber,
                    AccountingYear = m_postDateTime.Year,
                    Balance = 0,
                    UpdateDateTimeUtc = m_utcNow
                };
                _ = m_ctx.LedgerAccountSubtotals.Add(dbLedgerAccountSubtotal);
            }

            return dbLedgerAccountSubtotal;
        }
    }
}