//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Notification
    {
        public Notification()
        {
            NotificationEmailRequests = new HashSet<NotificationEmailRequest>();
        }

        public long NotificationId { get; set; }
        public string NotificationTypeCode { get; set; }
        public long ParticipantId { get; set; }
        public long? TopicId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual NotificationType NotificationTypeCodeNavigation { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<NotificationEmailRequest> NotificationEmailRequests { get; set; }
    }
}
