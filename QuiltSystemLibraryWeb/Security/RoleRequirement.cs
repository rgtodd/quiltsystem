//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Authorization;

namespace RichTodd.QuiltSystem.Security
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RoleName { get; }

        public RoleRequirement(string roleName)
        {
            RoleName = roleName;
        }
    }
}
