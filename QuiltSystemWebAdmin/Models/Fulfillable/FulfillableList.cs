//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fulfillable
{
    public class FulfillableList
    {
        public FulfillableListFilter Filter { get; set; }
        public IPagedList<FulfillableListItem> Items { get; set; }
    }

    public class FulfillableListFilter
    {
        [Display(Name = "Fulfillable Status")]
        public MFulfillment_FulfillableStatus FulfillableStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> FulfillableStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}
