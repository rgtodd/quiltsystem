//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Message
{
    public class Message
    {
        public AMessage_Message AMessage { get; }
        public MCommunication_Message MMessage { get; }
        public bool Highlight { get; }
        public IApplicationLocale Locale { get; }
        public MCommunication_Email LastEmail { get; }

        public Message(
            AMessage_Message aMessage,
            MCommunication_Message mMessage,
            bool highlight,
            IApplicationLocale locale)
        {
            AMessage = aMessage;
            MMessage = aMessage?.MMessage ?? mMessage;
            Highlight = highlight;
            Locale = locale;

            LastEmail = aMessage.MMessage.Emails?.Last();
        }

        public long MessageId => MMessage.MessageId;

        public string UserId => MMessage.UserId;

        public long? OrderId => null; //HACK

        [Display(Name = "From")]
        public string From => MMessage.From;

        [Display(Name = "To")]
        public string To => MMessage.To;

        [Display(Name = "Subject")]
        public string Subject => MMessage.Subject;

        [Display(Name = "Message")]
        public string Text => MMessage.Text;

        [Display(Name = "Sent")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime SentDateTime => Locale.GetLocalTimeFromUtc(MMessage.CreateDateTimeUtc);

        [Display(Name = "Acknowledged")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime? AcknowledgementDateTime => Locale.GetLocalTimeFromUtc(MMessage.AcknowledgementDateTimeUtc);

        [Display(Name = "New")]
        public bool New => MMessage.AcknowledgementDateTimeUtc == null;

        [Display(Name = "Email Request ID")]
        public long? EmailRequestId => LastEmail?.EmailRequestId;

        [Display(Name = "Status")]
        public string EmailRequestStatusCode => LastEmail?.EmailRequestStatusCode;

        [Display(Name = "Date/Time")]
        public DateTime? StatusDateTime => Locale.GetLocalTimeFromUtc(LastEmail?.StatusDateTimeUtc);

        public bool EnableAcknowledge => (AMessage?.AllowAcknowledge ?? false) && MMessage.CanAcknowledge;

        private IList<Message> m_conversation;
        public IList<Message> Conversation
        {
            get
            {
                if (m_conversation == null)
                {
                    m_conversation = new List<Message>();
                    var idx = 0;
                    foreach (var mRelatedMessage in MMessage.Conversation.OrderByDescending(r => r.CreateDateTimeUtc))
                    {
                        if (idx == 0 && mRelatedMessage.MessageId == MMessage.MessageId)
                        {
                            // Skip message if it's the latest one in the conversation.
                        }
                        else
                        {
                            bool highlight = mRelatedMessage.MessageId == MMessage.MessageId;
                            m_conversation.Add(new Message(null, mRelatedMessage, highlight, Locale));
                        }

                        idx += 1;
                    }
                }
                return m_conversation;
            }
        }
    }
}