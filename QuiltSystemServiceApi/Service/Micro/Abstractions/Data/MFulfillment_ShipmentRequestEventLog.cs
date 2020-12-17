﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ShipmentRequestEventLog : MCommon_EventLog
    {
        public override string Source => MSources.ShipmentRequest;
        public long ShipmentRequestEventId => EventId;
        public long ShipmentRequestTransactionId => TransactionId;
    }
}
