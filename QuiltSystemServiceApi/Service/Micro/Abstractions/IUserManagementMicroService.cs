//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IUserManagementMicroService
    {
        Task<bool> AddUserRoleAsync(string userId, string role);
        Task<bool> RemoveUserRoleAsync(string userId, string role);
        Task ChangeEmailAsync(string userId, string email);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task ConfirmEmailAsync(string userId, string code);
        Task<string> CreateNewUserAsync(MUser_CreateUser request);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        Task ResetPasswordAsync(string userId, string password, string code);
    }
}
