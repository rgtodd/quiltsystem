//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest
{
    public class ReturnRequestListItem
    {
        public AReturn_ReturnRequestSummary AReturnRequestSummary { get; set; }
        public IApplicationLocale Locale { get; set; }

        [Display(Name = "Return Request ID")]
        public long ReturnRequestId => AReturnRequestSummary.ReturnRequestId;

        [Display(Name = "Return Request Number")]
        public string ReturnRequestNumber => AReturnRequestSummary.ReturnRequestNumber;

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => AReturnRequestSummary.FulfillableId;

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => AReturnRequestSummary.FulfillableName;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => AReturnRequestSummary.FulfillableReference;

        [Display(Name = "Return Request Status")]
        public string ReturnRequestStatus => AReturnRequestSummary.ReturnRequestStatus;

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ReturnRequestDateTime => Locale.GetLocalTimeFromUtc(AReturnRequestSummary.ReturnRequestDateTimeUtc);

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ReturnRequestStatusDateTime => Locale.GetLocalTimeFromUtc(AReturnRequestSummary.ReturnRequestStatusDateTimeUtc);
    }
}