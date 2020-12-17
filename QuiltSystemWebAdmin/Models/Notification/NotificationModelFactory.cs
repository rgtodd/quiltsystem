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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Notification
{
    public class NotificationModelFactory : ApplicationModelFactory
    {
        public Notification CreateNotification(ANotification_Notification aNotification)
        {
            return new Notification(aNotification, Locale);
        }

        public NotificationList CreateNotificationListModel(IList<ANotification_Notification> svcNotifications, PagingState pagingState)
        {
            var summaries = svcNotifications.Select(r => CreateNotificationListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedNotifications = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            int pageSize = PagingState.PageSize;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedNotifications.Count, pageSize);
            var pagedSummaries = sortedNotifications.ToPagedList(pageNumber, pageSize);

            var (acknowledged, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new NotificationList()
            {
                Items = pagedSummaries,
                Filter = new NotificationListFilter()
                {
                    Acknowledged = ToString(acknowledged),
                    RecordCount = recordCount,

                    AcknowledgedList = CreateNullableBooleanList(),
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public NotificationListItem CreateNotificationListItem(ANotification_Notification aNotification)
        {
            var model = new NotificationListItem(aNotification, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.NotificationId);
        }

        public string CreatePagingStateFilter(bool? isAcknowledged, int recordCount)
        {
            return $"{isAcknowledged}|{recordCount}";
        }

        public string CreatePagingStateFilter(NotificationListFilter alertListFilter)
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

        private ModelMetadata<NotificationListItem> m_listItemMetadata;
        private ModelMetadata<NotificationListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<NotificationListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<NotificationListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<NotificationListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new NotificationListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<NotificationListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.AcknowledgementDateTime), r => r.AcknowledgementDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.UserId), r => r.UserId },
                        { ListItemMetadata.GetDisplayName(m => m.UserEmail), r => r.UserEmail },
                        { ListItemMetadata.GetDisplayName(m => m.CreatedDateTime), r => r.CreatedDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.NotificationId), r => r.NotificationId },
                        { ListItemMetadata.GetDisplayName(m => m.NotificationType), r => r.NotificationType },
                        { ListItemMetadata.GetDisplayName(m => m.OrderId), r => r.OrderId }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<NotificationListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}