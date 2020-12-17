//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Message
{
    public class MessageList
    {
        public MessageListFilter Filter { get; set; }
        public IPagedList<Message> Messages { get; set; }
    }

    public class MessageListFilter
    {
        [Display(Name = "Mailbox")]
        public MCommunication_MessageMailbox Mailbox { get; set; }

        [Display(Name = "Status")]
        public MCommunication_MessageStatus Status { get; set; }

        [Display(Name = "Maximum Results")]
        public int RecordCount { get; set; }

        public IList<SelectListItem> MailboxList { get; set; }
        public IList<SelectListItem> StatusList { get; set; }
        public IList<SelectListItem> RecordCountList { get; set; }
    }
}