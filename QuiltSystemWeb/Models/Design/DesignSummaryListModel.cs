﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.Web.Models.Design
{
    public class DesignSummaryListModel
    {
        [Display(Name = "Design Summaries")]
        public IPagedList<DesignSummaryModel> DesignSummaries { get; set; }

        [Display(Name = "Create Design")]
        public DesignCreateModel CreateDesign { get; set; }

        [Display(Name = "Rename Design")]
        public DesignRenameModel RenameDesign { get; set; }

        [Display(Name = "Filter")]
        public string Filter { get; set; }

        [Display(Name = "Filters")]
        public IList<SelectListItem> Filters { get; set; }

        public bool HasDeletedDesigns { get; set; }
    }
}