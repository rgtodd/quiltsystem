//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MCommunication_Message
    {
        public long MessageId { get; set; }
        public string UserId { get; set; }
        public Guid ConversationId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string SendReceiveCode { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public long? TopicId { get; set; }
        public string TopicReference { get; set; }

        public bool CanAcknowledge { get; set; }

        public IList<MCommunication_TopicField> Fields { get; set; }
        public IList<MCommunication_Email> Emails { get; set; }
        public IList<MCommunication_Message> Conversation { get; set; }
    }
}
