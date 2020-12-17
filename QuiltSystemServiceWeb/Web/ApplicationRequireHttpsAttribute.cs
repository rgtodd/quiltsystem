//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RichTodd.QuiltSystem.Web
{
    public sealed class ApplicationRequireHttpsAttribute : RequireHttpsAttribute
    {

        public override void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (IsLocal(filterContext.HttpContext))
            {
                // No action required.

                return;
            }

            base.OnAuthorization(filterContext);
        }

        private bool IsLocal(HttpContext httpConntext)
        {
            var connection = httpConntext.Connection;

            return connection.RemoteIpAddress != null
                ? connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress)
                : connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }
    }
}