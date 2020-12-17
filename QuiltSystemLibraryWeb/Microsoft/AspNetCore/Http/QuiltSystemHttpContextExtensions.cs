//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Security.Claims;

namespace Microsoft.AspNetCore.Http
{
    public static class QuiltSystemHttpContextExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
