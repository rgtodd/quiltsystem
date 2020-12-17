//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Return
{
    public class ReturnListItem
    {
        public AReturn_ReturnSummary AReturnSummary { get; }
        public IApplicationLocale Locale { get; }

        public ReturnListItem(
            AReturn_ReturnSummary aReturnSummary,
            IApplicationLocale locale)
        {
            AReturnSummary = aReturnSummary;
            Locale = locale;
        }

        [Display(Name = "Return ID")]
        public long ReturnId => AReturnSummary.ReturnId;

        [Display(Name = "Return Number")]
        public string ReturnNumber => AReturnSummary.ReturnNumber;

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => AReturnSummary.FulfillableId;

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => AReturnSummary.FulfillableName;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => AReturnSummary.FulfillableReference;

        [Display(Name = "Return Status")]
        public string ReturnStatusName => AReturnSummary.ReturnStatusName;

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(AReturnSummary.StatusDateTimeUtc);

        [Display(Name = "Shipping Vendor ID")]
        public int ShippingVendorId => AReturnSummary.ShippingVendorId;

        [Display(Name = "Tracking Code")]
        public string TrackingCode => AReturnSummary.TrackingCode;

        [Display(Name = "Return Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ReturnDateTime => Locale.GetLocalTimeFromUtc(AReturnSummary.ReturnDateTimeUtc);
    }
}