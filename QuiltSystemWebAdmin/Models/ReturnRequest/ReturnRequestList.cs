//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest
{
    public class ReturnRequestList
    {
        public ReturnRequestListFilter Filter { get; set; }
        public IPagedList<ReturnRequestListItem> Items { get; set; }
    }

    public class ReturnRequestListFilter
    {
        [Display(Name = "Fulfillable Status")]
        public MFulfillment_ReturnRequestStatus ReturnRequestStatus { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> ReturnRequestStatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}