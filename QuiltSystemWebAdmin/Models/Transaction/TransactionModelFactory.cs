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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Transaction
{
    public class TransactionModelFactory : ApplicationModelFactory
    {
        public const string AnySource = "*ANY";

        public TransactionList CreateTransactionList(ATransaction_TransactionList mTransaction, PagingState pagingState)
        {
            var summaries = CreateTransactions(mTransaction);

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (unitOfWork, source) = ParsePagingStateFilter(pagingState.Filter);

            var model = new TransactionList()
            {
                Items = pagedSummaries,
                Filter = new TransactionListFilter()
                {
                    UnitOfWork = unitOfWork,
                    Source = source,
                    SourceList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "(Any)", Value = AnySource },
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
                }
            };

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.UnitOfWork);
        }

        public string CreatePagingStateFilter(string unitOfWork, string source)
        {
            source = !string.IsNullOrEmpty(source) ? source : AnySource;

            return $"{unitOfWork}|{source}";
        }

        public (string unitOfWork, string source) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, null);
            }

            var idxDelimiter = filter.IndexOf('|');
            return idxDelimiter != -1
                ? (filter.Substring(0, idxDelimiter), filter.Substring(idxDelimiter + 1))
                : (null, null);
        }

        private List<TransactionListItem> CreateTransactions(ATransaction_TransactionList aTransactionList)
        {
            var transactions = new List<TransactionListItem>();

            transactions.AddRange(aTransactionList.MFunderTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MFundableTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MFulfillableTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MShipmentRequestTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MShipmentTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MReturnRequestTrnsactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MReturnTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MLedgerTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MOrderTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MSquarePaymentTransactions.Summaries.Select(r => CreateTransactionListItem(r)));
            transactions.AddRange(aTransactionList.MSquareRefundTransactions.Summaries.Select(r => CreateTransactionListItem(r)));

            return transactions;
        }

        private TransactionListItem CreateTransactionListItem(MCommon_TransactionSummary mSummary)
        {
            return new TransactionListItem(mSummary, Locale);
        }

        private ModelMetadata<TransactionListItem> m_listItemMetadata;
        private ModelMetadata<TransactionListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<TransactionListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<TransactionListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<TransactionListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    TransactionListItem heading = new TransactionListItem(null, null);

                    var sortFunctions = new Dictionary<string, Func<TransactionListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.TransactionId), r => r.TransactionId },
                        { ListItemMetadata.GetDisplayName(m => m.Source), r => r.Source },
                        { ListItemMetadata.GetDisplayName(m => m.TransactionDateTime), r => r.TransactionDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.Description), r => r.Description },
                        { ListItemMetadata.GetDisplayName(m => m.UnitOfWork), r => r.UnitOfWork }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<TransactionListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }
    }
}