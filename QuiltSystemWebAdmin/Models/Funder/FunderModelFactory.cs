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
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Funder
{
    public class FunderModelFactory : ApplicationModelFactory
    {
        public Funder CreateFunder(AFunding_Funder aFunder)
        {
            return new Funder(aFunder, Locale);
        }

        public FunderList CreateFunderList(IList<MFunding_FunderSummary> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateFunderListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (hasFundsAvailable, hasFundsRefundable, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new FunderList()
            {
                Items = pagedSummaries,
                Filter = new FunderListFilter()
                {
                    HasFundsAvailable = ToString(hasFundsAvailable),
                    HasFundsRefundable = ToString(hasFundsRefundable),
                    RecordCount = recordCount,

                    HasFundsAvailableList = CreateNullableBooleanList(),
                    HasFundsRefundableList = CreateNullableBooleanList(),
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public FunderListItem CreateFunderListItem(MFunding_FunderSummary mSummary)
        {
            var model = new FunderListItem(mSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.FunderId);
        }

        public string CreatePagingStateFilter(bool? hasFundsAvailable, bool? hasFundsRefundable, int recordCount)
        {
            return $"{hasFundsAvailable}|{hasFundsRefundable}|{recordCount}";
        }

        public string CreatePagingStateFilter(FunderListFilter funderListFilter)
        {
            return CreatePagingStateFilter(
                ParseNullableBoolean(funderListFilter.HasFundsAvailable),
                ParseNullableBoolean(funderListFilter.HasFundsRefundable),
                funderListFilter.RecordCount);
        }

        public (bool? hasFundsAvailable, bool? hasFundsRefundable, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var hasFundsAvailable = fields.Length >= 1 && bool.TryParse(fields[0], out var hasFundsAvailableField)
                ? (bool?)hasFundsAvailableField
                : null;

            var hasFundsRefundable = fields.Length >= 2 && bool.TryParse(fields[1], out var hasFundsRefundableField)
                ? (bool?)hasFundsRefundableField
                : null;

            var recordCount = fields.Length >= 3 && int.TryParse(fields[2], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (hasFundsAvailable, hasFundsRefundable, recordCount);
        }

        private ModelMetadata<FunderListItem> m_listItemMetadata;
        private ModelMetadata<FunderListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<FunderListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<FunderListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<FunderListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new FunderListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<FunderListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.FunderId), r => r.FunderId },
                        { ListItemMetadata.GetDisplayName(m => m.FunderReference), r => r.FunderReference },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFundsReceived), r => r.TotalFundsReceived },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFundsAvailable), r => r.TotalFundsAvailable },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFundsRefunded), r => r.TotalFundsRefunded },
                        { ListItemMetadata.GetDisplayName(m => m.TotalFundsRefundable), r => r.TotalFundsRefundable },
                        { ListItemMetadata.GetDisplayName(m => m.TotalProcessingFee), r => r.TotalProcessingFee },
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<FunderListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}