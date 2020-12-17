//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.WebAdmin.Models.User
{
    public class UserListItem
    {
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}