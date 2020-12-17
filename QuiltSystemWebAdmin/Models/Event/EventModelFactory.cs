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
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Event
{
    public class EventModelFactory : ApplicationModelFactory
    {
        public EventLogList CreateEventLogList(AEvent_EventLogList mEventLog, PagingState pagingState)
        {
            var summaries = CreateEventLogs(mEventLog);

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            ParseFilter(pagingState.Filter, out string unitOfWork, out string source);

            var model = new EventLogList()
            {
                Items = pagedSummaries,
                UnitOfWork = unitOfWork,
                Source = source,
                SourceList = new List<SelectListItem>
                {
                    new SelectListItem() { Text = "(Any)", Value = "*ANY" },
                    new SelectListItem() { Text = "Fulfillable", Value = MSources.Fulfillable },
                    new SelectListItem() { Text = "Fundable", Value = MSources.Fundable },
                    new SelectListItem() { Text = "Funder", Value = MSources.Funder },
                    new SelectListItem() { Text = "Order", Value = MSources.Order },
                    new SelectListItem() { Text = "Shipment", Value = MSources.Shipment },
                    new SelectListItem() { Text = "Shipment Request", Value = MSources.ShipmentRequest },
                    new SelectListItem() { Text = "Square Payment", Value = MSources.SquarePayment },
                    new SelectListItem() { Text = "Square Refund", Value = MSources.SquareRefund },
                    new SelectListItem() { Text = "Return", Value = MSources.Return },
                    new SelectListItem() { Text = "Return Request", Value = MSources.ReturnRequest }
                }
            };

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.UnitOfWork);
        }

        public string CreateFilter(string unitOfWork, string source)
        {
            return $"{unitOfWork}|{source}";
        }

        public void ParseFilter(string filter, out string unitOfWork, out string source)
        {
            if (string.IsNullOrEmpty(filter))
            {
                unitOfWork = null;
                source = null;
            }
            else
            {
                var idxDelimiter = filter.IndexOf('|');
                if (idxDelimiter == -1)
                {
                    unitOfWork = null;
                    source = null;
                }
                else
                {
                    unitOfWork = filter.Substring(0, idxDelimiter);
                    source = filter.Substring(idxDelimiter + 1);
                }
            }
        }

        private List<EventLogListItem> CreateEventLogs(AEvent_EventLogList aEventLogList)
        {
            var events = new List<EventLogListItem>();

            events.AddRange(aEventLogList.MFunderEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MFundableEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MFulfillableEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MShipmentRequestEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MShipmentEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MReturnRequestTrnsactions.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MReturnEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MOrderEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MSquarePaymentEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));
            events.AddRange(aEventLogList.MSquareRefundEventLogs.Summaries.Select(r => CreateEventLogListItem(r)));

            return events;
        }

        private EventLogListItem CreateEventLogListItem(MCommon_EventLogSummary mSummary)
        {
            return new EventLogListItem(mSummary, Locale);
        }

        private ModelMetadata<EventLogListItem> m_listItemMetadata;
        private ModelMetadata<EventLogListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<EventLogListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<EventLogListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<EventLogListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    EventLogListItem heading = new EventLogListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<EventLogListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.Source), r => r.Source },
                        { ListItemMetadata.GetDisplayName(m => m.EventId), r => r.EventId},
                        { ListItemMetadata.GetDisplayName(m => m.TransactionId), r => r.TransactionId },
                        { ListItemMetadata.GetDisplayName(m => m.EventType), r => r.EventType },
                        { ListItemMetadata.GetDisplayName(m => m.EventDateTime), r => r.EventDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.ProcessingStatus), r => r.ProcessingStatus },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.UnitOfWork), r => r.UnitOfWork }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<EventLogListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}