//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Shipment
    {
        public Shipment()
        {
            ShipmentItems = new HashSet<ShipmentItem>();
            ShipmentTransactions = new HashSet<ShipmentTransaction>();
        }

        public long ShipmentId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string ShipmentStatusCode { get; set; }
        public DateTime ShipmentStatusDateTimeUtc { get; set; }
        public string ShipmentNumber { get; set; }
        public DateTime ShipmentDateTimeUtc { get; set; }
        public string ShippingVendorId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ShipmentStatusType ShipmentStatusCodeNavigation { get; set; }
        public virtual ShippingVendor ShippingVendor { get; set; }
        public virtual ShipmentAddress ShipmentAddress { get; set; }
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; }
        public virtual ICollection<ShipmentTransaction> ShipmentTransactions { get; set; }
    }
}
