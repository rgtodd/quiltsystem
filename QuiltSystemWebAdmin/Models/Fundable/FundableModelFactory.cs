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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fundable
{
    public class FundableModelFactory : ApplicationModelFactory
    {
        public Fundable CreateFundable(AFunding_Fundable aFundable)
        {
            return new Fundable(aFundable, Locale);
        }

        public FundableList CreateFundableList(IList<MFunding_FundableSummary> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateFundableListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (hasFundsRequired, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new FundableList()
            {
                Items = pagedSummaries,
                Filter = new FundableListFilter()
                {
                    HasFundsRequired = ToString(hasFundsRequired),
                    RecordCount = recordCount,

                    HasFundsRequiredList = CreateNullableBooleanList(),
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public FundableListItem CreateFundableListItem(MFunding_FundableSummary mSummary)
        {
            var model = new FundableListItem(mSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.FundableId);
        }

        public string CreatePagingStateFilter(bool? hasFundsRequired, int recordCount)
        {
            return $"{hasFundsRequired}|{recordCount}";
        }

        public string CreatePagingStateFilter(FundableListFilter fundableListFilter)
        {
            return CreatePagingStateFilter(ParseNullableBoolean(fundableListFilter.HasFundsRequired), fundableListFilter.RecordCount);
        }

        public (bool? hasFundsRequired, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var hasFundsRequired = fields.Length >= 1 && bool.TryParse(fields[0], out var hasFundsRequiredField)
                ? hasFundsRequiredField
                : (bool?)null;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (hasFundsRequired, recordCount);
        }

        private ModelMetadata<FundableListItem> m_listItemMetadata;
        private ModelMetadata<FundableListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<FundableListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<FundableListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<FundableListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new FundableListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<FundableListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.FundableId), r => r.FundableId },
                        { ListItemMetadata.GetDisplayName(m => m.FundableReference), r => r.FundableReference },
                        { ListItemMetadata.GetDisplayName(m => m.FundsRequired), r => r.FundsRequired },
                        { ListItemMetadata.GetDisplayName(m => m.FundsReceived), r => r.FundsReceived }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<FundableListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}