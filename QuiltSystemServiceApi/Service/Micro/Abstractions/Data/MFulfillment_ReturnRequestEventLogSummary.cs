﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_ReturnRequestEventLogSummary : MCommon_EventLogSummary
    {
        public override string Source => MSources.ReturnRequest;
        public long ReturnRequestEventId => EventId;
        public long ReturnRequestTransactionId => TransactionId;
    }
}
