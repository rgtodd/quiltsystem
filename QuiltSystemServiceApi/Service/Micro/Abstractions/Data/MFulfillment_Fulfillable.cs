//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_Fulfillable
    {
        public long FulfillableId { get; set; }
        public string FulfillableReference { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public MFulfillment_FulfillableStatus FulfillableStatus { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public MFulfillment_Address ShipToAddress { get; set; }

        public bool CanCreateReturnRequest { get; set; }

        // Optionally populated
        //
        public IList<MFulfillment_FulfillableItem> FulfillableItems { get; set; }
        public IList<MFulfillment_ShipmentRequest> ShipmentRequests { get; set; }
        public IList<MFulfillment_Shipment> Shipments { get; set; }
        public IList<MFulfillment_ReturnRequest> ReturnRequests { get; set; }
        public IList<MFulfillment_Return> Returns { get; set; }

    }

    public class MFulfillment_FulfillableItem
    {
        public long FulfillableItemId { get; set; }
        public string FulfillableItemReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int RequestQuantity { get; set; }
        public int CompleteQuantity { get; set; }
        public int ReturnQuantity { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        public IList<MFulfillment_FulfillableItemComponent> FulfillableItemComponents { get; set; }
    }

    public class MFulfillment_FulfillableItemComponent
    {
        public long FulfillableItemComponentId { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
