//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Core.Implementations
{
    public class ApplicationSecurityPolicyStatic : IApplicationSecurityPolicy
    {
        private readonly string m_userId;

        public ApplicationSecurityPolicyStatic(string userId)
        {
            m_userId = userId ?? throw new ArgumentNullException(userId);
        }

        public async Task<bool> IsAuthorized(string toUserId)
        {
            return m_userId == toUserId || await IsPrivileged().ConfigureAwait(false);
        }

        #region User Class Policies

        public async Task<bool> IsAdministrator()
        {
            await Task.CompletedTask.ConfigureAwait(false);

            return m_userId == BuiltInUsers.AdminUser;
        }

        public async Task<bool> IsService()
        {
            await Task.CompletedTask.ConfigureAwait(false);

            return m_userId == BuiltInUsers.ServiceUser;
        }

        public async Task<bool> IsEndUser()
        {
            return !await IsAdministrator().ConfigureAwait(false) && !await IsService().ConfigureAwait(false);
        }

        public async Task<bool> IsPrivileged()
        {
            return await IsAdministrator().ConfigureAwait(false) || await IsService().ConfigureAwait(false);
        }

        #endregion

        #region Application Policies

        public Task<bool> AllowViewFinancial()
        {
            return IsPrivileged();
        }

        public Task<bool> AllowEditFinancial()
        {
            return IsPrivileged();
        }

        public Task<bool> AllowViewFulfillment()
        {
            return IsPrivileged();
        }

        public Task<bool> AllowEditFulfillment()
        {
            return IsPrivileged();
        }

        public Task<bool> AllowViewUser()
        {
            return IsPrivileged();
        }

        public Task<bool> AllowEditUser()
        {
            return IsPrivileged();
        }

        #endregion
    }
}
