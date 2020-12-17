//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface ICommunicationMicroService : IEventService
    {
        Task<long> AllocateParticipantAsync(string participantPeference);
        Task<long> AllocateTopicAsync(string topicPeference, IDictionary<string, string> fields);
        Task<MCommunication_Dashboard> GetDashboardAsync();
        Task<MCommunication_Summary> GetSummaryAsync(long participantId);

        Task<MCommunication_MessageList> GetMessagesAsync(MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount, long? participantId = null);
        Task<MCommunication_Message> GetMessageAsync(long messageId, bool acknowledge, long? participantId = null);
        Task<long> SendMessageFromParticipantAsync(long participantId, string subject, string text, long? replyToMessageId = null, long? topicId = null);
        Task<long> SendMessageToParticipantAsync(long participantId, string subject, string text, long? replyToMessageId = null, long? topicId = null);
        Task AcknowledgeMessageAsync(long messageId, long? participantId = null);

        Task<MCommunication_NotificationList> GetNotificationsAsync(int recordCount, bool? acknowledged = null, long? participantId = null);
        Task<MCommunication_Notification> GetNotificationAsync(long notificationId);
        Task SendNotification(long participantId, string notificationTypeCode, long? topicId = null);
        Task AcknowledgeNotificationAsync(long notificationId, long? participantId = null);
        Task AcknowledgeNotificationsAsync(long? participantId = null);

        Task<MCommunication_AlertList> GetAlertsAsync(int recordCount, bool? acknowledged = null);
        Task<MCommunication_Alert> GetAlertAsync(long alertId);
        Task<long> CreateAlert(Exception exception, long? participantId = null, long? topicId = null);
        Task AcknowledgeAlertAsync(long alertId);
        Task AcknowledgeAlertsAsync();

        Task TransmitPendingEmailsAsync();
    }
}
