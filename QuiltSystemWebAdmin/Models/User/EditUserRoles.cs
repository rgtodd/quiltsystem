//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.User
{
    public class EditUserRoles
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Current Roles")]
        public IList<string> CurrentRoles { get; set; }

        [Display(Name = "New Roles")]
        public IList<string> NewRoles { get; set; }
    }
}