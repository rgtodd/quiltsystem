//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_FulfillableEventLog : MCommon_EventLog
    {
        public override string Source => MSources.Fulfillable;
        public long FulfillableEventId => EventId;
        public long FulfillableTransactionId => TransactionId;
    }
}
