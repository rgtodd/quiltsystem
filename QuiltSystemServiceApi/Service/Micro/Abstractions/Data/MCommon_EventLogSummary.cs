//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public abstract class MCommon_EventLogSummary
    {
        public abstract string Source { get; }
        public long EventId { get; set; }
        public long TransactionId { get; set; }
        public long EntityId { get; set; }
        public string EventTypeCode { get; set; }
        public DateTime EventDateTimeUtc { get; set; }
        public string ProcessingStatusCode { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public string UnitOfWork { get; set; }
    }
}
