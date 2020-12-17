//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class AccountingYear
    {
        public AccountingYear()
        {
            LedgerAccountSubtotals = new HashSet<LedgerAccountSubtotal>();
        }

        public int Year { get; set; }
        public string AccountingYearStatusCode { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AccountingYearStatusType AccountingYearStatusCodeNavigation { get; set; }
        public virtual ICollection<LedgerAccountSubtotal> LedgerAccountSubtotals { get; set; }
    }
}
