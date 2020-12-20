//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Log
{
    public class LogEntryModel
    {
        [Display(Name = "Log Entry ID")]
        public long LogEntryId { get; set; }

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        [Display(Name = "Log Entry Date/Time")]
        public DateTime LogEntryDateTime { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Duration (ms)")]
        public int DurationMilliseconds { get; set; }

        [Display(Name = "Log Name")]
        public string LogName { get; set; }

        [Display(Name = "Log Type")]
        public string LogEntryTypeName { get; set; }

        [Display(Name = "Severity")]
        public string SeverityCode { get; set; }
    }
}