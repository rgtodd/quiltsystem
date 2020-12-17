//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class KitDetailPartVcModel
    {
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public string Id { get; set; }

        [Display(Name = "SKU")]
        public string Sku { get; set; }

        [Display(Name = "Size")]
        public string Size { get; set; }

        [Display(Name = "Web Color")]
        public string WebColor { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Collection")]
        public string Collection { get; set; }
    }
}