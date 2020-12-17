//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Ledger
{
    public class LedgerSummary
    {
        public int AccountingYear { get; set; }

        public IList<LedgerSummaryIten> DebitItems { get; set; }

        public IList<LedgerSummaryIten> CreditItems { get; set; }
    }

    public class LedgerSummaryIten
    {
        public int LedgerAccountNumber { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat, ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
    }
}
