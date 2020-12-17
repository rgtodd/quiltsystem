//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web.Bootstrap;

namespace RichTodd.QuiltSystem.Web.Models.Message
{
    public class MessageReplyModel
    {
        public long ReplyToMessageId { get; set; }

        public long? OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        [Required]
        [StringLength(5000)]
        [Multiline(10)]
        public string Text { get; set; }

        public IList<MessageDetailModel> Conversation { get; set; }
    }
}