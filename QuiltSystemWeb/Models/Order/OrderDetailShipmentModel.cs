//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderDetailShipmentModel
    {
        [Display(Name = "Shipment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ShipmentDateTime { get; set; }

        [Display(Name = "Shipping Vendor")]
        public string ShippingVendor { get; set; }

        [Display(Name = "Tracking Code")]
        public string TrackingCode { get; set; }

        [Display(Name = "Items")]
        public IList<OrderDetailItemModel> Items { get; set; }

    }
}