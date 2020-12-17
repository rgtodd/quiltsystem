//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IUserAdminService
    {
        Task<AUser_UserSummaryList> GetUsersAsync(AUser_GetUsers request);
        Task<AUser_User> GetUserAsync(string userId);
        Task<bool> AddUserRoleAsync(string userId, string role);
        Task<bool> RemoveUserRoleAsync(string userId, string role);
    }
}
