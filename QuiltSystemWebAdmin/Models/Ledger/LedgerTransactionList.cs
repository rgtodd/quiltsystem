//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Ledger
{
    public class LedgerTransactionList
    {
        public LedgerTransactionListFilter Filter { get; set; }
        public IPagedList<LedgerTransaction> Items { get; set; }
    }

    public class LedgerTransactionListFilter
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = Standard.IsoDateFormat, ApplyFormatInEditMode = true)]
        [Display(Name = "Post Date")]
        public DateTime? PostDate { get; set; }

        [Display(Name = "Ledger Account")]
        public int LedgerAccountNumber { get; set; }

        [Display(Name = "Unit of Work")]
        public string UnitOfWork { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> LedgerAccountNumberList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
