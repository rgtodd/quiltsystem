//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SourceCode
{
    public class SourceCodeModel
    {
        [Display(Name = "Types")]
        public IList<SourceCodeTypeModel> Types { get; set; }

        [Display(Name = "Filter")]
        public string Filter { get; set; }
    }
}