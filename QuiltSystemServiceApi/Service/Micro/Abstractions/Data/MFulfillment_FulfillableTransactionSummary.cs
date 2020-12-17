//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MFulfillment_FulfillableTransactionSummary : MCommon_TransactionSummary
    {
        public override string Source => MSources.Fulfillable;
        public long FulfillableTransactionId => TransactionId;
        public long FulfillableId => EntityId;
    }
}
