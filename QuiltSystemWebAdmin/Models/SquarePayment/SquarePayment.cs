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

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquarePayment
{
    public class SquarePayment
    {
        public ASquare_Payment APayment { get; }
        public IApplicationLocale Locale { get; }

        public SquarePayment(
            ASquare_Payment aPayment,
            IApplicationLocale locale)
        {
            APayment = aPayment;
            Locale = locale;
        }

        public MSquare_Payment MPayment => APayment.MPayment;
        public MSquare_PaymentTransactionSummaryList MPaymentTransactions => APayment.MPaymentTransactions;
        public MSquare_PaymentEventLogSummaryList MPaymentEvents => APayment.MPaymentEvents;
        public MSquare_RefundTransactionSummaryList MRefundTransactions => APayment.MRefundTransactions;
        public MSquare_RefundEventLogSummaryList MRefundEvents => APayment.MRefundEvents;
        public MUser_User MUser => APayment.MUser;

        [Display(Name = "Square Payment ID")]
        public long SquarePaymentId => MPayment.SquarePaymentId;

        [Display(Name = "Square Payment Reference")]
        public string SquarePaymentReference => MPayment.SquarePaymentReference;

        [Display(Name = "Square Customer ID")]
        public long SquareCustomerId => MPayment.SquareCustomerId;

        [Display(Name = "Payment Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal PaymentAmount => MPayment.PaymentAmount;

        [Display(Name = "Refund Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal RefundAmount => MPayment.RefundAmount;

        [Display(Name = "Processing Fee Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ProcessingFeeAmount => MPayment.ProcessingFeeAmount;

        [Display(Name = "Square Payment Record ID")]
        public string SquarePaymentRecordId => MPayment.SquarePaymentRecordId;

        [Display(Name = "Version Number")]
        public int? VersionNumber => MPayment.VersionNumber;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MPayment.UpdateDateTimeUtc);

        [Display(Name = "User ID")]
        public string UserId => MUser?.UserId;

        [Display(Name = "User")]
        public string Email => MUser?.Email;

        [Display(Name = "Funder ID")]
        public long? FunderId => APayment.FunderId;

        private IList<PaymentTransaction> m_transactions;
        public IList<PaymentTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MPaymentTransactions != null
                        ? MPaymentTransactions.Summaries
                            .Select(r => new PaymentTransaction(r, Locale))
                            .ToList()
                        : new List<PaymentTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<PaymentEvent> m_events;
        public IList<PaymentEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MPaymentEvents != null
                        ? MPaymentEvents.Summaries
                            .Select(r => new PaymentEvent(r, Locale))
                            .ToList()
                        : new List<PaymentEvent>(0);
                }
                return m_events;
            }
        }

        private IList<Refund> m_refunds;
        public IList<Refund> Refunds
        {
            get
            {
                if (m_refunds == null)
                {
                    var refunds = MPayment.Refunds?.Select(rRefund =>
                    {
                        var mTransactions = MRefundTransactions.Summaries?.Where(rTransaction => rTransaction.SquareRefundId == rRefund.SquareRefundId).ToList();
                        var mEvents = MRefundEvents.Summaries?.Where(rEvent => rEvent.EntityId == rRefund.SquareRefundId).ToList();
                        return new Refund(
                            rRefund,
                            mTransactions,
                            mEvents,
                            Locale);
                    }).ToList();

                    m_refunds = refunds ?? new List<Refund>(0);
                }
                return m_refunds;
            }
        }
    }

    public class PaymentTransaction
    {
        public MSquare_PaymentTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public PaymentTransaction(
            MSquare_PaymentTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"SPT:{MTransaction.SquarePaymentTransactionId}";

        public long SquarePaymentTransactionId => MTransaction.SquarePaymentTransactionId;

        public long SquarePaymentId => MTransaction.SquarePaymentId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class PaymentEvent
    {
        public MSquare_PaymentEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public PaymentEvent(
            MSquare_PaymentEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.PaymentEventId}";

        public long PaymentEventId => MEvent.PaymentEventId;

        public long PaymentTransactionId => MEvent.PaymentTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }

    public class Refund
    {
        public MSquare_Refund MRefund { get; }
        public IList<MSquare_RefundTransactionSummary> MTransactions { get; }
        public IList<MSquare_RefundEventLogSummary> MEvents { get; }
        public IApplicationLocale Locale { get; }

        public Refund(
            MSquare_Refund mRefund,
            IList<MSquare_RefundTransactionSummary> mTransactions,
            IList<MSquare_RefundEventLogSummary> mEvents,
            IApplicationLocale locale)
        {
            MRefund = mRefund;
            MTransactions = mTransactions;
            MEvents = mEvents;
            Locale = locale;
        }

        [Display(Name = "Square Refund ID")]
        public long SquareRefundId => MRefund.SquareRefundId;

        [Display(Name = "Square Payment ID")]
        public long SquarePaymentId => MRefund.SquarePaymentId;

        [Display(Name = "Refund Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal RefundAmount => MRefund.RefundAmount;

        [Display(Name = "Processing Fee Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ProcessingFeeAmount => MRefund.ProcessingFeeAmount;

        [Display(Name = "Square Refund Record ID")]
        public string SquareRefundRecordId => MRefund.SquareRefundRecordId;

        [Display(Name = "Version Number")]
        public int? VersionNumber => MRefund.VersionNumber;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MRefund.UpdateDateTimeUtc);

        private IList<RefundTransaction> m_transactions;
        public IList<RefundTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions != null
                        ? MTransactions
                            .Select(r => new RefundTransaction(r, Locale))
                            .ToList()
                        : new List<RefundTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<RefundEvent> m_events;
        public IList<RefundEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents != null
                        ? MEvents
                            .Select(r => new RefundEvent(r, Locale))
                            .ToList()
                        : new List<RefundEvent>(0);
                }
                return m_events;
            }
        }
    }

    public class RefundTransaction
    {
        public MSquare_RefundTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public RefundTransaction(
            MSquare_RefundTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"SPT:{MTransaction.SquareRefundTransactionId}";

        public long SquareRefundTransactionId => MTransaction.SquareRefundTransactionId;

        public long SquareRefundId => MTransaction.SquareRefundId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class RefundEvent
    {
        public MSquare_RefundEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public RefundEvent(
            MSquare_RefundEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.RefundEventId}";

        public long RefundEventId => MEvent.RefundEventId;

        public long RefundTransactionId => MEvent.RefundTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }

}
