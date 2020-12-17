//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.ShipmentRequest
{
    public class ShipmentRequestListItem
    {
        public AShipment_ShipmentRequestSummary AShipmentRequestSummary { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentRequestListItem(
            AShipment_ShipmentRequestSummary aShipmentRequestSummary,
            IApplicationLocale locale)
        {
            AShipmentRequestSummary = aShipmentRequestSummary;
            Locale = locale;
        }

        [Display(Name = "Shipment Request ID")]
        public long ShipmentRequestId => AShipmentRequestSummary.ShipmentRequestId;

        [Display(Name = "Shipment Request Number")]
        public string ShipmentRequestNumber => AShipmentRequestSummary.ShipmentRequestNumber;

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => AShipmentRequestSummary.FulfillableId;

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => AShipmentRequestSummary.FulfillableName;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => AShipmentRequestSummary.FulfillableReference;

        [Display(Name = "Shipment Request Status")]
        public string ShipmentRequestStatus => AShipmentRequestSummary.ShipmentRequestStatus;

        [Display(Name = "Create Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(AShipmentRequestSummary.CreateDateTimeUtc);

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(AShipmentRequestSummary.StatusDateTimeUtc);
    }
}