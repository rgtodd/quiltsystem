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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fulfillable
{
    public class FulfillableModelFactory : ApplicationModelFactory
    {
        public Fulfillable CreateFulfillable(AFulfillable_Fulfillable aFulfillable)
        {
            return new Fulfillable(aFulfillable, Locale);
        }

        public FulfillableList CreateFulfillableList(IList<MFulfillment_FulfillableSummary> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateFulfillableListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (fulfillableStatus, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new FulfillableList()
            {
                Items = pagedSummaries,
                Filter = new FulfillableListFilter()
                {
                    FulfillableStatus = fulfillableStatus,
                    RecordCount = recordCount,

                    FulfillableStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MFulfillment_FulfillableStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Active", Value = MFulfillment_FulfillableStatus.MetaActive.ToString() },
                        new SelectListItem() { Text = "Open", Value = MFulfillment_FulfillableStatus.Open.ToString() },
                        new SelectListItem() { Text = "Closed", Value = MFulfillment_FulfillableStatus.Closed.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public FulfillableListItem CreateFulfillableListItem(MFulfillment_FulfillableSummary mSummary)
        {
            var model = new FulfillableListItem(mSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.FulfillableId);
        }

        public string CreatePagingStateFilter(MFulfillment_FulfillableStatus fulfillableStatus, int recordCount)
        {
            return $"{fulfillableStatus}|{recordCount}";
        }

        public string CreatePagingStateFilter(FulfillableListFilter fulfillableListFilter)
        {
            return CreatePagingStateFilter(fulfillableListFilter.FulfillableStatus, fulfillableListFilter.RecordCount);
        }

        public (MFulfillment_FulfillableStatus fulfillableStatus, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (MFulfillment_FulfillableStatus.MetaAll, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var fulfillableStatus = fields.Length >= 1 && Enum.TryParse(fields[0], out MFulfillment_FulfillableStatus fulfillableStatusField)
                ? fulfillableStatusField
                : MFulfillment_FulfillableStatus.MetaAll;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (fulfillableStatus, recordCount);
        }

        private ModelMetadata<FulfillableListItem> m_listItemMetadata;
        private ModelMetadata<FulfillableListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<FulfillableListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<FulfillableListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<FulfillableListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new FulfillableListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<FulfillableListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableId), r => r.FulfillableId },
                        { ListItemMetadata.GetDisplayName(m => m.Name), r => r.Name },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableReference), r => r.FulfillableReference },
                        { ListItemMetadata.GetDisplayName(m => m.FulfillableStatus), r => r.FulfillableStatus },
                        { ListItemMetadata.GetDisplayName(m => m.CreateDateTime), r => r.CreateDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFulfillmentRequiredQuantity), r => r.TotalFulfillmentRequiredQuantity },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFulfillmentCompleteQuantity), r => r.TotalFulfillmentCompleteQuantity },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFulfillmentReturnQuantity), r => r.TotalFulfillmentReturnQuantity }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<FulfillableListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}