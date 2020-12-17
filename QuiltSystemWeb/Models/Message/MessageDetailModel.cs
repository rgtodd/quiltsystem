//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Message
{
    public class MessageDetailModel
    {
        public long MessageId { get; set; }

        public string UserId { get; set; }

        public long? OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "To")]
        public string To { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        public string Text { get; set; }

        [Display(Name = "Sent")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime SentDateTime { get; set; }

        [Display(Name = "Received")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime ReceivedDateTime { get; set; }

        [Display(Name = "Acknowledged")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? AcknowledgementDateTime { get; set; }

        [Display(Name = "New")]
        public bool New { get; set; }

        public bool Incoming { get; set; }

        public IList<MessageDetailModel> Conversation { get; set; }
    }
}