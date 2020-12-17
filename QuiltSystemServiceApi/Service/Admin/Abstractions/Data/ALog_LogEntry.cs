//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ALog_LogEntry
    {
        public long LogEntryId { get; set; }
        public DateTime LogEntryDateTimeLocalUtc { get; set; }
        public string Message { get; set; }
        public int DurationMilliseconds { get; set; }
        public string LogName { get; set; }
        public string LogEntryTypeName { get; set; }
        public string SeverityCode { get; set; }
    }
}
