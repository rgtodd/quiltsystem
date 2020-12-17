//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

namespace RichTodd.QuiltSystem.WebAdmin.Models.User
{
    public class UserList
    {
        [Display(Name = "Users")]
        public IPagedList<UserListItem> UserSummaries { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Roles")]
        public IList<SelectListItem> Roles { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}