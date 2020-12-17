//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class KitDetailBuildItemVcModel
    {
        #region Properties

        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Description")]
        public string Name { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Width")]
        public string Width { get; set; }

        [Display(Name = "Height")]
        public string Height { get; set; }

        [Display(Name = "SKU")]
        public string Sku1 { get; set; }

        [Display(Name = "SKU")]
        public string Sku2 { get; set; }

        public string SkuName1 { get; set; }

        public string SkuName2 { get; set; }

        [Display(Name = "Web Color")]
        public string WebColor1 { get; set; }

        [Display(Name = "Web Color")]
        public string WebColor2 { get; set; }

        public string PartId { get; set; }

        public byte[] Image { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        #endregion Properties
    }
}