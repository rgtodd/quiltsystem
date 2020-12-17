//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ReportInstance
    {
        public ReportInstance()
        {
            IncomeStatementReportInstances = new HashSet<IncomeStatementReportInstance>();
        }

        public long ReportInstanceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThroughDate { get; set; }
        public DateTime RunDate { get; set; }

        public virtual ICollection<IncomeStatementReportInstance> IncomeStatementReportInstances { get; set; }
    }
}
