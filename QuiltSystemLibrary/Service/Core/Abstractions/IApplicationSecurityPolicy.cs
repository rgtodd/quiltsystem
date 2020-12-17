//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

namespace RichTodd.QuiltSystem.Service.Core.Abstractions
{
    public interface IApplicationSecurityPolicy
    {
        Task<bool> IsAuthorized(string toUserId);

        Task<bool> IsAdministrator();
        Task<bool> IsService();
        Task<bool> IsEndUser();
        Task<bool> IsPrivileged();

        Task<bool> AllowViewFinancial();
        Task<bool> AllowEditFinancial();
        Task<bool> AllowViewFulfillment();
        Task<bool> AllowEditFulfillment();
        Task<bool> AllowViewUser();
        Task<bool> AllowEditUser();
    }
}
