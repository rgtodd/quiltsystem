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
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class FundingMicroService : MicroService, IFundingMicroService
    {
        private IFundingEventMicroService FundingEventService { get; }
        private ILedgerMicroService LedgerMicroService { get; }

        public FundingMicroService(
            IApplicationLocale locale,
            ILogger<FundingMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IFundingEventMicroService fundingEventService,
            ILedgerMicroService ledgerMicroService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            FundingEventService = fundingEventService ?? throw new ArgumentNullException(nameof(fundingEventService));
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));
        }

        public async Task<MFunding_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MFunding_Dashboard()
                {
                    TotalFundables = await ctx.Fundables.CountAsync(),
                    TotalFunders = await ctx.Funders.CountAsync(),
                    TotalTransactions = await ctx.FunderTransactions.CountAsync() + await ctx.FundableTransactions.CountAsync(),
                    TotalEvents = await ctx.FunderEvents.CountAsync() + await ctx.FundableEvents.CountAsync()
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

        #region Funder

        public async Task<long> AllocateFunderAsync(string funderReference)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(AllocateFunderAsync), funderReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbFunder = await ctx.Funders.Where(r => r.FunderReference == funderReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbFunder == null)
                {
                    dbFunder = new Funder()
                    {
                        FunderReference = funderReference,
                        UpdateDateTimeUtc = GetUtcNow()
                    };

                    _ = ctx.Funders.Add(dbFunder);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbFunder.FunderId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupFunderAsync(string funderReference)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(LookupFunderAsync), funderReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var funderId = await ctx.Funders.Where(r => r.FunderReference == funderReference).Select(r => r.FunderId).FirstOrDefaultAsync().ConfigureAwait(false);

                var result = funderId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_Funder> GetFunderAsync(long funderId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderAsync), funderId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var funder = await ctx.Funders
                    .Include(r => r.FunderAccounts)
                        .ThenInclude(r => r.FunderTransactions)
                    .Where(r => r.FunderId == funderId)
                    .Select(r => Create.MFunding_Funder(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = funder;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FunderSummaryList> GetFunderSummariesAsync(long? fundableId, bool? hasFundsAvailable, bool? hasFundsRefundable, int? recordCount)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderSummariesAsync), fundableId, hasFundsAvailable, hasFundsRefundable, recordCount);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fundableReference = fundableId != null
                    ? await ctx.Fundables.Where(r => r.FundableId == fundableId.Value).Select(r => r.FundableReference).SingleAsync().ConfigureAwait(false)
                    : null;

                var query = (IQueryable<Funder>)ctx.Funders
                    .Include(r => r.FunderAccounts);

                if (fundableReference != null)
                {
                    query = query.Where(r => r.FunderAccounts.Any(r => r.FundableReference == fundableReference));
                }
                if (hasFundsAvailable != null)
                {
                    query = hasFundsAvailable.Value
                        ? query.Where(r => r.FunderAccounts.Any(r => r.FundsAvailable > 0m))
                        : query.Where(r => r.FunderAccounts.All(r => r.FundsAvailable == 0m));
                }
                if (hasFundsRefundable != null)
                {
                    query = hasFundsRefundable.Value
                        ? query.Where(r => r.FunderAccounts.Any(r => r.FundsRefundable > 0m))
                        : query.Where(r => r.FunderAccounts.All(r => r.FundsRefundable == 0m));
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Select(r => Create.MFunding_FunderSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FunderSummaryList
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

        public async Task SetFundsReceivedAsync(long funderId, string fundableReference, decimal fundsReceived, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(SetFundsReceivedAsync), funderId, fundableReference, fundsReceived, unitOfWorkRoot);
            try
            {
                if (fundableReference == null) throw new ArgumentNullException(nameof(fundableReference));

                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                // Apply funds received to funder.
                {
                    using var ctx = CreateQuiltContext();

                    var dbFunder = await ctx.Funders
                        .Where(r => r.FunderId == funderId)
                        .Include(r => r.FunderAccounts)
                        .FirstAsync().ConfigureAwait(false);

                    // Note: Use Sum function since a FunderAccount may not exist at this point.
                    //
                    var fundsReceivedDelta = fundsReceived - dbFunder.FunderAccounts.Where(r => r.FundableReference == fundableReference).Sum(r => r.FundsReceived);

                    if (fundsReceivedDelta != 0m)
                    {
                        var dbFundingTransaction = ctx.CreateFunderTransactionBuilder()
                            .Begin(funderId, fundableReference, utcNow)
                            .UnitOfWork(unitOfWork)
                            .AddFundsReceived(fundsReceivedDelta)
                            .Create();

                        var referenceValues = ParseReferenceValues.From(dbFunder.FunderReference);
                        var dbLedgerTransaction = ctx.CreateLedgerAccountTransactionBuilder()
                            .Begin($"Funds received for funder {funderId} ({referenceValues}).", Locale.GetLocalTimeFromUtc(utcNow), utcNow)
                            .UnitOfWork(unitOfWork)
                            .Debit(LedgerAccountNumbers.Cash, fundsReceivedDelta)
                            .Credit(LedgerAccountNumbers.FundsSuspense, fundsReceivedDelta)
                            .Create();

                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                        var fundsTransferred = await TransferFundsToFundableAsync(funderId, fundableReference, null, unitOfWork);
                        log.Message($"{fundsTransferred:c} funds transferred from funder {funderId} to fundable {fundableReference}.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetFundsRefundedAsync(long funderId, string fundableReference, decimal fundsRefunded, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(SetFundsRefundedAsync), funderId, fundableReference, fundsRefunded, unitOfWorkRoot);
            try
            {
                if (fundableReference == null) throw new ArgumentNullException(nameof(fundableReference));

                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                // Apply funds refunded to funder.
                {
                    using var ctx = CreateQuiltContext();

                    var dbFunder = await ctx.Funders
                        .Where(r => r.FunderId == funderId)
                        .Include(r => r.FunderAccounts)
                        .FirstAsync().ConfigureAwait(false);

                    var fundsRefundedDelta = fundsRefunded - dbFunder.FunderAccounts.Where(r => r.FundableReference == fundableReference).Sum(r => r.FundsRefunded);

                    if (fundsRefundedDelta != 0m)
                    {
                        var dbFundingTransaction = ctx.CreateFunderTransactionBuilder()
                            .Begin(funderId, fundableReference, utcNow)
                            .UnitOfWork(unitOfWork)
                            .AddFundsRefunded(fundsRefundedDelta)
                            .AddFundsRefundable(-fundsRefundedDelta)
                            .Create();

                        var referenceValues = ParseReferenceValues.From(dbFunder.FunderReference);
                        var dbLedgerTransaction = ctx.CreateLedgerAccountTransactionBuilder()
                            .Begin($"Funds received for funder {funderId} ({referenceValues}).", Locale.GetLocalTimeFromUtc(utcNow), utcNow)
                            .UnitOfWork(unitOfWork)
                            .Debit(LedgerAccountNumbers.AccountPayable, fundsRefundedDelta)
                            .Credit(LedgerAccountNumbers.Cash, fundsRefundedDelta)
                            .Create();

                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetProcessingFeeAsync(long funderId, string fundableReference, decimal processingFee, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(SetProcessingFeeAsync), funderId, fundableReference, processingFee, unitOfWorkRoot);
            try
            {
                if (fundableReference == null) throw new ArgumentNullException(nameof(fundableReference));

                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                // Apply funds received to funder.
                {
                    using var ctx = CreateQuiltContext();

                    var dbFunder = await ctx.Funders
                        .Where(r => r.FunderId == funderId)
                        .Include(r => r.FunderAccounts)
                        .FirstAsync().ConfigureAwait(false);

                    var processingFeeDelta = processingFee - dbFunder.FunderAccounts.Where(r => r.FundableReference == fundableReference).Sum(r => r.ProcessingFee);

                    if (processingFeeDelta != 0m)
                    {
                        var dbFundingTransaction = ctx.CreateFunderTransactionBuilder()
                            .Begin(funderId, fundableReference, utcNow)
                            .UnitOfWork(unitOfWork)
                            .AddProcessingFee(processingFeeDelta)
                            .Create();

                        var referenceValues = ParseReferenceValues.From(dbFunder.FunderReference);
                        var dbLedgerTransaction = ctx.CreateLedgerAccountTransactionBuilder()
                            .Begin($"Processing fee for funder {funderId} ({referenceValues}).", Locale.GetLocalTimeFromUtc(utcNow), utcNow)
                            .UnitOfWork(unitOfWork)
                            .Debit(LedgerAccountNumbers.PaymentFeeExpense, processingFeeDelta)
                            .Credit(LedgerAccountNumbers.Cash, processingFeeDelta)
                            .Create();

                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FunderTransaction> GetFunderTransactionAsync(long funderTransactionId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderTransactionAsync), funderTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableTransaction = await ctx.FunderTransactions
                    .Where(r => r.FunderTransactionId == funderTransactionId)
                    .Select(r => Create.MFunding_FunderTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = fulfillableTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FunderTransactionSummaryList> GetFunderTransactionSummariesAsync(long? funderId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderTransactionSummariesAsync), funderId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Funder)
                {
                    return new MFunding_FunderTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFunding_FunderTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FunderTransaction>)ctx.FunderTransactions;
                if (funderId != null)
                {
                    query = query.Where(r => r.FunderId == funderId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFunding_FunderTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FunderTransactionSummaryList
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

        public async Task<MFunding_FunderEventLog> GetFunderEventLogAsync(long funderEventId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderEventLogAsync), funderEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableEventLog = await ctx.FunderEvents
                    .Where(r => r.FunderEventId == funderEventId)
                    .Select(r => Create.MFunding_FunderEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = fulfillableEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FunderEventLogSummaryList> GetFunderEventLogSummariesAsync(long? funderId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFunderEventLogSummariesAsync), funderId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Funder)
                {
                    return new MFunding_FunderEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFunding_FunderEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FunderEvent>)ctx.FunderEvents.Include(r => r.FunderTransaction);
                if (funderId != null)
                {
                    query = query.Where(r => r.FunderTransaction.FunderId == funderId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    query = query.Where(r => r.FunderTransaction.UnitOfWork == unitOfWork);
                }
                var summaries = await query
                    .Select(r => Create.MFunding_FunderEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FunderEventLogSummaryList
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

        #endregion

        #region Fundable

        public async Task<long> AllocateFundableAsync(string fundableReference)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(AllocateFundableAsync), fundableReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbFundable = await ctx.Fundables.Where(r => r.FundableReference == fundableReference).FirstOrDefaultAsync().ConfigureAwait(false);
                if (dbFundable == null)
                {
                    dbFundable = new Fundable()
                    {
                        FundableReference = fundableReference,
                        FundsRequiredTotal = 0,
                        FundsRequiredIncome = 0,
                        FundsRequiredSalesTax = 0,
                        FundsRequiredSalesTaxJurisdiction = string.Empty,
                        FundsReceived = 0,
                        UpdateDateTimeUtc = GetUtcNow()
                    };
                    _ = ctx.Fundables.Add(dbFundable);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbFundable.FundableId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupFundableAsync(string fundableReference)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(LookupFundableAsync), fundableReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var fundableId = await ctx.Fundables.Where(r => r.FundableReference == fundableReference).Select(r => (long?)r.FundableId).FirstOrDefaultAsync().ConfigureAwait(false);

                var result = fundableId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_Fundable> GetFundableAsync(long fundableId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableAsync), fundableId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fundable = await ctx.Fundables
                    .Include(r => r.FundableTransactions)
                    .Where(r => r.FundableId == fundableId)
                    .Select(r => Create.MFunding_Fundable(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = fundable;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FundableSummaryList> GetFundableSummariesAsync(long? funderId, bool? hasFundsRequired, int? recordCount)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableSummariesAsync), funderId, hasFundsRequired, recordCount);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<Fundable>)ctx.Fundables;

                if (funderId != null)
                {
                    var joinQuery = query.Join(
                        ctx.FunderAccounts,
                        Funder => Funder.FundableReference,
                        FunderAccount => FunderAccount.FundableReference,
                        (Fundable, FunderAccount) => new { Fundable, FunderAccount });

                    query = joinQuery.Where(r => r.FunderAccount.FunderId == funderId.Value).Select(r => r.Fundable);
                }
                if (hasFundsRequired != null)
                {
                    query = hasFundsRequired.Value
                        ? query.Where(r => r.FundsRequiredTotal > 0m)
                        : query.Where(r => r.FundsRequiredTotal == 0m);
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Select(r => Create.MFunding_FundableSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FundableSummaryList
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

        public async Task SetFundsRequiredAsync(long fundableId, decimal? fundsRequiredIncome, decimal? fundsRequiredSalesTax, string fundsRequiredSalesTaxJurisdiction, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(SetFundsRequiredAsync), fundableId, fundsRequiredIncome, fundsRequiredSalesTax, fundsRequiredSalesTaxJurisdiction, unitOfWorkRoot);
            try
            {
                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                using var ctx = CreateQuiltContext();

                var dbFundable = await ctx.Fundables.Where(r => r.FundableId == fundableId).FirstAsync().ConfigureAwait(false);

                var fundsRequiredIncomeDelta = fundsRequiredIncome != null
                    ? fundsRequiredIncome.Value - dbFundable.FundsRequiredIncome
                    : 0m;

                var fundsRequiredSalesTaxDelta = fundsRequiredSalesTax != null
                    ? fundsRequiredSalesTax.Value - dbFundable.FundsRequiredSalesTax
                    : 0m;

                if (fundsRequiredSalesTaxJurisdiction == dbFundable.FundsRequiredSalesTaxJurisdiction)
                {
                    fundsRequiredSalesTaxJurisdiction = null;
                }

                if (fundsRequiredIncomeDelta != 0 || fundsRequiredSalesTaxDelta != 0 || fundsRequiredSalesTaxJurisdiction != null)
                {
                    var fundingTransaction = ctx.CreateFundableTransactionBuilder()
                        .Begin(fundableId, utcNow)
                        .UnitOfWork(unitOfWork)
                        .AddFundsRequiredIncome(fundsRequiredIncomeDelta)
                        .AddFundsRequiredSalesTax(fundsRequiredSalesTaxDelta)
                        .SetFundsRequiredSalesTaxJurisdiction(fundsRequiredSalesTaxJurisdiction)
                        .Create();

                    var referenceValues = ParseReferenceValues.From(dbFundable.FundableReference);
                    var dbLedgerTransaction = ctx.CreateLedgerAccountTransactionBuilder()
                        .Begin($"Funds required for fundable {fundableId} ({referenceValues}).", Locale.GetLocalTimeFromUtc(utcNow), utcNow)
                        .UnitOfWork(unitOfWork)
                        .Debit(LedgerAccountNumbers.AccountReceivable, Math.Max(0m, fundsRequiredIncomeDelta + fundsRequiredSalesTaxDelta))
                        .Debit(LedgerAccountNumbers.AccountPayable, Math.Min(0m, fundsRequiredIncomeDelta + fundsRequiredSalesTaxDelta))
                        .Credit(LedgerAccountNumbers.Income, fundsRequiredIncomeDelta)
                        .Credit(LedgerAccountNumbers.SalesTaxPayable, fundsRequiredSalesTaxDelta)
                        .Create();

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                    var dbFunderAccounts = await ctx.FunderAccounts.Where(r => r.FundableReference == dbFundable.FundableReference).ToListAsync().ConfigureAwait(false);
                    foreach (var dbFunderAccount in dbFunderAccounts)
                    {
                        var fundsTransferred = await TransferFundsToFundableAsync(dbFunderAccount.FunderId, dbFunderAccount.FundableReference, null, unitOfWork);
                        log.Message($"{fundsTransferred:c} funds transferred from funder {dbFunderAccount.FunderId} to {dbFundable.FundableId} ({dbFundable.FundableReference}).");

                        var fundsRefunded = await TransferRefundToFunderAsync(dbFunderAccount.FunderId, dbFunderAccount.FundableReference, null, unitOfWork);
                        log.Message($"{fundsRefunded:c} funds refunded from {dbFundable.FundableId} ({dbFundable.FundableReference}) to funder {dbFunderAccount.FunderId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FundableTransaction> GetFundableTransactionAsync(long fundableTransactionId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableTransactionAsync), fundableTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableTransaction = await ctx.FundableTransactions
                    .Where(r => r.FundableTransactionId == fundableTransactionId)
                    .Select(r => Create.MFunding_FundableTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = fulfillableTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FundableTransactionSummaryList> GetFundableTransactionSummariesAsync(long? fundableId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableTransactionSummariesAsync), fundableId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Fundable)
                {
                    return new MFunding_FundableTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFunding_FundableTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FundableTransaction>)ctx.FundableTransactions;
                if (fundableId != null)
                {
                    query = query.Where(r => r.FundableId == fundableId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFunding_FundableTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FundableTransactionSummaryList
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

        public async Task<MFunding_FundableEventLog> GetFundableEventLogAsync(long fundableEventId)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableEventLogAsync), fundableEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableEventLog = await ctx.FundableEvents
                    .Where(r => r.FundableEventId == fundableEventId)
                    .Select(r => Create.MFunding_FundableEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = fulfillableEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFunding_FundableEventLogSummaryList> GetFundableEventLogSummariesAsync(long? fundableId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(GetFundableEventLogSummariesAsync), fundableId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Fundable)
                {
                    return new MFunding_FundableEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFunding_FundableEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FundableEvent>)ctx.FundableEvents.Include(r => r.FundableTransaction);
                if (fundableId != null)
                {
                    query = query.Where(r => r.FundableTransaction.FundableId == fundableId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    query = query.Where(r => r.FundableTransaction.UnitOfWork == unitOfWork);
                }
                var summaries = await query
                    .Select(r => Create.MFunding_FundableEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFunding_FundableEventLogSummaryList
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

        #endregion

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(ProcessEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = 0;

                var dbFundableEvents = await ctx.FundableEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
                foreach (var dbFundableEvent in dbFundableEvents)
                {
                    count += 1;

                    try
                    {
                        var dbFundableTransaction = dbFundableEvent.FundableTransaction;
                        var dbFundable = dbFundableTransaction.Fundable;

                        var eventData = new MFunding_FundableEvent()
                        {
                            EventTypeCode = dbFundableEvent.EventTypeCode,
                            FundableId = dbFundable.FundableId,
                            FundableReference = dbFundable.FundableReference,
                            FundsRequired = dbFundable.FundsRequiredIncome,
                            FundsReceived = dbFundable.FundsReceived,
                            UnitOfWork = dbFundableTransaction.UnitOfWork
                        };

                        await FundingEventService.HandleFundableEventAsync(eventData).ConfigureAwait(false);

                        dbFundableEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);

                        dbFundableEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                var dbFunderEvents = await ctx.FunderEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
                foreach (var dbFunderEvent in dbFunderEvents)
                {
                    count += 1;

                    try
                    {
                        var dbFunderTransaction = dbFunderEvent.FunderTransaction;
                        var dbFunder = dbFunderTransaction.Fund.Funder;

                        var eventData = new MFunding_FunderEvent()
                        {
                            EventTypeCode = dbFunderEvent.EventTypeCode,
                            FunderId = dbFunder.FunderId,
                            FunderReference = dbFunder.FunderReference,
                            FundsAvailable = dbFunder.FunderAccounts.Sum(r => r.FundsAvailable),
                            UnitOfWork = dbFunderTransaction.UnitOfWork
                        };

                        await FundingEventService.HandleFunderEventAsync(eventData).ConfigureAwait(false);

                        dbFunderEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);

                        dbFunderEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                var result = count;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<int> CancelEventsAsync()
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(CancelEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = await ctx.Database.ExecuteSqlInterpolatedAsync($"update FundableEvent set ProcessingStatusCode = {EventProcessingStatusCodes.Cancelled} where ProcessingStatusCode = {EventProcessingStatusCodes.Pending}").ConfigureAwait(false);

                var result = count;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private async Task<FunderAccount> GetFunderAccount(QuiltContext ctx, long funderId)
        {
            var funderAccount = await ctx.FunderAccounts.Where(r => r.FunderId == funderId).SingleOrDefaultAsync().ConfigureAwait(false);
            if (funderAccount == null)
            {
                funderAccount = new FunderAccount()
                {
                    FunderId = funderId,
                    FundsAvailable = 0
                };
                _ = ctx.FunderAccounts.Add(funderAccount);
            }

            return funderAccount;
        }

        private async Task<decimal> TransferFundsToFundableAsync(long funderId, string fundableReference, decimal? amount, UnitOfWork unitOfWork)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(TransferFundsToFundableAsync), funderId, fundableReference, amount, unitOfWork);
            try
            {
                if (string.IsNullOrEmpty(fundableReference)) throw new ArgumentNullException(nameof(fundableReference));
                if (amount != null && amount < 0) throw new ArgumentException($"Negative amount {amount:c} specified.", nameof(amount));

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbFunderAccount = await ctx.FunderAccounts.FindAsync(funderId, fundableReference);
                var dbFundable = await ctx.Fundables.Where(r => r.FundableReference == fundableReference).FirstOrDefaultAsync();
                if (dbFundable == null)
                {
                    return 0m;
                }

                // Determine maximum amount of funds required and available.
                //
                var maxAmount = Math.Max(dbFundable.FundsRequiredTotal - dbFundable.FundsReceived, 0m);
                maxAmount = Math.Min(maxAmount, dbFunderAccount.FundsAvailable);

                // Determine the amount of funds we're going to transfer.
                //
                decimal fundsToTransfer;
                if (amount != null)
                {
                    if (amount.Value > maxAmount)
                    {
                        throw new ArgumentException($"Specified amount {amount:c} exceeds maximum aount {maxAmount:c}.");
                    }
                    fundsToTransfer = amount.Value;
                }
                else
                {
                    // Amount not specified.  Transfer the maximum amount required and available.
                    //
                    fundsToTransfer = maxAmount;
                }

                if (fundsToTransfer != 0m)
                {
                    var fundableTransaction = ctx.CreateFundableTransactionBuilder()
                      .Begin(dbFundable.FundableId, utcNow)
                      .UnitOfWork(unitOfWork)
                      .AddFundsReceived(fundsToTransfer)
                      .Event(FundingEventTypeCodes.Receipt)
                      .Create();

                    var funderTransaction = ctx.CreateFunderTransactionBuilder()
                      .Begin(dbFunderAccount.FunderId, dbFundable.FundableReference, utcNow)
                      .UnitOfWork(unitOfWork)
                      .AddFundsAvailable(-fundsToTransfer)
                      .Event(FundingEventTypeCodes.Receipt)
                      .Create();

                    var referenceValues = ParseReferenceValues.From(dbFundable.FundableReference);
                    var dbLedgerTransaction = ctx.CreateLedgerAccountTransactionBuilder()
                        .Begin($"Transfer funds to fundable {dbFundable.FundableId} ({referenceValues}).", Locale.GetLocalTimeFromUtc(utcNow), utcNow)
                        .UnitOfWork(unitOfWork)
                        .Debit(LedgerAccountNumbers.FundsSuspense, fundsToTransfer)
                        .Credit(LedgerAccountNumbers.AccountReceivable, fundsToTransfer)
                        .Create();

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                    return fundsToTransfer;
                }
                else
                {
                    // No funds transferred.
                    return 0m;
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private async Task<decimal> TransferRefundToFunderAsync(long funderId, string fundableReference, decimal? amount, UnitOfWork unitOfWork)
        {
            using var log = BeginFunction(nameof(FundingMicroService), nameof(TransferRefundToFunderAsync), funderId, fundableReference, amount, unitOfWork);
            try
            {
                if (string.IsNullOrEmpty(fundableReference)) throw new ArgumentNullException(nameof(fundableReference));
                if (amount != null && amount < 0) throw new ArgumentException($"Negative amount {amount:c} specified.", nameof(amount));

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbFunderAccount = await ctx.FunderAccounts.FindAsync(funderId, fundableReference);
                var dbFundable = await ctx.Fundables.Where(r => r.FundableReference == fundableReference).FirstAsync();

                // Determine maximum amount of funds required.
                //
                var maxAmount = Math.Max(dbFundable.FundsReceived - dbFundable.FundsRequiredTotal, 0m);

                // Determine the amount of funds we're going to transfer.
                //
                decimal fundsToTransfer;
                if (amount != null)
                {
                    if (amount.Value > maxAmount)
                    {
                        throw new ArgumentException($"Specified amount {amount:c} exceeds maximum aount {maxAmount:c}.");
                    }
                    fundsToTransfer = amount.Value;
                }
                else
                {
                    // Amount not specified.  Transfer the maximum amount required.
                    //
                    fundsToTransfer = maxAmount;
                }

                if (fundsToTransfer != 0m)
                {
                    var fundableTransaction = ctx.CreateFundableTransactionBuilder()
                      .Begin(dbFundable.FundableId, utcNow)
                      .UnitOfWork(unitOfWork)
                      .AddFundsReceived(-fundsToTransfer)
                      .Event(FundingEventTypeCodes.Payment)
                      .Create();

                    var funderTransaction = ctx.CreateFunderTransactionBuilder()
                      .Begin(dbFunderAccount.FunderId, dbFundable.FundableReference, utcNow)
                      .UnitOfWork(unitOfWork)
                      .AddFundsRefundable(fundsToTransfer)
                      .Event(FundingEventTypeCodes.Payment)
                      .Create();

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                    return fundsToTransfer;
                }
                else
                {
                    // No funds transferred.
                    return 0m;
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private static class Create
        {
            #region Funder

            public static MFunding_Funder MFunding_Funder(Funder dbFunder)
            {
                return new MFunding_Funder()
                {
                    FunderId = dbFunder.FunderId,
                    FunderReference = dbFunder.FunderReference,
                    UpdateDateTimeUtc = dbFunder.UpdateDateTimeUtc,
                    TotalFundsAvailable = dbFunder.FunderAccounts.Sum(r => r.FundsAvailable),
                    TotalFundsReceived = dbFunder.FunderAccounts.Sum(r => r.FundsReceived),
                    TotalProcessingFee = dbFunder.FunderAccounts.Sum(r => r.ProcessingFee),
                    TotalFundsRefunded = dbFunder.FunderAccounts.Sum(r => r.FundsRefunded),
                    TotalFundsRefundable = dbFunder.FunderAccounts.Sum(r => r.FundsRefundable),
                    Accounts = MFunding_FunderAccounts(dbFunder.FunderAccounts),
                    FunderTransactions = MFunding_FunderTransactions(dbFunder.FunderAccounts.SelectMany(r => r.FunderTransactions))
                };
            }

            public static MFunding_FunderSummary MFunding_FunderSummary(Funder dbFunder)
            {
                return new MFunding_FunderSummary()
                {
                    FunderId = dbFunder.FunderId,
                    FunderReference = dbFunder.FunderReference,
                    TotalFundsAvailable = dbFunder.FunderAccounts.Sum(r => r.FundsAvailable),
                    TotalFundsReceived = dbFunder.FunderAccounts.Sum(r => r.FundsReceived),
                    TotalProcessingFee = dbFunder.FunderAccounts.Sum(r => r.ProcessingFee),
                    TotalFundsRefunded = dbFunder.FunderAccounts.Sum(r => r.FundsRefunded),
                    TotalFundsRefundable = dbFunder.FunderAccounts.Sum(r => r.FundsRefundable)
                };
            }

            public static MFunding_FunderTransaction MFunding_FunderTransaction(FunderTransaction dbFunderTransaction)
            {
                return new MFunding_FunderTransaction()
                {
                    TransactionId = dbFunderTransaction.FunderTransactionId,
                    EntityId = dbFunderTransaction.FunderId,
                    TransactionDateTimeUtc = dbFunderTransaction.TransactionDateTimeUtc,
                    Description = dbFunderTransaction.Description,
                    UnitOfWork = dbFunderTransaction.UnitOfWork,

                    FundableReference = dbFunderTransaction.FundableReference,
                    FundsReceived = dbFunderTransaction.FundsReceived,
                    FundsAvailable = dbFunderTransaction.FundsAvailable,
                    FundsRefunded = dbFunderTransaction.FundsRefunded,
                    FundsRefundable = dbFunderTransaction.FundsRefundable
                };
            }

            public static MFunding_FunderTransactionSummary MFunding_FunderTransactionSummary(FunderTransaction dbFunderTransaction)
            {
                return new MFunding_FunderTransactionSummary()
                {
                    TransactionId = dbFunderTransaction.FunderTransactionId,
                    EntityId = dbFunderTransaction.FunderId,
                    TransactionDateTimeUtc = dbFunderTransaction.TransactionDateTimeUtc,
                    Description = dbFunderTransaction.Description,
                    UnitOfWork = dbFunderTransaction.UnitOfWork,

                    FundableReference = dbFunderTransaction.FundableReference,
                    FundsReceived = dbFunderTransaction.FundsReceived,
                    FundsAvailable = dbFunderTransaction.FundsAvailable,
                    FundsRefunded = dbFunderTransaction.FundsRefunded,
                    FundsRefundable = dbFunderTransaction.FundsRefundable,
                    ProcessingFee = dbFunderTransaction.ProcessingFee,
                };
            }

            public static MFunding_FunderEventLog MFunding_FunderEventLog(FunderEvent dbFunderEvent)
            {
                return new MFunding_FunderEventLog()
                {
                    EventId = dbFunderEvent.FunderEventId,
                    TransactionId = dbFunderEvent.FunderTransactionId,
                    EventTypeCode = dbFunderEvent.EventTypeCode,
                    EventDateTimeUtc = dbFunderEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFunderEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFunderEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFunding_FunderEventLogSummary MFunding_FunderEventLogSummary(FunderEvent dbFunderEvent)
            {
                return new MFunding_FunderEventLogSummary()
                {
                    EventId = dbFunderEvent.FunderEventId,
                    TransactionId = dbFunderEvent.FunderTransactionId,
                    EntityId = dbFunderEvent.FunderTransaction.FunderId,
                    EventTypeCode = dbFunderEvent.EventTypeCode,
                    EventDateTimeUtc = dbFunderEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFunderEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFunderEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbFunderEvent.FunderTransaction.UnitOfWork
                };
            }

            private static MFunding_FunderAccount MFunding_FunderAccount(FunderAccount dbFunderAccount)
            {
                return new MFunding_FunderAccount()
                {
                    FundableReference = dbFunderAccount.FundableReference,
                    FundsReceived = dbFunderAccount.FundsReceived,
                    FundsAvailable = dbFunderAccount.FundsAvailable,
                    FundsRefunded = dbFunderAccount.FundsRefunded,
                    FundsRefundable = dbFunderAccount.FundsRefundable,
                    ProcessingFee = dbFunderAccount.ProcessingFee,
                    UpdateDateTimeUtc = dbFunderAccount.UpdateDateTimeUtc
                };
            }

            private static IList<MFunding_FunderAccount> MFunding_FunderAccounts(IEnumerable<FunderAccount> dbFunderAccounts)
            {
                return dbFunderAccounts?.Select(r => MFunding_FunderAccount(r)).ToList();
            }

            private static IList<MFunding_FunderTransaction> MFunding_FunderTransactions(IEnumerable<FunderTransaction> dbFunderTransactions)
            {
                return dbFunderTransactions?.Select(r => MFunding_FunderTransaction(r)).ToList();
            }

            private static IList<MFunding_FundableTransaction> MFunding_FundableTransactions(IEnumerable<FundableTransaction> dbFundableTransactions)
            {
                return dbFundableTransactions?.Select(r => MFunding_FundableTransaction(r)).ToList();
            }

            #endregion

            #region Fundable

            public static MFunding_Fundable MFunding_Fundable(Fundable dbFundable)
            {
                return new MFunding_Fundable()
                {
                    FundableId = dbFundable.FundableId,
                    FundableReference = dbFundable.FundableReference,
                    FundsRequiredTotal = dbFundable.FundsRequiredTotal,
                    FundsRequiredIncome = dbFundable.FundsRequiredIncome,
                    FundsRequiredSalesTax = dbFundable.FundsRequiredSalesTax,
                    FundsRequiredSalesTaxJurisdiction = dbFundable.FundsRequiredSalesTaxJurisdiction,
                    FundsReceived = dbFundable.FundsReceived,
                    UpdateDateTimeUtc = dbFundable.UpdateDateTimeUtc,
                    FundableTransactions = MFunding_FundableTransactions(dbFundable.FundableTransactions)
                };
            }

            public static MFunding_FundableSummary MFunding_FundableSummary(Fundable dbFundable)
            {
                return new MFunding_FundableSummary()
                {
                    FundableId = dbFundable.FundableId,
                    FundableReference = dbFundable.FundableReference,
                    FundsRequired = dbFundable.FundsRequiredIncome,
                    FundsReceived = dbFundable.FundsReceived
                };
            }

            public static MFunding_FundableTransaction MFunding_FundableTransaction(FundableTransaction dbFundableTransaction)
            {
                return new MFunding_FundableTransaction()
                {
                    TransactionId = dbFundableTransaction.FundableTransactionId,
                    EntityId = dbFundableTransaction.FundableId,
                    TransactionDateTimeUtc = dbFundableTransaction.TransactionDateTimeUtc,
                    Description = dbFundableTransaction.Description,
                    UnitOfWork = dbFundableTransaction.UnitOfWork,

                    FundsRequiredIncome = dbFundableTransaction.FundsRequiredIncome,
                    FundsRequiredSalesTax = dbFundableTransaction.FundsRequiredSalesTax,
                    FundsRequiredSalesTaxJurisdiction = dbFundableTransaction.FundsRequiredSalesTaxJurisdiction,
                    FundsReceived = dbFundableTransaction.FundsReceived
                };
            }

            public static MFunding_FundableTransactionSummary MFunding_FundableTransactionSummary(FundableTransaction dbFundableTransaction)
            {
                return new MFunding_FundableTransactionSummary()
                {
                    TransactionId = dbFundableTransaction.FundableTransactionId,
                    EntityId = dbFundableTransaction.FundableId,
                    TransactionDateTimeUtc = dbFundableTransaction.TransactionDateTimeUtc,
                    Description = dbFundableTransaction.Description,
                    UnitOfWork = dbFundableTransaction.UnitOfWork,

                    FundsRequiredIncome = dbFundableTransaction.FundsRequiredIncome,
                    FundsRequiredSalesTax = dbFundableTransaction.FundsRequiredSalesTax,
                    FundsRequiredSalesTaxJurisdiction = dbFundableTransaction.FundsRequiredSalesTaxJurisdiction,
                    FundsReceived = dbFundableTransaction.FundsReceived
                };
            }

            public static MFunding_FundableEventLog MFunding_FundableEventLog(FundableEvent dbFundableEvent)
            {
                return new MFunding_FundableEventLog()
                {
                    EventId = dbFundableEvent.FundableEventId,
                    TransactionId = dbFundableEvent.FundableTransactionId,
                    EventTypeCode = dbFundableEvent.EventTypeCode,
                    EventDateTimeUtc = dbFundableEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFundableEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFundableEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFunding_FundableEventLogSummary MFunding_FundableEventLogSummary(FundableEvent dbFundableEvent)
            {
                return new MFunding_FundableEventLogSummary()
                {
                    EventId = dbFundableEvent.FundableEventId,
                    TransactionId = dbFundableEvent.FundableTransactionId,
                    EntityId = dbFundableEvent.FundableTransaction.FundableId,
                    EventTypeCode = dbFundableEvent.EventTypeCode,
                    EventDateTimeUtc = dbFundableEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFundableEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFundableEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbFundableEvent.FundableTransaction.UnitOfWork
                };
            }


            #endregion
        }
    }
}
