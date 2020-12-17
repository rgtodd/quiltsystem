//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fundable
{
    public class FundableListItem
    {
        public MFunding_FundableSummary MFundableSummary { get; }
        public IApplicationLocale Locale { get; }

        public FundableListItem(
            MFunding_FundableSummary mFundableSummary,
            IApplicationLocale locale)
        {
            MFundableSummary = mFundableSummary;
            Locale = locale;
        }

        [Display(Name = "Fundable ID")]
        public long FundableId => MFundableSummary.FundableId;

        [Display(Name = "Fundable Reference")]
        public string FundableReference => MFundableSummary.FundableReference;

        [Display(Name = "Funds Required")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsRequired => MFundableSummary.FundsRequired;

        [Display(Name = "Funds Received")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal FundsReceived => MFundableSummary.FundsReceived;
    }
}
