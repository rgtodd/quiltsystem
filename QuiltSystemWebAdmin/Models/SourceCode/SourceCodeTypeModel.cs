﻿//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SourceCode
{
    public class SourceCodeTypeModel
    {
        [Display(Name = "Type Name")]
        public string TypeName { get; set; }

        [Display(Name = "Properties")]
        public IList<SourceCodePropertyModel> Properties { get; set; }
    }
}