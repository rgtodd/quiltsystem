//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Profile
{
    public class ProfileEditNicknameModel
    {
        [Display(Name = "Nickname")]
        [StringLength(50)]
        public string Nickname { get; set; }
    }
}