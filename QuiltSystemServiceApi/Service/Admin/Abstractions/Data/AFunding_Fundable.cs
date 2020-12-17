//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AFunding_Fundable
    {
        public MFunding_Fundable MFundable { get; set; }
        public MFunding_FunderSummaryList MFunders { get; set; }
        public MFunding_FundableTransactionSummaryList MTransactions { get; set; }
        public MFunding_FundableEventLogSummaryList MEvents { get; set; }
    }
}
