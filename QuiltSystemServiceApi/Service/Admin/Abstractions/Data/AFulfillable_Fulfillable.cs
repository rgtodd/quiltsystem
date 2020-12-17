//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AFulfillable_Fulfillable
    {
        public MFulfillment_Fulfillable MFulfillable { get; set; }
        public MFulfillment_FulfillableTransactionSummaryList MTransactions { get; set; }
        public MFulfillment_FulfillableEventLogSummaryList MEvents { get; set; }

        public bool AllowEdit { get; set; }
    }
}
