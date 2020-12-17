//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Ledger
{
    public class LedgerModelFactory : ApplicationModelFactory
    {
        public LedgerTransaction CreateLedgerTransaction(MLedger_LedgerTransaction mLedgerTransaction)
        {
            return new LedgerTransaction(mLedgerTransaction, Locale);
        }

        public LedgerTransactionList CreateLedgerTransactionList(IList<MLedger_LedgerTransaction> mLedgerTransactions, PagingState pagingState)
        {
            var transactions = mLedgerTransactions.Select(r => CreateLedgerTransaction(r)).ToList();

            var sortedTransactions = transactions;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedTransactions.Count, pageSize);
            var pagedTransactions = sortedTransactions.ToPagedList(pageNumber, pageSize);

            var (postDate, ledgerAccountNumber, unitOfWork, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new LedgerTransactionList()
            {
                Items = pagedTransactions,
                Filter = new LedgerTransactionListFilter()
                {
                    PostDate = postDate,
                    LedgerAccountNumber = ledgerAccountNumber ?? 0,
                    UnitOfWork = unitOfWork,
                    RecordCount = recordCount,

                    LedgerAccountNumberList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = $"(All)", Value = "0" },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.Cash} - Cash", Value = LedgerAccountNumbers.Cash.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.AccountReceivable} - Accounts Receivable", Value = LedgerAccountNumbers.AccountReceivable.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.SalesTaxReceivable} - Sales Tax Receivable", Value = LedgerAccountNumbers.SalesTaxReceivable.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.FabricSupplyAsset} - Fabric Supply", Value = LedgerAccountNumbers.FabricSupplyAsset.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.FabricSupplySuspense} - Fabric Supply Suspense", Value = LedgerAccountNumbers.FabricSupplySuspense.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.AccountPayable} - Accounts Payable", Value = LedgerAccountNumbers.AccountPayable.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.SalesTaxPayable} - Sales Tax Payable", Value = LedgerAccountNumbers.SalesTaxPayable.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.FundsSuspense} - Funds Suspense", Value = LedgerAccountNumbers.FundsSuspense.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.OwnersEquity} - Owners Equity", Value = LedgerAccountNumbers.OwnersEquity.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.Income} - Income", Value = LedgerAccountNumbers.Income.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.PaymentFeeExpense} - Payment Fee Expense", Value = LedgerAccountNumbers.PaymentFeeExpense.ToString() },
                        new SelectListItem() { Text = $"{LedgerAccountNumbers.FabricSupplyExpense} - Fabric Supply Expense", Value = LedgerAccountNumbers.FabricSupplyExpense.ToString() },
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public LedgerSummary CreateLedgerSummary(MLedger_LedgerAccountSummaryList mLedgerAccountSummaryList)
        {
            var debitItems = new List<LedgerSummaryIten>();
            var creditItems = new List<LedgerSummaryIten>();
            foreach (var mSummary in mLedgerAccountSummaryList.Summaries.OrderBy(r => r.LedgerAccountNumber))
            {
                var item = new LedgerSummaryIten()
                {
                    LedgerAccountNumber = mSummary.LedgerAccountNumber,
                    Name = mSummary.Name,
                    Amount = mSummary.Amount
                };
                if (mSummary.DebitCreditCode == LedgerAccountCodes.Debit)
                {
                    debitItems.Add(item);
                }
                else
                {
                    creditItems.Add(item);
                }
            }

            var result = new LedgerSummary()
            {
                AccountingYear = mLedgerAccountSummaryList.AccountingYear,
                DebitItems = debitItems,
                CreditItems = creditItems
            };

            return result;
        }

        public string CreatePagingStateFilter(DateTime? postDate, int? ledgerAccountNumber, string unitOfWork, int recordCount)
        {
            return $"{postDate}|{ledgerAccountNumber}|{unitOfWork}|{recordCount}";
        }

        public string CreatePagingStateFilter(LedgerTransactionListFilter ledgerTransactionListFilter)
        {
            var ledgerAccountNumber = ledgerTransactionListFilter.LedgerAccountNumber != 0
                ? (int?)ledgerTransactionListFilter.LedgerAccountNumber
                : null;

            return CreatePagingStateFilter(
                ledgerTransactionListFilter.PostDate,
                ledgerAccountNumber,
                ledgerTransactionListFilter.UnitOfWork,
                ledgerTransactionListFilter.RecordCount);
        }

        public (DateTime? postDate, int? ledgerAccountNumber, string unitOfWork, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, null, null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var postDate = fields.Length >= 1 && DateTime.TryParse(fields[0], out var postDateField)
                ? (DateTime?)postDateField
                : null;

            var ledgerAccountNumber = fields.Length >= 2 && int.TryParse(fields[1], out var ledgerAccountNumberField)
                ? (int?)ledgerAccountNumberField
                : null;

            var unitOfWork = fields.Length >= 3
                ? fields[2]
                : null;

            var recordCount = fields.Length >= 4 && int.TryParse(fields[3], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (postDate, ledgerAccountNumber, unitOfWork, recordCount);
        }
    }
}