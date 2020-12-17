//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Login
{
    public class LoginSendCodeViewModel
    {
        [Display(Name = "Selected Provider")]
        public string SelectedProvider { get; set; }

        [Display(Name = "Providers")]
        public ICollection<SelectListItem> Providers { get; set; }

        [Display(Name = "Return URL")]
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}