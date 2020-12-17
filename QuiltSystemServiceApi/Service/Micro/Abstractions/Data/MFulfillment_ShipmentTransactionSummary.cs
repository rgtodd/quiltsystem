//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ShipmentTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.Shipment;
        public long ShipmentTransactionId => TransactionId;
        public long ShipmentId => EntityId;

        public MFulfillment_ShipmentStatus ShipmentStatus { get; set; }
    }
}
