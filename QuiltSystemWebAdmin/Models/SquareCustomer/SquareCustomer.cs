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

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquareCustomer
{
    public class SquareCustomer
    {
        public ASquare_Customer ACustomer { get; }
        public IApplicationLocale Locale { get; }

        public SquareCustomer(
            ASquare_Customer aCustomer,
            IApplicationLocale locale)
        {
            ACustomer = aCustomer;
            Locale = locale;
        }

        public MSquare_Customer MCustomer => ACustomer.MCustomer;
        public IList<MSquare_CustomerPayment> MPayments => MCustomer?.Payments;

        [Display(Name = "Square Customer ID")]
        public long SquareCustomerId => MCustomer.SquareCustomerId;

        [Display(Name = "Square Customer Reference")]
        public string SquareCustomerReference => MCustomer.SquareCustomerReference;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MCustomer.UpdateDateTimeUtc);

        private IList<SquareCustomerPayment> m_payments;
        public IList<SquareCustomerPayment> Payments
        {
            get
            {
                if (m_payments == null)
                {
                    m_payments = MPayments != null
                        ? MPayments
                            .Select(r => new SquareCustomerPayment(r, Locale))
                            .ToList()
                        : new List<SquareCustomerPayment>(0);
                }
                return m_payments;
            }
        }
    }

    public class SquareCustomerPayment
    {
        public MSquare_CustomerPayment MPayment { get; }
        public IApplicationLocale Locale { get; }

        public SquareCustomerPayment(
            MSquare_CustomerPayment mPayment,
            IApplicationLocale locale)
        {
            MPayment = mPayment;
            Locale = locale;
        }

        [Display(Name = "Square Payment ID")]
        public long SquarePaymentId => MPayment.SquarePaymentId;

        [Display(Name = "Square Payment Reference")]
        public string SquarePaymentReference => MPayment.SquarePaymentReference;

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
    }
}
