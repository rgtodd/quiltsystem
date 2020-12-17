//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface ILogAdminService
    {
        string ViewMode_All { get; }
        string ViewMode_Exception { get; }

        Task<IReadOnlyList<ALog_LogEntry>> GetLogEntriesAsync(string viewMode);
        Task<ALog_LogEntry> GetLogEntryAsync(long logEntryId);
    }
}
