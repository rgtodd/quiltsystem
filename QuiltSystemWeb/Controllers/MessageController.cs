//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Models.Message;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class MessageController : ApplicationController<MessageModelFactory>
    {
        private ICommunicationMicroService CommunicationMicroService { get; }

        public MessageController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ICommunicationMicroService communicationMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
        }

        public async Task<ActionResult> Create(int? replyToMessageId, long? orderId)
        {
            if (replyToMessageId.HasValue)
            {
                var participantId = await GetParticipantIdAsync();
                var svcMessage = await CommunicationMicroService.GetMessageAsync(replyToMessageId.Value, false, participantId);

                var model = ModelFactory.CreateMessageReplyModel(svcMessage);

                return View("Reply", model);
            }
            else
            {
                var model = new MessageCreateModel()
                {
                    Subject = null,
                    Subjects = CreateSubjects(),
                    Text = null,
                    OrderId = orderId
                };

                return View("Create", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(MessageCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Subjects = CreateSubjects();

                return View("Create", model);
            }

            try
            {
                var participantId = await GetParticipantIdAsync();
                long? topicId = await GetTopicIdAsync(model.OrderId);

                _ = await CommunicationMicroService.SendMessageFromParticipantAsync(participantId, model.Subject, model.Text, null, topicId);

                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Message sent.");

                return RedirectToAction("Index", new { id = "" });
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);

                model.Subjects = CreateSubjects();

                return View("Create", model);
            }
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id.HasValue)
            {
                var participantId = await GetParticipantIdAsync();
                var mMessage = await CommunicationMicroService.GetMessageAsync(id.Value, true, participantId);

                var model = ModelFactory.CreateMessageDetailModel(mMessage);

                return View("Message", model);
            }
            else
            {
                var notificationsPagingState = this.GetPagingState(MessageSummaryModel.NotificationSection);
                var notificationIncludeOld = notificationsPagingState.Filter == "all";

                var incomingMessagesPagingState = this.GetPagingState(MessageSummaryModel.IncomingSection);
                var incomingIncludeOld = incomingMessagesPagingState.Filter == "all";

                var outgoingMessagesPagingState = this.GetPagingState(MessageSummaryModel.OutgoingSection);
                var outgoingIncludeOld = outgoingMessagesPagingState.Filter == "all";

                var participantId = await GetParticipantIdAsync();
                var mNotifications = await CommunicationMicroService.GetNotificationsAsync(int.MaxValue, notificationIncludeOld, participantId);
                var mIncomingMessages = await CommunicationMicroService.GetMessagesAsync(MCommunication_MessageMailbox.ToUser, incomingIncludeOld ? MCommunication_MessageStatus.MetaAll : MCommunication_MessageStatus.Unacknowledged, int.MaxValue, participantId);
                var mOutgoingMessages = await CommunicationMicroService.GetMessagesAsync(MCommunication_MessageMailbox.FromUser, outgoingIncludeOld ? MCommunication_MessageStatus.MetaAll : MCommunication_MessageStatus.Unacknowledged, int.MaxValue, participantId);

                var model = ModelFactory.CreateMessageSummaryModel(
                    mIncomingMessages.Messages,
                    incomingMessagesPagingState,
                    mOutgoingMessages.Messages,
                    outgoingMessagesPagingState,
                    mNotifications.Notifications,
                    notificationsPagingState);

                model.HasNewNotifications = mNotifications.Notifications.Any(r => r.AcknowledgementDateTimeUtc == null);
                model.HasNewIncomingMessages = mIncomingMessages.Messages.Any(r => r.AcknowledgementDateTimeUtc == null);
                model.HasNewOutgoingMessages = mOutgoingMessages.Messages.Any(r => r.AcknowledgementDateTimeUtc == null);

                model.IncludeOldNotifications = notificationIncludeOld;
                model.IncludeOldIncomingMessages = incomingIncludeOld;
                model.IncludeOldOutgoingMessages = outgoingIncludeOld;

                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit()
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Acknowledge:
                    var participantId = await GetParticipantIdAsync();
                    await CommunicationMicroService.AcknowledgeNotificationsAsync(participantId);
                    break;
            }

            return await Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> Message(MessageDetailModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Reply:
                    return RedirectToAction("Create", new { replyToMessageId = model.MessageId });

                case Actions.Acknowledge:
                    {
                        var participantId = await GetParticipantIdAsync();
                        await CommunicationMicroService.AcknowledgeMessageAsync(model.MessageId, participantId);
                    }
                    break;
            }

            return await Index(model.MessageId);
        }

        [HttpPost]
        public async Task<ActionResult> Reply(MessageReplyModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Reply", model);
            }

            try
            {
                var participantId = await GetParticipantIdAsync();
                long? topicId = await GetTopicIdAsync(model.OrderId);

                _ = await CommunicationMicroService.SendMessageFromParticipantAsync(participantId, model.Subject, model.Text, model.ReplyToMessageId, topicId);

                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Message sent.");

                return RedirectToAction("Index", new { id = "" });
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);

                return View("Reply", model);
            }
        }

        private static List<SelectListItem> CreateSubjects()
        {
            var subjects = new List<SelectListItem>
            {
                new SelectListItem() { Value = "", Text = "(Select)" },
                new SelectListItem() { Value = "Question", Text = "Question" },
                new SelectListItem() { Value = "Problem", Text = "Problem" }
            };

            return subjects;
        }

        private async Task<long> GetParticipantIdAsync()
        {
            var participantReference = CreateParticipantReference.FromUserId(GetUserId());
            var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference);
            return participantId;
        }

        private async Task<long?> GetTopicIdAsync(long? orderId)
        {
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

            return topicId;
        }
    }
}