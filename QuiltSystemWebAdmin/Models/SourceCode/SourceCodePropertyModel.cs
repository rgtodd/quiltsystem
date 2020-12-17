//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.SourceCode
{
    public class SourceCodePropertyModel
    {
        [Display(Name = "Property Name")]
        public string PropertyName { get; set; }

        [Display(Name = "Data Type")]
        public string DataType { get; set; }

        [Display(Name = "Display")]
        public string DisplayName { get; set; }

        [Display(Name = "Display Format")]
        public string DisplayFormatDataFormatString { get; set; }
    }
}