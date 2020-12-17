//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentRequest
    {
        public ShipmentRequest()
        {
            ShipmentRequestItems = new HashSet<ShipmentRequestItem>();
            ShipmentRequestTransactions = new HashSet<ShipmentRequestTransaction>();
        }

        public long ShipmentRequestId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string ShipmentRequestStatusCode { get; set; }
        public DateTime ShipmentRequestStatusDateTimeUtc { get; set; }
        public string ShipmentRequestNumber { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ShipmentRequestStatusType ShipmentRequestStatusCodeNavigation { get; set; }
        public virtual ShipmentRequestAddress ShipmentRequestAddress { get; set; }
        public virtual ICollection<ShipmentRequestItem> ShipmentRequestItems { get; set; }
        public virtual ICollection<ShipmentRequestTransaction> ShipmentRequestTransactions { get; set; }
    }
}
