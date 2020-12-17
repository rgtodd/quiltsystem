//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquareCustomer
{
    public class SquareCustomerListItem
    {
        public MSquare_CustomerSummary MCustomerSummary { get; }
        public IApplicationLocale Locale { get; }

        public SquareCustomerListItem(
            MSquare_CustomerSummary mCustomerSummary,
            IApplicationLocale locale)
        {
            MCustomerSummary = mCustomerSummary;
            Locale = locale;
        }

        [Display(Name = "Square Customer ID")]
        public long SquareCustomerId => MCustomerSummary.SquareCustomerId;

        [Display(Name = "Square Customer Reference")]
        public string SquareCustomerReference => MCustomerSummary.SquareCustomerReference;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MCustomerSummary.UpdateDateTimeUtc);
    }
}
