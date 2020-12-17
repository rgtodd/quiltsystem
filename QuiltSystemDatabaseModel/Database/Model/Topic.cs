//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Topic
    {
        public Topic()
        {
            Alerts = new HashSet<Alert>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            TopicFields = new HashSet<TopicField>();
        }

        public long TopicId { get; set; }
        public string TopicReference { get; set; }

        public virtual ICollection<Alert> Alerts { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<TopicField> TopicFields { get; set; }
    }
}
