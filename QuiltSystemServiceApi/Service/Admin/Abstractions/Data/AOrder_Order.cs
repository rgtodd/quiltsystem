//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AOrder_Order
    {
        public MOrder_Order MOrder { get; set; }
        public MOrder_OrderTransactionSummaryList MTransactions { get; set; }
        public MOrder_OrderEventLogSummaryList MEvents { get; set; }

        public MFulfillment_Fulfillable MFulfillable { get; set; }
        public MFunding_Fundable MFundable { get; set; }

        public MUser_User MUser { get; set; }
    }
}
