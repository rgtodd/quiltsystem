//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Log
{
    public class LogModelFactory : ApplicationModelFactory
    {

        private static IDictionary<string, Func<LogEntryModel, object>> s_sortFunctions;

        private ModelMetadata<LogEntryModel> m_logEntryModelMetadata;

        private ModelMetadata<LogEntryModel> LogEntryModelMetadata
        {
            get
            {
                if (m_logEntryModelMetadata == null)
                {
                    m_logEntryModelMetadata = ModelMetadata<LogEntryModel>.Create(HttpContext);
                }

                return m_logEntryModelMetadata;
            }
        }

        private IDictionary<string, Func<LogEntryModel, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new LogEntryModel();

                    var sortFunctions = new Dictionary<string, Func<LogEntryModel, object>>
                    {
                        { LogEntryModelMetadata.GetDisplayName(m => m.DurationMilliseconds), r => r.DurationMilliseconds },
                        { LogEntryModelMetadata.GetDisplayName(m => m.LogEntryDateTime), r => r.LogEntryDateTime },
                        { LogEntryModelMetadata.GetDisplayName(m => m.LogEntryId), r => r.LogEntryId },
                        { LogEntryModelMetadata.GetDisplayName(m => m.LogEntryTypeName), r => r.LogEntryTypeName },
                        { LogEntryModelMetadata.GetDisplayName(m => m.LogName), r => r.LogName },
                        { LogEntryModelMetadata.GetDisplayName(m => m.Message), r => r.Message },
                        { LogEntryModelMetadata.GetDisplayName(m => m.SeverityCode), r => r.SeverityCode }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        public LogEntryDetailModel CreateLogEntryDetailModel(ALog_LogEntry svcLogEntry)
        {
            var model = new LogEntryDetailModel()
            {
                LogEntry = CreateLogEntryModel(svcLogEntry)
            };

            return model;
        }

        public LogEntryListModel CreateLogEntryListModel(IReadOnlyList<ALog_LogEntry> svcLogEntries, PagingState pagingState, ILogAdminService logService)
        {
            var logEntries = svcLogEntries.Select(r => CreateLogEntryModel(r)).ToList();

            IReadOnlyList<LogEntryModel> sortedLogEntries;
            var sortFunction = GetSortFunction(pagingState.Sort);
            sortedLogEntries = sortFunction != null
                ? pagingState.Descending
                    ? logEntries.OrderByDescending(sortFunction).ToList()
                    : logEntries.OrderBy(sortFunction).ToList()
                : logEntries;

            var pageSize = 10;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedLogEntries.Count, pageSize);
            var pagedLogEntries = sortedLogEntries.ToPagedList(pageNumber, pageSize);

            var model = new LogEntryListModel()
            {
                LogEntries = pagedLogEntries,
                Filter = pagingState.Filter,
                Filters = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "All", Value = logService.ViewMode_All },
                    new SelectListItem() { Text = "Exception", Value = logService.ViewMode_Exception }
                }
            };

            return model;
        }

        private LogEntryModel CreateLogEntryModel(ALog_LogEntry svcLogEntry)
        {
            var logEntry = new LogEntryModel()
            {
                LogEntryId = svcLogEntry.LogEntryId,
                LogEntryDateTime = Locale.GetLocalTimeFromUtc(svcLogEntry.LogEntryDateTimeLocalUtc),
                Message = svcLogEntry.Message,
                DurationMilliseconds = svcLogEntry.DurationMilliseconds,
                LogEntryTypeName = svcLogEntry.LogEntryTypeName,
                LogName = svcLogEntry.LogName,
                SeverityCode = svcLogEntry.SeverityCode
            };

            return logEntry;
        }

        private Func<LogEntryModel, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }

    }
}