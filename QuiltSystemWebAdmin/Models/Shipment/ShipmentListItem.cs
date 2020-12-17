//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Shipment
{
    public class ShipmentListItem
    {
        public AShipment_ShipmentSummary AShipmentSummary { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentListItem(
            AShipment_ShipmentSummary aShipmentSummary,
            IApplicationLocale locale)
        {
            AShipmentSummary = aShipmentSummary;
            Locale = locale;
        }

        [Display(Name = "Shipment ID")]
        public long ShipmentId => AShipmentSummary.ShipmentId;

        [Display(Name = "Shipment Number")]
        public string ShipmentNumber => AShipmentSummary.ShipmentNumber;

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => AShipmentSummary.FulfillableId;

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => AShipmentSummary.FulfillableName;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => AShipmentSummary.FulfillableReference;

        [Display(Name = "Shipment Status")]
        public string ShipmentStatusName => AShipmentSummary.ShipmentStatusName;

        [Display(Name = "Shipping Vendor ID")]
        public string ShippingVendorId => AShipmentSummary.ShippingVendorId;

        [Display(Name = "Tracking Code")]
        public string TrackingCode => AShipmentSummary.TrackingCode;

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(AShipmentSummary.CreateDateTimeUtc);

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(AShipmentSummary.StatusDateTimeUtc);
    }
}