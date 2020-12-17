//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MLedger_Dashboard
    {
        public IList<MLedger_DashboardItem> DashboardItems { get; set; }
    }

    public class MLedger_DashboardItem
    {
        public string DebitCreditCode { get; set; }
        public int AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public decimal Amount { get; set; }
    }
}
