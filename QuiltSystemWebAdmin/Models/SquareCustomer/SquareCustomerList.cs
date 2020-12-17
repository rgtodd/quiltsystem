//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquareCustomer
{
    public class SquareCustomerList
    {
        public SquareCustomerListFilter Filter { get; set; }
        public IPagedList<SquareCustomerListItem> Items { get; set; }
    }

    public class SquareCustomerListFilter
    {
        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
