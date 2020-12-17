//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace RichTodd.QuiltSystem.Security
{
    public class RolesRequirementHandler : AuthorizationHandler<RolesRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
        {
            await Task.CompletedTask.ConfigureAwait(false);

            foreach (string roleName in requirement.RoleNames)
            {
                if (context.User.IsInRole(roleName))
                {
                    context.Succeed(requirement);
                    break;
                }
            }
        }
    }
}
