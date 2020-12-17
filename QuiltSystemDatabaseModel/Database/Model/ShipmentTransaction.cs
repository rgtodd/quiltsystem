//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentTransaction
    {
        public ShipmentTransaction()
        {
            ShipmentEvents = new HashSet<ShipmentEvent>();
        }

        public long ShipmentTransactionId { get; set; }
        public long ShipmentId { get; set; }
        public string ShipmentStatusCode { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual Shipment Shipment { get; set; }
        public virtual ICollection<ShipmentEvent> ShipmentEvents { get; set; }
    }
}
