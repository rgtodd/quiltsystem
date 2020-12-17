//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AReturn_ReturnRequest
    {
        public MFulfillment_ReturnRequest MReturnRequest { get; set; }
        public MFulfillment_ReturnRequestTransactionSummaryList MTransactions { get; set; }
        public MFulfillment_ReturnRequestEventLogSummaryList MEvents { get; set; }
        public IList<MFulfillment_Fulfillable> MFulfillables { get; set; }

        public bool AllowEdit { get; set; }
    }
}
