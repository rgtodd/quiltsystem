//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace RichTodd.QuiltSystem.Security
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            await Task.CompletedTask.ConfigureAwait(false);

            if (context.User.IsInRole(requirement.RoleName))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
