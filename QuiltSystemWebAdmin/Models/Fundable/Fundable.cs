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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fundable
{
    public class Fundable
    {
        public AFunding_Fundable AFundable { get; }
        public IApplicationLocale Locale { get; }

        public Fundable(
            AFunding_Fundable aFundable,
            IApplicationLocale locale)
        {
            AFundable = aFundable;
            Locale = locale;
        }

        public MFunding_Fundable MFundable => AFundable.MFundable;
        public MFunding_FunderSummaryList MFunders => AFundable.MFunders;
        public MFunding_FundableTransactionSummaryList MTransactions => AFundable.MTransactions;
        public MFunding_FundableEventLogSummaryList MEvents => AFundable.MEvents;

        [Display(Name = "Fundable ID")]
        public long FundableId => MFundable.FundableId;

        [Display(Name = "Fundable Reference")]
        public string FundableReference => MFundable.FundableReference;

        [Display(Name = "Funds Required - Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequiredTotal => MFundable.FundsRequiredTotal;

        [Display(Name = "Funds Required - Income")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequiredIncome => MFundable.FundsRequiredIncome;

        [Display(Name = "Funds Required - Sales Tax")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequiredSalesTax => MFundable.FundsRequiredSalesTax;

        [Display(Name = "Funds Required - Sales Tax Jurisdiction")]
        public string FundsRequiredSalesTaxJurisdiction => MFundable.FundsRequiredSalesTaxJurisdiction;

        [Display(Name = "Funds Received")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsReceived => MFundable.FundsReceived;

        [Display(Name = "Update Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFundable.UpdateDateTimeUtc);

        private IList<FundableFunder> m_funders;
        public IList<FundableFunder> Funders
        {
            get
            {
                if (m_funders == null)
                {
                    m_funders = MFunders != null
                        ? MFunders.Summaries
                            .Select(r => new FundableFunder(r, Locale))
                            .ToList()
                        : new List<FundableFunder>(0);
                }
                return m_funders;
            }
        }

        private IList<FundableTransaction> m_transactions;
        public IList<FundableTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions != null
                        ? MTransactions.Summaries
                            .Select(r => new FundableTransaction(r, Locale))
                            .ToList()
                        : new List<FundableTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<FundableEvent> m_events;
        public IList<FundableEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents != null
                        ? MEvents.Summaries
                            .Select(r => new FundableEvent(r, Locale))
                            .ToList()
                        : new List<FundableEvent>(0);
                }
                return m_events;
            }
        }
    }

    public class FundableFunder
    {
        public MFunding_FunderSummary MFunder { get; }
        public IApplicationLocale Locale { get; }

        public FundableFunder(
            MFunding_FunderSummary mFunder,
            IApplicationLocale locale)
        {
            MFunder = mFunder;
            Locale = locale;
        }

        [Display(Name = "Funder ID")]
        public long FunderId => MFunder.FunderId;

        [Display(Name = "Funder Reference")]
        public string FunderReference => MFunder.FunderReference;
    }

    public class FundableTransaction
    {
        public MFunding_FundableTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public FundableTransaction(
            MFunding_FundableTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.FundableTransactionId}";

        public long FundableTransactionId => MTransaction.FundableTransactionId;

        public long FundableId => MTransaction.FundableId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequiredIncome => MTransaction.FundsRequiredIncome;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequiredSalesTax => MTransaction.FundsRequiredSalesTax;

        public string FundsRequiredSalesTaxJurisdiction => MTransaction.FundsRequiredSalesTaxJurisdiction;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsReceived => MTransaction.FundsReceived;
    }

    public class FundableEvent
    {
        public MFunding_FundableEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public FundableEvent(
            MFunding_FundableEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.FundableEventId}";

        public long FundableEventId => MEvent.FundableEventId;

        public long FundableTransactionId => MEvent.FundableTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}
