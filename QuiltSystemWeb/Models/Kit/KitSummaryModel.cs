//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Kit
{
    public class KitSummaryModel
    {

        [Display(Name = "Kit ID")]
        public Guid KitId { get; set; }

        [Display(Name = "Kit Name")]
        public string KitName { get; set; }

    }
}