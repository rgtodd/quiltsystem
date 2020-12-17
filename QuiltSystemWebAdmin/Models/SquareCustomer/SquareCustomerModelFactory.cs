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

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquareCustomer
{
    public class SquareCustomerModelFactory : ApplicationModelFactory
    {
        public SquareCustomer CreateSquareCustomer(ASquare_Customer aCustomer)
        {
            return new SquareCustomer(aCustomer, Locale);
        }

        public SquareCustomerList CreateSquareCustomerList(IList<MSquare_CustomerSummary> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateSquareCustomerListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var recordCount = ParsePagingStateFilter(pagingState.Filter);

            var model = new SquareCustomerList()
            {
                Items = pagedSummaries,
                Filter = new SquareCustomerListFilter()
                {
                    RecordCount = recordCount,

                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public SquareCustomerListItem CreateSquareCustomerListItem(MSquare_CustomerSummary mSummary)
        {
            var model = new SquareCustomerListItem(mSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.SquareCustomerId);
        }

        public string CreatePagingStateFilter(int recordCount)
        {
            return $"{recordCount}";
        }

        public string CreatePagingStateFilter(SquareCustomerListFilter squareCustomerListFilter)
        {
            return CreatePagingStateFilter(squareCustomerListFilter.RecordCount);
        }

        public int ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return DefaultRecordCount;
            }

            var fields = filter.Split('|');

            var recordCount = fields.Length >= 1 && int.TryParse(fields[0], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return recordCount;
        }

        private ModelMetadata<SquareCustomerListItem> m_listItemMetadata;
        private ModelMetadata<SquareCustomerListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<SquareCustomerListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<SquareCustomerListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<SquareCustomerListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new SquareCustomerListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<SquareCustomerListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.SquareCustomerId), r => r.SquareCustomerId },
                        { ListItemMetadata.GetDisplayName(m => m.SquareCustomerReference), r => r.SquareCustomerReference },
                        { ListItemMetadata.GetDisplayName(m => m.UpdateDateTime), r => r.UpdateDateTime }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<SquareCustomerListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}