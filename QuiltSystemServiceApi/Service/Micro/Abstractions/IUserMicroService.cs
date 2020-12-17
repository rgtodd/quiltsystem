//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IUserMicroService : IEventService
    {
        Task<MUser_Dashboard> GetDashboardAsync();

        Task<MUser_User> GetUserAsync(string userId);
        Task<string> LookupUserIdAsync(string userName);

        Task<MUser_UserSummaryList> GetUserSummariesAsync(string userName, string role, int? recordCount);

        Task UpdateNameAsync(string userId, MUser_UpdateName name);
        Task UpdateNicknameAsync(string userId, MUser_UpdateNickname nickname);
        Task UpdateShippingAddressAsync(string userId, MUser_UpdateShippingAddress shippingAddress);
        Task UpdateTimeZoneAsync(string userId, MUser_UpdateTimeZone timeZone);
        Task UpdateWebsiteAsync(string userId, MUser_UpdateWebsite website);

        Task<bool> GetEmailConfirmedAsync(string userId);
        Task<MUser_User> GetUserByIdAsync(string userId);
        Task<MUser_User> GetUserByNameAsync(string userName);
        Task<string> GetUserEmailAsync(string userId);
        Task SendConfirmationEmailAsync(string userId, string callbackUrl);
        Task SendResetEmailAsync(string userId, string callbackUrl);
    }
}
