﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Design
{
    public class DesignRenameModel
    {
        [Display(Name = "Name")]
        public string NewDesignName { get; set; }

        [Display(Name = "Design ID")]
        public Guid DesignId { get; set; }
    }
}