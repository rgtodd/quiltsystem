//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MCommunication_Notification
    {
        public long NotificationId { get; set; }
        public MCommunication_NotificationTypes NotificationType { get; set; }
        public string Text { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public long ParticipantId { get; set; }
        public string ParticipantReference { get; set; }
        public long? TopicId { get; set; }
        public string TopicReference { get; set; }

        public IList<MCommunication_TopicField> Fields { get; set; }
        public IList<MCommunication_Email> Emails { get; set; }
    }
}
