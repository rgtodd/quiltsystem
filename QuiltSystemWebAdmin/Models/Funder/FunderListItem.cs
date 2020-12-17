//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Funder
{
    public class FunderListItem
    {
        public MFunding_FunderSummary MFunderSummary { get; }
        public IApplicationLocale Locale { get; }

        public FunderListItem(
            MFunding_FunderSummary mFunderSummary,
            IApplicationLocale locale)
        {
            MFunderSummary = mFunderSummary;
            Locale = locale;
        }

        [Display(Name = "Funder ID")]
        public long FunderId => MFunderSummary.FunderId;

        [Display(Name = "Funder Reference")]
        public string FunderReference => MFunderSummary.FunderReference;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Received")]
        public decimal TotalFundsReceived => MFunderSummary.TotalFundsReceived;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Available")]
        public decimal TotalFundsAvailable => MFunderSummary.TotalFundsAvailable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Refunded")]
        public decimal TotalFundsRefunded => MFunderSummary.TotalFundsRefunded;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Funds Refundable")]
        public decimal TotalFundsRefundable => MFunderSummary.TotalFundsRefundable;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Processing Fee")]
        public decimal TotalProcessingFee => MFunderSummary.TotalProcessingFee;
    }
}
