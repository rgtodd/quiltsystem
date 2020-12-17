//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using RichTodd.QuiltSystem.Database;
using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

using Square;
using Square.Models;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class SquareMicroService : MicroService, ISquareMicroService
    {
        private ILedgerMicroService LedgerMicroService { get; }
        private ISquareEventMicroService SquareEventService { get; }

        private JsonSerializerSettings JsonSerializerSettings { get; }

        public SquareMicroService(
            IApplicationLocale locale,
            ILogger<SquareMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            ILedgerMicroService ledgerMicroService,
            ISquareEventMicroService squareEventService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            SquareEventService = squareEventService ?? throw new ArgumentNullException(nameof(squareEventService));
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));

            JsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }

        public async Task<MSquare_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MSquare_Dashboard()
                {
                    TotalCustomers = await ctx.SquareCustomers.CountAsync(),
                    TotalPayments = await ctx.SquarePayments.CountAsync()
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

        #region Customer

        public async Task<long> AllocateSquareCustomerAsync(string squareCustomerReference)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(AllocateSquareCustomerAsync), squareCustomerReference);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbSquareCustomer = await ctx.SquareCustomers.Where(r => r.SquareCustomerReference == squareCustomerReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbSquareCustomer == null)
                {
                    dbSquareCustomer = new SquareCustomer()
                    {
                        SquareCustomerReference = squareCustomerReference,
                        UpdateDateTimeUtc = GetUtcNow()
                    };
                    _ = ctx.SquareCustomers.Add(dbSquareCustomer);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbSquareCustomer.SquareCustomerId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupSquareCustomerIdAsync(string squareCustomerReference)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(LookupSquareCustomerIdAsync), squareCustomerReference);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var result = await ctx.SquareCustomers.Where(r => r.SquareCustomerReference == squareCustomerReference).Select(r => (long?)r.SquareCustomerId).FirstOrDefaultAsync().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_Customer> GetSquareCustomerAsync(long squareCustomerId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetSquareCustomerAsync), squareCustomerId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbCustomer = await ctx.SquareCustomers
                    .Include(r => r.SquarePayments)
                        .ThenInclude(r => r.SquareRefunds)
                    .Where(r => r.SquareCustomerId == squareCustomerId)
                    .FirstAsync().ConfigureAwait(false);

                var result = Create.MSquare_Customer(dbCustomer);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_CustomerSummaryList> GetSquareCustomerSummariesAsync(long? squareCustomerId, int? recordCount)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetSquareCustomerSummariesAsync), squareCustomerId, recordCount);
            try
            {
                var ctx = CreateQuiltContext();

                IQueryable<SquareCustomer> query = ctx.SquareCustomers;

                if (squareCustomerId != null)
                {
                    query = query.Where(r => r.SquareCustomerId == squareCustomerId.Value);
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Select(r => Create.MSquare_CustomerSummary(r))
                    .ToListAsync();

                var result = new MSquare_CustomerSummaryList()
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

        #region Payment

        public async Task<long> AllocateSquarePaymentAsync(string squarePaymentReference, long squareCustomerId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(AllocateSquarePaymentAsync), squarePaymentReference, squareCustomerId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbSquarePayment = await ctx.SquarePayments.Where(r => r.SquarePaymentReference == squarePaymentReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbSquarePayment == null)
                {
                    dbSquarePayment = new SquarePayment()
                    {
                        SquarePaymentReference = squarePaymentReference,
                        SquareCustomerId = squareCustomerId,
                        PaymentAmount = 0,
                        ProcessingFeeAmount = 0,
                        UpdateDateTimeUtc = utcNow
                    };
                    _ = ctx.SquarePayments.Add(dbSquarePayment);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbSquarePayment.SquarePaymentId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupSquarePaymentIdAsync(string squarePaymentReference)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(LookupSquareCustomerIdAsync), squarePaymentReference);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var result = await ctx.SquarePayments.Where(r => r.SquarePaymentReference == squarePaymentReference).Select(r => (long?)r.SquarePaymentId).FirstOrDefaultAsync().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_Payment> GetSquarePaymentAsync(long squarePaymentId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetSquarePaymentAsync), squarePaymentId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbSquarePayment = await ctx.SquarePayments
                    .Include(r => r.SquareCustomer)
                    .Include(r => r.SquareWebPaymentRequests)
                    .Include(r => r.SquarePaymentTransactions)
                    .Include(r => r.SquareRefunds)
                        .ThenInclude(r => r.SquareRefundTransactions)
                    .Where(r => r.SquarePaymentId == squarePaymentId)
                    .FirstAsync().ConfigureAwait(false);

                var mPayment = Create.MSquare_Payment(dbSquarePayment);

                var result = mPayment;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_Payment> GetSquarePaymentByRefundAsync(long squareRefundId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetSquarePaymentByRefundAsync), squareRefundId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbSquareRefund = await ctx.SquareRefunds.FindAsync(squareRefundId).ConfigureAwait(false);

                var dbSquarePayment = await ctx.SquarePayments
                    .Include(r => r.SquareCustomer)
                    .Include(r => r.SquareWebPaymentRequests)
                    .Include(r => r.SquarePaymentTransactions)
                    .Include(r => r.SquareRefunds)
                        .ThenInclude(r => r.SquareRefundTransactions)
                    .Where(r => r.SquarePaymentId == dbSquareRefund.SquarePaymentId)
                    .FirstAsync().ConfigureAwait(false);

                var mPayment = Create.MSquare_Payment(dbSquarePayment);

                var result = mPayment;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_PaymentSummaryList> GetSquarePaymentSummariesAsync(long? squareCustomerId, DateTime? paymentDateUtc, int? recordCount)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetSquarePaymentSummariesAsync), squareCustomerId, paymentDateUtc, recordCount);
            try
            {
                var ctx = CreateQuiltContext();

                IQueryable<SquarePayment> query = ctx.SquarePayments.Include(r => r.SquareRefunds);
                if (squareCustomerId != null)
                {
                    query = query.Where(r => r.SquareCustomerId == squareCustomerId.Value);
                }
                if (paymentDateUtc != null)
                {
                    var from = paymentDateUtc.Value;
                    var to = from.AddDays(1);

                    query = query.Where(r => r.UpdateDateTimeUtc >= from && r.UpdateDateTimeUtc < to);
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Select(r => Create.MSquare_PaymentSummary(r))
                    .ToListAsync();

                var result = new MSquare_PaymentSummaryList()
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

        public async Task<MSquare_PaymentTransaction> GetPaymentTransactionAsync(long squarePaymentTransactionId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetPaymentTransactionAsync), squarePaymentTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var paymentTransaction = await ctx.SquarePaymentTransactions
                    .Where(r => r.SquarePaymentTransactionId == squarePaymentTransactionId)
                    .Select(r => Create.MSquare_PaymentTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = paymentTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_PaymentTransactionSummaryList> GetPaymentTransactionSummariesAsync(long? squarePaymentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetPaymentTransactionSummariesAsync), squarePaymentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.SquarePayment)
                {
                    return new MSquare_PaymentTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MSquare_PaymentTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<SquarePaymentTransaction>)ctx.SquarePaymentTransactions;
                if (squarePaymentId != null)
                {
                    query = query.Where(r => r.SquarePaymentId == squarePaymentId);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MSquare_PaymentTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MSquare_PaymentTransactionSummaryList
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

        public async Task<MSquare_PaymentEventLog> GetPaymentEventLogAsync(long squarePaymentEventId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetPaymentEventLogAsync), squarePaymentEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var paymentEventLog = await ctx.SquarePaymentEvents
                    .Where(r => r.SquarePaymentEventId == squarePaymentEventId)
                    .Select(r => Create.MSquare_PaymentEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = paymentEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_PaymentEventLogSummaryList> GetPaymentEventLogSummariesAsync(long? squarePaymentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetPaymentEventLogSummariesAsync), squarePaymentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.SquarePayment)
                {
                    return new MSquare_PaymentEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MSquare_PaymentEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<SquarePaymentEvent>)ctx.SquarePaymentEvents
                    .Include(r => r.SquarePaymentTransaction);
                if (squarePaymentId != null)
                {
                    query = query.Where(r => r.SquarePaymentTransaction.SquarePaymentId == squarePaymentId);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.SquarePaymentTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MSquare_PaymentEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MSquare_PaymentEventLogSummaryList
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

        #region Refund

        public async Task<MSquare_RefundTransaction> GetRefundTransactionAsync(long squareRefundTransactionId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetRefundTransactionAsync), squareRefundTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var refundTransaction = await ctx.SquareRefundTransactions
                    .Where(r => r.SquareRefundTransactionId == squareRefundTransactionId)
                    .Select(r => Create.MSquare_RefundTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = refundTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_RefundTransactionSummaryList> GetRefundTransactionSummariesAsync(long? squareRefundId, long? squarePaymentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetRefundTransactionSummariesAsync), squareRefundId, squarePaymentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.SquareRefund)
                {
                    return new MSquare_RefundTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MSquare_RefundTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<SquareRefundTransaction>)ctx.SquareRefundTransactions;
                if (squareRefundId != null)
                {
                    query = query.Where(r => r.SquareRefundId == squareRefundId.Value);
                }
                if (squarePaymentId != null)
                {
                    query = query.Where(r => r.SquareRefund.SquarePaymentId == squarePaymentId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MSquare_RefundTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MSquare_RefundTransactionSummaryList
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

        public async Task<MSquare_RefundEventLog> GetRefundEventLogAsync(long squareRefundEventId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetRefundEventLogAsync), squareRefundEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var refundEventLog = await ctx.SquareRefundEvents
                    .Where(r => r.SquareRefundEventId == squareRefundEventId)
                    .Select(r => Create.MSquare_RefundEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = refundEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_RefundEventLogSummaryList> GetRefundEventLogSummariesAsync(long? squareRefundId, long? squarePaymentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(GetRefundEventLogSummariesAsync), squareRefundId, squarePaymentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.SquareRefund)
                {
                    return new MSquare_RefundEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MSquare_RefundEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<SquareRefundEvent>)ctx.SquareRefundEvents
                    .Include(r => r.SquareRefundTransaction);
                if (squareRefundId != null)
                {
                    query = query.Where(r => r.SquareRefundTransaction.SquareRefundId == squareRefundId.Value);
                }
                if (squarePaymentId != null)
                {
                    query = query.Where(r => r.SquareRefundTransaction.SquareRefund.SquarePaymentId == squarePaymentId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.SquareRefundTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MSquare_RefundEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MSquare_RefundEventLogSummaryList
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

        #region Payment Transaction

        public async Task<long> CreateSquareWebPaymentRequestAsync(long squarePaymentId, decimal paymentAmount, string nonce)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(CreateSquareWebPaymentRequestAsync), squarePaymentId, paymentAmount, nonce);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbSquarePayment = await ctx.SquarePayments.FindAsync(squarePaymentId).ConfigureAwait(false);

                var dbSquareWebPaymentRequests = dbSquarePayment.SquareWebPaymentRequests.Where(r => r.ProcessingStatusCode != SquareProcessingStatusCodes.Cancelled).ToList();

                // There should never be more than one transaction.
                //
                if (dbSquareWebPaymentRequests.Count() > 1)
                {
                    throw new InvalidOperationException($"More than one valid payment transaction exists for payment {squarePaymentId}.");
                }

                var dbSquareWebPaymentRequest = dbSquareWebPaymentRequests.FirstOrDefault();
                if (dbSquareWebPaymentRequest != null)
                {
                    switch (dbSquareWebPaymentRequest.ProcessingStatusCode)
                    {
                        // Transaction has not be submitted.  Modify it.
                        //
                        case SquareProcessingStatusCodes.Pending:
                            dbSquareWebPaymentRequest.Nonce = nonce;
                            dbSquareWebPaymentRequest.TransactionAmount = paymentAmount;
                            break;

                        // Previous payment attempt was rejected.  Update request and move it back to open status.
                        //
                        case SquareProcessingStatusCodes.Rejected:
                            dbSquareWebPaymentRequest.ProcessingStatusCode = SquareProcessingStatusCodes.Pending;
                            dbSquareWebPaymentRequest.Nonce = nonce;
                            dbSquareWebPaymentRequest.TransactionAmount = paymentAmount;
                            break;

                        // Transactions in the following statuses cannot be modified.
                        //
                        case SquareProcessingStatusCodes.Processing:
                        case SquareProcessingStatusCodes.Processed:
                        case SquareProcessingStatusCodes.Exception:
                            throw new InvalidOperationException($"Payment {squarePaymentId} is in status {dbSquareWebPaymentRequest.ProcessingStatusCode} and cannot be updated.");

                        default:
                            throw new InvalidOperationException($"Payment {squarePaymentId} is in unknown status {dbSquareWebPaymentRequest.ProcessingStatusCode}.");
                    }
                }
                else
                {
                    // Create the payment transaction.
                    //
                    dbSquareWebPaymentRequest = new SquareWebPaymentRequest()
                    {
                        WebRequestTypeCode = SquarePaymentWebRequestTypeCodes.Payment,
                        ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                        ProcessingStatusDateTimeUtc = utcNow,
                        IndempotencyKey = Guid.NewGuid().ToString("D"),
                        Nonce = nonce,
                        Autocomplete = true,
                        TransactionAmount = paymentAmount,
                        RequestJson = "{}",
                        UpdateDateTimeUtc = utcNow
                    };
                    dbSquarePayment.SquareWebPaymentRequests.Add(dbSquareWebPaymentRequest);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var squareWebPaymentRequestId = dbSquareWebPaymentRequest.SquareWebPaymentRequestId;

                var result = squareWebPaymentRequestId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MSquare_ProcessWebPaymentRequestResponse> ProcessWebPaymentRequestAsync(long squareWebPaymentRequestId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(ProcessWebPaymentRequestAsync), squareWebPaymentRequestId);
            try
            {
                var utcNow = GetUtcNow();

                var unitOfWork = CreateUnitOfWork.SquarePayment(squareWebPaymentRequestId);

                var result = await TransmitSquareWebPaymentRequest(squareWebPaymentRequestId).ConfigureAwait(false);

                if (result.Errors == null)
                {
                    using var ctx = CreateQuiltContext();

                    var squarePayloadIds = await ctx.SquarePayloads
                        .Where(r => r.SquareWebPaymentRequestId == squareWebPaymentRequestId && r.ProcessingStatusCode == SquareProcessingStatusCodes.Pending)
                        .Select(r => r.SquarePayloadId)
                        .ToListAsync().ConfigureAwait(false);

                    foreach (var squarePayloadId in squarePayloadIds)
                    {
                        _ = await ProcessSquarePayloadAsync(squarePayloadId, unitOfWork);
                    }
                }

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

        #region Webhook

        public async Task<long> CreateWebhookPayloadAsync(string payload)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(CreateWebhookPayloadAsync), payload);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbSquarePayload = new SquarePayload()
                {
                    PayloadTypeCode = SquarePayloadTypeCodes.Webhook,
                    PayloadJson = payload,
                    ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                    ProcessingStatusDateTimeUtc = utcNow,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.SquarePayloads.Add(dbSquarePayload);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbSquarePayload.SquarePayloadId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ProcessWebhookPayloadAsync(long squarePayloadId)
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(ProcessWebhookPayloadAsync), squarePayloadId);
            try
            {
                var unitOfWork = CreateUnitOfWork.SquareWebhook(squarePayloadId);

                _ = await ProcessSquarePayloadAsync(squarePayloadId, unitOfWork);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Event

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(ProcessEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = 0;

                var dbSquarePaymentEvents = await ctx.SquarePaymentEvents
                    .Include(r => r.SquarePaymentTransaction)
                        .ThenInclude(r => r.SquarePayment)
                    .Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var dbSquarePaymentEvent in dbSquarePaymentEvents)
                {
                    count += 1;

                    try
                    {
                        var eventTypeCode = dbSquarePaymentEvent.EventTypeCode;
                        var dbSquarePaymentTransaction = dbSquarePaymentEvent.SquarePaymentTransaction;
                        var unitOfWork = dbSquarePaymentTransaction.UnitOfWork;
                        long? squarePaymentTransactionId = dbSquarePaymentTransaction.SquarePaymentTransactionId;
                        long? squareRefundTransactionId = null;

                        var dbSquarePayment = dbSquarePaymentTransaction.SquarePayment;
                        await RaiseEventAsync(dbSquarePayment, eventTypeCode, unitOfWork, squarePaymentTransactionId, squareRefundTransactionId).ConfigureAwait(false);

                        dbSquarePaymentEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);

                        dbSquarePaymentEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                var dbSquareRefundEvents = await ctx.SquareRefundEvents
                    .Include(r => r.SquareRefundTransaction)
                        .ThenInclude(r => r.SquareRefund)
                            .ThenInclude(r => r.SquarePayment)
                    .Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending)
                    .ToListAsync().ConfigureAwait(false);

                foreach (var dbSquareRefundEvent in dbSquareRefundEvents)
                {
                    count += 1;

                    try
                    {
                        var eventTypeCode = dbSquareRefundEvent.EventTypeCode;
                        var dbSquareRefundTransaction = dbSquareRefundEvent.SquareRefundTransaction;
                        var unitOfWork = dbSquareRefundTransaction.UnitOfWork;
                        long? squarePaymentTransactionId = null;
                        long? squareRefundTransactionId = dbSquareRefundTransaction.SquareRefundTransactionId;

                        var dbSquarePayment = dbSquareRefundTransaction.SquareRefund.SquarePayment;
                        await RaiseEventAsync(dbSquarePayment, eventTypeCode, unitOfWork, squarePaymentTransactionId, squareRefundTransactionId).ConfigureAwait(false);

                        dbSquareRefundEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);

                        dbSquareRefundEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
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

        private async Task RaiseEventAsync(SquarePayment dbSquarePayment, string eventTypeCode, string unitOfWork, long? squarePaymentTransactionId, long? squareRefundTransactionId)
        {
            var dbSquareCustomer = dbSquarePayment.SquareCustomer;

            // Compute total amounts based on payment and any subsequent refunds.
            //
            var paymentAmount = dbSquarePayment.PaymentAmount;
            var processingFeeAmount = dbSquarePayment.ProcessingFeeAmount;
            var refundAmount = 0m;
            foreach (var dbSquareRefund in dbSquarePayment.SquareRefunds)
            {
                refundAmount += dbSquareRefund.RefundAmount;
                processingFeeAmount += dbSquareRefund.ProcessingFeeAmount;
            }

            var netPaymentAmount = paymentAmount - refundAmount;

            var eventData = new MSquare_Event()
            {
                EventTypeCode = eventTypeCode,
                UnitOfWork = unitOfWork,

                SquarePaymentTransactionId = squarePaymentTransactionId,
                SquareRefundTransactionId = squareRefundTransactionId,

                SquareCustomerId = dbSquareCustomer.SquareCustomerId,
                SquareCustomerReference = dbSquareCustomer.SquareCustomerReference,

                SquarePaymentId = dbSquarePayment.SquarePaymentId,
                SquarePaymentReference = dbSquarePayment.SquarePaymentReference,
                PaymentAmount = paymentAmount,
                RefundAmount = refundAmount,
                ProcessingFeeAmount = processingFeeAmount,
                NetPaymentAmount = netPaymentAmount
            };

            await SquareEventService.HandleEventAsync(eventData).ConfigureAwait(false);
        }

        public async Task<int> CancelEventsAsync()
        {
            using var log = BeginFunction(nameof(SquareMicroService), nameof(CancelEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = await ctx.Database.ExecuteSqlInterpolatedAsync($"update SquarePaymentEvent set ProcessingStatusCode = {EventProcessingStatusCodes.Cancelled} where ProcessingStatusCode = {EventProcessingStatusCodes.Pending}").ConfigureAwait(false);

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

        #endregion

        private async Task<MSquare_ProcessWebPaymentRequestResponse> TransmitSquareWebPaymentRequest(long squareWebPaymentRequestId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            // Find request and ensure it is eligible to be transmitted.
            //
            var dbSquareWebPaymentRequest = await ctx.SquareWebPaymentRequests.Where(r => r.SquareWebPaymentRequestId == squareWebPaymentRequestId).SingleAsync().ConfigureAwait(false);
            if (dbSquareWebPaymentRequest.ProcessingStatusCode != SquareProcessingStatusCodes.Pending)
            {
                throw new InvalidOperationException($"Cannot transmit request {dbSquareWebPaymentRequest} in status {dbSquareWebPaymentRequest.ProcessingStatusCode}.");
            }

            // Indicate we're transmitting the web request.
            //
            dbSquareWebPaymentRequest.ProcessingStatusCode = SquareProcessingStatusCodes.Transmitting;
            dbSquareWebPaymentRequest.ProcessingStatusDateTimeUtc = utcNow;
            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            // Transmit the request
            //
            MSquare_ProcessWebPaymentRequestResponse result;
            switch (dbSquareWebPaymentRequest.WebRequestTypeCode)
            {
                case SquarePaymentWebRequestTypeCodes.Payment:
                    {
                        result = await TransmitSquareWebPaymentRequest(dbSquareWebPaymentRequest, utcNow).ConfigureAwait(false);
                    }
                    break;

                //case SquarePaymentWebRequestTypeCodes.Refund:
                //    {
                //        result = await TransmitSquareRefundTransaction(dbSquareWebPaymentRequest, utcNow).ConfigureAwait(false);
                //    }
                //    break;

                default:
                    throw new InvalidOperationException($"Unknown web request type code {dbSquareWebPaymentRequest.WebRequestTypeCode}");
            }

            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

            return result;
        }

        private async Task<MSquare_ProcessWebPaymentRequestResponse> TransmitSquareWebPaymentRequest(SquareWebPaymentRequest dbSquareWebPaymentRequest, DateTime utcNow)
        {
            if (dbSquareWebPaymentRequest.WebRequestTypeCode != SquarePaymentWebRequestTypeCodes.Payment)
            {
                throw new InvalidOperationException($"Invalid transaction type {dbSquareWebPaymentRequest.WebRequestTypeCode}");
            }

            var referenceId = CreateReferenceId(dbSquareWebPaymentRequest.SquarePaymentId);

            //var dbSquareWebPayment = dbSquareWebPaymentRequest.SquareWebPayment;

            var sPaymentAmount = new Money.Builder()
               .Amount((long)(dbSquareWebPaymentRequest.TransactionAmount * 100))
               .Currency("USD")
               .Build();

            var sPaymentRequest = new CreatePaymentRequest.Builder(dbSquareWebPaymentRequest.Nonce, dbSquareWebPaymentRequest.IndempotencyKey, sPaymentAmount)
                .Autocomplete(dbSquareWebPaymentRequest.Autocomplete)
                .ReferenceId(referenceId)
                .Note($"Payment {dbSquareWebPaymentRequest.TransactionAmount} for {referenceId}")
                .Build();

            var sRequestJson = JsonConvert.SerializeObject(sPaymentRequest, JsonSerializerSettings);
            //log.LogMessage($"Request = {sRequestJson}");

            var accessToken = "*SECRET*";

            var sClient = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(accessToken)
                .Build();

            var sResult = await sClient.PaymentsApi.CreatePaymentAsync(sPaymentRequest).ConfigureAwait(false);

            var sResultJson = JsonConvert.SerializeObject(sResult, JsonSerializerSettings);
            //log.LogMessage($"Result = {sResultJson}");

            dbSquareWebPaymentRequest.RequestJson = sRequestJson;

            IList<MSquare_ProcessWebPaymentRequestResponseError> errors = null;
            if (sResult.Errors == null)
            {
                dbSquareWebPaymentRequest.ProcessingStatusCode = SquareProcessingStatusCodes.Transmitted;
                dbSquareWebPaymentRequest.ProcessingStatusDateTimeUtc = utcNow;

                var dbSquarePayload = new SquarePayload()
                {
                    PayloadTypeCode = SquarePayloadTypeCodes.Payment,
                    PayloadJson = sResultJson,
                    ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                    ProcessingStatusDateTimeUtc = utcNow,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                dbSquareWebPaymentRequest.SquarePayloads.Add(dbSquarePayload);
            }
            else
            {
                errors = CreateErrors(sResult.Errors);
                dbSquareWebPaymentRequest.ProcessingStatusCode = errors.All(r => r.Category == "PAYMENT_METHOD_ERROR")
                    ? SquareProcessingStatusCodes.Rejected
                    : SquareProcessingStatusCodes.Exception;
                dbSquareWebPaymentRequest.ProcessingStatusDateTimeUtc = utcNow;
            }

            return new MSquare_ProcessWebPaymentRequestResponse()
            {
                SquareWebPaymentRequestId = dbSquareWebPaymentRequest.SquareWebPaymentRequestId,
                Errors = errors
            };
        }

        private decimal GetPaymentProcessingFeeAmount(JsonElement jsonPayment)
        {
            JsonElement? jsonNullableProcessingFee;
            try
            {
                jsonNullableProcessingFee = jsonPayment.GetProperty("processing_fee");
            }
            catch (KeyNotFoundException)
            {
                jsonNullableProcessingFee = null;
            }

            if (jsonNullableProcessingFee == null)
            {
                return 0m;
            }

            var jsonProcessingFee = jsonNullableProcessingFee.Value;
            if (jsonProcessingFee.ValueKind == JsonValueKind.Null)
            {
                return 0m;
            }

            if (jsonProcessingFee.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException($"Unexpected processing fee element {jsonProcessingFee.ValueKind}");
            }

            var processingFeeAmount = 0m;

            var arraySize = jsonProcessingFee.GetArrayLength();
            for (int idx = 0; idx < arraySize; ++idx)
            {
                var jsonProcessingFeeElement = jsonProcessingFee[idx];
                var amount = ConvertSquareCurrency(jsonProcessingFeeElement.GetProperty("amount_money").GetProperty("amount").GetInt32());
                processingFeeAmount += amount;
            }

            return processingFeeAmount;
        }

        private async Task<string> ProcessSquarePayloadAsync(long squarePayloadId, UnitOfWork unitOfWork)
        {
            var payloadDataType = await ProcessSquarePayloadAsync(squarePayloadId);

            switch (payloadDataType)
            {
                case SquarePayloadDataTypeCodes.Payment:
                    await ProcessSquarePaymentPayloadAsync(squarePayloadId, unitOfWork);
                    break;

                case SquarePayloadDataTypeCodes.Refund:
                    await ProcessSquareRefundPayloadAsync(squarePayloadId, unitOfWork);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown payload data type {payloadDataType}.");
            }

            return payloadDataType;
        }

        private async Task<string> ProcessSquarePayloadAsync(long squarePayloadId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquarePayload = await ctx.SquarePayloads.FindAsync(squarePayloadId).ConfigureAwait(false);

            if (dbSquarePayload.ProcessingStatusCode != SquareProcessingStatusCodes.Pending)
            {
                throw new InvalidOperationException($"Square payload {squarePayloadId} in {dbSquarePayload.ProcessingStatusCode} status.");
            }

            try
            {
                switch (dbSquarePayload.PayloadTypeCode)
                {
                    case SquarePayloadTypeCodes.Payment:
                        {
                            using var jsonPaymentDocument = JsonDocument.Parse(dbSquarePayload.PayloadJson);
                            var jsonPayment = jsonPaymentDocument.RootElement.GetProperty("payment");

                            var squarePaymentRecordId = jsonPayment.GetProperty("id").GetString();
                            var versionNumber = 1;

                            var dbSquarePaymentPayload = new SquarePaymentPayload()
                            {
                                SquarePayloadId = squarePayloadId,
                                SquarePaymentRecordId = squarePaymentRecordId,
                                VersionNumber = versionNumber,
                                PaymentPayloadJson = jsonPayment.ToString(),
                                ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                                ProcessingStatusDateTimeUtc = utcNow,
                                CreateDateTimeUtc = utcNow,
                                UpdateDateTimeUtc = utcNow
                            };
                            _ = ctx.SquarePaymentPayloads.Add(dbSquarePaymentPayload);

                            dbSquarePayload.ProcessingStatusCode = SquareProcessingStatusCodes.Processed;
                            dbSquarePayload.ProcessingStatusDateTimeUtc = utcNow;

                            _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                            return SquarePayloadDataTypeCodes.Payment;
                        }

                    case SquarePayloadTypeCodes.Webhook:
                        {
                            using var jsonWebhookDocument = JsonDocument.Parse(dbSquarePayload.PayloadJson);

                            var jsonData = jsonWebhookDocument.RootElement.GetProperty("data");

                            var dataType = jsonData.GetProperty("type").ToString();
                            switch (dataType)
                            {
                                case "payment":
                                    {
                                        var jsonPayment = jsonData.GetProperty("object").GetProperty("payment");

                                        var squarePaymentRecordId = jsonPayment.GetProperty("id").GetString();
                                        var versionNumber = jsonPayment.GetProperty("version").GetInt32();

                                        var dbSquarePaymentPayload = new SquarePaymentPayload()
                                        {
                                            SquarePayloadId = squarePayloadId,
                                            SquarePaymentRecordId = squarePaymentRecordId,
                                            VersionNumber = versionNumber,
                                            PaymentPayloadJson = jsonPayment.ToString(),
                                            ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                                            ProcessingStatusDateTimeUtc = utcNow,
                                            CreateDateTimeUtc = utcNow,
                                            UpdateDateTimeUtc = utcNow
                                        };
                                        _ = ctx.SquarePaymentPayloads.Add(dbSquarePaymentPayload);

                                        dbSquarePayload.ProcessingStatusCode = SquareProcessingStatusCodes.Processed;
                                        dbSquarePayload.ProcessingStatusDateTimeUtc = utcNow;

                                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                                        return SquarePayloadDataTypeCodes.Payment;
                                    }

                                case "refund":
                                    {
                                        var jsonRefund = jsonData.GetProperty("object").GetProperty("refund");

                                        var squareRefundRecordId = jsonRefund.GetProperty("id").GetString();
                                        var versionNumber = jsonRefund.GetProperty("version").GetInt32();

                                        var squarePaymentRecordId = jsonRefund.GetProperty("payment_id").GetString();

                                        var dbSquareRefundPayload = new SquareRefundPayload()
                                        {
                                            SquarePayloadId = squarePayloadId,
                                            SquareRefundRecordId = squareRefundRecordId,
                                            VersionNumber = versionNumber,
                                            SquarePaymentRecordId = squarePaymentRecordId,
                                            RefundPayloadJson = jsonRefund.ToString(),
                                            ProcessingStatusCode = SquareProcessingStatusCodes.Pending,
                                            ProcessingStatusDateTimeUtc = utcNow,
                                            CreateDateTimeUtc = utcNow,
                                            UpdateDateTimeUtc = utcNow
                                        };
                                        _ = ctx.SquareRefundPayloads.Add(dbSquareRefundPayload);

                                        dbSquarePayload.ProcessingStatusCode = SquareProcessingStatusCodes.Processed;
                                        dbSquarePayload.ProcessingStatusDateTimeUtc = utcNow;

                                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                                        return SquarePayloadDataTypeCodes.Refund;
                                    }

                                default:
                                    throw new InvalidOperationException($"Unkown webhook data type {dataType}.");
                            }
                        }

                    default:
                        throw new InvalidOperationException($"Unknown payload type code {dbSquarePayload.PayloadTypeCode}.");
                }
            }
            catch (Exception)
            {
                //log.LogException(ex);

                await MovePayloadToExceptionStatus(squarePayloadId);

                throw;
            }

        }

        private async Task MovePayloadToExceptionStatus(long squarePayloadId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquarePayload = await ctx.SquarePayloads.FindAsync(squarePayloadId).ConfigureAwait(false);
            dbSquarePayload.ProcessingStatusCode = SquareProcessingStatusCodes.Exception;
            dbSquarePayload.ProcessingStatusDateTimeUtc = utcNow;

            _ = await ctx.SaveChangesAsync();
        }

        private async Task ProcessSquarePaymentPayloadAsync(long squarePayloadId, UnitOfWork unitOfWork)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquarePaymentPayload = await ctx.SquarePaymentPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);

            if (dbSquarePaymentPayload.ProcessingStatusCode != SquareProcessingStatusCodes.Pending)
            {
                throw new InvalidOperationException($"Square payment payload {squarePayloadId} in {dbSquarePaymentPayload.ProcessingStatusCode} status.");
            }

            try
            {
                using var jsonPaymentDocument = JsonDocument.Parse(dbSquarePaymentPayload.PaymentPayloadJson);
                var jsonPayment = jsonPaymentDocument.RootElement;

                var referenceId = jsonPayment.GetProperty("reference_id").ToString();
                var squarePaymentId = ParseReferenceId(referenceId);

                var dbSquarePayment = await ctx.SquarePayments
                    .Include(r => r.SquarePaymentTransactions)
                    .Where(r => r.SquarePaymentId == squarePaymentId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                if (dbSquarePayment == null)
                {
                    throw new InvalidOperationException($"Payment {squarePaymentId} not found.  Reference = {referenceId}.");
                }

                bool ignorePayload;
                if (dbSquarePayment.SquarePaymentRecordId != null)
                {
                    if (dbSquarePayment.SquarePaymentRecordId != dbSquarePaymentPayload.SquarePaymentRecordId)
                    {
                        throw new InvalidOperationException($"Square payment record ID mismatch.  Expected = {dbSquarePaymentPayload.SquarePaymentRecordId}, actual = {dbSquarePayment.SquarePaymentRecordId}.");
                    }
                    if (dbSquarePayment.VersionNumber == null)
                    {
                        throw new InvalidOperationException($"Square payment version number not set.");
                    }

                    ignorePayload = dbSquarePayment.VersionNumber >= dbSquarePaymentPayload.VersionNumber;
                }
                else
                {
                    ignorePayload = false;
                }

                if (!ignorePayload)
                {
                    var paymentAmount = ConvertSquareCurrency(jsonPayment.GetProperty("amount_money").GetProperty("amount").GetInt32());
                    var processingFeeAmount = GetPaymentProcessingFeeAmount(jsonPayment);

                    var paymentAmountDelta = paymentAmount - dbSquarePayment.PaymentAmount;
                    var processingFeeDelta = processingFeeAmount - dbSquarePayment.ProcessingFeeAmount;

                    if (paymentAmountDelta != 0 || processingFeeDelta != 0)
                    {
                        dbSquarePayment.SquarePaymentRecordId = dbSquarePaymentPayload.SquarePaymentRecordId;
                        dbSquarePayment.VersionNumber = dbSquarePaymentPayload.VersionNumber;
                        dbSquarePayment.PaymentAmount = paymentAmount;
                        dbSquarePayment.ProcessingFeeAmount = processingFeeAmount;
                        dbSquarePayment.UpdateDateTimeUtc = utcNow;

                        var description = new TransactionDescriptionBuilder();
                        if (paymentAmountDelta != 0)
                        {
                            description.Append($"Payment amount updated by {paymentAmountDelta:c}.");
                        }
                        if (processingFeeDelta != 0)
                        {
                            description.Append($"Processing fee updated by {processingFeeDelta:c}.");
                        }

                        var dbSquarePaymentTransaction = new SquarePaymentTransaction()
                        {
                            SquarePaymentId = dbSquarePayment.SquarePaymentId,
                            SquarePayloadId = dbSquarePaymentPayload.SquarePayloadId,
                            PaymentAmount = paymentAmountDelta,
                            ProcessingFeeAmount = processingFeeDelta,
                            SquarePaymentRecordId = dbSquarePaymentPayload.SquarePaymentRecordId,
                            VersionNumber = dbSquarePaymentPayload.VersionNumber,
                            TransactionDateTimeUtc = utcNow,
                            Description = description.ToString(),
                            UnitOfWork = unitOfWork.Next()
                        };
                        _ = ctx.SquarePaymentTransactions.Add(dbSquarePaymentTransaction);

                        var dbSquarePaymentEvent = new SquarePaymentEvent()
                        {
                            EventTypeCode = SquarePaymentEventTypeCodes.Payment,
                            EventDateTimeUtc = utcNow,
                            ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                            ProcessingStatusDateTimeUtc = utcNow
                        };
                        dbSquarePaymentTransaction.SquarePaymentEvents.Add(dbSquarePaymentEvent);
                    }
                }

                dbSquarePaymentPayload.ProcessingStatusCode = SquareProcessingStatusCodes.Processed;
                dbSquarePaymentPayload.ProcessingStatusDateTimeUtc = utcNow;

                _ = await ctx.SaveChangesAsync();
            }
            catch (Exception)
            {
                //log.LogException(ex);

                await MovePaymentPayloadToExceptionStatus(squarePayloadId);

                throw;
            }
        }

        private async Task MovePaymentPayloadToExceptionStatus(long squarePayloadId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquarePaymentPayload = await ctx.SquarePaymentPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);
            dbSquarePaymentPayload.ProcessingStatusCode = SquareProcessingStatusCodes.Exception;
            dbSquarePaymentPayload.ProcessingStatusDateTimeUtc = utcNow;

            _ = await ctx.SaveChangesAsync();
        }

        private async Task ProcessSquareRefundPayloadAsync(long squarePayloadId, UnitOfWork unitOfWork)
        {
            try
            {
                // Step 1 - Ensure a SquareRefund record exists for the payload.
                //
                await ProcessSquareRefundPayloadStep1Async(squarePayloadId, unitOfWork);

                // Step 2 - Update the SquareRefund.  Retry if a concurrency exception occurs.
                //
                await DbRetry.FunctionAsync(() => ProcessSquareRefundPayloadStep2Async(squarePayloadId, unitOfWork));

                await MoveRefundPayloadToProcessedStatus(squarePayloadId);
            }
            catch (Exception)
            {
                //log.LogException(ex);

                await MoveRefundPayloadToExceptionStatus(squarePayloadId);

                throw;
            }
        }

        // Ensure a SquareRefund record exists for the payload.
        //
        private async Task ProcessSquareRefundPayloadStep1Async(long squarePayloadId, UnitOfWork unitOfWork)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquareRefundPayload = await ctx.SquareRefundPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);

            if (dbSquareRefundPayload.ProcessingStatusCode != SquareProcessingStatusCodes.Pending)
            {
                throw new InvalidOperationException($"Square refund payload {squarePayloadId} in {dbSquareRefundPayload.ProcessingStatusCode} status.");
            }

            using var jsonRefundDocument = JsonDocument.Parse(dbSquareRefundPayload.RefundPayloadJson);
            var jsonRefund = jsonRefundDocument.RootElement;

            var squarePaymentRecordId = jsonRefund.GetProperty("payment_id").ToString();

            var dbSquarePayment = await ctx.SquarePayments
                .Where(r => r.SquarePaymentRecordId == squarePaymentRecordId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (dbSquarePayment == null)
            {
                throw new InvalidOperationException($"Square payment record ID {squarePaymentRecordId} not found.");
            }

            var dbSquareRefund = await ctx.SquareRefunds
                .Include(r => r.SquareRefundTransactions)
                .Where(r => r.SquareRefundRecordId == dbSquareRefundPayload.SquareRefundRecordId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (dbSquareRefund == null)
            {
                dbSquareRefund = new SquareRefund()
                {
                    RefundAmount = 0m,
                    ProcessingFeeAmount = 0m,
                    SquareRefundRecordId = dbSquareRefundPayload.SquareRefundRecordId,
                    VersionNumber = 0,
                    UpdateDateTimeUtc = utcNow
                };
                dbSquarePayment.SquareRefunds.Add(dbSquareRefund);

                // Separate threads may be attempting to create a new SquareRefund record.  Tolerate DbUpdateException's if they occur.
                //
                try
                {
                    _ = await ctx.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    //log.LogMessage("DbUpdateException occurred during save.  Ignoring...");
                }
            }
        }

        // Update SquareRefund record with payload data.
        //
        private async Task ProcessSquareRefundPayloadStep2Async(long squarePayloadId, UnitOfWork unitOfWork)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquareRefundPayload = await ctx.SquareRefundPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);

            if (dbSquareRefundPayload.ProcessingStatusCode != SquareProcessingStatusCodes.Pending)
            {
                throw new InvalidOperationException($"Square refund payload {squarePayloadId} in {dbSquareRefundPayload.ProcessingStatusCode} status.");
            }

            var dbSquareRefund = await ctx.SquareRefunds
                .Include(r => r.SquareRefundTransactions)
                .Where(r => r.SquareRefundRecordId == dbSquareRefundPayload.SquareRefundRecordId)
                .FirstAsync().ConfigureAwait(false);

            var dbSquarePayment = dbSquareRefund.SquarePayment;

            using var jsonRefundDocument = JsonDocument.Parse(dbSquareRefundPayload.RefundPayloadJson);
            var jsonRefund = jsonRefundDocument.RootElement;

            // Ensure the Square payment ID specified in the JSON data matches the value in the SquarePayment record.
            //
            var squarePaymentRecordId = jsonRefund.GetProperty("payment_id").ToString();
            if (dbSquarePayment.SquarePaymentRecordId != squarePaymentRecordId)
            {
                throw new InvalidOperationException($"Refund payment mismatch.  Expected = {squarePaymentRecordId}, Actual = {dbSquarePayment.SquarePaymentRecordId}.");
            }

            if (dbSquareRefund.VersionNumber >= dbSquareRefundPayload.VersionNumber)
            {
                //LogMessage("Square payment already updated by more recent version.");
                return;
            }

            var refundAmount = ConvertSquareCurrency(jsonRefund.GetProperty("amount_money").GetProperty("amount").GetInt32());
            var processingFeeAmount = GetPaymentProcessingFeeAmount(jsonRefund);

            var refundAmountDelta = refundAmount - dbSquareRefund.RefundAmount;
            var processingFeeDelta = processingFeeAmount - dbSquareRefund.ProcessingFeeAmount;

            if (refundAmountDelta != 0 || processingFeeDelta != 0)
            {
                // Note: processingFeeDelta is normally negative for refunds.
                //
                //var ledgeAccountTransactionId = await new LedgerServiceAccountTransactionBuilder(LedgerMicroService)
                //    .Begin("SquareMicrosService.ProcessSquarePayment", Locale.GetLocalTimeFromUtc(utcNow).Date)
                //    .UnitOfWork(unitOfWork)
                //    .Credit(LedgerAccountNumbers.SquareFundsSuspense, refundAmountDelta + processingFeeDelta)
                //    .Debit(LedgerAccountNumbers.Cash, refundAmountDelta)
                //    .Debit(LedgerAccountNumbers.SquareFeeExpense, -processingFeeDelta)
                //    .CreateAsync();

                dbSquareRefund.VersionNumber = dbSquareRefundPayload.VersionNumber;
                dbSquareRefund.RefundAmount = refundAmount;
                dbSquareRefund.ProcessingFeeAmount = processingFeeAmount;
                dbSquareRefund.UpdateDateTimeUtc = utcNow;

                var description = new TransactionDescriptionBuilder();
                if (refundAmountDelta != 0)
                {
                    description.Append($"Refund amount updated by {refundAmountDelta:c}.");
                }
                if (processingFeeDelta != 0)
                {
                    description.Append($"Processing fee updated by {processingFeeDelta:c}.");
                }

                var dbSquareRefundTransaction = new SquareRefundTransaction()
                {
                    SquarePayloadId = dbSquareRefundPayload.SquarePayloadId,
                    RefundAmount = refundAmountDelta,
                    ProcessingFeeAmount = processingFeeDelta,
                    SquareRefundRecordId = dbSquareRefundPayload.SquareRefundRecordId,
                    VersionNumber = dbSquareRefundPayload.VersionNumber,
                    TransactionDateTimeUtc = utcNow,
                    Description = description.ToString(),
                    UnitOfWork = unitOfWork.Next()
                };
                dbSquareRefund.SquareRefundTransactions.Add(dbSquareRefundTransaction);

                var dbSquareRefundEvent = new SquareRefundEvent()
                {
                    EventTypeCode = SquarePaymentEventTypeCodes.Payment,
                    EventDateTimeUtc = utcNow,
                    ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                    ProcessingStatusDateTimeUtc = utcNow
                };
                dbSquareRefundTransaction.SquareRefundEvents.Add(dbSquareRefundEvent);
            }

            try
            {
                _ = await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //log.LogException(ex);
                throw new DbRetryException("ProcessSquareRefundPayloadStep2Async", ex);
            }
        }

        private async Task MoveRefundPayloadToExceptionStatus(long squarePayloadId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquareRefundPayload = await ctx.SquareRefundPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);
            dbSquareRefundPayload.ProcessingStatusCode = SquareProcessingStatusCodes.Exception;
            dbSquareRefundPayload.ProcessingStatusDateTimeUtc = utcNow;

            _ = await ctx.SaveChangesAsync();
        }

        private async Task MoveRefundPayloadToProcessedStatus(long squarePayloadId)
        {
            var utcNow = GetUtcNow();

            using var ctx = CreateQuiltContext();

            var dbSquareRefundPayload = await ctx.SquareRefundPayloads.FindAsync(squarePayloadId).ConfigureAwait(false);
            dbSquareRefundPayload.ProcessingStatusCode = SquareProcessingStatusCodes.Processed;
            dbSquareRefundPayload.ProcessingStatusDateTimeUtc = utcNow;

            _ = await ctx.SaveChangesAsync();
        }

        private string CreateReferenceId(long squarePaymentId)
        {
            return $"SP:{squarePaymentId}";
        }

        private long ParseReferenceId(string referenceId)
        {
            if (!referenceId.StartsWith("SP:"))
            {
                throw new ArgumentException($"Invalid reference ID {referenceId}.");
            }

            var squarePaymentId = long.Parse(referenceId.Substring(3));

            return squarePaymentId;
        }

        private decimal ConvertSquareCurrency(int value)
        {
            var result = value / 100m;

            return result;
        }

        private IList<MSquare_ProcessWebPaymentRequestResponseError> CreateErrors(IList<Error> sErrors)
        {
            if (sErrors == null)
            {
                return null;
            }

            var errors = new List<MSquare_ProcessWebPaymentRequestResponseError>();
            foreach (var sError in sErrors)
            {
                errors.Add(new MSquare_ProcessWebPaymentRequestResponseError()
                {
                    Category = sError.Category,
                    Code = sError.Code,
                    Detail = sError.Detail,
                    Field = sError.Field
                });
            }

            return errors;
        }

        private static class Create
        {
            public static MSquare_Customer MSquare_Customer(SquareCustomer dbCustomer)
            {
                var mPayments = dbCustomer.SquarePayments.Select(r => MSquare_CustomerPayment(r)).ToList();

                return new MSquare_Customer()
                {
                    SquareCustomerId = dbCustomer.SquareCustomerId,
                    SquareCustomerReference = dbCustomer.SquareCustomerReference,
                    UpdateDateTimeUtc = dbCustomer.UpdateDateTimeUtc,
                    Payments = mPayments
                };
            }

            public static MSquare_CustomerSummary MSquare_CustomerSummary(SquareCustomer dbCustomer)
            {
                return new MSquare_CustomerSummary()
                {
                    SquareCustomerId = dbCustomer.SquareCustomerId,
                    SquareCustomerReference = dbCustomer.SquareCustomerReference,
                    UpdateDateTimeUtc = dbCustomer.UpdateDateTimeUtc
                };
            }

            public static MSquare_Payment MSquare_Payment(SquarePayment dbSquarePayment)
            {
                return new MSquare_Payment()
                {
                    SquarePaymentId = dbSquarePayment.SquarePaymentId,
                    SquarePaymentReference = dbSquarePayment.SquarePaymentReference,
                    SquareCustomerId = dbSquarePayment.SquareCustomerId,
                    SquareCustomerReference = dbSquarePayment.SquareCustomer.SquareCustomerReference,
                    PaymentAmount = dbSquarePayment.PaymentAmount,
                    RefundAmount = dbSquarePayment.SquareRefunds.Sum(r => r.RefundAmount),
                    ProcessingFeeAmount = dbSquarePayment.ProcessingFeeAmount + dbSquarePayment.SquareRefunds.Sum(r => r.ProcessingFeeAmount),
                    SquarePaymentRecordId = dbSquarePayment.SquarePaymentRecordId,
                    VersionNumber = dbSquarePayment.VersionNumber,
                    UpdateDateTimeUtc = dbSquarePayment.UpdateDateTimeUtc,
                    WebPaymentRequests = dbSquarePayment.SquareWebPaymentRequests.Select(r => MSquare_WebPaymentRequest(r)).ToList(),
                    Refunds = dbSquarePayment.SquareRefunds.Select(r => MSquare_Refund(r)).ToList()
                };
            }

            public static MSquare_PaymentSummary MSquare_PaymentSummary(SquarePayment dbSquarePayment)
            {
                return new MSquare_PaymentSummary()
                {
                    SquarePaymentId = dbSquarePayment.SquarePaymentId,
                    SquarePaymentReference = dbSquarePayment.SquarePaymentReference,
                    SquareCustomerId = dbSquarePayment.SquareCustomerId,
                    PaymentAmount = dbSquarePayment.PaymentAmount,
                    RefundAmount = dbSquarePayment.SquareRefunds.Sum(r => r.RefundAmount),
                    ProcessingFeeAmount = dbSquarePayment.ProcessingFeeAmount + dbSquarePayment.SquareRefunds.Sum(r => r.ProcessingFeeAmount),
                    SquarePaymentRecordId = dbSquarePayment.SquarePaymentRecordId,
                    VersionNumber = dbSquarePayment.VersionNumber,
                    UpdateDateTimeUtc = dbSquarePayment.UpdateDateTimeUtc
                };
            }

            public static MSquare_PaymentTransaction MSquare_PaymentTransaction(SquarePaymentTransaction dbSquarePaymentTransaction)
            {
                return new MSquare_PaymentTransaction()
                {
                    TransactionId = dbSquarePaymentTransaction.SquarePaymentTransactionId,
                    EntityId = dbSquarePaymentTransaction.SquarePaymentId,
                    TransactionDateTimeUtc = dbSquarePaymentTransaction.TransactionDateTimeUtc,
                    Description = dbSquarePaymentTransaction.Description,
                    UnitOfWork = dbSquarePaymentTransaction.UnitOfWork,

                    SquarePaymentRecordId = dbSquarePaymentTransaction.SquarePaymentRecordId,
                    VersionNumber = dbSquarePaymentTransaction.VersionNumber,
                    PaymentAmount = dbSquarePaymentTransaction.PaymentAmount,
                    ProcessingFeeAmount = dbSquarePaymentTransaction.ProcessingFeeAmount,
                };
            }

            public static MSquare_PaymentTransactionSummary MSquare_PaymentTransactionSummary(SquarePaymentTransaction dbSquarePaymentTransaction)
            {
                return new MSquare_PaymentTransactionSummary()
                {
                    TransactionId = dbSquarePaymentTransaction.SquarePaymentTransactionId,
                    EntityId = dbSquarePaymentTransaction.SquarePaymentId,
                    TransactionDateTimeUtc = dbSquarePaymentTransaction.TransactionDateTimeUtc,
                    Description = dbSquarePaymentTransaction.Description,
                    UnitOfWork = dbSquarePaymentTransaction.UnitOfWork,

                    SquarePaymentRecordId = dbSquarePaymentTransaction.SquarePaymentRecordId,
                    VersionNumber = dbSquarePaymentTransaction.VersionNumber,
                    PaymentAmount = dbSquarePaymentTransaction.PaymentAmount,
                    ProcessingFeeAmount = dbSquarePaymentTransaction.ProcessingFeeAmount,
                };
            }

            public static MSquare_PaymentEventLog MSquare_PaymentEventLog(SquarePaymentEvent dbSquarePaymentEvent)
            {
                return new MSquare_PaymentEventLog()
                {
                    EventId = dbSquarePaymentEvent.SquarePaymentEventId,
                    TransactionId = dbSquarePaymentEvent.SquarePaymentTransactionId,
                    EventTypeCode = dbSquarePaymentEvent.EventTypeCode,
                    EventDateTimeUtc = dbSquarePaymentEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbSquarePaymentEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbSquarePaymentEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MSquare_PaymentEventLogSummary MSquare_PaymentEventLogSummary(SquarePaymentEvent dbSquarePaymentEvent)
            {
                return new MSquare_PaymentEventLogSummary()
                {
                    EventId = dbSquarePaymentEvent.SquarePaymentEventId,
                    TransactionId = dbSquarePaymentEvent.SquarePaymentTransactionId,
                    EntityId = dbSquarePaymentEvent.SquarePaymentTransaction.SquarePaymentId,
                    EventTypeCode = dbSquarePaymentEvent.EventTypeCode,
                    EventDateTimeUtc = dbSquarePaymentEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbSquarePaymentEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbSquarePaymentEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbSquarePaymentEvent.SquarePaymentTransaction.UnitOfWork
                };
            }

            private static MSquare_CustomerPayment MSquare_CustomerPayment(SquarePayment dbSquarePayment)
            {
                return new MSquare_CustomerPayment()
                {
                    SquarePaymentId = dbSquarePayment.SquarePaymentId,
                    SquarePaymentReference = dbSquarePayment.SquarePaymentReference,
                    PaymentAmount = dbSquarePayment.PaymentAmount,
                    RefundAmount = dbSquarePayment.SquareRefunds.Sum(r => r.RefundAmount),
                    ProcessingFeeAmount = dbSquarePayment.ProcessingFeeAmount + dbSquarePayment.SquareRefunds.Sum(r => r.ProcessingFeeAmount),
                    SquarePaymentRecordId = dbSquarePayment.SquarePaymentRecordId,
                    VersionNumber = dbSquarePayment.VersionNumber,
                    UpdateDateTimeUtc = dbSquarePayment.UpdateDateTimeUtc,
                };
            }

            private static MSquare_WebPaymentRequest MSquare_WebPaymentRequest(SquareWebPaymentRequest dbSquareWebPaymentRequest)
            {
                return new MSquare_WebPaymentRequest()
                {
                    SquareWebPaymentRequestId = dbSquareWebPaymentRequest.SquareWebPaymentRequestId,
                    WebRequestTypeCode = dbSquareWebPaymentRequest.WebRequestTypeCode,
                    ProcessingStatusCode = dbSquareWebPaymentRequest.ProcessingStatusCode,
                    StatusDateTimeUtc = dbSquareWebPaymentRequest.ProcessingStatusDateTimeUtc
                };
            }

            public static MSquare_Refund MSquare_Refund(SquareRefund dbSquareRefund)
            {
                //var mRefundTransactions = MSquare_RefundTransactions(dbSquareRefund);

                return new MSquare_Refund()
                {
                    SquareRefundId = dbSquareRefund.SquareRefundId,
                    SquarePaymentId = dbSquareRefund.SquarePaymentId,
                    RefundAmount = dbSquareRefund.RefundAmount,
                    ProcessingFeeAmount = dbSquareRefund.ProcessingFeeAmount,
                    SquareRefundRecordId = dbSquareRefund.SquareRefundRecordId,
                    VersionNumber = dbSquareRefund.VersionNumber,
                    UpdateDateTimeUtc = dbSquareRefund.UpdateDateTimeUtc
                };
            }

            public static MSquare_RefundTransaction MSquare_RefundTransaction(SquareRefundTransaction dbSquareRefundTransaction)
            {
                return new MSquare_RefundTransaction()
                {
                    TransactionId = dbSquareRefundTransaction.SquareRefundTransactionId,
                    EntityId = dbSquareRefundTransaction.SquareRefundId,
                    TransactionDateTimeUtc = dbSquareRefundTransaction.TransactionDateTimeUtc,
                    Description = dbSquareRefundTransaction.Description,
                    UnitOfWork = dbSquareRefundTransaction.UnitOfWork,

                    SquareRefundRecordId = dbSquareRefundTransaction.SquareRefundRecordId,
                    VersionNumber = dbSquareRefundTransaction.VersionNumber,
                    RefundAmount = dbSquareRefundTransaction.RefundAmount,
                    ProcessingFeeAmount = dbSquareRefundTransaction.ProcessingFeeAmount,
                };
            }

            public static IList<MSquare_RefundTransaction> MSquare_RefundTransactions(SquareRefund dbRefund)
            {
                var transactions = new List<MSquare_RefundTransaction>();
                foreach (var dbSquareRefundTransaction in dbRefund.SquareRefundTransactions)
                {
                    transactions.Add(MSquare_RefundTransaction(dbSquareRefundTransaction));
                }

                return transactions;
            }

            public static MSquare_RefundTransactionSummary MSquare_RefundTransactionSummary(SquareRefundTransaction dbSquareRefundTransaction)
            {
                return new MSquare_RefundTransactionSummary()
                {
                    TransactionId = dbSquareRefundTransaction.SquareRefundTransactionId,
                    EntityId = dbSquareRefundTransaction.SquareRefundId,
                    TransactionDateTimeUtc = dbSquareRefundTransaction.TransactionDateTimeUtc,
                    Description = dbSquareRefundTransaction.Description,
                    UnitOfWork = dbSquareRefundTransaction.UnitOfWork,

                    SquareRefundRecordId = dbSquareRefundTransaction.SquareRefundRecordId,
                    VersionNumber = dbSquareRefundTransaction.VersionNumber,
                    RefundAmount = dbSquareRefundTransaction.RefundAmount,
                    ProcessingFeeAmount = dbSquareRefundTransaction.ProcessingFeeAmount,
                };
            }

            public static MSquare_RefundEventLog MSquare_RefundEventLog(SquareRefundEvent dbSquareRefundEvent)
            {
                return new MSquare_RefundEventLog()
                {
                    EventId = dbSquareRefundEvent.SquareRefundEventId,
                    TransactionId = dbSquareRefundEvent.SquareRefundTransactionId,
                    EventTypeCode = dbSquareRefundEvent.EventTypeCode,
                    EventDateTimeUtc = dbSquareRefundEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbSquareRefundEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbSquareRefundEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MSquare_RefundEventLogSummary MSquare_RefundEventLogSummary(SquareRefundEvent dbSquareRefundEvent)
            {
                return new MSquare_RefundEventLogSummary()
                {
                    EventId = dbSquareRefundEvent.SquareRefundEventId,
                    TransactionId = dbSquareRefundEvent.SquareRefundTransactionId,
                    EntityId = dbSquareRefundEvent.SquareRefundTransaction.SquareRefundId,
                    EventTypeCode = dbSquareRefundEvent.EventTypeCode,
                    EventDateTimeUtc = dbSquareRefundEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbSquareRefundEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbSquareRefundEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbSquareRefundEvent.SquareRefundTransaction.UnitOfWork,
                };
            }
        }
    }
}