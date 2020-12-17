//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Event
{
    public class EventLogList
    {
        public IPagedList<EventLogListItem> Items { get; set; }

        [Display(Name = "Unit of Work")]
        public string UnitOfWork { get; set; }

        [Display(Name = "Source")]
        public string Source { get; set; }

        public IList<SelectListItem> SourceList { get; set; }
    }
}
