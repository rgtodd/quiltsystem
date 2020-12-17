//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Business.Email;
using RichTodd.QuiltSystem.Business.Operation;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Extensions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;


namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class CommunicationMicroService : MicroService, ICommunicationMicroService
    {
        private IApplicationEmailSender ApplicationEmailSender { get; }
        private ICommunicationEventMicroService CommunicationEventService { get; }

        public CommunicationMicroService(
            IApplicationLocale locale,
            ILogger<CommunicationMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IApplicationEmailSender applicationEmailSender,
            ICommunicationEventMicroService communicationEventService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            ApplicationEmailSender = applicationEmailSender ?? throw new ArgumentNullException(nameof(applicationEmailSender));
            CommunicationEventService = communicationEventService ?? throw new ArgumentNullException(nameof(communicationEventService));
        }

        public async Task<long> AllocateParticipantAsync(string participantReference)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AllocateParticipantAsync), participantReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbParticipant = await ctx.Participants.Where(r => r.ParticipantReference == participantReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbParticipant == null)
                {
                    dbParticipant = new Participant()
                    {
                        ParticipantReference = participantReference
                    };
                    _ = ctx.Participants.Add(dbParticipant);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbParticipant.ParticipantId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> AllocateTopicAsync(string topicReference, IDictionary<string, string> fields)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AllocateTopicAsync), topicReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbTopic = await ctx.Topics.Where(r => r.TopicReference == topicReference)
                    .Include(r => r.TopicFields)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (dbTopic == null)
                {
                    dbTopic = new Topic()
                    {
                        TopicReference = topicReference
                    };

                    if (fields != null)
                    {
                        foreach (var key in fields.Keys)
                        {
                            var dbTopicField = new TopicField()
                            {
                                FieldCode = key,
                                FieldValue = fields[key]
                            };
                            dbTopic.TopicFields.Add(dbTopicField);
                        }
                    }

                    _ = ctx.Topics.Add(dbTopic);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    if (fields != null)
                    {
                        foreach (var key in fields.Keys)
                        {
                            var dbTopicField = dbTopic.TopicFields.Where(r => r.FieldCode == key).FirstOrDefault();
                            if (dbTopicField != null)
                            {
                                dbTopicField.FieldValue = fields[key];
                            }
                            else
                            {
                                dbTopicField = new TopicField()
                                {
                                    FieldCode = key,
                                    FieldValue = fields[key]
                                };
                                dbTopic.TopicFields.Add(dbTopicField);
                            }
                        }
                    }
                }

                var result = dbTopic.TopicId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MCommunication_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MCommunication_Dashboard()
                {
                    TotalAlerts = await ctx.Alerts.CountAsync(),
                    TotalMessages = await ctx.Messages.CountAsync(),
                    TotalNotifications = await ctx.Notifications.CountAsync()
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

        public async Task<MCommunication_Summary> GetSummaryAsync(long participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetSummaryAsync), participantId);
            try
            {
                using var ctx = CreateQuiltContext();

                var result = new MCommunication_Summary
                {
                    HasNotifications = await ctx.Notifications.AnyAsync(r => r.ParticipantId == participantId && r.AcknowledgementDateTimeUtc == null).ConfigureAwait(false),
                    HasMessages = await ctx.Messages.AnyAsync(r => r.ParticipantId == participantId && r.SendReceiveCode == SendReceiveCodes.ToUser && r.AcknowledgementDateTimeUtc == null).ConfigureAwait(false)
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

        public async Task TransmitPendingEmailsAsync()
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(TransmitPendingEmailsAsync));
            try
            {
                IEnumerable<long> emailRequestIds;
                using (var ctx = QuiltContextFactory.Create())
                {
                    emailRequestIds = await ctx.EmailRequests.Where(r => r.EmailRequestStatusCode == EmailRequestStatusCodes.Posted).Select(r => r.EmailRequestId).ToListAsync().ConfigureAwait(false);
                }

                foreach (var emailRequestId in emailRequestIds)
                {
                    await TransmitPendingEmailAsync(emailRequestId);
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #region Message

        public async Task<MCommunication_Message> GetMessageAsync(long messageId, bool acknowledge, long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetMessageAsync), participantId, messageId, acknowledge);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = ctx.Messages.Where(r => r.MessageId == messageId);

                if (participantId != null)
                {
                    query = query.Where(r => r.ParticipantId == participantId.Value);
                }

                var dbMessage = await query.SingleAsync().ConfigureAwait(false);

                if (acknowledge && dbMessage.SendReceiveCode == SendReceiveCodes.ToUser && dbMessage.AcknowledgementDateTimeUtc == null)
                {
                    dbMessage.AcknowledgementDateTimeUtc = GetUtcNow();
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var conversation = new List<MCommunication_Message>();
                foreach (var dbRelatedMessage in await ctx.Messages.Where(r => r.ConversationId == dbMessage.ConversationId).OrderBy(r => r.CreateDateTimeUtc).ToListAsync().ConfigureAwait(false))
                {
                    conversation.Add(Create.MCommunication_Message(ctx, dbRelatedMessage, CanAcknowledge(dbRelatedMessage), false, null));
                }

                var result = Create.MCommunication_Message(ctx, dbMessage, CanAcknowledge(dbMessage), true, conversation);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MCommunication_MessageList> GetMessagesAsync(MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount, long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetMessagesAsync), participantId, mailbox, status, recordCount);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = (IQueryable<Message>)ctx.Messages;

                if (participantId != null)
                {
                    query = query.Where(r => r.ParticipantId == participantId);
                }

                switch (mailbox)
                {
                    case MCommunication_MessageMailbox.ToUser:
                        query = query.Where(r => r.SendReceiveCode == SendReceiveCodes.ToUser);
                        break;

                    case MCommunication_MessageMailbox.FromUser:
                        query = query.Where(r => r.SendReceiveCode == SendReceiveCodes.FromUser);
                        break;

                    case MCommunication_MessageMailbox.MetaAll:
                        // No action required.
                        break;

                    default:
                        throw new ArgumentException($"Unknown mailbox {mailbox}.");
                }

                switch (status)
                {
                    case MCommunication_MessageStatus.Acknowledged:
                        query = query.Where(r => r.AcknowledgementDateTimeUtc != null);
                        break;

                    case MCommunication_MessageStatus.Unacknowledged:
                        query = query.Where(r => r.AcknowledgementDateTimeUtc == null);
                        break;

                    case MCommunication_MessageStatus.MetaAll:
                        // No action required.
                        break;

                    default:
                        throw new ArgumentException($"Unknown status {status}.");
                }

                query = query.Take(recordCount);

                var messages = new List<MCommunication_Message>();
                foreach (var dbMessage in await query.ToListAsync().ConfigureAwait(false))
                {
                    var canAcknowledge = CanAcknowledge(dbMessage);

                    var message = Create.MCommunication_Message(ctx, dbMessage, canAcknowledge, false, null);
                    messages.Add(message);
                }

                var result = new MCommunication_MessageList()
                {
                    Messages = messages
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

        public async Task<long> SendMessageFromParticipantAsync(long participantId, string subject, string text, long? replyToMessageId, long? topicId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(SendMessageFromParticipantAsync), participantId, subject, text, replyToMessageId, topicId);
            try
            {
                var result = await SendMessageAsync(SendReceiveCodes.FromUser, participantId, subject, text, replyToMessageId, topicId);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> SendMessageToParticipantAsync(long participantId, string subject, string text, long? replyToMessageId, long? topicId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(SendMessageToParticipantAsync), participantId, subject, text, replyToMessageId, topicId);
            try
            {
                var result = await SendMessageAsync(SendReceiveCodes.ToUser, participantId, subject, text, replyToMessageId, topicId);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeMessageAsync(long messageId, long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AcknowledgeMessageAsync), participantId, messageId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = ctx.Messages.Where(r => r.MessageId == messageId);

                if (participantId != null)
                {
                    query = query.Where(r => r.ParticipantId == participantId.Value);
                }

                var dbMessage = await query.SingleAsync().ConfigureAwait(false);

                if (dbMessage.AcknowledgementDateTimeUtc == null)
                {
                    dbMessage.AcknowledgementDateTimeUtc = GetUtcNow();
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Notification

        public async Task<MCommunication_NotificationList> GetNotificationsAsync(int recordCount, bool? acknowledged, long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetNotificationsAsync), participantId, acknowledged, recordCount);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = (IQueryable<Notification>)ctx.Notifications;

                if (participantId != null)
                {
                    query = query.Where(r => r.ParticipantId == participantId.Value);
                }

                if (acknowledged != null)
                {
                    query = acknowledged.Value
                        ? query.Where(r => r.AcknowledgementDateTimeUtc != null)
                        : query.Where(r => r.AcknowledgementDateTimeUtc == null);
                }

                query = query.Take(recordCount);

                var notifications = new List<MCommunication_Notification>();
                foreach (var dbNotification in await query.ToListAsync().ConfigureAwait(false))
                {
                    var notification = Create.MCommunication_Notification(dbNotification, false);
                    notifications.Add(notification);
                }

                var result = new MCommunication_NotificationList()
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

        public async Task<MCommunication_Notification> GetNotificationAsync(long notificationId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetNotificationAsync), notificationId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbNotification = await ctx.Notifications
                    .Include(r => r.NotificationEmailRequests)
                        .ThenInclude(r => r.EmailRequest)
                    .Where(r => r.NotificationId == notificationId)
                    .FirstAsync().ConfigureAwait(false);

                var notification = Create.MCommunication_Notification(dbNotification, true);

                var result = notification;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SendNotification(long participantId, string notificationTypeCode, long? topicId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(SendNotification), participantId, notificationTypeCode, topicId);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbNotification = new Notification()
                {
                    ParticipantId = participantId,
                    NotificationTypeCode = notificationTypeCode,
                    TopicId = topicId,
                    CreateDateTimeUtc = GetUtcNow()
                };
                _ = ctx.Notifications.Add(dbNotification);

                var dbNotificationType = ctx.NotificationTypes.Find(dbNotification.NotificationTypeCode);

                var formatter = new NotificationEmailFormatter(dbNotificationType.Subject);

                var dbParticipant = ctx.Participants.Where(r => r.ParticipantId == dbNotification.ParticipantId).Single();

                string email;
                if (TryParseUserId.FromParticipantReference(dbParticipant.ParticipantReference, out string userId))
                {
                    var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.Id == userId).SingleAsync().ConfigureAwait(false);
                    email = dbAspNetUser.Email;

                    var dbEmailRequest = new EmailRequest()
                    {
                        EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                        SenderEmail = Constants.DoNotReplyEmail,
                        SenderEmailName = Constants.DoNotReplyEmailName,
                        RecipientEmail = email,
                        RecipientEmailName = email,
                        RecipientParticipant = dbNotification.Participant,
                        Subject = formatter.GetSubject(),
                        BodyText = formatter.GetText(),
                        BodyHtml = formatter.GetHtml(),
                        BodyTypeCode = EmailBodyTypes.Notification,
                        CreateDateTimeUtc = GetUtcNow(),
                        EmailRequestStatusDateTimeUtc = GetUtcNow(),
                    };
                    _ = ctx.EmailRequests.Add(dbEmailRequest);

                    var dbNotificationEmailRequest = new NotificationEmailRequest()
                    {
                        Notification = dbNotification,
                        EmailRequest = dbEmailRequest
                    };
                    _ = ctx.NotificationEmailRequests.Add(dbNotificationEmailRequest);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeNotificationAsync(long notificationId, long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AcknowledgeNotificationAsync), participantId, notificationId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = ctx.Notifications.Where(r => r.NotificationId == notificationId);

                if (participantId != null)
                {
                    query = query.Where(r => participantId == participantId.Value);
                }

                var dbNotification = await query.SingleAsync().ConfigureAwait(false);

                if (dbNotification.AcknowledgementDateTimeUtc == null)
                {
                    dbNotification.AcknowledgementDateTimeUtc = GetUtcNow();
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeNotificationsAsync(long? participantId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AcknowledgeNotificationsAsync), participantId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = ctx.Notifications.Where(r => r.AcknowledgementDateTimeUtc == null);

                if (participantId != null)
                {
                    query = query.Where(r => r.ParticipantId == participantId.Value);
                }

                var dbNotifications = await query.ToListAsync().ConfigureAwait(false);

                foreach (var dbNotification in dbNotifications)
                {
                    dbNotification.AcknowledgementDateTimeUtc = GetUtcNow();
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Alert

        public async Task<MCommunication_AlertList> GetAlertsAsync(int recordCount, bool? acknowledged)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetAlertsAsync), acknowledged, recordCount);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var query = (IQueryable<Alert>)ctx.Alerts;

                if (acknowledged != null)
                {
                    query = acknowledged.Value
                        ? query.Where(r => r.AcknowledgementDateTimeUtc != null)
                        : query.Where(r => r.AcknowledgementDateTimeUtc == null);
                }

                query = query.Take(recordCount);

                var alerts = new List<MCommunication_Alert>();
                foreach (var dbAlert in await query.ToListAsync().ConfigureAwait(false))
                {
                    var alert = Create.MCommunication_Alert(dbAlert);
                    alerts.Add(alert);
                }

                var result = new MCommunication_AlertList()
                {
                    Alerts = alerts
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

        public async Task<MCommunication_Alert> GetAlertAsync(long alertId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(GetAlertAsync), alertId);
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var dbAlert = await ctx.Alerts.Where(r => r.AlertId == alertId).FirstAsync().ConfigureAwait(false);

                var alert = Create.MCommunication_Alert(dbAlert);

                var result = alert;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateAlert(Exception exception, long? participantId = null, long? topicId = null)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(CreateAlert), exception, topicId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                var dbAlert = new Alert()
                {
                    AlertTypeCode = AlertTypeCodes.OperationException,
                    CreateDateTimeUtc = utcNow,
                    Description = "Operation Exception",
                    Exception = exception.GetDetail(),
                    ParticipantId = participantId,
                    TopicId = topicId
                };
                _ = ctx.Alerts.Add(dbAlert);

                var dbAlertType = ctx.AlertTypes.Find(dbAlert.AlertTypeCode);

                var formatter = new AlertEmailFormatter(dbAlertType.Name);

                var dbEmailRequest = new EmailRequest()
                {
                    EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                    SenderEmail = Constants.DoNotReplyEmail,
                    SenderEmailName = Constants.DoNotReplyEmailName,
                    RecipientEmail = Constants.AdminMailEmail,
                    RecipientEmailName = Constants.AdminMailEmailName,
                    RecipientParticipantId = null,
                    Subject = formatter.GetSubject(),
                    BodyText = formatter.GetText(),
                    BodyHtml = formatter.GetHtml(),
                    BodyTypeCode = EmailBodyTypes.Alert,
                    CreateDateTimeUtc = utcNow,
                    EmailRequestStatusDateTimeUtc = utcNow,
                };
                _ = ctx.EmailRequests.Add(dbEmailRequest);

                var dbAlertEmailRequest = new AlertEmailRequest()
                {
                    Alert = dbAlert,
                    EmailRequest = dbEmailRequest,
                };
                _ = ctx.AlertEmailRequests.Add(dbAlertEmailRequest);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                return dbAlert.AlertId;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeAlertAsync(long alertId)
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AcknowledgeAlertAsync), alertId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                var dbAlert = await ctx.Alerts.Where(r => r.AlertId == alertId).SingleAsync().ConfigureAwait(false);

                if (dbAlert.AcknowledgementDateTimeUtc == null)
                {
                    dbAlert.AcknowledgementDateTimeUtc = GetUtcNow();
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task AcknowledgeAlertsAsync()
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(AcknowledgeAlertsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                await ctx.Alerts.Where(r => r.AcknowledgementDateTimeUtc == null).ForEachAsync(r => r.AcknowledgementDateTimeUtc = utcNow).ConfigureAwait(false);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Event

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(ProcessEventsAsync));
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
            using var log = BeginFunction(nameof(CommunicationMicroService), nameof(CancelEventsAsync));
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

        #endregion

        private bool CanAcknowledge(Message dbMessage)
        {
            return dbMessage.AcknowledgementDateTimeUtc == null;
        }

        private async Task TransmitPendingEmailAsync(long emailRequestId)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbEmailRequest = await ctx.EmailRequests.Where(r => r.EmailRequestId == emailRequestId).SingleAsync().ConfigureAwait(false);
            if (dbEmailRequest.EmailRequestStatusCode != EmailRequestStatusCodes.Posted)
            {
                throw new BusinessOperationException(string.Format("Invalid email request status (ID = {0}, Status = {1}).", emailRequestId, dbEmailRequest.EmailRequestStatusCode));
            }

            var bodyType = dbEmailRequest.BodyTypeCode;
            var bodyText = dbEmailRequest.BodyText;
            var bodyHtml = dbEmailRequest.BodyHtml;

            switch (bodyType)
            {
                case EmailBodyTypes.Alert:
                    {
                        var dbAlertEmailRequest = dbEmailRequest.AlertEmailRequests.FirstOrDefault();
                        if (dbAlertEmailRequest != null)
                        {
                            bodyText = bodyText.Replace(TemplateVariables.DeferredAlertId, dbAlertEmailRequest.AlertId.ToString());
                            bodyHtml = bodyHtml.Replace(TemplateVariables.DeferredAlertId, dbAlertEmailRequest.AlertId.ToString());
                        }
                    }
                    break;

                case EmailBodyTypes.Message:
                    {
                        var dbMessageEmailRequest = dbEmailRequest.MessageEmailRequests.FirstOrDefault();
                        if (dbMessageEmailRequest != null)
                        {
                            bodyText = bodyText.Replace(TemplateVariables.DeferredMessageId, dbMessageEmailRequest.MessageId.ToString());
                            bodyHtml = bodyHtml.Replace(TemplateVariables.DeferredMessageId, dbMessageEmailRequest.MessageId.ToString());
                        }
                    }
                    break;

                case EmailBodyTypes.Notification:
                    {
                        var dbNotificationEmailRequest = dbEmailRequest.NotificationEmailRequests.FirstOrDefault();
                        if (dbNotificationEmailRequest != null)
                        {
                            bodyText = bodyText.Replace(TemplateVariables.DeferredNotificationId, dbNotificationEmailRequest.NotificationId.ToString());
                            bodyHtml = bodyHtml.Replace(TemplateVariables.DeferredNotificationId, dbNotificationEmailRequest.NotificationId.ToString());
                        }
                    }
                    break;
            }

            var emailRequest = new ApplicationEmailRequest()
            {
                SenderEmail = dbEmailRequest.SenderEmail,
                SenderEmailName = dbEmailRequest.SenderEmailName,
                RecipientEmail = dbEmailRequest.RecipientEmail,
                RecipientEmailName = dbEmailRequest.RecipientEmailName,
                Subject = dbEmailRequest.Subject,
                BodyText = bodyText,
                BodyHtml = bodyHtml
            };

            await ApplicationEmailSender.SendEmailAsync(emailRequest);

            // Update the email request status.
            //
            dbEmailRequest.EmailRequestStatusCode = EmailRequestStatusCodes.Complete;
            dbEmailRequest.EmailRequestStatusDateTimeUtc = GetUtcNow();

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<long> SendMessageAsync(string sendReceiveCode, long participantId, string subject, string text, long? replyToMessageId, long? topicId)
        {
            using var ctx = QuiltContextFactory.Create();

            var dbParticipant = await ctx.Participants.Where(r => r.ParticipantId == participantId).FirstAsync().ConfigureAwait(false);

            string formattedName;
            string email;
            if (TryParseUserId.FromParticipantReference(dbParticipant.ParticipantReference, out string userId))
            {
                var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.Id == userId).SingleAsync().ConfigureAwait(false);
                var dbUserProfile = await ctx.UserProfiles.Where(r => r.UserProfileAspNetUser.AspNetUserId == userId).SingleOrDefaultAsync().ConfigureAwait(false);

                formattedName = Create.FormattedName(dbUserProfile);
                email = dbAspNetUser.Email;
            }
            else
            {
                formattedName = dbParticipant.ParticipantReference;
                email = $"{dbParticipant.ParticipantId}@richtodd.com";
            }

            Message dbReplyToMessage;
            if (replyToMessageId != null)
            {
                dbReplyToMessage = await ctx.Messages.Where(r => r.MessageId == replyToMessageId.Value).SingleAsync().ConfigureAwait(false);

                if (dbReplyToMessage.ParticipantId != participantId)
                {
                    throw new InvalidOperationException(string.Format("Reply to message user ID mismatch (user ID = {0}, message user ID = {1}). ", participantId, dbReplyToMessage.Participant.ParticipantReference));
                }

                if (topicId == null)
                {
                    // Propagate order ID from original message.
                    //
                    topicId = dbReplyToMessage.TopicId;
                }
                else
                {
                    if (dbReplyToMessage.TopicId != null)
                    {
                        if (dbReplyToMessage.TopicId != topicId)
                        {
                            throw new InvalidOperationException(string.Format("Reply to message topic ID mismatch (topic ID = {0}, message topic ID = {1}). ", topicId, dbReplyToMessage.TopicId));
                        }
                    }
                }
            }
            else
            {
                dbReplyToMessage = null;
            }

            var dbMessage = new Message()
            {
                ParticipantId = participantId,
                ConversationId = dbReplyToMessage != null ? dbReplyToMessage.ConversationId : Guid.NewGuid(),
                SendReceiveCode = sendReceiveCode,
                Subject = subject,
                Text = text,
                Email = email,
                TopicId = topicId,
                CreateDateTimeUtc = GetUtcNow()
            };
            _ = ctx.Messages.Add(dbMessage);

            IDictionary<string, string> topicFields = new Dictionary<string, string>();
            if (topicId.HasValue)
            {
                foreach (var dbTopicField in await ctx.TopicFields.Where(r => r.TopicId == topicId.Value).ToListAsync())
                {
                    topicFields[dbTopicField.FieldCode] = dbTopicField.FieldValue;
                }
            }

            var formatter = new InboundMessageEmailFormatter(formattedName, email, subject, text, topicFields);

            var dbEmailRequest = new EmailRequest()
            {
                EmailRequestStatusCode = EmailRequestStatusCodes.Posted,
                SenderEmail = Constants.DoNotReplyEmail, // dbAspNetUser.Email
                SenderEmailName = Constants.DoNotReplyEmailName, // dbAspNetUser.EmailName
                RecipientEmail = Constants.AdminMailEmail,
                RecipientEmailName = Constants.AdminMailEmailName,
                RecipientParticipantId = null,
                Subject = formatter.GetSubject(),
                BodyText = formatter.GetText(),
                BodyHtml = formatter.GetHtml(),
                BodyTypeCode = EmailBodyTypes.Message,
                CreateDateTimeUtc = GetUtcNow(),
                EmailRequestStatusDateTimeUtc = GetUtcNow()
            };
            _ = ctx.EmailRequests.Add(dbEmailRequest);

            var dbMessageEmailRequest = new MessageEmailRequest()
            {
                Message = dbMessage,
                EmailRequest = dbEmailRequest
            };
            _ = ctx.MessageEmailRequests.Add(dbMessageEmailRequest);

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            return dbMessage.MessageId;
        }

        private static class Create
        {
            public static MCommunication_Message MCommunication_Message(
                QuiltContext ctx,
                Message dbMessage,
                bool canAcknowledge,
                bool includeEmails,
                IList<MCommunication_Message> conversation)
            {
                string email;
                if (TryParseUserId.FromParticipantReference(dbMessage.Participant.ParticipantReference, out string userId))
                {
                    var dbAspNetUser = ctx.AspNetUsers.Where(r => r.Id == userId).Single();
                    email = dbAspNetUser.Email;
                }
                else
                {
                    userId = null;
                    email = $"{dbMessage.Participant.ParticipantId}@richtodd.com";
                }

                string from;
                string to;
                if (dbMessage.SendReceiveCode == SendReceiveCodes.FromUser)
                {
                    from = email;
                    to = Constants.AdminMailEmailName;
                }
                else // ToUser
                {
                    from = Constants.AdminMailEmailName;
                    to = email;
                }

                IList<MCommunication_Email> emails = includeEmails
                    ? dbMessage.MessageEmailRequests.Select(r => MCommunication_Email(r.EmailRequest)).ToList()
                    : null;

                var message = new MCommunication_Message()
                {
                    MessageId = dbMessage.MessageId,
                    UserId = userId,
                    ConversationId = dbMessage.ConversationId,
                    From = from,
                    To = to,
                    Subject = dbMessage.Subject,
                    Text = dbMessage.Text,
                    SendReceiveCode = dbMessage.SendReceiveCode,
                    CreateDateTimeUtc = dbMessage.CreateDateTimeUtc,
                    AcknowledgementDateTimeUtc = dbMessage.AcknowledgementDateTimeUtc,
                    TopicId = dbMessage.TopicId,
                    TopicReference = dbMessage.Topic?.TopicReference,

                    CanAcknowledge = canAcknowledge,

                    Fields = MCommunication_TopicFields(dbMessage.Topic),
                    Emails = emails,
                    Conversation = conversation
                };

                return message;
            }

            public static MCommunication_Notification MCommunication_Notification(Notification dbNotification, bool includeEmails)
            {
                IList<MCommunication_Email> emails = includeEmails
                    ? dbNotification.NotificationEmailRequests.Select(r => MCommunication_Email(r.EmailRequest)).ToList()
                    : null;

                return new MCommunication_Notification()
                {
                    NotificationId = dbNotification.NotificationId,
                    NotificationType = GetValue.MCommunication_NotificationType(dbNotification.NotificationTypeCode),
                    Text = dbNotification.NotificationTypeCodeNavigation.Name,
                    CreateDateTimeUtc = dbNotification.CreateDateTimeUtc,
                    AcknowledgementDateTimeUtc = dbNotification.AcknowledgementDateTimeUtc,
                    Fields = MCommunication_TopicFields(dbNotification.Topic),
                    ParticipantId = dbNotification.ParticipantId,
                    ParticipantReference = dbNotification.Participant.ParticipantReference,
                    TopicId = dbNotification.TopicId,
                    TopicReference = dbNotification.Topic.TopicReference,
                    Emails = emails
                };
            }

            public static MCommunication_Email MCommunication_Email(EmailRequest dbEmailRequest)
            {
                return new MCommunication_Email()
                {
                    EmailRequestId = dbEmailRequest.EmailRequestId,
                    CreateDateTimeUtc = dbEmailRequest.CreateDateTimeUtc,
                    EmailRequestStatusCode = dbEmailRequest.EmailRequestStatusCode,
                    StatusDateTimeUtc = dbEmailRequest.EmailRequestStatusDateTimeUtc,
                    SenderEmail = dbEmailRequest.SenderEmail,
                    SenderEmailName = dbEmailRequest.SenderEmailName,
                    RecipientEmail = dbEmailRequest.RecipientEmail,
                    RecipientEmailName = dbEmailRequest.RecipientEmailName,
                    RecipientParticipantId = dbEmailRequest.RecipientParticipantId,
                    Subject = dbEmailRequest.Subject,
                    BodyText = dbEmailRequest.BodyText,
                    BodyHtml = dbEmailRequest.BodyHtml,
                    BodyTypeCode = dbEmailRequest.BodyTypeCode
                };
            }

            public static MCommunication_Alert MCommunication_Alert(Alert dbAlert)
            {
                return new MCommunication_Alert()
                {
                    AlertId = dbAlert.AlertId,
                    AlertType = GetValue.MCommunication_AlertType(dbAlert.AlertTypeCode),
                    CreateDateTimeUtc = dbAlert.CreateDateTimeUtc,
                    AcknowledgementDateTimeUtc = dbAlert.AcknowledgementDateTimeUtc,
                    Description = dbAlert.Description,
                    Exception = dbAlert.Exception,
                    ParticipantId = dbAlert.ParticipantId,
                    ParticipantReference = dbAlert.Participant?.ParticipantReference,
                    TopicId = dbAlert.TopicId,
                    TopicReference = dbAlert.Topic?.TopicReference,
                    Email = dbAlert.EmailRequestId.HasValue ? MCommunication_Email(dbAlert.EmailRequest) : null
                };
            }

            public static string FormattedName(UserProfile dbUserProfile)
            {
                var sb = new StringBuilder();

                if (dbUserProfile != null)
                {
                    var prefix = "";

                    if (!string.IsNullOrEmpty(dbUserProfile.FirstName))
                    {
                        _ = sb.Append(prefix); prefix = " ";

                        _ = sb.Append(dbUserProfile.FirstName);
                    }

                    if (!string.IsNullOrEmpty(dbUserProfile.LastName))
                    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                        _ = sb.Append(prefix); prefix = " ";
#pragma warning restore IDE0059 // Unnecessary assignment of a value

                        _ = sb.Append(dbUserProfile.LastName);
                    }
                }

                var result = sb.ToString();

                return !string.IsNullOrEmpty(result) ? result : "Unknown";
            }

            private static IList<MCommunication_TopicField> MCommunication_TopicFields(Topic dbTopic)
            {
                if (dbTopic == null)
                {
                    return null;
                }

                var fields = new List<MCommunication_TopicField>();
                foreach (var dbTopicField in dbTopic.TopicFields)
                {
                    fields.Add(new MCommunication_TopicField()
                    {
                        FieldCode = dbTopicField.FieldCode,
                        FieldValue = dbTopicField.FieldValue
                    });
                }

                return fields;
            }
        }
    }
}
