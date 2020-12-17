//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class KitDetailSpecificationVcModel
    {
        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Border")]
        public string Border { get; set; }

        [Display(Name = "Binding")]
        public string Binding { get; set; }

        [Display(Name = "Backing")]
        public string Backing { get; set; }

        [Display(Name = "Trim Half Square Triangles")]
        public bool TrimTriangles { get; set; }
    }
}