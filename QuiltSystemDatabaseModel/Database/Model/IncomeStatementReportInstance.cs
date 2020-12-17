//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class IncomeStatementReportInstance
    {
        public long ReportInstanceId { get; set; }
        public int OrderLedgerAccountTypeId { get; set; }
        public string OrderLedgerAccountTypeName { get; set; }
        public decimal Amount { get; set; }

        public virtual ReportInstance ReportInstance { get; set; }
    }
}
