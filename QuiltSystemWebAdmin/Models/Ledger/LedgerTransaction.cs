//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Ledger
{
    public class LedgerTransaction
    {
        public MLedger_LedgerTransaction MLedgerTransaction { get; }
        public IApplicationLocale Locale { get; }

        public LedgerTransaction(
            MLedger_LedgerTransaction mLedgerTransaction,
            IApplicationLocale locale)
        {
            MLedgerTransaction = mLedgerTransaction;
            Locale = locale;
        }

        [Display(Name = "Ledger Transaction ID")]
        public long LedgerTransactionId => MLedgerTransaction.LedgerTransactionId;

        [Display(Name = "Transaction Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MLedgerTransaction.TransactionDateTimeUtc);

        [Display(Name = "Post Date")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime PostDateTime => MLedgerTransaction.PostDateTime;

        [Display(Name = "Description")]
        public string Description => MLedgerTransaction.Description;

        [Display(Name = "Unit of Work")]
        public string UnitOfWork => MLedgerTransaction.UnitOfWork;

        private IList<LedgerTransactionItem> m_debitItems;
        public IList<LedgerTransactionItem> DebitItems
        {
            get
            {
                if (m_debitItems == null)
                {
                    m_debitItems = MLedgerTransaction.Entries
                        .Where(r => r.DebitCreditCode == LedgerAccountCodes.Debit)
                        .Select(r => new LedgerTransactionItem(r)).ToList();
                }

                return m_debitItems;
            }
        }

        private IList<LedgerTransactionItem> m_creditItems;
        public IList<LedgerTransactionItem> CreditItems
        {
            get
            {
                if (m_creditItems == null)
                {
                    m_creditItems = MLedgerTransaction.Entries
                        .Where(r => r.DebitCreditCode == LedgerAccountCodes.Credit)
                        .Select(r => new LedgerTransactionItem(r)).ToList();
                }

                return m_creditItems;
            }
        }
    }

    public class LedgerTransactionItem
    {
        public MLedger_LedgerTransactionEntry MLedgerTransactionEntry { get; }

        public LedgerTransactionItem(
            MLedger_LedgerTransactionEntry mLedgerTransactionEntry)
        {
            MLedgerTransactionEntry = mLedgerTransactionEntry;
        }

        public long LedgerTransactionEntryId => MLedgerTransactionEntry.LedgerTransactionEntryId;

        public int LedgerAccountNumber => MLedgerTransactionEntry.LedgerAccountNumber;

        public string LedgerAccountName => MLedgerTransactionEntry.LedgerAccountName;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat, ApplyFormatInEditMode = true)]
        public decimal EntryAmount => MLedgerTransactionEntry.EntryAmount;

        public string DebitCreditCode => MLedgerTransactionEntry.DebitCreditCode;

        public string LedgerReference => MLedgerTransactionEntry.LedgerReference;

        public string SalesTaxJurisdiction => MLedgerTransactionEntry.SalesTaxJurisdiction;
    }
}
