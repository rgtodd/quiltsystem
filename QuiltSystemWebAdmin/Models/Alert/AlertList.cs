//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Alert
{
    public class AlertList
    {
        public AlertListFilter Filter { get; set; }
        public IPagedList<AlertListItem> Items { get; set; }
    }

    public class AlertListFilter
    {
        [Display(Name = "Is Acknowledged")]
        public string Acknowledged { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> AcknowledgedList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}