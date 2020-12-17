//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ShipmentRequestTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.ShipmentRequest;
        public long ShipmentRequestTransactionId => TransactionId;
        public long ShipmentRequestId => EntityId;

        public MFulfillment_ShipmentRequestStatus ShipmentRequestStatus { get; set; }
    }
}
