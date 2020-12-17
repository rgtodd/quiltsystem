//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Shipment
{
    public class EditShipment
    {
        [Display(Name = "Shipment ID")]
        public long? ShipmentId { get; set; }

        [Display(Name = "Shipment Number")]
        public string ShipmentNumber { get; set; }

        [Display(Name = "Shipment Status")]
        public string ShipmentStatus { get; set; }

        [Required]
        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Shipment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ShipmentDate { get; set; }

        [Display(Name = "Shipping Vendor ID")]
        public string ShippingVendorId { get; set; }

        [Display(Name = "Shipping Vendors")]
        public SelectList ShippingVendors { get; set; }

        [Display(Name = "Item")]
        public IList<ShipmentItem> ShipmentItems { get; set; }

        public class ShipmentItem
        {
            [Display(Name = "Shipment Item ID")]
            public long? ShipmentItemId { get; set; }

            public long ShipmentRequestItemId { get; set; }

            public long FulfillableItemId { get; set; }

            public string FulfillableItemReference { get; set; }

            [Display(Name = "Quantity")]
            public int Quantity { get; set; }
        }
    }
}