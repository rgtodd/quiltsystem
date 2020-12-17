//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface ILedgerMicroService
    {
        public Task<MLedger_Dashboard> GetDashboardAsync();

        public Task<MLedger_LedgerTransaction> GetLedgerTransactionAsync(long ledgerTransactionId);

        public Task<MLedger_LedgerTransactionList> GetLedgerTransactionsAsync(DateTime? postDate, int? ledgerAccountNumber, string unitOfWork, int? recordCount);

        public Task<MLedger_LedgerAccountSummaryList> GetLedgerAccountSummariesAsync(int accountingYear);

        public Task<MLedger_LedgerTransactionSummaryList> GetLedgerTransactionSummariesAsync(string unitOfWork, string source);

        public Task<long> PostLedgerAccountTransactionAsync(MLedger_PostLedgerTransaction transaction);
    }
}
