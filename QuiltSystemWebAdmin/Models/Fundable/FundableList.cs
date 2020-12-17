//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fundable
{
    public class FundableList
    {
        public FundableListFilter Filter { get; set; }
        public IPagedList<FundableListItem> Items { get; set; }
    }

    public class FundableListFilter
    {
        [Display(Name = "Has Required Funds")]
        public string HasFundsRequired { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> HasFundsRequiredList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
