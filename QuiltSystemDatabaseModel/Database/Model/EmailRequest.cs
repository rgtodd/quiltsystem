//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class EmailRequest
    {
        public EmailRequest()
        {
            AlertEmailRequests = new HashSet<AlertEmailRequest>();
            Alerts = new HashSet<Alert>();
            MessageEmailRequests = new HashSet<MessageEmailRequest>();
            NotificationEmailRequests = new HashSet<NotificationEmailRequest>();
        }

        public long EmailRequestId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }
        public string EmailRequestStatusCode { get; set; }
        public DateTime EmailRequestStatusDateTimeUtc { get; set; }
        public string SenderEmail { get; set; }
        public string SenderEmailName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientEmailName { get; set; }
        public long? RecipientParticipantId { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public string BodyTypeCode { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Participant RecipientParticipant { get; set; }
        public virtual ICollection<AlertEmailRequest> AlertEmailRequests { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
        public virtual ICollection<MessageEmailRequest> MessageEmailRequests { get; set; }
        public virtual ICollection<NotificationEmailRequest> NotificationEmailRequests { get; set; }
    }
}
