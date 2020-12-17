//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Core.Implementations
{
    public class ApplicationSecurityPolicyWeb : IApplicationSecurityPolicy
    {
        private readonly IHttpContextAccessor m_httpContext;
        private readonly IAuthorizationService m_authorizationService;

        public ApplicationSecurityPolicyWeb(
            IHttpContextAccessor httpContext,
            IAuthorizationService authorizationService)
        {
            m_httpContext = httpContext;
            m_authorizationService = authorizationService;
        }

        public async Task<bool> IsAuthorized(string toUserId)
        {
            // Everyone is authorized to themselves.
            //
            var user = m_httpContext.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == toUserId)
            {
                return true;
            }

            // Priviledged users can access anyone.
            //
            var result = await m_authorizationService.AuthorizeAsync(user, ApplicationPolicies.IsPriviledged).ConfigureAwait(false);
            return result.Succeeded;
        }

        #region User User Class Policies

        public async Task<bool> IsAdministrator()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.IsAdministrator).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> IsService()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.IsService).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> IsEndUser()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.IsEndUser).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> IsPrivileged()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.IsPriviledged).ConfigureAwait(false);

            return result.Succeeded;
        }

        #endregion

        #region Application Policies

        public async Task<bool> AllowViewFinancial()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowViewFinancial).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> AllowEditFinancial()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowEditFinancial).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> AllowViewFulfillment()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowViewFulfillment).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> AllowEditFulfillment()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowEditFulfillment).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> AllowViewUser()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowViewUser).ConfigureAwait(false);

            return result.Succeeded;
        }

        public async Task<bool> AllowEditUser()
        {
            var result = await m_authorizationService.AuthorizeAsync(m_httpContext.HttpContext.User, ApplicationPolicies.AllowEditUser).ConfigureAwait(false);

            return result.Succeeded;
        }

        #endregion
    }
}
