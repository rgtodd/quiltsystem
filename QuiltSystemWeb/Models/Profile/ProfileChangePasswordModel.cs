//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileChangePasswordModel
    {
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        [Required]
        public string OldPassword { get; set; }

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, MinimumLength = Constants.MinimumPasswordLength)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, MinimumLength = Constants.MinimumPasswordLength)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}