//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AFunding_Funder
    {
        public MFunding_Funder MFunder { get; set; }
        public MFunding_FundableSummaryList MFundables { get; set; }
        public MFunding_FunderTransactionSummaryList MTransactions { get; set; }
        public MFunding_FunderEventLogSummaryList MEvents { get; set; }
    }
}
