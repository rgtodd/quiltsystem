//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using PagedList;

using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Alert
{
    public class AlertModelFactory : ApplicationModelFactory
    {
        public Alert CreateAlert(AAlert_Alert aAlert)
        {
            return new Alert(aAlert, Locale);
        }

        public AlertList CreateAlertList(IList<AAlert_Alert> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateAlertListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            int pageSize = PagingState.PageSize;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedAlerts = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (acknowledged, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new AlertList()
            {
                Items = pagedAlerts,
                Filter = new AlertListFilter()
                {
                    Acknowledged = ToString(acknowledged),
                    RecordCount = recordCount,

                    AcknowledgedList = CreateNullableBooleanList(),
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public AlertListItem CreateAlertListItem(AAlert_Alert aAlert)
        {
            var model = new AlertListItem(aAlert, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.AlertId);
        }

        public string CreatePagingStateFilter(bool? isAcknowledged, int recordCount)
        {
            return $"{isAcknowledged}|{recordCount}";
        }

        public string CreatePagingStateFilter(AlertListFilter alertListFilter)
        {
            return CreatePagingStateFilter(ParseNullableBoolean(alertListFilter.Acknowledged), alertListFilter.RecordCount);
        }

        public (bool? acknowledged, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var acknowledged = fields.Length >= 1 && bool.TryParse(fields[0], out var acknowledgedField)
                ? acknowledgedField
                : (bool?)null;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (acknowledged, recordCount);
        }

        private ModelMetadata<AlertListItem> m_listItemMetadata;
        private ModelMetadata<AlertListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<AlertListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<AlertListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<AlertListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var sortFunctions = new Dictionary<string, Func<AlertListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.AlertId), r => r.AlertId },
                        { ListItemMetadata.GetDisplayName(m => m.AlertType), r => r.AlertType },
                        { ListItemMetadata.GetDisplayName(m => m.AlertDateTime), r => r.AlertDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.CompletedDateTime), r => r.CompletedDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.Description), r => r.Description },
                        { ListItemMetadata.GetDisplayName(m => m.Exception), r => r.Exception },
                        { ListItemMetadata.GetDisplayName(m => m.UserId), r => r.UserId },
                        { ListItemMetadata.GetDisplayName(m => m.UserEmail), r => r.UserEmail },
                        { ListItemMetadata.GetDisplayName(m => m.TopicId), r => r.TopicId },
                        { ListItemMetadata.GetDisplayName(m => m.TopicReference), r => r.TopicReference }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<AlertListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}