//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Web.Models.Common;

namespace RichTodd.QuiltSystem.Web.Models.Kit
{
    public class KitSpecificationEditModel
    {
        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Width")]
        public string CustomSizeWidth { get; set; }

        [Display(Name = "Height")]
        public string CustomSizeHeight { get; set; }

        [Display(Name = "Border")]
        public string BorderWidth { get; set; }

        [Display(Name = "Border Width")]
        public string CustomBorderWidth { get; set; }

        [Display(Name = "Border Fabric")]
        public CommonColorModel BorderFabricStyle { get; set; }

        [Display(Name = "Binding")]
        public string BindingWidth { get; set; }

        [Display(Name = "Binding Width")]
        public string CustomBindingWidth { get; set; }

        [Display(Name = "Binding Fabric")]
        public CommonColorModel BindingFabricStyle { get; set; }

        [Display(Name = "Backing")]
        public bool HasBacking { get; set; }

        [Display(Name = "Backing Fabric")]
        public CommonColorModel BackingFabricStyle { get; set; }

        [Display(Name = "Trim Half Square Triangles")]
        public bool TrimTriangles { get; set; }

        //
        // Unbound Properties
        //

        public IList<SelectListItem> Sizes { get; set; }

        public IList<SelectListItem> BorderWidths { get; set; }

        public IList<SelectListItem> BindingWidths { get; set; }
    }
}