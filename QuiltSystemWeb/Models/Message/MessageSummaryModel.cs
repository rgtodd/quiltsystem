//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using PagedList;

namespace RichTodd.QuiltSystem.Web.Models.Message
{
    public class MessageSummaryModel
    {
        public static int NotificationSection = 0;
        public static int OutgoingSection = 1;
        public static int IncomingSection = 2;

        public bool HasNewNotifications { get; set; }

        public bool HasNewIncomingMessages { get; set; }

        public bool HasNewOutgoingMessages { get; set; }

        public bool IncludeOldNotifications { get; set; }

        public bool IncludeOldIncomingMessages { get; set; }

        public bool IncludeOldOutgoingMessages { get; set; }

        public IPagedList<NotificationDetailModel> Notifications { get; set; }

        public IPagedList<MessageDetailModel> IncomingMessages { get; set; }

        public IPagedList<MessageDetailModel> OutgoingMessages { get; set; }
    }
}