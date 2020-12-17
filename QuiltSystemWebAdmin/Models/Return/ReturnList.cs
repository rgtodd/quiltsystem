//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Return
{
    public class ReturnList
    {
        public ReturnListFilter Filter { get; set; }
        public IPagedList<ReturnListItem> Items { get; set; }
    }


    public class ReturnListFilter
    {
        [Display(Name = "Fulfillable Status")]
        public MFulfillment_ReturnStatus ReturnStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> ReturnStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}