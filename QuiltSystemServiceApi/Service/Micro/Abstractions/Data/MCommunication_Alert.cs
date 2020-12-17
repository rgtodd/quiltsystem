//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MCommunication_Alert
    {
        public long AlertId { get; set; }
        public MCommunication_AlertTypes AlertType { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public long? ParticipantId { get; set; }
        public string ParticipantReference { get; set; }
        public long? TopicId { get; set; }
        public string TopicReference { get; set; }
        public MCommunication_Email Email { get; set; }
    }
}
