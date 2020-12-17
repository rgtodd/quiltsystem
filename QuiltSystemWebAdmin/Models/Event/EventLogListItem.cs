//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.ComponentModel.DataAnnotations;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Event
{
    public class EventLogListItem
    {
        public MCommon_EventLogSummary MSummary { get; }
        public IApplicationLocale Locale { get; }

        public EventLogListItem(
            MCommon_EventLogSummary mSummary,
            IApplicationLocale locale)
        {
            MSummary = mSummary;
            Locale = locale;
        }

        [Display(Name = "Source")]
        public string Source => MSummary.Source;

        [Display(Name = "Event ID")]
        public long EventId => MSummary.EventId;

        [Display(Name = "Entity ID")]
        public long EntityId => MSummary.EntityId;

        [Display(Name = "Transaction ID")]
        public long TransactionId => MSummary.TransactionId;

        [Display(Name = "Entity Type")]
        public string EventType => MSummary.EventTypeCode;

        [Display(Name = "Event Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MSummary.EventDateTimeUtc);

        [Display(Name = "Processing Status")]
        public string ProcessingStatus => MSummary.ProcessingStatusCode;

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MSummary.StatusDateTimeUtc);

        [Display(Name = "Unit of Work")]
        public string UnitOfWork => MSummary.UnitOfWork;
    }
}
