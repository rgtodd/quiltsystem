//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AOrder_PostOrderTransaction
    {
        public long OrderId { get; set; }
        public int OrderTransactionTypeCode { get; set; }

        public IList<AOrder_PostOrderTransactionItem> Items { get; set; }
    }
}
