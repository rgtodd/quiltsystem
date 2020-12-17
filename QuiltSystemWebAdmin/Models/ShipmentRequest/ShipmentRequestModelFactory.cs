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

namespace RichTodd.QuiltSystem.WebAdmin.Models.ShipmentRequest
{
    public class ShipmentRequestModelFactory : ApplicationModelFactory
    {
        public ShipmentRequest CreateShipmentRequest(AShipment_ShipmentRequest aShipmentRequest)
        {
            return new ShipmentRequest(aShipmentRequest, Locale);
        }

        public ShipmentRequestList CreateShipmentRequestList(IReadOnlyList<AShipment_ShipmentRequestSummary> aSummaries, PagingState pagingState)
        {
            var summaries = aSummaries.Select(r => CreateShipmentRequestListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            int pageSize = PagingState.PageSize;
            int pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (shipmentRequestStatus, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new ShipmentRequestList()
            {
                Items = pagedSummaries,
                Filter = new ShipmentRequestListFilter()
                {
                    ShipmentRequestStatus = shipmentRequestStatus,
                    RecordCount = recordCount,

                    ShipmentRequestStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MFulfillment_ShipmentRequestStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Active", Value = MFulfillment_ShipmentRequestStatus.MetaActive.ToString() },
                        new SelectListItem() { Text = "Pending", Value = MFulfillment_ShipmentRequestStatus.Pending.ToString() },
                        new SelectListItem() { Text = "Open", Value = MFulfillment_ShipmentRequestStatus.Open.ToString() },
                        new SelectListItem() { Text = "Complete", Value = MFulfillment_ShipmentRequestStatus.Complete.ToString() },
                        new SelectListItem() { Text = "Cancelled", Value = MFulfillment_ShipmentRequestStatus.Cancelled.ToString() },
                        new SelectListItem() { Text = "Exception", Value = MFulfillment_ShipmentRequestStatus.Exception.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public ShipmentRequestListItem CreateShipmentRequestListItem(AShipment_ShipmentRequestSummary aSummary)
        {
            var model = new ShipmentRequestListItem(aSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.ShipmentRequestId);
        }

        public string CreatePagingStateFilter(MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int recordCount)
        {
            return $"{shipmentRequestStatus}|{recordCount}";
        }

        public string CreatePagingStateFilter(ShipmentRequestListFilter shipmentRequestListFilter)
        {
            return $"{shipmentRequestListFilter.ShipmentRequestStatus}|{shipmentRequestListFilter.RecordCount}";
        }

        public (MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MFulfillment_ShipmentRequestStatus.MetaAll, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var shipmentRequestStatus = fields.Length >= 1 && Enum.TryParse(fields[0], out MFulfillment_ShipmentRequestStatus shipmentRequestStatusField)
                ? shipmentRequestStatusField
                : MFulfillment_ShipmentRequestStatus.MetaAll;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (shipmentRequestStatus, recordCount);
        }

        private ModelMetadata<ShipmentRequestListItem> m_listItemMetadata;
        private ModelMetadata<ShipmentRequestListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<ShipmentRequestListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<ShipmentRequestListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<ShipmentRequestListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new ShipmentRequestListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<ShipmentRequestListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentRequestId), r => r.ShipmentRequestId },
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentRequestNumber), r => r.ShipmentRequestNumber },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableId), r => r.FulfillableId },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableName), r => r.FulfillableName },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableReference), r => r.FulfillableReference },
                        { ListItemMetadata.GetDisplayName(m => m.CreateDateTime), r => r.CreateDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.ShipmentRequestStatus), r => r.ShipmentRequestStatus },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<ShipmentRequestListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort) ? SortFunctions[sort] : null;
        }
    }
}