//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Message
    {
        public Message()
        {
            MessageEmailRequests = new HashSet<MessageEmailRequest>();
        }

        public long MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public long ParticipantId { get; set; }
        public string Email { get; set; }
        public string SendReceiveCode { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public long? TopicId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<MessageEmailRequest> MessageEmailRequests { get; set; }
    }
}
