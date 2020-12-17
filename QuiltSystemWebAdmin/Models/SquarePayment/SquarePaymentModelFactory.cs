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

namespace RichTodd.QuiltSystem.WebAdmin.Models.SquarePayment
{
    public class SquarePaymentModelFactory : ApplicationModelFactory
    {
        public SquarePayment CreateSquarePayment(ASquare_Payment aPayment)
        {
            return new SquarePayment(aPayment, Locale);
        }

        public SquarePaymentList CreateSquarePaymentList(IList<MSquare_PaymentSummary> mSummaries, PagingState pagingState)
        {
            var summaries = mSummaries.Select(r => CreateSquarePaymentListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (paymentDate, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new SquarePaymentList()
            {
                Items = pagedSummaries,
                Filter = new SquarePaymentListFilter()
                {
                    PaymentDate = paymentDate,
                    RecordCount = recordCount,
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public SquarePaymentListItem CreateSquarePaymentListItem(MSquare_PaymentSummary mSummary)
        {
            var model = new SquarePaymentListItem(mSummary, Locale);

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.SquarePaymentId);
        }

        public string CreatePagingStateFilter(DateTime? paymentDate, int recordCount)
        {
            return $"{paymentDate}|{recordCount}";
        }

        public string CreatePagingStateFilter(SquarePaymentListFilter squarePaymentListFilter)
        {
            return CreatePagingStateFilter(squarePaymentListFilter.PaymentDate, squarePaymentListFilter.RecordCount);
        }

        public (DateTime? paymentDate, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var paymentDate = fields.Length >= 1 && DateTime.TryParse(fields[0], out var orderDateField)
                ? (DateTime?)orderDateField
                : null;

            var recordCount = fields.Length >= 2 && int.TryParse(fields[1], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (paymentDate, recordCount);
        }

        private ModelMetadata<SquarePaymentListItem> m_listItemMetadata;
        private ModelMetadata<SquarePaymentListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<SquarePaymentListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<SquarePaymentListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<SquarePaymentListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new SquarePaymentListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<SquarePaymentListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.SquarePaymentId), r => r.SquarePaymentId },
                        { ListItemMetadata.GetDisplayName(m => m.SquarePaymentReference), r => r.SquarePaymentReference },
                        { ListItemMetadata.GetDisplayName(m => m.SquareCustomerId), r => r.SquareCustomerId },
                        { ListItemMetadata.GetDisplayName(m => m.PaymentAmount), r => r.PaymentAmount },
                        { ListItemMetadata.GetDisplayName(m => m.RefundAmount), r => r.RefundAmount },
                        { ListItemMetadata.GetDisplayName(m => m.ProcessingFeeAmount), r => r.ProcessingFeeAmount },
                        { ListItemMetadata.GetDisplayName(m => m.SquarePaymentRecordId), r => r.SquarePaymentRecordId },
                        { ListItemMetadata.GetDisplayName(m => m.VersionNumber), r => r.VersionNumber },
                        { ListItemMetadata.GetDisplayName(m => m.UpdateDateTime), r => r.UpdateDateTime }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<SquarePaymentListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}