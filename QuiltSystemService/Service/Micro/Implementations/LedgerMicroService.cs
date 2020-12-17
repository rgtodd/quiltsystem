//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class LedgerMicroService : MicroService, ILedgerMicroService
    {
        public LedgerMicroService(
            IApplicationLocale locale,
            ILogger<LedgerMicroService> logger,
            IQuiltContextFactory quiltContextFactory)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        { }

        public async Task<MLedger_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(LedgerMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboardItems = await ctx.LedgerAccountSubtotals
                    .Include(r => r.LedgerAccountNumberNavigation)
                    .Select(r => Create.MLedger_DashboardItem(r))
                    .ToListAsync().ConfigureAwait(false);

                var dashboard = new MLedger_Dashboard()
                {
                    DashboardItems = dashboardItems
                };

                var result = dashboard;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MLedger_LedgerTransaction> GetLedgerTransactionAsync(long ledgerTransactionId)
        {
            using var log = BeginFunction(nameof(LedgerMicroService), nameof(GetLedgerTransactionAsync), ledgerTransactionId);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbLedgerTransaction = await ctx.LedgerTransactions
                    .Include(r => r.LedgerTransactionEntries)
                        .ThenInclude(r => r.LedgerAccountNumberNavigation)
                    .Where(r => r.LedgerTransactionId == ledgerTransactionId)
                    .FirstAsync();

                var result = Create.MLedger_LedgerTransaction(dbLedgerTransaction);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MLedger_LedgerTransactionList> GetLedgerTransactionsAsync(DateTime? postDateTime, int? ledgerAccountNumber, string unitOfWork, int? recordCount)
        {
            using var log = BeginFunction(nameof(LedgerMicroService), nameof(GetLedgerTransactionsAsync), postDateTime, ledgerAccountNumber, unitOfWork, recordCount);
            try
            {
                using var ctx = CreateQuiltContext();

                var query = (IQueryable<LedgerTransaction>)ctx.LedgerTransactions
                    .Include(r => r.LedgerTransactionEntries)
                        .ThenInclude(r => r.LedgerAccountNumberNavigation);

                if (postDateTime != null)
                {
                    var fromDate = postDateTime.Value.Date;
                    var toDate = fromDate.AddDays(1);
                    query = query.Where(r => r.PostDateTime >= fromDate && r.PostDateTime < toDate);
                }

                if (ledgerAccountNumber != null)
                {
                    query = query.Where(r => r.LedgerTransactionEntries.Any(r => r.LedgerAccountNumber == ledgerAccountNumber.Value));
                }

                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    query = query.Where(r => r.UnitOfWork == unitOfWork);
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var dbLedgerTransactions = await query.ToListAsync().ConfigureAwait(false);

                MLedger_LedgerTransactionList result = Create.MLedger_LedgerTransactionList(dbLedgerTransactions);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MLedger_LedgerAccountSummaryList> GetLedgerAccountSummariesAsync(int accountingYear)
        {
            using var log = BeginFunction(nameof(LedgerMicroService), nameof(GetLedgerAccountSummariesAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dbLedgerAccounts = await ctx.LedgerAccounts
                    .Include(r => r.LedgerAccountSubtotals)
                    .ToListAsync().ConfigureAwait(false);

                var result = Create.MLedger_LedgerAccountSummaryList(accountingYear, dbLedgerAccounts);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> PostLedgerAccountTransactionAsync(MLedger_PostLedgerTransaction transaction)
        {
            using var log = BeginFunction(nameof(LedgerMicroService), nameof(PostLedgerAccountTransactionAsync), transaction);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var builder = ctx.CreateLedgerAccountTransactionBuilder()
                    .Begin(transaction.Description, transaction.PostDateTime, utcNow);

                if (transaction.UnitOfWork != null)
                {
                    var unitOfWork = new UnitOfWork(transaction.UnitOfWork);
                    _ = builder.UnitOfWork(unitOfWork);
                }

                foreach (var entry in transaction.Entries)
                {
                    _ = entry.DebitCreditCode switch
                    {
                        LedgerAccountCodes.Debit => builder.Debit(entry.LedgerAccountNumber, entry.EntryAmount, entry.LedgerReference, entry.SalesTaxJurisdiction),
                        LedgerAccountCodes.Credit => builder.Credit(entry.LedgerAccountNumber, entry.EntryAmount, entry.LedgerReference, entry.SalesTaxJurisdiction),
                        _ => throw new ArgumentException($"Invalid DebitCreditCode {entry.DebitCreditCode}."),
                    };
                }

                var dbLedgerTransaction = builder.Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbLedgerTransaction.LedgerTransactionId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MLedger_LedgerTransactionSummaryList> GetLedgerTransactionSummariesAsync(string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetLedgerTransactionSummariesAsync), unitOfWork, source);
            try
            {
                if (source != null && source != MSources.LedgerTransaction)
                {
                    return new MLedger_LedgerTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MLedger_LedgerTransactionSummary>().ToList()
                    };
                }

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<LedgerTransaction>)ctx.LedgerTransactions;
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MLedger_LedgerTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MLedger_LedgerTransactionSummaryList
                {
                    Summaries = summaries
                };

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            public static MLedger_DashboardItem MLedger_DashboardItem(LedgerAccountSubtotal dbLedgerAccountSubtotal)
            {
                return new MLedger_DashboardItem()
                {
                    AccountCode = dbLedgerAccountSubtotal.LedgerAccountNumberNavigation.LedgerAccountNumber,
                    AccountDescription = dbLedgerAccountSubtotal.LedgerAccountNumberNavigation.Name,
                    DebitCreditCode = dbLedgerAccountSubtotal.LedgerAccountNumberNavigation.DebitCreditCode,
                    Amount = dbLedgerAccountSubtotal.Balance
                };
            }

            public static MLedger_LedgerTransactionEntry MLedger_LedgerTransactionEntry(LedgerTransactionEntry dbLedgerTransactionEntry)
            {
                return new MLedger_LedgerTransactionEntry()
                {
                    LedgerTransactionEntryId = dbLedgerTransactionEntry.LedgerTransactionEntryId,
                    LedgerAccountNumber = dbLedgerTransactionEntry.LedgerAccountNumber,
                    LedgerAccountName = dbLedgerTransactionEntry.LedgerAccountNumberNavigation.Name,
                    DebitCreditCode = dbLedgerTransactionEntry.DebitCreditCode,
                    EntryAmount = dbLedgerTransactionEntry.TransactionEntryAmount,
                    LedgerReference = dbLedgerTransactionEntry.LedgerReference,
                    SalesTaxJurisdiction = dbLedgerTransactionEntry.SalesTaxJurisdiction
                };
            }

            public static MLedger_LedgerTransaction MLedger_LedgerTransaction(LedgerTransaction dbLedgerTransaction)
            {
                return new MLedger_LedgerTransaction()
                {
                    TransactionId = dbLedgerTransaction.LedgerTransactionId,
                    TransactionDateTimeUtc = dbLedgerTransaction.TransactionDateTimeUtc,
                    PostDateTime = dbLedgerTransaction.PostDateTime,
                    Description = dbLedgerTransaction.Description,
                    UnitOfWork = dbLedgerTransaction.UnitOfWork,
                    Entries = dbLedgerTransaction.LedgerTransactionEntries.Select(r => MLedger_LedgerTransactionEntry(r)).ToList()
                };
            }

            public static MLedger_LedgerTransactionList MLedger_LedgerTransactionList(IList<LedgerTransaction> dbLedgerTransactions)
            {
                return new MLedger_LedgerTransactionList()
                {
                    Transactions = dbLedgerTransactions.Select(r => MLedger_LedgerTransaction(r)).ToList()
                };
            }

            public static MLedger_LedgerAccountSummary MLedger_LedgerAccountSummary(int accountingYear, LedgerAccount dbLedgerAccount)
            {
                return new MLedger_LedgerAccountSummary()
                {
                    LedgerAccountNumber = dbLedgerAccount.LedgerAccountNumber,
                    Name = dbLedgerAccount.Name,
                    DebitCreditCode = dbLedgerAccount.DebitCreditCode,
                    Amount = dbLedgerAccount.LedgerAccountSubtotals.Where(r => r.AccountingYear == accountingYear).Select(r => r.Balance).FirstOrDefault()
                };
            }

            public static MLedger_LedgerAccountSummaryList MLedger_LedgerAccountSummaryList(int accountingYear, IList<LedgerAccount> dbLedgerAccounts)
            {
                var summaries = dbLedgerAccounts.Select(r => Create.MLedger_LedgerAccountSummary(accountingYear, r)).ToList();

                var result = new MLedger_LedgerAccountSummaryList()
                {
                    AccountingYear = accountingYear,
                    Summaries = summaries
                };
                return result;
            }

            public static MLedger_LedgerTransactionSummary MLedger_LedgerTransactionSummary(LedgerTransaction dbLedgerTransaction)
            {
                return new MLedger_LedgerTransactionSummary()
                {
                    TransactionId = dbLedgerTransaction.LedgerTransactionId,
                    EntityId = dbLedgerTransaction.LedgerTransactionId,
                    TransactionDateTimeUtc = dbLedgerTransaction.TransactionDateTimeUtc,
                    Description = dbLedgerTransaction.Description,
                    UnitOfWork = dbLedgerTransaction.UnitOfWork,
                    PostDateTime = dbLedgerTransaction.PostDateTime
                };
            }
        }
    }
}
