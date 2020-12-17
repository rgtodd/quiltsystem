//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class LogEntry
    {
        public long LogEntryId { get; set; }
        public string LogEntryTypeCode { get; set; }
        public string LogName { get; set; }
        public int DurationMilliseconds { get; set; }
        public string SeverityCode { get; set; }
        public DateTime LogDateTimeUtc { get; set; }
        public string Message { get; set; }

        public virtual LogEntryType LogEntryTypeCodeNavigation { get; set; }
    }
}
