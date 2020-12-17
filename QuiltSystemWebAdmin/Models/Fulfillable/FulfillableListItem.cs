//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fulfillable
{
    public class FulfillableListItem
    {
        public MFulfillment_FulfillableSummary MFulfillableSummary { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableListItem(
            MFulfillment_FulfillableSummary mFulfillableSummary,
            IApplicationLocale locale)
        {
            MFulfillableSummary = mFulfillableSummary;
            Locale = locale;
        }

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => MFulfillableSummary.FulfillableId;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => MFulfillableSummary.FulfillableReference;

        [Display(Name = "Name")]
        public string Name => MFulfillableSummary.Name;

        [Display(Name = "Fulfillable Status")]
        public string FulfillableStatus => MFulfillableSummary.FulfillableStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MFulfillableSummary.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MFulfillableSummary.CreateDateTimeUtc);

        [Display(Name = "Required Quantity")]
        public int TotalFulfillmentRequiredQuantity => MFulfillableSummary.TotalFulfillmentRequiredQuantity;

        [Display(Name = "Complete Quantity")]
        public int TotalFulfillmentCompleteQuantity => MFulfillableSummary.TotalFulfillmentCompleteQuantity;

        [Display(Name = "Return Quantity")]
        public int TotalFulfillmentReturnQuantity => MFulfillableSummary.TotalFulfillmentReturnQuantity;
    }
}
