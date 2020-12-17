//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Funder
{
    public class FunderList
    {
        public FunderListFilter Filter { get; set; }
        public IPagedList<FunderListItem> Items { get; set; }
    }

    public class FunderListFilter
    {
        [Display(Name = "Has Funds Available")]
        public string HasFundsAvailable { get; set; }

        [Display(Name = "Has Funds Refundable")]
        public string HasFundsRefundable { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> HasFundsAvailableList { get; set; }
        public IList<SelectListItem> HasFundsRefundableList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
