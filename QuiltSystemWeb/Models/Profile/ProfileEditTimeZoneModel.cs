//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileEditTimeZoneModel
    {
        [Display(Name = "Time Zone")]
        [Required]
        public string TimeZoneId { get; set; }

        public IList<SelectListItem> TimeZones { get; set; }
    }
}