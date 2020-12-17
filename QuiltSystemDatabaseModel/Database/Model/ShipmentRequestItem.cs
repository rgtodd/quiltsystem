//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentRequestItem
    {
        public ShipmentRequestItem()
        {
            ShipmentItems = new HashSet<ShipmentItem>();
        }

        public long ShipmentRequestItemId { get; set; }
        public long ShipmentRequestId { get; set; }
        public long FulfillableItemId { get; set; }
        public int Quantity { get; set; }

        public virtual FulfillableItem FulfillableItem { get; set; }
        public virtual ShipmentRequest ShipmentRequest { get; set; }
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; }
    }
}
