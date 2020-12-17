//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquarePayment
{
    public class SquarePaymentListItem
    {
        public MSquare_PaymentSummary MPaymentSummary { get; }
        public IApplicationLocale Locale { get; }

        public SquarePaymentListItem(
            MSquare_PaymentSummary mPaymentSummary,
            IApplicationLocale locale)
        {
            MPaymentSummary = mPaymentSummary;
            Locale = locale;
        }

        [Display(Name = "Square Payment ID")]
        public long SquarePaymentId => MPaymentSummary.SquarePaymentId;

        [Display(Name = "Square Payment Reference")]
        public string SquarePaymentReference => MPaymentSummary.SquarePaymentReference;

        [Display(Name = "Square Customer ID")]
        public long SquareCustomerId => MPaymentSummary.SquareCustomerId;

        [Display(Name = "Payment Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal PaymentAmount => MPaymentSummary.PaymentAmount;

        [Display(Name = "Refund Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal RefundAmount => MPaymentSummary.RefundAmount;

        [Display(Name = "Processing Fee Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ProcessingFeeAmount => MPaymentSummary.ProcessingFeeAmount;

        [Display(Name = "Square Payment Record ID")]
        public string SquarePaymentRecordId => MPaymentSummary.SquarePaymentRecordId;

        [Display(Name = "Version Number")]
        public int? VersionNumber => MPaymentSummary.VersionNumber;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MPaymentSummary.UpdateDateTimeUtc);
    }
}
