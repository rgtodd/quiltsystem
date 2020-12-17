//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface INotificationAdminService
    {
        Task<ANotification_NotificationList> GetNotificationsAsync(bool? acknowledged, int recordCount);
        Task<ANotification_Notification> GetNotificationAsync(long notificationId);
        Task AcknowledgeNotificationsAsync();
        Task AcknowledgeNotificationAsync(long notificationId);
    }
}
