//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Message
{
    public class MessageModelFactory : ApplicationModelFactory
    {
        public CreateMessage CreateCreateMessage(string userId, long? orderId)
        {
            var model = new CreateMessage()
            {
                UserId = userId,
                Subject = null,
                Subjects = CreateSubjects(),
                Text = null,
                OrderId = orderId
            };

            return model;
        }

        public Message CreateMessage(AMessage_Message aMessage, MCommunication_Message mMessage, bool highlight)
        {
            var model = new Message(aMessage, mMessage, highlight, Locale);

            return model;
        }

        public MessageList CreateMessageList(IList<AMessage_Message> aMessages, PagingState pagingState)
        {
            var messages = aMessages.Select(r => CreateMessage(null, r.MMessage, false)).ToList();

            IReadOnlyList<Message> sortedMessages;
            var sortFunction = GetSortFunction(pagingState.Sort);
            sortedMessages = sortFunction != null
                ? pagingState.Descending
                    ? messages.OrderByDescending(sortFunction).ToList()
                    : messages.OrderBy(sortFunction).ToList()
                : messages;

            int pageSize = PagingState.PageSize;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedMessages.Count, pageSize);
            var pagedMessages = sortedMessages.ToPagedList(pageNumber, pageSize);

            var (mailbox, status, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new MessageList()
            {
                Messages = pagedMessages,
                Filter = new MessageListFilter()
                {
                    Mailbox = mailbox,
                    Status = status,
                    RecordCount = recordCount,

                    MailboxList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "Inbox", Value = MCommunication_MessageMailbox.FromUser.ToString() },
                        new SelectListItem() { Text = "Outbox", Value = MCommunication_MessageMailbox.ToUser.ToString() },
                        new SelectListItem() { Text = "All", Value = MCommunication_MessageMailbox.MetaAll.ToString() },
                    },
                    StatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "Acknowledged", Value = MCommunication_MessageStatus.Acknowledged.ToString() },
                        new SelectListItem() { Text = "Unacknowledged", Value = MCommunication_MessageStatus.Unacknowledged.ToString() },
                        new SelectListItem() { Text = "All", Value = MCommunication_MessageStatus.MetaAll.ToString() },
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public ReplyMessage CreateReplyMessage(MCommunication_Message mMessage)
        {
            var model = new ReplyMessage()
            {
                UserId = mMessage.UserId,
                UserName = mMessage.From,
                ReplyToMessageId = mMessage.MessageId,
                Subject = mMessage.Subject,
                //OrderId = svcMessage.OrderId,
            };

            model.Conversation = new List<Message>();
            foreach (var mRelatedMessage in mMessage.Conversation.Where(r => r.CreateDateTimeUtc <= mMessage.CreateDateTimeUtc).OrderByDescending(r => r.CreateDateTimeUtc))
            {
                model.Conversation.Add(CreateMessage(null, mRelatedMessage, false));
            }

            return model;
        }

        public void RefreshMessageCreateModel(CreateMessage model)
        {
            model.Subjects = CreateSubjects();
        }

        public string GetDefaultSort()
        {
            return MessageMetadata.GetDisplayName(m => m.SentDateTime);
        }

        public string CreatePagingStateFilter(MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount)
        {
            return $"{mailbox}|{status}|{recordCount}";
        }

        public string CreatePagingStateFilter(MessageListFilter messageListFilter)
        {
            return CreatePagingStateFilter(messageListFilter.Mailbox, messageListFilter.Status, messageListFilter.RecordCount);
        }

        public (MCommunication_MessageMailbox mailbox, MCommunication_MessageStatus status, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MCommunication_MessageMailbox.FromUser, MCommunication_MessageStatus.Unacknowledged, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var mailbox = fields.Length >= 1 && Enum.TryParse(fields[0], out MCommunication_MessageMailbox mailboxField)
                ? mailboxField
                : MCommunication_MessageMailbox.FromUser;

            var status = fields.Length >= 2 && Enum.TryParse(fields[1], out MCommunication_MessageStatus statusField)
                ? statusField
                : MCommunication_MessageStatus.Unacknowledged;

            var recordCount = fields.Length >= 3 && int.TryParse(fields[2], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (mailbox, status, recordCount);
        }

        private ModelMetadata<Message> m_messageMetadata;
        private ModelMetadata<Message> MessageMetadata
        {
            get
            {
                if (m_messageMetadata == null)
                {
                    m_messageMetadata = ModelMetadata<Message>.Create(HttpContext);
                }

                return m_messageMetadata;
            }
        }

        private static IDictionary<string, Func<Message, object>> s_sortFunctions;
        private IDictionary<string, Func<Message, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new Message(null, null, false, null);

                    var sortFunctions = new Dictionary<string, Func<Message, object>>
                    {
                        { MessageMetadata.GetDisplayName(m => m.From), r => r.From },
                        { MessageMetadata.GetDisplayName(m => m.SentDateTime), r => r.SentDateTime },
                        { MessageMetadata.GetDisplayName(m => m.Subject), r => r.Subject },
                        { MessageMetadata.GetDisplayName(m => m.Text), r => r.Text },
                        { MessageMetadata.GetDisplayName(m => m.To), r => r.To }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private List<SelectListItem> CreateSubjects()
        {
            var subjects = new List<SelectListItem>
            {
                new SelectListItem() { Value = "", Text = "(Select)" },
                new SelectListItem() { Value = "Question", Text = "Question" },
                new SelectListItem() { Value = "Problem", Text = "Problem" }
            };

            return subjects;
        }

        private Func<Message, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }

    }
}