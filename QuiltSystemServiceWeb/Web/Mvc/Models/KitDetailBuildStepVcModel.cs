//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class KitDetailBuildStepVcModel
    {
        #region Properties

        public List<KitDetailBuildItemVcModel> Consumes { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<KitDetailBuildItemVcModel> Produces { get; set; }

        #endregion Properties
    }
}