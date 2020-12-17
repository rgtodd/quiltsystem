//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

namespace RichTodd.QuiltSystem.Web.Models.Message
{
    public class NotificationDetailModel
    {
        public long NotificationId { get; set; }

        public string OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Notification")]
        public string Text { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime SentDateTime { get; set; }

        [Display(Name = "Acknowledged")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime? AcknowledgementDateTime { get; set; }

        [Display(Name = "New")]
        public bool New { get; set; }
    }
}