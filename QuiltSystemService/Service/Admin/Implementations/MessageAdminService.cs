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
    internal class MessageAdminService : BaseService, IMessageAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }

        public MessageAdminService(
            IApplicationRequestServices requestServices,
            ILogger<MessageAdminService> logger,
            ICommunicationMicroService communicationMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
        }

        public async Task AcknowledgeMessageAsync(long messageId)
        {
            using var log = BeginFunction(nameof(MessageAdminService), nameof(AcknowledgeMessageAsync), messageId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await CommunicationMicroService.AcknowledgeMessageAsync(messageId);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AMessage_Message> GetMessageAsync(long messageId)
        {
            using var log = BeginFunction(nameof(MessageAdminService), nameof(GetMessageAsync), messageId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mMessage = await CommunicationMicroService.GetMessageAsync(messageId, false);

                var allowAcknowledge = AllowAcknowledge(mMessage);

                var result = Create.AMessage_Message(mMessage, allowAcknowledge);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private bool AllowAcknowledge(MCommunication_Message mMessage)
        {
            return mMessage.SendReceiveCode == "I"; // SendReceiveCodes.FromUser;
        }

        public async Task<AMessage_MessageList> GetMessagesAsync(MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount)
        {
            using var log = BeginFunction(nameof(MessageAdminService), nameof(GetMessagesAsync), mailbox, status, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var messages = new List<AMessage_Message>();
                var mMessages = await CommunicationMicroService.GetMessagesAsync(mailbox, status, recordCount);
                foreach (var mMessage in mMessages.Messages)
                {
                    var allowAcknowledge = AllowAcknowledge(mMessage);
                    var message = Create.AMessage_Message(mMessage, allowAcknowledge);
                    messages.Add(message);
                }

                var result = new AMessage_MessageList()
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

        public async Task<long> SendOutboundMessageAsync(string userId, string subject, string text, long? replyToMessageId, long? orderId)
        {
            using var log = BeginFunction(nameof(MessageAdminService), nameof(SendOutboundMessageAsync), userId, subject, text, replyToMessageId, orderId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var participantReference = CreateParticipantReference.FromUserId(userId);
                var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference);

                long? topicId;
                if (orderId.HasValue)
                {
                    var topicReference = CreateTopicReference.FromOrderId(orderId.Value);
                    topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, null);
                }
                else
                {
                    topicId = null;
                }

                var messageId = await CommunicationMicroService.SendMessageToParticipantAsync(participantId, subject, text, replyToMessageId, topicId);

                //using var ctx = QuiltContextFactory.Create();

                //var dbAspNetUser = await ctx.AspNetUsers.Where(r => r.Id == userId).SingleAsync().ConfigureAwait(false);


                //Message dbReplyToMessage;
                //if (replyToMessageId != null)
                //{
                //    dbReplyToMessage = await ctx.Messages.Where(r => r.MessageId == replyToMessageId.Value).SingleAsync().ConfigureAwait(false);
                //    var participantUserId = ParseUserId.FromParticipantReference(dbReplyToMessage.Participant.ParticipantReference);
                //    if (participantUserId != userId)
                //    {
                //        throw new InvalidOperationException(string.Format("Reply to message user ID mismatch (user ID = {0}, message user ID = {1}). ", userId, participantUserId));
                //    }

                //    if (dbReplyToMessage.AcknowledgementDateTimeUtc == null)
                //    {
                //        dbReplyToMessage.AcknowledgementDateTimeUtc = GetUtcNow();
                //    }

                //    if (topicId == null)
                //    {
                //        // Propagate order ID from original message.
                //        //
                //        topicId = dbReplyToMessage.TopicId;
                //    }
                //    else
                //    {
                //        if (dbReplyToMessage.TopicId != null)
                //        {
                //            if (dbReplyToMessage.TopicId != orderId)
                //            {
                //                throw new InvalidOperationException(string.Format("Reply to message topic ID mismatch (topic ID = {0}, message topic ID = {1}). ", topicId, dbReplyToMessage.TopicId));
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    dbReplyToMessage = null;
                //}

                //var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference).ConfigureAwait(false);

                //var dbMessage = new Message()
                //{
                //    ParticipantId = participantId,
                //    ConversationId = dbReplyToMessage != null ? dbReplyToMessage.ConversationId : Guid.NewGuid(),
                //    SendReceiveCode = SendReceiveCodes.ToUser,
                //    Subject = subject,
                //    Text = text,
                //    Email = dbAspNetUser.Email,
                //    TopicId = topicId,
                //    CreateDateTimeUtc = GetUtcNow()
                //};
                //_ = ctx.Messages.Add(dbMessage);

                //// Prepend order information.
                ////
                //string orderNumber;
                //if (orderId != null)
                //{
                //    var dbOrder = await ctx.Orders.Where(r => r.OrderId == orderId.Value).SingleAsync().ConfigureAwait(false);
                //    orderNumber = dbOrder.OrderNumber;

                //    var url = Constants.WebsiteFullUrl + "/Order/Index/" + orderId.Value.ToString();

                //    text = "Order " + dbOrder.OrderNumber + ": " + url + System.Environment.NewLine + System.Environment.NewLine + text;
                //}
                //else
                //{
                //    orderNumber = null;
                //}

                //var formatter = new OutboundMessageEmailFormatter(subject, text, orderId, orderNumber);

                //var dbEmailRequest = new EmailRequest()
                //{
                //    EmailRequestStatusTypeCodeNavigation = ctx.EmailRequestStatusType(EmailRequestStatusTypes.Posted),
                //    SenderEmail = Constants.DoNotReplyEmail,
                //    SenderEmailName = Constants.DoNotReplyEmailName,
                //    RecipientEmail = dbAspNetUser.Email,
                //    RecipientEmailName = dbAspNetUser.EmailName(),
                //    RecipientParticipantId = participantId,
                //    Subject = formatter.GetSubject(),
                //    BodyText = formatter.GetText(),
                //    BodyHtml = formatter.GetHtml(),
                //    BodyType = EmailBodyTypes.Message,
                //    CreateDateTimeUtc = GetUtcNow(),
                //    StatusDateTimeUtc = GetUtcNow()
                //};
                //_ = ctx.EmailRequests.Add(dbEmailRequest);

                //var dbMessageEmailRequest = new MessageEmailRequest()
                //{
                //    Message = dbMessage,
                //    EmailRequest = dbEmailRequest
                //};
                //_ = ctx.MessageEmailRequests.Add(dbMessageEmailRequest);

                //_ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                //var result = dbMessage.MessageId;

                var result = messageId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static AMessage_Message AMessage_Message(MCommunication_Message mMessage, bool allowAcknowledge)
            {
                var message = new AMessage_Message()
                {
                    MMessage = mMessage,
                    AllowAcknowledge = allowAcknowledge
                };

                return message;
            }
        }
    }
}