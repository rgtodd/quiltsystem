//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class UserAdminService : BaseService, IUserAdminService
    {
        private IQuiltContextFactory QuiltContextFactory { get; }
        private IOrderMicroService OrderMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }
        private IUserMicroService UserMicroService { get; }
        private IUserManagementMicroService UserManagementMicroService { get; }

        public UserAdminService(
            IApplicationRequestServices requestServices,
            ILogger<UserAdminService> logger,
            IQuiltContextFactory quiltContextFactory,
            //UserManager<IdentityUser> userManager,
            IOrderMicroService orderMicroService,
            ISquareMicroService squareMicroService,
            IUserMicroService userMicroService,
            IUserManagementMicroService userManagementMicroService)
            : base(requestServices, logger)
        {
            QuiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
            UserManagementMicroService = userManagementMicroService ?? throw new ArgumentNullException(nameof(userManagementMicroService));
        }

        #region IAdmin_UserService

        public async Task<AUser_User> GetUserAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserAdminService), nameof(GetUserAsync), userId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbAspNetUser = await ctx.AspNetUsers
                    .Include(r => r.AspNetUserRoles)
                        .ThenInclude(r => r.Role)
                    .Include(r => r.AspNetUserLogins)
                    .Where(r => r.Id == userId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                if (dbAspNetUser == null)
                {
                    log.Result(null);
                    return null;
                }

                var squareCustomerReference = CreateSquareCustomerReference.FromUserId(userId);
                var squareCustomerId = await SquareMicroService.LookupSquareCustomerIdAsync(squareCustomerReference).ConfigureAwait(false);
                var mSquareCustomerSummaryList = squareCustomerId != null
                    ? await SquareMicroService.GetSquareCustomerSummariesAsync(squareCustomerId.Value, null).ConfigureAwait(false)
                    : null;
                var mSquarePaymentSummaryList = squareCustomerId != null
                    ? await SquareMicroService.GetSquarePaymentSummariesAsync(squareCustomerId, null, null).ConfigureAwait(false)
                    : null;

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.LookupOrdererAsync(ordererReference).ConfigureAwait(false);
                var mOrderSummaryList = ordererId != null
                    ? await OrderMicroService.GetOrderSummariesAsync(null, null, MOrder_OrderStatus.MetaAll, ordererId, null).ConfigureAwait(false)
                    : null;

                var result = Create.AUser_User(
                    dbAspNetUser,
                    mOrderSummaryList,
                    mSquareCustomerSummaryList?.Summaries.FirstOrDefault(),
                    mSquarePaymentSummaryList);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AUser_UserSummaryList> GetUsersAsync(AUser_GetUsers request)
        {
            using var log = BeginFunction(nameof(UserAdminService), nameof(GetUsersAsync), request);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mSummaries = await UserMicroService.GetUserSummariesAsync(request.UserName, request.Role, request.RecordCount);

                var result = new AUser_UserSummaryList()
                {
                    MSummaries = mSummaries
                };

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> AddUserRoleAsync(string userId, string role)
        {
            using var log = BeginFunction(nameof(UserAdminService), nameof(AddUserRoleAsync), userId, role);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var success = await UserManagementMicroService.AddUserRoleAsync(userId, role).ConfigureAwait(false);

                var result = success;

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
            using var log = BeginFunction(nameof(UserAdminService), nameof(RemoveUserRoleAsync), userId, role);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var success = await UserManagementMicroService.RemoveUserRoleAsync(userId, role).ConfigureAwait(false);

                var result = success;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion IAdmin_UserService

        private static class Create
        {
            public static AUser_User AUser_User(AspNetUser user, MOrder_OrderSummaryList mOrderSummaryList, MSquare_CustomerSummary mSquareCustomerSummary, MSquare_PaymentSummaryList mSquarePaymentSummaryList)
            {
                var roles = user.AspNetUserRoles.Select(r => r.Role.NormalizedName).ToList();

                var loginProviders = user.AspNetUserLogins.Select(r => r.LoginProvider).ToList();

                var aUser = new AUser_User()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles,
                    LoginProviders = loginProviders,
                    MOrders = mOrderSummaryList,
                    MSquareCustomer = mSquareCustomerSummary,
                    MSquarePayments = mSquarePaymentSummaryList
                };

                return aUser;
            }
        }
    }
}
