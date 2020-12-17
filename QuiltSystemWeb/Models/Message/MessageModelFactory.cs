//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using PagedList;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Models.Message
{
    public class MessageModelFactory : ApplicationModelFactory
    {
        private static IDictionary<string, Func<MessageDetailModel, object>> s_messageSortFunctions;
        private static IDictionary<string, Func<NotificationDetailModel, object>> s_notificationSortFunctions;

        private ModelMetadata<MessageDetailModel> m_messageDetailModelMetadata;
        private ModelMetadata<MessageDetailModel> MessageDetailModelMetadata
        {
            get
            {
                if (m_messageDetailModelMetadata == null)
                {
                    m_messageDetailModelMetadata = ModelMetadata<MessageDetailModel>.Create(HttpContext);
                }

                return m_messageDetailModelMetadata;
            }
        }

        private ModelMetadata<NotificationDetailModel> m_notificationDetailModelMetadata;
        private ModelMetadata<NotificationDetailModel> NotificationDetailModelMetadata
        {
            get
            {
                if (m_notificationDetailModelMetadata == null)
                {
                    m_notificationDetailModelMetadata = ModelMetadata<NotificationDetailModel>.Create(HttpContext);
                }

                return m_notificationDetailModelMetadata;
            }
        }

        private IDictionary<string, Func<MessageDetailModel, object>> MessageSortFunctions
        {
            get
            {
                if (s_messageSortFunctions == null)
                {
                    var sortFunctions = new Dictionary<string, Func<MessageDetailModel, object>>
                    {
                        { MessageDetailModelMetadata.GetDisplayName(m => m.AcknowledgementDateTime), r => r.AcknowledgementDateTime },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.From), r => r.From },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.New), r => r.New },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.OrderNumber), r => r.OrderNumber },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.ReceivedDateTime), r => r.ReceivedDateTime },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.SentDateTime), r => r.SentDateTime},
                        { MessageDetailModelMetadata.GetDisplayName(m => m.Subject), r => r.Subject },
                        { MessageDetailModelMetadata.GetDisplayName(m => m.To), r => r.To }
                    };

                    s_messageSortFunctions = sortFunctions;
                }

                return s_messageSortFunctions;
            }
        }

        private IDictionary<string, Func<NotificationDetailModel, object>> NotificationSortFunctions
        {
            get
            {
                if (s_notificationSortFunctions == null)
                {
                    var heading = new NotificationDetailModel();

                    var sortFunctions = new Dictionary<string, Func<NotificationDetailModel, object>>
                    {
                        { NotificationDetailModelMetadata.GetDisplayName(m => m.New), r => r.New},
                        { NotificationDetailModelMetadata.GetDisplayName(m => m.Text), r => r.Text },
                        { NotificationDetailModelMetadata.GetDisplayName(m => m.OrderNumber), r => r.OrderNumber },
                        { NotificationDetailModelMetadata.GetDisplayName(m => m.SentDateTime), r => r.SentDateTime }
                    };

                    s_notificationSortFunctions = sortFunctions;
                }

                return s_notificationSortFunctions;
            }
        }

        public MessageDetailModel CreateMessageDetailModel(MCommunication_Message from)
        {
            var to = new MessageDetailModel();
            CopyMessageDetailModel(to, from);
            return to;
        }

        public MessageReplyModel CreateMessageReplyModel(MCommunication_Message from)
        {
            var to = new MessageReplyModel();
            CopyMessageReplyModel(to, from);
            return to;
        }

        public MessageSummaryModel CreateMessageSummaryModel(IEnumerable<MCommunication_Message> fromIncomingMessages, PagingState fromIncomingMessagesPagingState, IEnumerable<MCommunication_Message> fromOutgoingMessages, PagingState fromOutgoingMessagesPagingState, IEnumerable<MCommunication_Notification> fromNotifications, PagingState fromNotificationsPagingState)
        {
            var to = new MessageSummaryModel();
            CopyCreateMessageSummaryModel(
            fromIncomingMessages,
            fromIncomingMessagesPagingState,
            fromOutgoingMessages,
            fromOutgoingMessagesPagingState,
            fromNotifications,
            fromNotificationsPagingState,
            to);
            return to;
        }

        private void CopyCreateMessageSummaryModel(IEnumerable<MCommunication_Message> fromIncomingMessages, PagingState fromIncomingMessagesPagingState, IEnumerable<MCommunication_Message> fromOutgoingMessages, PagingState fromOutgoingMessagesPagingState, IEnumerable<MCommunication_Notification> fromNotifications, PagingState fromNotificationsPagingState, MessageSummaryModel to)
        {
            to.IncomingMessages = CreateMessageDetailModels(fromIncomingMessages, fromIncomingMessagesPagingState);
            to.OutgoingMessages = CreateMessageDetailModels(fromOutgoingMessages, fromOutgoingMessagesPagingState);
            to.Notifications = CreateNotificationDetailModels(fromNotifications, fromNotificationsPagingState);
        }

        private void CopyMessageDetailModel(MessageDetailModel to, MCommunication_Message from)
        {
            to.MessageId = from.MessageId;
            to.UserId = from.UserId;
            //to.OrderId = from.OrderId;
            //to.OrderNumber = from.OrderNumber;
            to.From = from.From;
            to.To = from.To;
            to.Subject = from.Subject;
            to.Text = from.Text;
            to.SentDateTime = Locale.GetLocalTimeFromUtc(from.CreateDateTimeUtc);
            to.ReceivedDateTime = Locale.GetLocalTimeFromUtc(from.CreateDateTimeUtc);
            to.AcknowledgementDateTime = Locale.GetLocalTimeFromUtc(from.AcknowledgementDateTimeUtc);
            to.New = from.AcknowledgementDateTimeUtc == null;
            to.Incoming = from.SendReceiveCode == SendReceiveCodes.ToUser;

            if (from.Conversation != null)
            {
                to.Conversation = new List<MessageDetailModel>();
                foreach (var svcRelatedMessage in from.Conversation.Where(r => r.CreateDateTimeUtc < from.CreateDateTimeUtc).OrderByDescending(r => r.CreateDateTimeUtc))
                {
                    to.Conversation.Add(CreateMessageDetailModel(svcRelatedMessage));
                }
            }
        }

        private void CopyMessageReplyModel(MessageReplyModel to, MCommunication_Message from)
        {
            to.ReplyToMessageId = from.MessageId;
            to.Subject = from.Subject;
            //to.OrderId = from.OrderId;
            //to.OrderNumber = from.OrderNumber;
            to.Conversation = new List<MessageDetailModel>();
            foreach (var svcRelatedMessage in from.Conversation.Where(r => r.CreateDateTimeUtc <= from.CreateDateTimeUtc).OrderByDescending(r => r.CreateDateTimeUtc))
            {
                to.Conversation.Add(CreateMessageDetailModel(svcRelatedMessage));
            }
        }

        private void CopyNotificationDetailModel(NotificationDetailModel to, MCommunication_Notification from)
        {
            to.NotificationId = from.NotificationId;
            to.Text = from.Text;
            to.SentDateTime = Locale.GetLocalTimeFromUtc(from.CreateDateTimeUtc);
            to.OrderId = from.Fields.Where(r => r.FieldCode == "OrderId").SingleOrDefault()?.FieldValue;
            to.OrderNumber = from.Fields.Where(r => r.FieldCode == "OrderNumber").SingleOrDefault()?.FieldValue;
            to.AcknowledgementDateTime = Locale.GetLocalTimeFromUtc(from.AcknowledgementDateTimeUtc);
            to.New = from.AcknowledgementDateTimeUtc == null;
        }

        private IPagedList<MessageDetailModel> CreateMessageDetailModels(IEnumerable<MCommunication_Message> from, PagingState pagingState)
        {
            var messages = new List<MessageDetailModel>();
            foreach (var fromItem in from)
            {
                messages.Add(CreateMessageDetailModel(fromItem));
            }

            IReadOnlyList<MessageDetailModel> sortedMessages;
            var sortFunction = GetMessageSortFunction(pagingState.Sort);
            sortedMessages = sortFunction != null
                ? pagingState.Descending
                    ? messages.OrderByDescending(sortFunction).ToList()
                    : messages.OrderBy(sortFunction).ToList()
                : messages.OrderByDescending(r => r.SentDateTime).ToList();

            var pageSize = 3;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedMessages.Count, pageSize);
            var pagedMessages = sortedMessages.ToPagedList(pageNumber, pageSize);

            return pagedMessages;
        }

        private NotificationDetailModel CreateNotificationDetailModel(MCommunication_Notification from)
        {
            var to = new NotificationDetailModel();
            CopyNotificationDetailModel(to, from);
            return to;
        }

        private IPagedList<NotificationDetailModel> CreateNotificationDetailModels(IEnumerable<MCommunication_Notification> from, PagingState pagingState)
        {
            var notifications = new List<NotificationDetailModel>();
            foreach (var fromItem in from)
            {
                notifications.Add(CreateNotificationDetailModel(fromItem));
            }

            IReadOnlyList<NotificationDetailModel> sortedNotifications;
            var sortFunction = GetNotificationSortFunction(pagingState.Sort);
            sortedNotifications = sortFunction != null
                ? pagingState.Descending
                    ? notifications.OrderByDescending(sortFunction).ToList()
                    : notifications.OrderBy(sortFunction).ToList()
                : notifications.OrderByDescending(r => r.SentDateTime).ToList();

            var pageSize = 3;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedNotifications.Count, pageSize);
            var pagedNotifications = sortedNotifications.ToPagedList(pageNumber, pageSize);

            return pagedNotifications;
        }

        private Func<MessageDetailModel, object> GetMessageSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? MessageSortFunctions[sort] : null;
        }

        private Func<NotificationDetailModel, object> GetNotificationSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? NotificationSortFunctions[sort] : null;
        }

    }
}