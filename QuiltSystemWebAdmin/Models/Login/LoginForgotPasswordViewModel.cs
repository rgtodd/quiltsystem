//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Login
{
    public class LoginForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}