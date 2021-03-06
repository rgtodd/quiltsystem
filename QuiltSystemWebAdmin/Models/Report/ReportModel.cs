﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Report
{
    public class ReportModel
    {
        public string HtmlTable { get; set; }

        [Display(Name = "Filter")]
        public string Filter { get; set; }

        [Display(Name = "Filters")]
        public IList<SelectListItem> Filters { get; set; }
    }
}