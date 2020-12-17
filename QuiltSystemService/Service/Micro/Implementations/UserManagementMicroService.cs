//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class UserManagementMicroService : MicroService, IUserManagementMicroService
    {
        private UserManager<IdentityUser> UserManager { get; }

        public UserManagementMicroService(
            IApplicationLocale locale,
            ILogger<UserMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            UserManager<IdentityUser> userManager)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<bool> AddUserRoleAsync(string userId, string role)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(AddUserRoleAsync), userId, role);
            try
            {
                //await AssertIsPrivilegedUser().ConfigureAwait(false);

                var identityUser = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var identityResult = await UserManager.AddToRoleAsync(identityUser, role).ConfigureAwait(false);

                var result = identityResult.Succeeded;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> RemoveUserRoleAsync(string userId, string role)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(RemoveUserRoleAsync), userId, role);
            try
            {
                //await AssertIsPrivilegedUser().ConfigureAwait(false);

                var identityUser = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var identityResult = await UserManager.RemoveFromRoleAsync(identityUser, role).ConfigureAwait(false);

                var result = identityResult.Succeeded;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ChangeEmailAsync(string userId, string email)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(ChangeEmailAsync), userId, email);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);

                if (user.UserName != email)
                {
                    user.UserName = email;
                    user.Email = email;
                    user.EmailConfirmed = false;

                    var identityResult = await UserManager.UpdateAsync(user).ConfigureAwait(false);
                    if (!identityResult.Succeeded)
                    {
                        throw CreateServiceException("Email could not be changed.", identityResult.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(ChangePasswordAsync), userId, currentPassword, newPassword);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var identityResult = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                {
                    throw CreateServiceException("Password could not be changed.", identityResult.Errors);
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ConfirmEmailAsync(string userId, string code)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(ConfirmEmailAsync), userId, code);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var identityResult = await UserManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                {
                    throw CreateServiceException("Email could not be confirmed.", identityResult.Errors);
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> CreateNewUserAsync(MUser_CreateUser request)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(CreateNewUserAsync), request);
            try
            {
                var userId = Guid.NewGuid().ToString();

                var user = new IdentityUser
                {
                    Id = userId,
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = request.SuppressEmailConfirmation
                };

                var identityResult = await UserManager.CreateAsync(user, request.Password).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                {
                    // HACK: Validate identity errors logic.
                    throw CreateServiceException("User could not be created.", identityResult.Errors.Where(r => !r.Description.StartsWith("Email")));
                }

                var result = userId;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GenerateEmailConfirmationTokenAsync), userId);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                var result = code;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GeneratePasswordResetTokenAsync), userId);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);
                var code = await UserManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                var result = code;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ResetPasswordAsync(string userId, string password, string code)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(ResetPasswordAsync), userId, password, code);
            try
            {
                var user = await UserManager.FindByIdAsync(userId).ConfigureAwait(false);

                var identityResult = await UserManager.ResetPasswordAsync(user, code, password).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                {
                    throw CreateServiceException("Password could not be reset.", identityResult.Errors);
                }

                var isLockedOut = await UserManager.IsLockedOutAsync(user).ConfigureAwait(false);
                if (isLockedOut)
                {
                    _ = await UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public ServiceException CreateServiceException(string message, IEnumerable<IdentityError> details)
        {
            IList<string> list;
            if (details != null)
            {
                list = new List<string>();
                foreach (var detail in details)
                {
                    list.Add(detail.Description);
                }
            }
            else
            {
                list = new List<string>();
            }

            return new ServiceException(message, list);
        }
    }
}
