//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Alert
    {
        public Alert()
        {
            AlertEmailRequests = new HashSet<AlertEmailRequest>();
        }

        public long AlertId { get; set; }
        public string AlertTypeCode { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public DateTime? AcknowledgementDateTimeUtc { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public long? ParticipantId { get; set; }
        public long? EmailRequestId { get; set; }
        public long? TopicId { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AlertType AlertTypeCodeNavigation { get; set; }
        public virtual EmailRequest EmailRequest { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<AlertEmailRequest> AlertEmailRequests { get; set; }
    }
}
