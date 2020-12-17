//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Member
{
    public class MemberProjectSummaryModel
    {

        [Display(Name = "Project ID")]
        public Guid ProjectId { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

    }
}