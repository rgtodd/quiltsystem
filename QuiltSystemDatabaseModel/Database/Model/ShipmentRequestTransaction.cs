//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentRequestTransaction
    {
        public ShipmentRequestTransaction()
        {
            ShipmentRequestEvents = new HashSet<ShipmentRequestEvent>();
        }

        public long ShipmentRequestTransactionId { get; set; }
        public long ShipmentRequestId { get; set; }
        public string ShipmentRequestStatusCode { get; set; }
        public DateTime TransactionDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string UnitOfWork { get; set; }

        public virtual ShipmentRequest ShipmentRequest { get; set; }
        public virtual ICollection<ShipmentRequestEvent> ShipmentRequestEvents { get; set; }
    }
}
