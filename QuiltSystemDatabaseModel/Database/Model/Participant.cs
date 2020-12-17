//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Participant
    {
        public Participant()
        {
            Alerts = new HashSet<Alert>();
            EmailRequests = new HashSet<EmailRequest>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
        }

        public long ParticipantId { get; set; }
        public string ParticipantReference { get; set; }

        public virtual ICollection<Alert> Alerts { get; set; }
        public virtual ICollection<EmailRequest> EmailRequests { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
