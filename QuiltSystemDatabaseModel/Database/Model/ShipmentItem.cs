//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ShipmentItem
    {
        public long ShipmentItemId { get; set; }
        public long ShipmentId { get; set; }
        public long ShipmentRequestItemId { get; set; }
        public int Quantity { get; set; }

        public virtual Shipment Shipment { get; set; }
        public virtual ShipmentRequestItem ShipmentRequestItem { get; set; }
    }
}
