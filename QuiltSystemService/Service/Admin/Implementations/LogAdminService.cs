//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class LogAdminService : BaseService, ILogAdminService
    {
        public LogAdminService(
            IApplicationRequestServices requestServices,
            ILogger<LogAdminService> logger)
            : base(requestServices, logger)
        { }

        #region IAdmin_LogService

        public string ViewMode_All
        {
            get { return "All"; }
        }

        public string ViewMode_Exception
        {
            get { return "Exception"; }
        }

        public async Task<IReadOnlyList<ALog_LogEntry>> GetLogEntriesAsync(string viewMode)
        {
            await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

            var logEntries = new List<ALog_LogEntry>();

            //using (var ctx = QuiltContextFactory.Create())
            //{
            //    IQueryable<LogEntry> dbLogEntries = ctx.LogEntries;
            //    if (viewMode == ViewMode_Exception)
            //    {
            //        dbLogEntries = dbLogEntries.Where(r => r.SeverityCode == LogEntrySeverityCodes.Error);
            //    }
            //    dbLogEntries = dbLogEntries.OrderByDescending(r => r.LogDateTimeUtc);

            //    var count = 0;
            //    foreach (var dbLogEntry in await dbLogEntries.ToListAsync().ConfigureAwait(false))
            //    {
            //        var logEntry = CreateLogEntry(dbLogEntry, true);

            //        logEntries.Add(logEntry);

            //        if (++count > 1000)
            //        {
            //            break;
            //        }
            //    }
            //}

            return logEntries;
        }

        public async Task<ALog_LogEntry> GetLogEntryAsync(long logEntryId)
        {
            await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

            //using var ctx = QuiltContextFactory.Create();

            //var dbLogEntry = await ctx.LogEntries.Where(r => r.LogEntryId == logEntryId).SingleAsync().ConfigureAwait(false);

            //var logEntry = CreateLogEntry(dbLogEntry, false);

            //return logEntry;

            return null;
        }

        #endregion
    }
}
