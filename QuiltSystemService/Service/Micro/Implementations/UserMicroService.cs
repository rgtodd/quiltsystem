//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Business.Email;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class UserMicroService : MicroService, IUserMicroService
    {
        private IUserEventMicroService UserEventService { get; }

        public UserMicroService(
            IApplicationLocale locale,
            ILogger<UserMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IUserEventMicroService userEventService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            UserEventService = userEventService ?? throw new ArgumentNullException(nameof(userEventService));
        }

        public async Task<MUser_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MUser_Dashboard()
                {
                    TotalUsers = await ctx.AspNetUsers.CountAsync()
                };

                var result = dashboard;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> LookupUserIdAsync(string userName)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(LookupUserIdAsync), userName);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var result = await ctx.AspNetUsers.Where(r => r.UserName == userName).Select(r => r.Id).FirstOrDefaultAsync().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MUser_User> GetUserAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetUserAsync), userId);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.Id == userId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbAspNetUser == null)
                {
                    log.Result(null);
                    return null;
                }

                var result = Create.MUser_User(dbAspNetUser);

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MUser_UserSummaryList> GetUserSummariesAsync(string userName, string role, int? recordCount)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetUserSummariesAsync), userName, role, recordCount);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                IQueryable<AspNetUser> query = ctx.AspNetUsers;

                if (!string.IsNullOrEmpty(userName))
                {
                    query = query.Where(r => r.NormalizedUserName.Contains(userName));
                }
                if (!string.IsNullOrEmpty(role))
                {
                    query = query.Where(r => r.AspNetUserRoles.Any(r => r.Role.NormalizedName == role));
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var dbUsers = await query.ToListAsync().ConfigureAwait(false);

                var aSummaries = new List<MUser_UserSummary>();
                foreach (var dbUser in dbUsers)
                {
                    var MUser = Create.MUser_UserSummary(dbUser);
                    aSummaries.Add(MUser);
                }

                var result = new MUser_UserSummaryList()
                {
                    Summaries = aSummaries
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

        public async Task UpdateNameAsync(string userId, MUser_UpdateName name)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(UpdateNameAsync), userId, name);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbUserProfile = ctx.GetUserProfile(userId);
                dbUserProfile.FirstName = name.FirstName;
                dbUserProfile.LastName = name.LastName;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateNicknameAsync(string userId, MUser_UpdateNickname nickname)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(UpdateNicknameAsync), userId, nickname);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbUserProfile = ctx.GetUserProfile(userId);
                dbUserProfile.Nickname = nickname.NickName;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateShippingAddressAsync(string userId, MUser_UpdateShippingAddress shippingAddress)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(UpdateShippingAddressAsync), userId, shippingAddress);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbUserProfile = ctx.GetUserProfile(userId);

                var dbUserAddress = dbUserProfile.UserAddresses.Where(r => r.AddressTypeCode == AddressTypeCodes.Shipping).SingleOrDefault();

                if (shippingAddress != null)
                {
                    if (dbUserAddress == null)
                    {
                        dbUserAddress = new UserAddress()
                        {
                            UserProfile = dbUserProfile,
                            AddressTypeCodeNavigation = ctx.AddressType(AddressTypeCodes.Shipping)
                        };
                        _ = ctx.UserAddresses.Add(dbUserAddress);
                    }
                    dbUserAddress.AddressLine1 = shippingAddress.AddressLine1;
                    dbUserAddress.AddressLine2 = shippingAddress.AddressLine2;
                    dbUserAddress.City = shippingAddress.City;
                    dbUserAddress.StateCode = shippingAddress.StateCode;
                    dbUserAddress.PostalCode = shippingAddress.PostalCode;
                    dbUserAddress.CountryCode = shippingAddress.CountryCode;
                }
                else
                {
                    if (dbUserAddress != null)
                    {
                        _ = ctx.UserAddresses.Remove(dbUserAddress);
                    }
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateTimeZoneAsync(string userId, MUser_UpdateTimeZone timeZone)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(UpdateTimeZoneAsync), userId, timeZone);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbUserProfile = ctx.GetUserProfile(userId);
                dbUserProfile.TimeZoneId = timeZone.TimeZoneId;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateWebsiteAsync(string userId, MUser_UpdateWebsite website)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(UpdateWebsiteAsync), userId, website);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbUserProfile = ctx.GetUserProfile(userId);
                dbUserProfile.WebsiteUrl = website.WebsiteUrl;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(ProcessEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<int> CancelEventsAsync()
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(CancelEventsAsync));
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);

                var result = 0;

                log.Result(result);

                return result;
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

        public async Task<bool> GetEmailConfirmedAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetEmailConfirmedAsync), userId);
            try
            {
                var user = await GetUserByIdAsync(userId);

                var result = user.EmailConfirmed;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MUser_User> GetUserByIdAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetUserByIdAsync), userId);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.Id == userId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbAspNetUser == null)
                {
                    log.Result(null);
                    return null;
                }

                var result = Create.MUser_User(dbAspNetUser);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MUser_User> GetUserByNameAsync(string userName)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetUserByNameAsync), userName);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.UserName == userName).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbAspNetUser == null)
                {
                    log.Result(null);
                    return null;
                }

                var result = Create.MUser_User(dbAspNetUser);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<string> GetUserEmailAsync(string userId)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(GetUserEmailAsync), userId);
            try
            {
                var user = await GetUserByIdAsync(userId);

                var result = user.Email;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SendConfirmationEmailAsync(string userId, string callbackUrl)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(SendConfirmationEmailAsync), userId, callbackUrl);
            try
            {
                var subject = Constants.WebsiteUrl + " Email Confirmation";
                var body = $"<p>A new account was created at <a href=\"{Constants.WebsiteFullUrl}\">{Constants.WebsiteUrl}</a> using this email address.<p>If you created this account, please confirm by clicking <a href=\"{callbackUrl}\">here</a>.</p>";

                var user = await GetUserByIdAsync(userId);
                await SendEmailAsyncCore(user.Email, subject, body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SendResetEmailAsync(string userId, string callbackUrl)
        {
            using var log = BeginFunction(nameof(UserMicroService), nameof(SendConfirmationEmailAsync), userId, callbackUrl);
            try
            {
                var subject = Constants.WebsiteUrl + " Email Reset";
                var body = $"<p>A request was made to reset the password for an account created at <a href=\"{Constants.WebsiteFullUrl}\">{Constants.WebsiteUrl}</a> using this email address.<p>If you made this request, you may specify a new password by clicking <a href=\"{callbackUrl}\">here</a>.";

                var user = await GetUserByIdAsync(userId);
                await SendEmailAsyncCore(user.Email, subject, body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SendEmailAsyncCore(string email, string subject, string htmlMessage)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbEmailRequest = new EmailRequest()
            {
                EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                SenderEmail = Constants.DoNotReplyEmail,
                SenderEmailName = Constants.DoNotReplyEmailName,
                RecipientEmail = email,
                RecipientEmailName = email,
                RecipientParticipantId = null,
                Subject = subject,
                BodyText = htmlMessage,
                BodyHtml = htmlMessage,
                BodyTypeCode = EmailBodyTypes.Html,
                CreateDateTimeUtc = GetUtcNow(),
                EmailRequestStatusDateTimeUtc = GetUtcNow()
            };
            _ = ctx.EmailRequests.Add(dbEmailRequest);

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
        }

        private static class Create
        {
            public static MUser_User MUser_User(AspNetUser dbAspNetUser)
            {
                var dbUserProfile = dbAspNetUser.UserProfileAspNetUsers.SingleOrDefault()?.UserProfile;
                var dbUserAddress = dbUserProfile?.UserAddresses.Where(r => r.AddressTypeCode == AddressTypeCodes.Shipping).SingleOrDefault();

                var user = new MUser_User()
                {
                    UserId = dbAspNetUser.Id,
                    Email = dbAspNetUser.Email,
                    EmailConfirmed = dbAspNetUser.EmailConfirmed,
                    FirstName = dbUserProfile?.FirstName,
                    LastName = dbUserProfile?.LastName,
                    NickName = dbUserProfile?.Nickname,
                    WebsiteUrl = dbUserProfile?.WebsiteUrl,
                    ShippingAddressLine1 = dbUserAddress?.AddressLine1,
                    ShippingAddressLine2 = dbUserAddress?.AddressLine2,
                    ShippingCity = dbUserAddress?.City,
                    ShippingStateCode = dbUserAddress?.StateCode,
                    ShippingPostalCode = dbUserAddress?.PostalCode,
                    ShippingCountryCode = dbUserAddress?.CountryCode,
                    TimeZoneId = dbUserProfile?.TimeZoneId
                };

                if (!string.IsNullOrEmpty(user.TimeZoneId))
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
                    user.TimeZoneName = timeZone.DisplayName;
                }

                return user;
            }

            public static MUser_User MUser_User(IdentityUser user)
            {
                return new MUser_User()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    FirstName = null,
                    LastName = null,
                    NickName = null,
                    WebsiteUrl = null,
                    ShippingAddressLine1 = null,
                    ShippingAddressLine2 = null,
                    ShippingCity = null,
                    ShippingStateCode = null,
                    ShippingPostalCode = null,
                    ShippingCountryCode = null,
                    TimeZoneId = null,
                    TimeZoneName = null
                };
            }

            public static MUser_UserSummary MUser_UserSummary(AspNetUser dbAspNetUser)
            {
                var userSummary = new MUser_UserSummary()
                {
                    UserId = dbAspNetUser.Id,
                    UserName = dbAspNetUser.UserName,
                    Email = dbAspNetUser.Email,
                    EmailConfirmed = dbAspNetUser.EmailConfirmed,
                    AccessFailedCount = dbAspNetUser.AccessFailedCount
                };

                return userSummary;
            }
        }
    }
}
