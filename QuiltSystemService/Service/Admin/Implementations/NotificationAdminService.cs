//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class NotificationAdminService : BaseService, INotificationAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public NotificationAdminService(
            IApplicationRequestServices requestServices,
            ILogger<NotificationAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task<ANotification_NotificationList> GetNotificationsAsync(bool? acknowledged, int recordCount)
        {
            using var log = BeginFunction(nameof(NotificationAdminService), nameof(GetNotificationsAsync), acknowledged, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var notifications = new List<ANotification_Notification>();
                var mNotifications = await CommunicationMicroService.GetNotificationsAsync(recordCount, acknowledged).ConfigureAwait(false);
                foreach (var mNotification in mNotifications.Notifications)
                {
                    var mUser = mNotification.ParticipantReference != null && TryParseUserId.FromParticipantReference(mNotification.ParticipantReference, out string userId)
                        ? await UserMicroService.GetUserAsync(userId)
                        : null;

                    var notification = Create.ANotification_Notification(mNotification, mUser);
                    notifications.Add(notification);
                }

                var result = new ANotification_NotificationList()
                {
                    Notifications = notifications
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

        public async Task<ANotification_Notification> GetNotificationAsync(long notificationId)
        {
            using var log = BeginFunction(nameof(NotificationAdminService), nameof(GetNotificationAsync), notificationId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mNotification = await CommunicationMicroService.GetNotificationAsync(notificationId).ConfigureAwait(false);

                var mUser = mNotification.ParticipantReference != null && TryParseUserId.FromParticipantReference(mNotification.ParticipantReference, out string userId)
                    ? await UserMicroService.GetUserAsync(userId)
                    : null;

                var result = Create.ANotification_Notification(mNotification, mUser);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeNotificationsAsync()
        {
            using var log = BeginFunction(nameof(NotificationAdminService), nameof(AcknowledgeNotificationsAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await CommunicationMicroService.AcknowledgeNotificationsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeNotificationAsync(long notificationId)
        {
            using var log = BeginFunction(nameof(NotificationAdminService), nameof(AcknowledgeNotificationAsync), notificationId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await CommunicationMicroService.AcknowledgeNotificationAsync(notificationId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static ANotification_Notification ANotification_Notification(MCommunication_Notification mNotification, MUser_User mUser)
            {
                return new ANotification_Notification()
                {
                    MNotification = mNotification,
                    MUser = mUser
                };
            }
        }
    }
}
