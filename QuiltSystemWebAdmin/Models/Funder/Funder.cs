//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Funder
{
    public class Funder
    {
        public AFunding_Funder AFunder { get; }
        public IApplicationLocale Locale { get; }

        public Funder(
            AFunding_Funder aFunder,
            IApplicationLocale locale)
        {
            AFunder = aFunder;
            Locale = locale;
        }

        public MFunding_Funder MFunder => AFunder.MFunder;
        public MFunding_FundableSummaryList MFundables => AFunder.MFundables;
        public MFunding_FunderTransactionSummaryList MTransactions => AFunder.MTransactions;
        public MFunding_FunderEventLogSummaryList MEvents => AFunder.MEvents;

        [Display(Name = "Funder ID")]
        public long FunderId => MFunder.FunderId;

        [Display(Name = "Funder Reference")]
        public string FunderReference => MFunder.FunderReference;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Update Date/Time")]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFunder.UpdateDateTimeUtc);

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Funds Received")]
        public decimal TotalFundsReceived => MFunder.TotalFundsReceived;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Funds Available")]
        public decimal TotalFundsAvailable => MFunder.TotalFundsAvailable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Processing Fee")]
        public decimal TotalProcessingFee => MFunder.TotalProcessingFee;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Funds Refunded")]
        public decimal TotalFundsRefunded => MFunder.TotalFundsRefunded;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Funds Refundable")]
        public decimal TotalFundsRefundable => MFunder.TotalFundsRefundable;

        private IList<FunderAccount> m_accounts;
        public IList<FunderAccount> Accounts
        {
            get
            {
                if (m_accounts == null)
                {
                    m_accounts = MFunder.Accounts != null
                        ? MFunder.Accounts
                            .Select(r => new FunderAccount(r, Locale))
                            .ToList()
                        : new List<FunderAccount>(0);
                }
                return m_accounts;
            }
        }

        private IList<FunderFundable> m_fundables;
        public IList<FunderFundable> Fundables
        {
            get
            {
                if (m_fundables == null)
                {
                    m_fundables = MFundables != null
                        ? MFundables.Summaries
                            .Select(r => new FunderFundable(r, Locale))
                            .ToList()
                        : new List<FunderFundable>(0);
                }
                return m_fundables;
            }
        }

        private IList<FunderTransaction> m_transactions;
        public IList<FunderTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions != null
                        ? MTransactions.Summaries
                            .Select(r => new FunderTransaction(r, Locale))
                            .ToList()
                        : new List<FunderTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<FunderEvent> m_events;
        public IList<FunderEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents != null
                        ? MEvents.Summaries
                            .Select(r => new FunderEvent(r, Locale))
                            .ToList()
                        : new List<FunderEvent>(0);
                }
                return m_events;
            }
        }
    }

    public class FunderAccount
    {
        public MFunding_FunderAccount MFunderAccount { get; }
        public IApplicationLocale Locale { get; }

        public FunderAccount(
            MFunding_FunderAccount mFunderAccount,
            IApplicationLocale locale)
        {
            MFunderAccount = mFunderAccount;
            Locale = locale;
        }

        public string FundableReference => MFunderAccount.FundableReference;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Received")]
        public decimal FundsReceived => MFunderAccount.FundsReceived;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Available")]
        public decimal FundsAvailable => MFunderAccount.FundsAvailable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Refunded")]
        public decimal FundsRefunded => MFunderAccount.FundsRefunded;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Refundable")]
        public decimal FundsRefundable => MFunderAccount.FundsRefundable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Processing Fee")]
        public decimal ProcessingFee => MFunderAccount.ProcessingFee;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Update Date/Time")]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFunderAccount.UpdateDateTimeUtc);
    }

    public class FunderFundable
    {
        public MFunding_FundableSummary MFundable { get; }
        public IApplicationLocale Locale { get; }

        public FunderFundable(
            MFunding_FundableSummary mFundable,
            IApplicationLocale locale)
        {
            MFundable = mFundable;
            Locale = locale;
        }

        [Display(Name = "Fundable ID")]
        public long FundableId => MFundable.FundableId;

        [Display(Name = "Fundable Reference")]
        public string FundableReference => MFundable.FundableReference;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Required")]
        public decimal FundsRequired => MFundable.FundsRequired;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Received")]
        public decimal FundsReceived => MFundable.FundsReceived;
    }

    public class FunderTransaction
    {
        public MFunding_FunderTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public FunderTransaction(
            MFunding_FunderTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.FunderTransactionId}";

        public long FunderTransactionId => MTransaction.FunderTransactionId;

        public long Funderd => MTransaction.FunderId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsReceived => MTransaction.FundsReceived;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsAvailable => MTransaction.FundsAvailable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRefunded => MTransaction.FundsRefunded;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRefundable => MTransaction.FundsRefundable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ProcessingFee => MTransaction.ProcessingFee;
    }

    public class FunderEvent
    {
        public MFunding_FunderEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public FunderEvent(
            MFunding_FunderEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.FunderEventId}";

        public long FunderEventId => MEvent.FunderEventId;

        public long FunderTransactionId => MEvent.FunderTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}
