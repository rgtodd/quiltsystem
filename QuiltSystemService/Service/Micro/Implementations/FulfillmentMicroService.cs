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

using RichTodd.QuiltSystem.Business.Operation;
using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Database.Model.Extensions;
using RichTodd.QuiltSystem.Extensions;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class FulfillmentMicroService : MicroService, IFulfillmentMicroService
    {
        private IFulfillmentEventMicroService FulfillmentEventService { get; }

        public FulfillmentMicroService(
            IApplicationLocale locale,
            ILogger<FulfillmentMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IFulfillmentEventMicroService fulfillmentEventService)
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            FulfillmentEventService = fulfillmentEventService ?? throw new ArgumentNullException(nameof(fulfillmentEventService));
        }

        public async Task<MFulfillment_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetDashboardAsync));
            //using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MFulfillment_Dashboard()
                {
                    TotalFulfillables = await ctx.Fulfillables.CountAsync(),
                    TotalShipmentRequests = await ctx.ShipmentRequests.CountAsync(),
                    TotalShipments = await ctx.Shipments.CountAsync(),
                    TotalReturnRequests = await ctx.ReturnRequests.CountAsync(),
                    TotalReturns = await ctx.Returns.CountAsync(),
                    TotalTransactions = await ctx.FulfillableTransactions.CountAsync(),
                    TotalEvents = await ctx.FulfillableEvents.CountAsync()
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

        #region Fulfillable 

        public async Task<MFulfillment_AllocateFulfillableResponse> AllocateFulfillableAsync(MFulfillment_AllocateFulfillable allocateFulfillable)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(AllocateFulfillableAsync), allocateFulfillable);
            try
            {
                var utcNow = GetUtcNow();

                var ctx = CreateQuiltContext();

                var dbFulfillable = await ctx.Fulfillables.Where(r => r.FulfillableReference == allocateFulfillable.FulfillableReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbFulfillable != null)
                {
                    throw new InvalidOperationException($"Fulfillable {dbFulfillable.FulfillableId} already exists.");
                }

                var shippingAddress = allocateFulfillable.ShippingAddress;

                dbFulfillable = new Fulfillable()
                {
                    FulfillableReference = allocateFulfillable.FulfillableReference,
                    Name = allocateFulfillable.Name,
                    CreateDateTimeUtc = utcNow,
                    FulfillableAddress = new FulfillableAddress()
                    {
                        ShipToName = shippingAddress.Name,
                        ShipToAddressLine1 = shippingAddress.AddressLine1,
                        ShipToAddressLine2 = shippingAddress.AddressLine2,
                        ShipToCity = shippingAddress.City,
                        ShipToStateCode = shippingAddress.StateCode,
                        ShipToPostalCode = shippingAddress.PostalCode,
                        ShipToCountryCode = shippingAddress.CountryCode
                    },
                    FulfillableStatusCode = FulfillableStatusCodes.Closed,
                    FulfillableStatusDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                foreach (var allocateFulfillableItem in allocateFulfillable.FulfillableItems)
                {
                    var dbFulfillableItem = new FulfillableItem()
                    {
                        FulfillableItemReference = allocateFulfillableItem.FulfillableItemReference,
                        Description = allocateFulfillableItem.Description,
                        ConsumableReference = allocateFulfillableItem.ConsumableReference,
                        UpdateDateTimeUtc = utcNow
                    };
                    foreach (var allocateFulFillableItemComponent in allocateFulfillableItem.FulfillableItemComponents)
                    {
                        var dbFulfillableItemComponent = new FulfillableItemComponent()
                        {
                            Description = allocateFulFillableItemComponent.Description,
                            ConsumableReference = allocateFulFillableItemComponent.ConsumableReference,
                            Quantity = allocateFulFillableItemComponent.Quantity,
                            UpdateDateTimeUtc = utcNow
                        };
                        dbFulfillableItem.FulfillableItemComponents.Add(dbFulfillableItemComponent);
                    }
                    dbFulfillable.FulfillableItems.Add(dbFulfillableItem);
                }
                _ = ctx.Fulfillables.Add(dbFulfillable);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var fulfillableResponse = new MFulfillment_AllocateFulfillableResponse()
                {
                    FulfillableReference = dbFulfillable.FulfillableReference,
                    FulfillableId = dbFulfillable.FulfillableId,
                    FulfillableItemResponses = new List<MFulfillment_AllocateFulfillableItemResponse>()
                };
                foreach (var dbFulfillableItem in dbFulfillable.FulfillableItems)
                {
                    var fulfillableItemResponse = new MFulfillment_AllocateFulfillableItemResponse()
                    {
                        FulfillableItemId = dbFulfillableItem.FulfillableItemId,
                        FulfillableItemReference = dbFulfillableItem.FulfillableItemReference
                    };
                    fulfillableResponse.FulfillableItemResponses.Add(fulfillableItemResponse);
                }

                var result = fulfillableResponse;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupFulfillableAsync(string fulfillableReference)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(LookupFulfillableAsync), fulfillableReference);
            try
            {
                var utcNow = GetUtcNow();

                var ctx = CreateQuiltContext();

                var dbFulfillable = await ctx.Fulfillables.Where(r => r.FulfillableReference == fulfillableReference).SingleOrDefaultAsync().ConfigureAwait(false);

                var fulfillableId = dbFulfillable != null
                    ? dbFulfillable.FulfillableId
                    : (long?)null;

                var result = fulfillableId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupFulfillableItemAsync(string fulfillableItemReference)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(LookupFulfillableItemAsync), fulfillableItemReference);
            try
            {
                var utcNow = GetUtcNow();

                var ctx = CreateQuiltContext();

                var dbFulfillableItem = await ctx.FulfillableItems.Where(r => r.FulfillableItemReference == fulfillableItemReference).SingleOrDefaultAsync().ConfigureAwait(false);

                var fulfillableItemId = dbFulfillableItem != null
                    ? dbFulfillableItem.FulfillableItemId
                    : (long?)null;

                var result = fulfillableItemId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetFulfillableShippingAddress(long fulfillableId, MCommon_Address shippingAddress)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(SetFulfillableShippingAddress), fulfillableId, shippingAddress);
            try
            {
                var ctx = CreateQuiltContext();

                var dbFulfillable = await ctx.Fulfillables
                    .Include(r => r.FulfillableAddress)
                    .Where(r => r.FulfillableId == fulfillableId)
                    .FirstAsync().ConfigureAwait(false);

                var dbFulfillableAddress = dbFulfillable.FulfillableAddress;
                if (dbFulfillableAddress == null)
                {
                    dbFulfillable.FulfillableAddress = new FulfillableAddress()
                    {
                        ShipToName = shippingAddress.Name,
                        ShipToAddressLine1 = shippingAddress.AddressLine1,
                        ShipToAddressLine2 = shippingAddress.AddressLine2,
                        ShipToCity = shippingAddress.City,
                        ShipToStateCode = shippingAddress.StateCode,
                        ShipToPostalCode = shippingAddress.PostalCode,
                        ShipToCountryCode = shippingAddress.CountryCode
                    };
                }
                else
                {
                    dbFulfillableAddress.ShipToName = shippingAddress.Name;
                    dbFulfillableAddress.ShipToAddressLine1 = shippingAddress.AddressLine1;
                    dbFulfillableAddress.ShipToAddressLine2 = shippingAddress.AddressLine2;
                    dbFulfillableAddress.ShipToCity = shippingAddress.City;
                    dbFulfillableAddress.ShipToStateCode = shippingAddress.StateCode;
                    dbFulfillableAddress.ShipToPostalCode = shippingAddress.PostalCode;
                    dbFulfillableAddress.ShipToCountryCode = shippingAddress.CountryCode;
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetFulfillmentRequestQuantity(long fulfillableItemId, int requestQuantity, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(SetFulfillmentRequestQuantity), fulfillableItemId, requestQuantity);
            try
            {
                var utcNow = GetUtcNow();

                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                using var ctx = CreateQuiltContext();

                var dbFulfillableItem = await ctx.FulfillableItems
                    .Include(r => r.Fulfillable)
                        .ThenInclude(r => r.FulfillableAddress)
                    .Where(r => r.FulfillableItemId == fulfillableItemId)
                    .FirstAsync().ConfigureAwait(false);
                var dbFulfillable = dbFulfillableItem.Fulfillable;
                var dbFulfillableAddress = dbFulfillable.FulfillableAddress;

                var requestQuantityDelta = requestQuantity - dbFulfillableItem.RequestQuantity;
                if (requestQuantityDelta != 0)
                {
                    var fulfillmentTransaction = ctx.CreateFulfillmentTransactionBuilder()
                        .Begin(utcNow)
                        .UnitOfWork(unitOfWork)
                        .SetRequestQuantity(fulfillableItemId, requestQuantityDelta)
                        .Create();

                    // Search for existing shipment request.
                    //
                    ShipmentRequest dbShipmentRequest = null;
                    var shipmentRequestIds = await ctx.FulfillableToShipmentRequestViews.Where(r => r.FulfillableId == dbFulfillableItem.FulfillableId).Select(r => r.ShipmentRequestId).ToListAsync().ConfigureAwait(false);
                    foreach (var shipmentRequestId in shipmentRequestIds)
                    {
                        var dbExistingShipmentRequest = await ctx.ShipmentRequests.FindAsync(shipmentRequestId).ConfigureAwait(false);
                        if (dbExistingShipmentRequest.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Pending)
                        {
                            dbShipmentRequest = dbExistingShipmentRequest;
                            break;
                        }
                    }

                    if (requestQuantityDelta > 0)
                    {
                        if (dbShipmentRequest == null)
                        {
                            var shipmentRequestNumber = ctx.GetShipmentRequestNumber(utcNow);
                            dbShipmentRequest = new ShipmentRequest()
                            {
                                CreateDateTimeUtc = utcNow,
                                ShipmentRequestStatusCode = ShipmentRequestStatusCodes.Pending,
                                ShipmentRequestStatusDateTimeUtc = utcNow,
                                ShipmentRequestNumber = shipmentRequestNumber,
                                UpdateDateTimeUtc = utcNow,
                            };
                            _ = ctx.ShipmentRequests.Add(dbShipmentRequest);

                            if (dbFulfillableAddress != null)
                            {
                                dbShipmentRequest.ShipmentRequestAddress = new ShipmentRequestAddress()
                                {
                                    ShipToName = dbFulfillableAddress.ShipToName,
                                    ShipToAddressLine1 = dbFulfillableAddress.ShipToAddressLine1,
                                    ShipToAddressLine2 = dbFulfillableAddress.ShipToAddressLine2,
                                    ShipToCity = dbFulfillableAddress.ShipToCity,
                                    ShipToStateCode = dbFulfillableAddress.ShipToStateCode,
                                    ShipToPostalCode = dbFulfillableAddress.ShipToPostalCode,
                                    ShipToCountryCode = dbFulfillableAddress.ShipToCountryCode
                                };
                            }
                        }

                        var dbShipmentRequestItem = dbShipmentRequest.ShipmentRequestItems.Where(r => r.FulfillableItemId == fulfillableItemId).SingleOrDefault();
                        if (dbShipmentRequestItem == null)
                        {
                            dbShipmentRequestItem = new ShipmentRequestItem()
                            {
                                FulfillableItemId = dbFulfillableItem.FulfillableItemId,
                                Quantity = 0
                            };
                            dbShipmentRequest.ShipmentRequestItems.Add(dbShipmentRequestItem);
                        }

                        dbShipmentRequestItem.Quantity += requestQuantityDelta;
                    }
                    else // delta < 0
                    {
                        if (dbShipmentRequest == null)
                        {
                            throw new InvalidOperationException($"Open shipment request not found for fulfillable item {fulfillableItemId}.");
                        }

                        var dbShipmentRequestItem = dbShipmentRequest.ShipmentRequestItems.Where(r => r.FulfillableItemId == dbFulfillableItem.FulfillableItemId).Single();
                        if (dbShipmentRequestItem.Quantity < -requestQuantityDelta)
                        {
                            throw new InvalidOperationException($"Delta {-requestQuantityDelta} exceeds pending quantity of {dbShipmentRequestItem.Quantity}.");
                        }

                        dbShipmentRequestItem.Quantity += requestQuantityDelta;
                    }
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_Fulfillable> GetFulfillableAsync(long fulfillableId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableAsync), fulfillableId);
            try
            {
                var ctx = CreateQuiltContext();

                var dbFulfillable = await ctx.Fulfillables
                    .Include(r => r.FulfillableItems)
                        .ThenInclude(r => r.FulfillableItemComponents)
                    .Include(r => r.FulfillableAddress)
                    .Where(r => r.FulfillableId == fulfillableId)
                    .FirstAsync().ConfigureAwait(false);

                var dbShipmentRequests = await LoadShipmentRequestsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbShipments = await LoadShipmentsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbReturnRequests = await LoadReturnRequestsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbReturns = await LoadReturnsAsync(ctx, fulfillableId).ConfigureAwait(false);

                MFulfillment_Fulfillable fulfillable = Create.MFulfillment_Fulfillable(
                    dbFulfillable,
                    dbShipmentRequests,
                    dbShipments,
                    dbReturnRequests,
                    dbReturns);

                var result = fulfillable;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_Fulfillable> GetFulfillableByItemAsync(long fulfillableItemId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableByItemAsync), fulfillableItemId);
            try
            {
                var ctx = CreateQuiltContext();

                var dbFulfillableItem = await ctx.FulfillableItems.FindAsync(fulfillableItemId).ConfigureAwait(false);
                var fulfillableId = dbFulfillableItem.FulfillableId;

                var dbFulfillable = await ctx.Fulfillables
                    .Include(r => r.FulfillableItems)
                        .ThenInclude(r => r.FulfillableItemComponents)
                    .Include(r => r.FulfillableAddress)
                    .Where(r => r.FulfillableId == fulfillableId)
                    .FirstAsync().ConfigureAwait(false);

                var dbShipmentRequests = await LoadShipmentRequestsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbShipments = await LoadShipmentsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbReturnRequests = await LoadReturnRequestsAsync(ctx, fulfillableId).ConfigureAwait(false);
                var dbReturns = await LoadReturnsAsync(ctx, fulfillableId).ConfigureAwait(false);

                MFulfillment_Fulfillable fulfillable = Create.MFulfillment_Fulfillable(
                    dbFulfillable,
                    dbShipmentRequests,
                    dbShipments,
                    dbReturnRequests,
                    dbReturns);

                var result = fulfillable;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_FulfillableSummaryList> GetFulfillableSummariesAsync(MFulfillment_FulfillableStatus fulfillableStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableSummariesAsync), fulfillableStatus, recordCount);
            try
            {
                var ctx = CreateQuiltContext();

                IQueryable<Fulfillable> query = ctx.Fulfillables;

                switch (fulfillableStatus)
                {
                    case MFulfillment_FulfillableStatus.MetaActive:
                        {
                            query = query.Where(r =>
                                r.FulfillableStatusCode == FulfillableStatusCodes.Open);
                            break;
                        }

                    case MFulfillment_FulfillableStatus.MetaAll:
                        break;

                    default:
                        {
                            var fulfillableStatusCode = GetCode.From(fulfillableStatus);
                            query = query.Where(r => r.FulfillableStatusCode == fulfillableStatusCode);
                            break;
                        }
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Include(r => r.FulfillableItems)
                    .Select(r => Create.MFulfillment_FulfillableSummary(r))
                    .ToListAsync();

                var result = new MFulfillment_FulfillableSummaryList()
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

        public async Task<MFulfillment_FulfillableTransaction> GetFulfillableTransactionAsync(long fulfillableTransactionId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableTransactionAsync), fulfillableTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableTransaction = await ctx.FulfillableTransactions
                    .Include(r => r.FulfillableTransactionItems)
                    .Where(r => r.FulfillableTransactionId == fulfillableTransactionId)
                    .Select(r => Create.MFulfillment_FulfillableTransaction(r))
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

        public async Task<MFulfillment_FulfillableTransactionSummaryList> GetFulfillableTransactionSummariesAsync(long? fulfillableId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableTransactionSummariesAsync), fulfillableId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Fulfillable)
                {
                    return new MFulfillment_FulfillableTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_FulfillableTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FulfillableTransaction>)ctx.FulfillableTransactions;
                if (fulfillableId != null)
                {
                    query = query.Where(r => r.FulfillableTransactionItems.Any(r => r.FulfillableItem.FulfillableId == fulfillableId));
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Include(r => r.FulfillableTransactionItems)
                        .ThenInclude(r => r.FulfillableItem)
                    .Select(r => Create.MFulfillment_FulfillableTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_FulfillableTransactionSummaryList
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

        public async Task<MFulfillment_FulfillableEventLog> GetFulfillableEventLogAsync(long fulfillableEventId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableEventLogAsync), fulfillableEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var fulfillableEventLog = await ctx.FulfillableEvents
                    .Where(r => r.FulfillableEventId == fulfillableEventId)
                    .Select(r => Create.MFulfillment_FulfillableEventLog(r))
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

        public async Task<MFulfillment_FulfillableEventLogSummaryList> GetFulfillableEventLogSummariesAsync(long? fulfillableId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetFulfillableEventLogSummariesAsync), fulfillableId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Fulfillable)
                {
                    return new MFulfillment_FulfillableEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_FulfillableEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<FulfillableEvent>)ctx.FulfillableEvents
                    .Include(r => r.FulfillableTransaction)
                        .ThenInclude(r => r.FulfillableTransactionItems)
                            .ThenInclude(r => r.FulfillableItem);
                if (fulfillableId != null)
                {
                    query = query.Where(r => r.FulfillableTransaction.FulfillableTransactionItems.Any(r => r.FulfillableItem.FulfillableId == fulfillableId));
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.FulfillableTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_FulfillableEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_FulfillableEventLogSummaryList
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

        #region Shipment Request

        public async Task<MFulfillment_ShipmentRequestSummaryList> GetShipmentRequestSummariesAsync(MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestSummariesAsync), shipmentRequestStatus, recordCount);
            try
            {
                using var ctx = CreateQuiltContext();

                IQueryable<ShipmentRequestSummaryView> query = ctx.ShipmentRequestSummaryViews;

                switch (shipmentRequestStatus)
                {
                    case MFulfillment_ShipmentRequestStatus.MetaActive:
                        {
                            query = query.Where(r =>
                                r.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Pending ||
                                r.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Open ||
                                r.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Exception);
                            break;
                        }

                    case MFulfillment_ShipmentRequestStatus.MetaAll:
                        break;

                    default:
                        {
                            var shipmentRequestStatusCode = GetCode.From(shipmentRequestStatus);
                            query = query.Where(r => r.ShipmentRequestStatusCode == shipmentRequestStatusCode);
                            break;
                        }
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var summaries = await query
                    .Select(r => Create.MFulfillment_ShipmentRequestSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ShipmentRequestSummaryList()
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

        public async Task<MFulfillment_ShipmentRequest> GetShipmentRequestAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestAsync), shipmentRequestId);
            try
            {
                using var ctx = CreateQuiltContext();

                var result = await ctx.ShipmentRequests
                    .Where(r => r.ShipmentRequestId == shipmentRequestId)
                    .Include(r => r.ShipmentRequestAddress)
                    .Include(r => r.ShipmentRequestItems)
                        .ThenInclude(r => r.ShipmentItems)
                            .ThenInclude(r => r.Shipment)
                    .Include(r => r.ShipmentRequestItems)
                        .ThenInclude(r => r.FulfillableItem)
                            .ThenInclude(r => r.Fulfillable)
                                .ThenInclude(r => r.FulfillableAddress)
                    .Include(r => r.ShipmentRequestItems)
                        .ThenInclude(r => r.FulfillableItem)
                            .ThenInclude(r => r.FulfillableItemComponents)
                    .Select(r => Create.MFulfillment_ShipmentRequest(r, false)) // shallow = false
                    .FirstAsync().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> GetPendingShipmentRequestAsync(long fulfillableId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetPendingShipmentRequestAsync), fulfillableId);
            try
            {
                using var ctx = CreateQuiltContext();

                var shipmentRequestIds = await ctx.FulfillableToShipmentRequestViews.Where(r => r.FulfillableId == fulfillableId).Select(r => r.ShipmentRequestId).ToListAsync().ConfigureAwait(false);
                foreach (var shipmentRequestId in shipmentRequestIds)
                {
                    var isOpen = await ctx.ShipmentRequests
                        .AnyAsync(r => r.ShipmentRequestId == shipmentRequestId && r.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Pending)
                        .ConfigureAwait(false);

                    if (isOpen)
                    {
                        log.Result(shipmentRequestId);

                        return shipmentRequestId;
                    }
                }

                log.Result(null);

                return null;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task OpenShipmentRequestAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(OpenShipmentRequestAsync), shipmentRequestId);
            try
            {
                using var ctx = CreateQuiltContext();

                var utcNow = GetUtcNow();

                var dbShipmentRequest = await ctx.ShipmentRequests.FindAsync(shipmentRequestId).ConfigureAwait(false);

                if (dbShipmentRequest.ShipmentRequestStatusCode != ShipmentRequestStatusCodes.Pending)
                {
                    throw new InvalidOperationException($"Shipment request {shipmentRequestId} in {dbShipmentRequest.ShipmentRequestStatusCode} status.");
                }

                dbShipmentRequest.ShipmentRequestStatusCode = ShipmentRequestStatusCodes.Open;
                dbShipmentRequest.ShipmentRequestStatusDateTimeUtc = utcNow;
                dbShipmentRequest.UpdateDateTimeUtc = utcNow;

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelShipmentRequestAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CancelShipmentRequestAsync), shipmentRequestId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                var dbShipmentRequest = await ctx.ShipmentRequests
                    .Include(r => r.ShipmentRequestItems)
                        .ThenInclude(r => r.ShipmentItems)
                            .ThenInclude(r => r.Shipment)
                    .Where(r => r.ShipmentRequestId == shipmentRequestId)
                    .SingleAsync().ConfigureAwait(false);

                if (!CanCancel(dbShipmentRequest))
                {
                    throw new BusinessOperationException($"Cannot cancel shipment request {shipmentRequestId}.");
                }

                var unitOfWork = CreateUnitOfWork.CancelShipmentRequest(shipmentRequestId);

                // Cancel any outstanding shipments.
                //
                var outstandingShipmentIds = new HashSet<long>();
                foreach (var dbShipmentRequestItem in dbShipmentRequest.ShipmentRequestItems)
                {
                    foreach (var dbShipmentItem in dbShipmentRequestItem.ShipmentItems)
                    {
                        var dbShipment = dbShipmentItem.Shipment;
                        if (dbShipment.ShipmentStatusCode.In(ShipmentStatusCodes.Posted, ShipmentStatusCodes.Complete))
                        {
                            _ = outstandingShipmentIds.Add(dbShipment.ShipmentId);
                        }
                    }
                }
                foreach (var shipmentId in outstandingShipmentIds)
                {
                    var dbShipment = await ctx.Shipments.FindAsync(shipmentId).ConfigureAwait(false);
                    _ = ctx.CreateShipmentTransactionBuilder()
                        .Begin(dbShipment, $"Cancelling shipment {shipmentId}.", utcNow)
                        .UnitOfWork(unitOfWork)
                        .SetStatusTypeCode(ShipmentStatusCodes.Cancelled)
                        .Event(ShipmentEventTypeCodes.Cancel)
                        .Create();
                }

                _ = ctx.CreateShipmentRequestTransactionBuilder()
                    .Begin(dbShipmentRequest, $"Cancelling shipment request {shipmentRequestId}.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ShipmentRequestStatusCodes.Cancelled)
                    .Event(ShipmentRequestEventTypeCodes.Cancel)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentRequestTransaction> GetShipmentRequestTransactionAsync(long shipmentRequestTransactionId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestTransactionAsync), shipmentRequestTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var shipmentRequestTransaction = await ctx.ShipmentRequestTransactions
                    .Where(r => r.ShipmentRequestTransactionId == shipmentRequestTransactionId)
                    .Select(r => Create.MFulfillment_ShipmentRequestTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = shipmentRequestTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentRequestTransactionSummaryList> GetShipmentRequestTransactionSummariesAsync(long? shipmentRequestId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestTransactionSummariesAsync), shipmentRequestId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.ShipmentRequest)
                {
                    return new MFulfillment_ShipmentRequestTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ShipmentRequestTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ShipmentRequestTransaction>)ctx.ShipmentRequestTransactions;
                if (shipmentRequestId != null)
                {
                    query = query.Where(r => r.ShipmentRequestId == shipmentRequestId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ShipmentRequestTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ShipmentRequestTransactionSummaryList
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

        public async Task<MFulfillment_ShipmentRequestEventLog> GetShipmentRequestEventLogAsync(long shipmentRequestEventId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestEventLogAsync), shipmentRequestEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var shipmentRequestEventLog = await ctx.ShipmentRequestEvents
                    .Where(r => r.ShipmentRequestEventId == shipmentRequestEventId)
                    .Select(r => Create.MFulfillment_ShipmentRequestEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = shipmentRequestEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentRequestEventLogSummaryList> GetShipmentRequestEventLogSummariesAsync(long? shipmentRequestId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentRequestEventLogSummariesAsync), shipmentRequestId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.ShipmentRequest)
                {
                    return new MFulfillment_ShipmentRequestEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ShipmentRequestEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ShipmentRequestEvent>)ctx.ShipmentRequestEvents.Include(r => r.ShipmentRequestTransaction);
                if (shipmentRequestId != null)
                {
                    query = query.Where(r => r.ShipmentRequestTransaction.ShipmentRequestId == shipmentRequestId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.ShipmentRequestTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ShipmentRequestEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ShipmentRequestEventLogSummaryList
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

        #region Shipment

        public async Task<MFulfillment_ShipmentSummaryList> GetShipmentSummariesAsync(MFulfillment_ShipmentStatus shipmentStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentSummariesAsync), shipmentStatus, recordCount);
            try
            {
                using var ctx = CreateQuiltContext();

                IQueryable<ShipmentSummaryView> query = ctx.ShipmentSummaryViews;

                switch (shipmentStatus)
                {
                    case MFulfillment_ShipmentStatus.MetaActive:
                        {
                            query = query.Where(r =>
                                r.ShipmentStatusCode == ShipmentStatusCodes.Open ||
                                r.ShipmentStatusCode == ShipmentStatusCodes.Posted ||
                                r.ShipmentStatusCode == ShipmentStatusCodes.Exception);
                            break;
                        }

                    case MFulfillment_ShipmentStatus.MetaAll:
                        break;

                    default:
                        {
                            var shipmentStatusCode = GetCode.From(shipmentStatus);
                            query = query.Where(r => r.ShipmentStatusCode == shipmentStatusCode);
                            break;
                        }
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var dbShipments = await query.ToListAsync().ConfigureAwait(false);

                var summaries = new List<MFulfillment_ShipmentSummary>();
                foreach (var dbShipment in dbShipments)
                {
                    summaries.Add(Create.MFulfillment_ShipmentSummary(dbShipment));
                }

                var result = new MFulfillment_ShipmentSummaryList()
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

        public async Task<MFulfillment_Shipment> GetShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentAsync), shipmentId);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbShipment = await ctx.Shipments.FindAsync(shipmentId).ConfigureAwait(false);

                var shipment = Create.MFulfillment_Shipment(dbShipment, shallow: false);

                var result = shipment;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateShipmentAsync(MFulfillment_CreateShipment createShipment)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CreateShipmentAsync), createShipment);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                ShipmentRequest dbShipmentRequest = null;
                foreach (var createShipmentItem in createShipment.CreateShipmentItems)
                {
                    var dbShipmentRequestItem = await ctx.ShipmentRequestItems.FindAsync(createShipmentItem.ShipmentRequestItemId).ConfigureAwait(false);
                    if (dbShipmentRequest == null)
                    {
                        dbShipmentRequest = dbShipmentRequestItem.ShipmentRequest;
                    }
                    else if (dbShipmentRequest != dbShipmentRequestItem.ShipmentRequest)
                    {
                        throw new InvalidOperationException("Shipment references multiple shipment requests.");
                    }
                }
                if (dbShipmentRequest == null)
                {
                    throw new InvalidOperationException("No shipment request specified.");
                }

                var fulfillableId = await ctx.FulfillableToShipmentRequestViews.Where(r => r.ShipmentRequestId == dbShipmentRequest.ShipmentRequestId).Select(r => r.FulfillableId).FirstAsync().ConfigureAwait(false);
                var dbFulfillable = await ctx.Fulfillables.FindAsync(fulfillableId).ConfigureAwait(false);
                var dbFulfillableAddress = dbFulfillable.FulfillableAddress;

                if (dbShipmentRequest.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Pending)
                {
                    dbShipmentRequest.ShipmentRequestStatusCode = ShipmentRequestStatusCodes.Open;
                    dbShipmentRequest.ShipmentRequestStatusDateTimeUtc = utcNow;
                }
                else if (dbShipmentRequest.ShipmentRequestStatusCode != ShipmentRequestStatusCodes.Open)
                {
                    throw new InvalidOperationException($"Shipment request {dbShipmentRequest.ShipmentRequestId} is not open.");
                }

                var shipmentNumber = ctx.GetShipmentNumber(utcNow);
                var dbShipment = new Shipment()
                {
                    ShipmentNumber = shipmentNumber,
                    CreateDateTimeUtc = utcNow,
                    ShipmentDateTimeUtc = createShipment.ShipmentDateTimeUtc,
                    ShipmentStatusCode = ShipmentStatusCodes.Open,
                    ShipmentStatusDateTimeUtc = utcNow,
                    TrackingCode = createShipment.TrackingCode,
                    ShippingVendorId = createShipment.ShippingVendorId,
                    UpdateDateTimeUtc = utcNow,
                    ShipmentAddress = new ShipmentAddress()
                    {
                        ShipToName = dbFulfillableAddress.ShipToName,
                        ShipToAddressLine1 = dbFulfillableAddress.ShipToAddressLine1,
                        ShipToAddressLine2 = dbFulfillableAddress.ShipToAddressLine2,
                        ShipToCity = dbFulfillableAddress.ShipToCity,
                        ShipToStateCode = dbFulfillableAddress.ShipToStateCode,
                        ShipToPostalCode = dbFulfillableAddress.ShipToPostalCode,
                        ShipToCountryCode = dbFulfillableAddress.ShipToCountryCode
                    }
                };
                _ = ctx.Shipments.Add(dbShipment);

                foreach (var item in createShipment.CreateShipmentItems)
                {
                    var dbShipmentRequestItem = dbShipmentRequest.ShipmentRequestItems.Where(r => r.ShipmentRequestItemId == item.ShipmentRequestItemId).First();
                    var dbShipmentItem = new ShipmentItem()
                    {
                        Shipment = dbShipment,
                        ShipmentRequestItemId = dbShipmentRequestItem.ShipmentRequestItemId,
                        Quantity = item.Quantity
                    };
                    _ = ctx.ShipmentItems.Add(dbShipmentItem);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbShipment.ShipmentId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateShipmentAsync(MFulfillment_UpdateShipment updateShipment)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(UpdateShipmentAsync), updateShipment);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                Shipment dbShipment = null;
                foreach (var updateShipmentItem in updateShipment.UpdateShipmentItems)
                {
                    var dbShipmentItem = await ctx.ShipmentItems.FindAsync(updateShipmentItem.ShipmentItemId).ConfigureAwait(false);
                    if (dbShipment == null)
                    {
                        dbShipment = dbShipmentItem.Shipment;
                    }
                    else if (dbShipment != dbShipmentItem.Shipment)
                    {
                        throw new InvalidOperationException("Multiple shipments referenced.");
                    }
                }
                if (dbShipment == null)
                {
                    throw new InvalidOperationException("No shipment specified.");
                }

                dbShipment.CreateDateTimeUtc = updateShipment.ShipmentDateTimeUtc;
                dbShipment.ShippingVendorId = updateShipment.ShippingVendorId;
                dbShipment.TrackingCode = updateShipment.TrackingCode;
                foreach (var updateShipmentItem in updateShipment.UpdateShipmentItems)
                {
                    var dbShipmentItem = dbShipment.ShipmentItems.Where(r => r.ShipmentItemId == updateShipmentItem.ShipmentItemId).First();
                    dbShipmentItem.Quantity = updateShipmentItem.Quantity;
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(PostShipmentAsync), shipmentId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

                if (!CanPost(dbShipment))
                {
                    throw new BusinessOperationException($"Shipment {shipmentId} cannot be posted.");
                }

                var unitOfWork = CreateUnitOfWork.PostShipment(shipmentId);

                _ = ctx.CreateShipmentTransactionBuilder()
                    .Begin(dbShipment, $"Posting shipment {shipmentId}.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ShipmentStatusCodes.Posted)
                    .Event(ShipmentEventTypeCodes.Post)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ProcessShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(ProcessShipmentAsync), shipmentId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

                if (dbShipment.ShipmentStatusCode != ShipmentStatusCodes.Posted)
                {
                    throw new InvalidOperationException($"Shipment {0} not posted.");
                }

                var unitOfWork = CreateUnitOfWork.ProcessShipment(shipmentId);

                // Create transaction.
                //
                {
                    var builder = ctx.CreateFulfillmentTransactionBuilder()
                            .Begin(utcNow)
                            .UnitOfWork(unitOfWork);
                    foreach (var dbShipmentItem in dbShipment.ShipmentItems)
                    {
                        _ = builder.SetCompleteQuantity(dbShipmentItem.ShipmentRequestItem.FulfillableItemId, dbShipmentItem.Quantity);
                    }
                    var dbFulfillmentEvent = builder
                        .Event(FulfillmentEventTypeCodes.Shipment)
                        .Create();
                }

                // Check to see if associated shipment request is complete.
                //
                var shipmentRequestId = await ctx.ShipmentRequestToShipmentViews.Where(r => r.ShipmentId == shipmentId).Select(r => r.ShipmentRequestId).FirstAsync().ConfigureAwait(false);
                var dbShipmentRequest = await ctx.ShipmentRequests.FindAsync(shipmentRequestId).ConfigureAwait(false);
                if (IsComplete(dbShipmentRequest))
                {
                    _ = ctx.CreateShipmentRequestTransactionBuilder()
                        .Begin(dbShipmentRequest, "Shipment request is complete", utcNow)
                        .UnitOfWork(unitOfWork)
                        .SetStatusTypeCode(ShipmentRequestStatusCodes.Complete)
                        .Event(ShipmentRequestEventTypeCodes.Complete)
                        .Create();
                }

                _ = ctx.CreateShipmentTransactionBuilder()
                    .Begin(dbShipment, "Shipment is complete.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ShipmentStatusCodes.Complete)
                    .Event(ShipmentEventTypeCodes.Process)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CancelShipmentAsync), shipmentId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

                if (!CanCancel(dbShipment))
                {
                    throw new BusinessOperationException($"Cannot cancel shipment request {shipmentId}.");
                }

                var unitOfWork = CreateUnitOfWork.CancelShipment(shipmentId);

                _ = ctx.CreateShipmentTransactionBuilder()
                    .Begin(dbShipment, $"Cancelling shipment {shipmentId}.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ShipmentStatusCodes.Cancelled)
                    .Event(ShipmentEventTypeCodes.Cancel)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                //var op = new ShipmentCancelOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
                //await op.ExecuteAsync(shipmentId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentTransaction> GetShipmentTransactionAsync(long shipmentTransactionId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentTransactionAsync), shipmentTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var shipmentTransaction = await ctx.ShipmentTransactions
                    .Where(r => r.ShipmentTransactionId == shipmentTransactionId)
                    .Select(r => Create.MFulfillment_ShipmentTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = shipmentTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentTransactionSummaryList> GetShipmentTransactionSummariesAsync(long? shipmentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentTransactionSummariesAsync), shipmentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Shipment)
                {
                    return new MFulfillment_ShipmentTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ShipmentTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ShipmentTransaction>)ctx.ShipmentTransactions;
                if (shipmentId != null)
                {
                    query = query.Where(r => r.ShipmentId == shipmentId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ShipmentTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ShipmentTransactionSummaryList
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

        public async Task<MFulfillment_ShipmentEventLog> GetShipmentEventLogAsync(long shipmentEventId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentEventLogAsync), shipmentEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var shipmentEventLog = await ctx.ShipmentEvents
                    .Where(r => r.ShipmentEventId == shipmentEventId)
                    .Select(r => Create.MFulfillment_ShipmentEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = shipmentEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ShipmentEventLogSummaryList> GetShipmentEventLogSummariesAsync(long? shipmentId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetShipmentEventLogSummariesAsync), shipmentId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Shipment)
                {
                    return new MFulfillment_ShipmentEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ShipmentEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ShipmentEvent>)ctx.ShipmentEvents.Include(r => r.ShipmentTransaction);
                if (shipmentId != null)
                {
                    query = query.Where(r => r.ShipmentTransaction.ShipmentId == shipmentId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.ShipmentTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ShipmentEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ShipmentEventLogSummaryList
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

        #region Return Request

        public async Task<MFulfillment_ReturnRequestSummaryList> GetReturnRequestSummariesAsync(MFulfillment_ReturnRequestStatus returnRequestStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestSummariesAsync), returnRequestStatus, recordCount);
            try
            {
                using var ctx = CreateQuiltContext();

                IQueryable<ReturnRequestSummaryView> query = ctx.ReturnRequestSummaryViews;

                switch (returnRequestStatus)
                {
                    case MFulfillment_ReturnRequestStatus.MetaActive:
                        {
                            query = query.Where(r =>
                                r.ReturnRequestStatusCode == ReturnRequestStatusCodes.Open ||
                                r.ReturnRequestStatusCode == ReturnRequestStatusCodes.Posted ||
                                r.ReturnRequestStatusCode == ReturnRequestStatusCodes.Exception);
                            break;
                        }

                    case MFulfillment_ReturnRequestStatus.MetaAll:
                        break;

                    default:
                        {
                            var returnRequestStatusCode = GetCode.From(returnRequestStatus);
                            query = query.Where(r => r.ReturnRequestStatusCode == returnRequestStatusCode);
                            break;
                        }
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var dbReturnRequests = await query.ToListAsync().ConfigureAwait(false);

                var summaries = new List<MFulfillment_ReturnRequestSummary>();
                foreach (var dbReturnRequest in dbReturnRequests)
                {
                    summaries.Add(Create.MFulfillment_ReturnRequestSummary(dbReturnRequest));
                }

                var result = new MFulfillment_ReturnRequestSummaryList()
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

        public async Task<MFulfillment_ReturnRequest> GetReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestAsync), returnRequestId);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbReturnRequest = await ctx.ReturnRequests.FindAsync(returnRequestId).ConfigureAwait(false);

                var result = Create.MFulfillment_ReturnRequest(dbReturnRequest, shallow: false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<MFulfillment_ReturnRequestReason>> GetReturnRequestReasonsAsync()
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestReasonsAsync));
            try
            {
                using var ctx = QuiltContextFactory.Create();

                var result = new List<MFulfillment_ReturnRequestReason>();

                var dbReasonTypes = await ctx.ReturnRequestReasons.Where(r => r.Active).OrderBy(r => r.SortOrder).ToListAsync().ConfigureAwait(false);
                foreach (var dbReasonType in dbReasonTypes)
                {
                    var reason = new MFulfillment_ReturnRequestReason()
                    {
                        ReturnRequestReasonTypeCode = dbReasonType.ReturnRequestReasonCode,
                        Name = dbReasonType.Name,
                        AllowRefund = dbReasonType.AllowRefund,
                        AllowReplacement = dbReasonType.AllowReplacement
                    };
                    result.Add(reason);
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

        public async Task<long> CreateReturnRequestAsync(MFulfillment_CreateReturnRequest createReturnRequest)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CreateReturnRequestAsync), createReturnRequest);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                var returnRequestNumber = ctx.GetReturnRequestNumber(utcNow);

                var dbOrderReturnRequest = new ReturnRequest()
                {
                    ReturnRequestNumber = returnRequestNumber,
                    CreateDateTimeUtc = utcNow,
                    ReturnRequestStatusCode = ReturnRequestStatusCodes.Open,
                    ReturnRequestStatusDateTimeUtc = utcNow,

                    ReturnRequestTypeCode = GetCode.From(createReturnRequest.ReturnRequestType),
                    ReturnRequestReasonCode = createReturnRequest.ReturnRequestReasonCode,
                    Notes = createReturnRequest.Notes,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.ReturnRequests.Add(dbOrderReturnRequest);

                foreach (var item in createReturnRequest.CreateReturnRequestItems)
                {
                    var dbOrderReturnRequestItem = new ReturnRequestItem()
                    {
                        ReturnRequest = dbOrderReturnRequest,
                        FulfillableItemId = item.FulfillableItemId,
                        Quantity = item.Quantity
                    };
                    _ = ctx.ReturnRequestItems.Add(dbOrderReturnRequestItem);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbOrderReturnRequest.ReturnRequestId;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateReturnRequestAsync(MFulfillment_UpdateReturnRequest updateReturnRequest)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(UpdateReturnRequestAsync), updateReturnRequest);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();
                var dbReturnRequest = await ctx.ReturnRequests.FindAsync(updateReturnRequest.ReturnRequestId).ConfigureAwait(false);

                if (dbReturnRequest.ReturnRequestStatusCode != ReturnRequestStatusCodes.Open)
                {
                    throw new ServiceException("Order return request is not open.");
                }

                // Update order return request.
                //
                dbReturnRequest.ReturnRequestTypeCode = GetCode.From(updateReturnRequest.ReturnRequestType);
                dbReturnRequest.ReturnRequestReasonCode = updateReturnRequest.ReturnRequestReasonCode;
                dbReturnRequest.Notes = updateReturnRequest.Notes;

                foreach (var item in updateReturnRequest.UpdateReturnRequestItems)
                {
                    var dbReturnRequestItem = dbReturnRequest.ReturnRequestItems.Where(r => r.ReturnRequestItemId == item.ReturnRequestItemId).Single();
                    dbReturnRequestItem.Quantity = item.Quantity;
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(PostReturnRequestAsync), returnRequestId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                // Locate return request.
                //
                var dbReturnRequest = await ctx.ReturnRequests.Where(r => r.ReturnRequestId == returnRequestId).SingleAsync().ConfigureAwait(false);

                //var orderId = ParseOrderId.FromFulfillableReference(dbReturnRequest.Fulfillable.FulfillableReference);
                //var dbOrder = await ctx.Orders.Where(r => r.OrderId == orderId).SingleAsync().ConfigureAwait(false);

                // Ensure user is associated with order.
                //
                //var ordererReference = CreateOrdererReference.FromUserId(userId);
                //if (dbOrder.Orderer.OrdererReference != ordererReference)
                //{
                //    throw new ServiceException("Order is not associated with specified user.");
                //}

                // Ensure request is open.
                //
                if (dbReturnRequest.ReturnRequestStatusCode != ReturnRequestStatusCodes.Open)
                {
                    throw new ServiceException("Order return request is not open.");
                }

                dbReturnRequest.ReturnRequestStatusCodeNavigation = ctx.ReturnRequestStatusType(ReturnRequestStatusCodes.Posted);
                dbReturnRequest.ReturnRequestStatusDateTimeUtc = GetUtcNow();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelReturnRequestAsync(long returnRequestId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CancelReturnRequestAsync), returnRequestId);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                using var ctx = QuiltContextFactory.Create();

                // Locate return request.
                //
                var dbReturnRequest = await ctx.ReturnRequests.Where(r => r.ReturnRequestId == returnRequestId).SingleAsync().ConfigureAwait(false);

                //var orderId = ParseOrderId.FromFulfillableReference(dbReturnRequest.Fulfillable.FulfillableReference);
                //var dbOrder = await ctx.Orders.Where(r => r.OrderId == orderId).SingleAsync().ConfigureAwait(false);

                // Ensure user is associated with order.
                //
                //var ordererReference = CreateOrdererReference.FromUserId(userId);
                //if (dbOrder.Orderer.OrdererReference != ordererReference)
                //{
                //    throw new ServiceException("Order is not associated with specified user.");
                //}

                // Ensure request is open or posted
                //
                if (dbReturnRequest.ReturnRequestStatusCode.In(ReturnRequestStatusCodes.Open, ReturnRequestStatusCodes.Posted))
                {
                    throw new ServiceException("Order return request is not open or posted.");
                }

                dbReturnRequest.ReturnRequestStatusCodeNavigation = ctx.ReturnRequestStatusType(ReturnRequestStatusCodes.Cancelled);
                dbReturnRequest.ReturnRequestStatusDateTimeUtc = GetUtcNow();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ReturnRequestTransaction> GetReturnRequestTransactionAsync(long returnRequestTransactionId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestTransactionAsync), returnRequestTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var returnRequestTransaction = await ctx.ReturnRequestTransactions
                    .Where(r => r.ReturnRequestTransactionId == returnRequestTransactionId)
                    .Select(r => Create.MFulfillment_ReturnRequestTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = returnRequestTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ReturnRequestTransactionSummaryList> GetReturnRequestTransactionSummariesAsync(long? returnRequestId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestTransactionSummariesAsync), returnRequestId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.ReturnRequest)
                {
                    return new MFulfillment_ReturnRequestTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ReturnRequestTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ReturnRequestTransaction>)ctx.ReturnRequestTransactions;
                if (returnRequestId != null)
                {
                    query = query.Where(r => r.ReturnRequestId == returnRequestId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ReturnRequestTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ReturnRequestTransactionSummaryList
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

        public async Task<MFulfillment_ReturnRequestEventLog> GetReturnRequestEventLogAsync(long returnRequestEventId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestEventLogAsync), returnRequestEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var returnRequestEventLog = await ctx.ReturnRequestEvents
                    .Where(r => r.ReturnRequestEventId == returnRequestEventId)
                    .Select(r => Create.MFulfillment_ReturnRequestEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = returnRequestEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ReturnRequestEventLogSummaryList> GetReturnRequestEventLogSummariesAsync(long? returnRequestId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnRequestEventLogSummariesAsync));
            try
            {
                if (source != null && source != MSources.ReturnRequest)
                {
                    return new MFulfillment_ReturnRequestEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ReturnRequestEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ReturnRequestEvent>)ctx.ReturnRequestEvents.Include(r => r.ReturnRequestTransaction);
                if (returnRequestId != null)
                {
                    query = query.Where(r => r.ReturnRequestTransaction.ReturnRequestId == returnRequestId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.ReturnRequestTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ReturnRequestEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ReturnRequestEventLogSummaryList
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

        #region Return

        public async Task<MFulfillment_ReturnSummaryList> GetReturnSummariesAsync(MFulfillment_ReturnStatus returnStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnSummariesAsync), returnStatus, recordCount);
            try
            {
                using var ctx = CreateQuiltContext();

                IQueryable<ReturnSummaryView> query = ctx.ReturnSummaryViews;

                switch (returnStatus)
                {
                    case MFulfillment_ReturnStatus.MetaActive:
                        {
                            query = query.Where(r =>
                                r.ReturnStatusCode == ReturnStatusCodes.Open ||
                                r.ReturnStatusCode == ReturnStatusCodes.Posted ||
                                r.ReturnStatusCode == ReturnStatusCodes.Exception);
                            break;
                        }

                    case MFulfillment_ReturnStatus.MetaAll:
                        break;

                    default:
                        {
                            var returnStatusCode = GetCode.From(returnStatus);
                            query = query.Where(r => r.ReturnStatusCode == returnStatusCode);
                            break;
                        }
                }

                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var dbReturns = await query.ToListAsync().ConfigureAwait(false);

                var summaries = new List<MFulfillment_ReturnSummary>();
                foreach (var dbReturn in dbReturns)
                {
                    summaries.Add(Create.MFulfillment_ReturnSummary(dbReturn));
                }

                var result = new MFulfillment_ReturnSummaryList()
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

        public async Task<MFulfillment_Return> GetReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnAsync), returnId);
            try
            {
                using var ctx = CreateQuiltContext();

                var dbReturn = await ctx.Returns.FindAsync(returnId).ConfigureAwait(false);

                var result = Create.MFulfillment_Return(dbReturn, shallow: false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateReturnAsync(MFulfillment_CreateReturn createReturn)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CreateReturnAsync), createReturn);
            try
            {
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                // Ensure request doesn't specify a return request ID.
                //

                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                ReturnRequest dbReturnRequest = null;
                foreach (var createReturnItem in createReturn.CreateReturnItems)
                {
                    var dbReturnRequestItem = await ctx.ReturnRequestItems.FindAsync(createReturnItem.ReturnRequestItemId).ConfigureAwait(false);
                    if (dbReturnRequest == null)
                    {
                        dbReturnRequest = dbReturnRequestItem.ReturnRequest;
                    }
                    else if (dbReturnRequest != dbReturnRequestItem.ReturnRequest)
                    {
                        throw new InvalidOperationException("Return references multiple return requests.");
                    }
                }
                if (dbReturnRequest == null)
                {
                    throw new InvalidOperationException("No return request specified.");
                }

                var returnNumber = ctx.GetReturnNumber(utcNow);
                var dbReturn = new Return()
                {
                    ReturnNumber = returnNumber,
                    ReturnStatusCode = ReturnStatusCodes.Open,
                    ReturnStatusDateTimeUtc = utcNow,
                    CreateDateTimeUtc = utcNow,
                    UpdateDateTimeUtc = utcNow
                };
                _ = ctx.Returns.Add(dbReturn);

                foreach (var dbReturnRequestItem in dbReturnRequest.ReturnRequestItems)
                {
                    var dbReturnItem = new ReturnItem()
                    {
                        Return = dbReturn,
                        ReturnRequestItemId = dbReturnRequestItem.ReturnRequestItemId,
                        Quantity = dbReturnRequestItem.Quantity
                    };
                    _ = ctx.ReturnItems.Add(dbReturnItem);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbReturn.ReturnId;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateReturnAsync(MFulfillment_UpdateReturn updateReturn)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(UpdateReturnAsync), updateReturn);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = QuiltContextFactory.Create();

                Return dbReturn = null;
                foreach (var updateReturnItem in updateReturn.UpdateReturnItems)
                {
                    var dbReturnItem = await ctx.ReturnItems.FindAsync(updateReturnItem.ReturnItemId).ConfigureAwait(false);
                    if (dbReturn == null)
                    {
                        dbReturn = dbReturnItem.Return;
                    }
                    else if (dbReturn != dbReturnItem.Return)
                    {
                        throw new InvalidOperationException("Multiple returns referenced.");
                    }
                }
                if (dbReturn == null)
                {
                    throw new InvalidOperationException("No return specified.");
                }

                dbReturn.CreateDateTimeUtc = updateReturn.CreateDateTimeUtc;
                foreach (var updateReturnItem in updateReturn.UpdateReturnItems)
                {
                    var dbReturnItem = dbReturn.ReturnItems.Where(r => r.ReturnItemId == updateReturnItem.ReturnItemId).First();
                    dbReturnItem.Quantity = updateReturnItem.Quantity;
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(PostReturnAsync), returnId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbReturn = await ctx.Returns.Where(r => r.ReturnId == returnId).SingleAsync().ConfigureAwait(false);

                if (!CanPost(dbReturn))
                {
                    throw new BusinessOperationException($"Return {returnId} cannot be posted.");
                }

                var unitOfWork = CreateUnitOfWork.PostReturn(returnId);

                _ = ctx.CreateReturnTransactionBuilder()
                    .Begin(dbReturn, $"Posting return {returnId}.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ReturnStatusCodes.Posted)
                    .Event(ReturnEventTypeCodes.Post)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ProcessReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(ProcessReturnAsync), returnId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbReturn = await ctx.Returns
                    .Include(r => r.ReturnItems)
                        .ThenInclude(r => r.ReturnRequestItem)
                            .ThenInclude(r => r.ReturnRequest)
                    .Where(r => r.ReturnId == returnId)
                    .SingleAsync().ConfigureAwait(false);

                if (dbReturn.ReturnStatusCode != ReturnStatusCodes.Posted)
                {
                    throw new InvalidOperationException($"Return {0} not posted.");
                }

                var unitOfWork = CreateUnitOfWork.ProcessReturn(returnId);

                // Update the fulfillment.
                //
                {
                    var builder = ctx.CreateFulfillmentTransactionBuilder()
                            .Begin(utcNow)
                            .UnitOfWork(unitOfWork);
                    foreach (var dbReturnItem in dbReturn.ReturnItems)
                    {
                        _ = builder.SetReturnQuantity(dbReturnItem.ReturnRequestItem.FulfillableItemId, dbReturnItem.Quantity);
                    }
                    var dbFulfillmentEvent = builder
                        .Event(FulfillmentEventTypeCodes.Return)
                        .Create();
                }

                // Complete the return.
                //
                _ = ctx.CreateReturnTransactionBuilder()
                    .Begin(returnId, utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ReturnStatusCodes.Complete)
                    .Event(ReturnEventTypeCodes.Process)
                    .Create();

                // Complete associated return requests.
                //
                var returnRequestIds = new HashSet<long>();
                foreach (var dbReturnItem in dbReturn.ReturnItems)
                {
                    _ = returnRequestIds.Add(dbReturnItem.ReturnRequestItem.ReturnRequestId);
                }
                foreach (var returnRequestId in returnRequestIds)
                {
                    var dbReturnRequest = await ctx.ReturnRequests.FindAsync(returnRequestId).ConfigureAwait(false);
                    if (IsComplete(dbReturnRequest))
                    {
                        _ = ctx.CreateReturnRequestTransactionBuilder()
                            .Begin(returnRequestId, utcNow)
                            .UnitOfWork(unitOfWork)
                            .SetStatusTypeCode(ReturnRequestStatusCodes.Complete)
                            .Event(ReturnRequestEventTypeCodes.Process)
                            .Create();
                    }
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelReturnAsync(long returnId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(CancelReturnAsync), returnId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbReturn = await ctx.Returns.Where(r => r.ReturnId == returnId).SingleAsync().ConfigureAwait(false);

                if (!CanCancel(dbReturn))
                {
                    throw new BusinessOperationException($"Cannot cancel return request {returnId}.");
                }

                var unitOfWork = CreateUnitOfWork.CancelReturn(returnId);

                _ = ctx.CreateReturnTransactionBuilder()
                    .Begin(dbReturn, $"Cancelling return {returnId}.", utcNow)
                    .UnitOfWork(unitOfWork)
                    .SetStatusTypeCode(ReturnStatusCodes.Cancelled)
                    .Event(ReturnEventTypeCodes.Cancel)
                    .Create();

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                //var op = new ReturnCancelOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
                //await op.ExecuteAsync(returnId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ReturnTransaction> GetReturnTransactionAsync(long returnTransactionId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnTransactionAsync), returnTransactionId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var returnTransaction = await ctx.ReturnTransactions
                    .Where(r => r.ReturnTransactionId == returnTransactionId)
                    .Select(r => Create.MFulfillment_ReturnTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = returnTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }

        }

        public async Task<MFulfillment_ReturnTransactionSummaryList> GetReturnTransactionSummariesAsync(long? returnId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnTransactionSummariesAsync), returnId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Return)
                {
                    return new MFulfillment_ReturnTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ReturnTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ReturnTransaction>)ctx.ReturnTransactions;
                if (returnId != null)
                {
                    query = query.Where(r => r.ReturnId == returnId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ReturnTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ReturnTransactionSummaryList
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

        public async Task<MFulfillment_ReturnEventLog> GetReturnEventLogAsync(long returnEventId)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnEventLogAsync), returnEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var returnEventLog = await ctx.ReturnEvents
                    .Where(r => r.ReturnEventId == returnEventId)
                    .Select(r => Create.MFulfillment_ReturnEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = returnEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MFulfillment_ReturnEventLogSummaryList> GetReturnEventLogSummariesAsync(long? returnId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(GetReturnEventLogSummariesAsync), returnId, unitOfWork);
            try
            {
                if (source != null && source != MSources.Return)
                {
                    return new MFulfillment_ReturnEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MFulfillment_ReturnEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<ReturnEvent>)ctx.ReturnEvents.Include(r => r.ReturnTransaction);
                if (returnId != null)
                {
                    query = query.Where(r => r.ReturnTransaction.ReturnId == returnId.Value);
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.ReturnTransaction.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Select(r => Create.MFulfillment_ReturnEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MFulfillment_ReturnEventLogSummaryList
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

        #region Event

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(FulfillmentMicroService), nameof(ProcessEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = 0;

                count += await ProcessFulfillmentEvents(ctx).ConfigureAwait(false);
                count += await ProcessShipmentRequestEvents(ctx).ConfigureAwait(false);
                count += await ProcessShipmentEvents(ctx).ConfigureAwait(false);
                count += await ProcessReturnRequestEvents(ctx).ConfigureAwait(false);
                count += await ProcessReturnEvents(ctx).ConfigureAwait(false);

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

        public Task<int> CancelEventsAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        private async Task<int> ProcessFulfillmentEvents(QuiltContext ctx)
        {
            int count = 0;

            var dbFulfillableEvents = await ctx.FulfillableEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
            foreach (var dbFulfillableEvent in dbFulfillableEvents)
            {
                count += 1;

                try
                {
                    var dbTransaction = dbFulfillableEvent.FulfillableTransaction;
                    var fulfillmentEvent = new MFulfillment_FulfillableEvent()
                    {
                        FulfillableEventId = dbFulfillableEvent.FulfillableEventId,
                        EventType = GetValue.MFulfillment_FulfillmentEventType(dbFulfillableEvent.EventTypeCode),
                        UnitOfWork = dbTransaction.UnitOfWork,
                        FulfillableEventItems = new List<MFulfillment_FulfillableEventItem>()
                    };

                    foreach (var dbTransactionItem in dbTransaction.FulfillableTransactionItems)
                    {
                        var dbFulfillable = dbTransactionItem.FulfillableItem;
                        fulfillmentEvent.FulfillableEventItems.Add(new MFulfillment_FulfillableEventItem()
                        {
                            FulfillableItemId = dbTransactionItem.FulfillableItemId,
                            FulfillmentItemReference = dbFulfillable.FulfillableItemReference,
                            FulfillmentRequiredQuantity = dbFulfillable.RequestQuantity,
                            FulfillmentReturnQuantity = dbFulfillable.ReturnQuantity,
                            FulfillmentCompleteQuantity = dbFulfillable.CompleteQuantity
                        });
                    }

                    await FulfillmentEventService.HandleFulfillmentEventAsync(fulfillmentEvent).ConfigureAwait(false);

                    dbFulfillableEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //log.LogException(ex);

                    dbFulfillableEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return count;
        }

        private async Task<int> ProcessShipmentRequestEvents(QuiltContext ctx)
        {
            int count = 0;

            var dbShipmentRequestEvents = await ctx.ShipmentRequestEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
            foreach (var dbShipmentRequestEvent in dbShipmentRequestEvents)
            {
                count += 1;

                try
                {
                    var dbTransaction = dbShipmentRequestEvent.ShipmentRequestTransaction;
                    var eventData = new MFulfillment_ShipmentRequestEvent()
                    {
                        EventType = GetValue.MFulfillment_ShipmentRequestEventType(dbShipmentRequestEvent.EventTypeCode),
                        UnitOfWork = dbTransaction.UnitOfWork,
                        ShipmentRequestId = dbTransaction.ShipmentRequestId
                    };

                    await FulfillmentEventService.HandleShipmentRequestEventAsync(eventData).ConfigureAwait(false);

                    dbShipmentRequestEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //log.LogException(ex);

                    dbShipmentRequestEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return count;
        }

        private async Task<int> ProcessShipmentEvents(QuiltContext ctx)
        {
            int count = 0;

            var dbShipmentEvents = await ctx.ShipmentEvents
                .Include(r => r.ShipmentTransaction)
                    .ThenInclude(r => r.Shipment)
                        .ThenInclude(r => r.ShipmentItems)
                            .ThenInclude(r => r.ShipmentRequestItem)
                                .ThenInclude(r => r.ShipmentRequest)
                .Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending)
                .ToListAsync().ConfigureAwait(false);

            foreach (var dbShipmentEvent in dbShipmentEvents)
            {
                count += 1;

                try
                {
                    var dbTransaction = dbShipmentEvent.ShipmentTransaction;

                    var shipmentRequestIds = new HashSet<long>(1);
                    foreach (var dbShipmentItem in dbTransaction.Shipment.ShipmentItems)
                    {
                        _ = shipmentRequestIds.Add(dbShipmentItem.ShipmentRequestItem.ShipmentRequestId);
                    }

                    var eventData = new MFulfillment_ShipmentEvent()
                    {
                        EventType = GetValue.MFulfillment_ShipmentEventType(dbShipmentEvent.EventTypeCode),
                        UnitOfWork = dbTransaction.UnitOfWork,
                        ShipmentId = dbTransaction.ShipmentId,
                        ShipmentRequestIds = shipmentRequestIds.ToList()
                    };

                    await FulfillmentEventService.HandleShipmentEventAsync(eventData).ConfigureAwait(false);

                    dbShipmentEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //log.LogException(ex);

                    dbShipmentEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return count;
        }

        private async Task<int> ProcessReturnRequestEvents(QuiltContext ctx)
        {
            int count = 0;

            var dbReturnRequestEvents = await ctx.ReturnRequestEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
            foreach (var dbReturnRequestEvent in dbReturnRequestEvents)
            {
                count += 1;

                try
                {
                    var dbTransaction = dbReturnRequestEvent.ReturnRequestTransaction;

                    var eventData = new MFulfillment_ReturnRequestEvent()
                    {
                        EventType = GetValue.MFulfillment_ReturnRequestEventType(dbReturnRequestEvent.EventTypeCode),
                        UnitOfWork = dbTransaction.UnitOfWork,
                        ReturnRequestId = dbTransaction.ReturnRequestId
                    };

                    await FulfillmentEventService.HandleReturnRequestEventAsync(eventData).ConfigureAwait(false);

                    dbReturnRequestEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //log.LogException(ex);

                    dbReturnRequestEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return count;
        }

        private async Task<int> ProcessReturnEvents(QuiltContext ctx)
        {
            int count = 0;

            var dbReturnEvents = await ctx.ReturnEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);
            foreach (var dbReturnEvent in dbReturnEvents)
            {
                count += 1;

                try
                {
                    var dbTransaction = dbReturnEvent.ReturnTransaction;

                    var returnRequestIds = new HashSet<long>(1);
                    foreach (var dbReturnItem in dbTransaction.Return.ReturnItems)
                    {
                        _ = returnRequestIds.Add(dbReturnItem.ReturnRequestItem.ReturnRequestId);
                    }

                    var eventData = new MFulfillment_ReturnEvent()
                    {
                        EventType = GetValue.MFulfillment_ReturnEventType(dbReturnEvent.EventTypeCode),
                        UnitOfWork = dbTransaction.UnitOfWork,
                        ReturnId = dbTransaction.ReturnId,
                        ReturnRequestIds = returnRequestIds.ToList()
                    };

                    await FulfillmentEventService.HandleReturnEventAsync(eventData).ConfigureAwait(false);

                    dbReturnEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //log.LogException(ex);

                    dbReturnEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return count;
        }

        private async Task<FulfillableAddress> CreateAddress(QuiltContext ctx, MCommon_Address shippingAddress)
        {
            var dbFulfillableAddress = await ctx.FulfillableAddresses.Where(r =>
                r.ShipToName == shippingAddress.Name &&
                r.ShipToAddressLine1 == shippingAddress.AddressLine1 &&
                r.ShipToAddressLine2 == shippingAddress.AddressLine2 &&
                r.ShipToCity == shippingAddress.City &&
                r.ShipToStateCode == shippingAddress.StateCode &&
                r.ShipToPostalCode == shippingAddress.PostalCode &&
                r.ShipToCountryCode == shippingAddress.CountryCode).FirstOrDefaultAsync().ConfigureAwait(false);

            if (dbFulfillableAddress == null)
            {
                dbFulfillableAddress = new FulfillableAddress()
                {
                    ShipToName = shippingAddress.Name,
                    ShipToAddressLine1 = shippingAddress.AddressLine1,
                    ShipToAddressLine2 = shippingAddress.AddressLine2,
                    ShipToCity = shippingAddress.City,
                    ShipToStateCode = shippingAddress.StateCode,
                    ShipToPostalCode = shippingAddress.PostalCode,
                    ShipToCountryCode = shippingAddress.CountryCode
                };
                _ = ctx.FulfillableAddresses.Add(dbFulfillableAddress);
            }

            return dbFulfillableAddress;
        }

        private static bool IsComplete(ShipmentRequest dbShipmentRequest)
        {
            var isComplete = true; // assume success.
            foreach (var dbShipmentRequestItem in dbShipmentRequest.ShipmentRequestItems)
            {
                if (dbShipmentRequestItem.FulfillableItem.CompleteQuantity < dbShipmentRequestItem.Quantity)
                {
                    isComplete = false;
                    break;
                }
            }

            return isComplete;
        }

        private static bool IsComplete(ReturnRequest dbReturnRequest)
        {
            foreach (var dbReturnRequestItem in dbReturnRequest.ReturnRequestItems)
            {
                int returnedQuantity = 0;
                foreach (var dbReturnItem in dbReturnRequestItem.ReturnItems)
                {
                    if (dbReturnItem.Return.ReturnStatusCode == ReturnStatusCodes.Complete)
                    {
                        returnedQuantity += dbReturnItem.Quantity;
                    }
                }

                if (returnedQuantity < dbReturnRequestItem.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanCreateShipment(ShipmentRequest dbShipmentRequest)
        {
            // Can only create shipments for open shipment requests.
            //
            return dbShipmentRequest.ShipmentRequestStatusCode == ShipmentRequestStatusCodes.Pending;
        }

        private static bool CanCancel(ShipmentRequest dbShipmentRequest)
        {
            // Can only cancel open shipment requests.
            //
            if (dbShipmentRequest.ShipmentRequestStatusCode != ShipmentRequestStatusCodes.Open)
            {
                return false;
            }

            // See if any associated shipment has been posted or completed.
            //
            foreach (var dbShipmentRequestItem in dbShipmentRequest.ShipmentRequestItems)
            {
                foreach (var dbShipmentItem in dbShipmentRequestItem.ShipmentItems)
                {
                    if (dbShipmentItem.Shipment.ShipmentStatusCode.In(ShipmentStatusCodes.Posted, ShipmentStatusCodes.Complete))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CanEdit(Shipment dbShipment)
        {
            return dbShipment.ShipmentStatusCode == ShipmentStatusCodes.Open;
        }

        private static bool CanPost(Shipment dbShipment)
        {
            return dbShipment.ShipmentStatusCode == ShipmentStatusCodes.Open;
        }

        private static bool CanProcess(Shipment dbShipment)
        {
            return dbShipment.ShipmentStatusCode == ShipmentStatusCodes.Posted;
        }

        private static bool CanCancel(Shipment dbShipment)
        {
            return dbShipment.ShipmentStatusCode.In(ShipmentStatusCodes.Open, ShipmentStatusCodes.Posted);
        }

        private static bool CanEdit(ReturnRequest dbReturnRequest)
        {
            return dbReturnRequest.ReturnRequestStatusCode == ReturnRequestStatusCodes.Open;
        }

        private static bool CanCreateReturn(ReturnRequest dbReturnRequest)
        {
            return dbReturnRequest.ReturnRequestStatusCode == ReturnRequestStatusCodes.Open;
        }

        private static bool CanEdit(Return dbReturn)
        {
            return dbReturn.ReturnStatusCode == ReturnStatusCodes.Open;
        }

        private static bool CanPost(Return dbReturn)
        {
            return dbReturn.ReturnStatusCode == ReturnStatusCodes.Open;
        }

        private static bool CanProcess(Return dbReturn)
        {
            return dbReturn.ReturnStatusCode == ReturnStatusCodes.Posted;
        }

        private static bool CanCancel(Return dbReturn)
        {
            return dbReturn.ReturnStatusCode.In(ReturnStatusCodes.Open, ReturnStatusCodes.Posted);
        }

        private static bool CanCreateReturnRequest(Fulfillable dbFulfillable)
        {
            return dbFulfillable.FulfillableItems.Any(r => r.CompleteQuantity > r.ReturnQuantity);
        }

        private static async Task<IList<ShipmentRequest>> LoadShipmentRequestsAsync(QuiltContext ctx, long fulfillableId)
        {
            var shipmentRequestIds = await ctx.FulfillableToShipmentRequestViews.Where(r => r.FulfillableId == fulfillableId).Select(r => r.ShipmentRequestId).ToListAsync().ConfigureAwait(false);
            List<ShipmentRequest> dbShipmentRequests = null;
            foreach (var shipmentRequestId in shipmentRequestIds)
            {
                var items = await ctx.ShipmentRequests
                    .Include(r => r.ShipmentRequestItems)
                    .Where(r => r.ShipmentRequestId == shipmentRequestId)
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbShipmentRequests == null)
                {
                    dbShipmentRequests = items;
                }
                else
                {
                    dbShipmentRequests.AddRange(items);
                }
            }

            return dbShipmentRequests;
        }

        private static async Task<IList<Shipment>> LoadShipmentsAsync(QuiltContext ctx, long fulfillableId)
        {
            var shipmentIds = await ctx.FulfillableToShipmentViews.Where(r => r.FulfillableId == fulfillableId).Select(r => r.ShipmentId).ToListAsync().ConfigureAwait(false);
            List<Shipment> dbShipments = null;
            foreach (var shipmentId in shipmentIds)
            {
                var items = await ctx.Shipments
                    .Include(r => r.ShipmentItems)
                    .Where(r => r.ShipmentId == shipmentId)
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbShipments == null)
                {
                    dbShipments = items;
                }
                else
                {
                    dbShipments.AddRange(items);
                }
            }

            return dbShipments;
        }

        private static async Task<IList<ReturnRequest>> LoadReturnRequestsAsync(QuiltContext ctx, long fulfillableId)
        {
            var returnRequestIds = await ctx.FulfillableToReturnRequestViews.Where(r => r.FulfillableId == fulfillableId).Select(r => r.ReturnRequestId).ToListAsync().ConfigureAwait(false);
            List<ReturnRequest> dbReturnRequests = null;
            foreach (var returnRequestId in returnRequestIds)
            {
                var items = await ctx.ReturnRequests
                    .Include(r => r.ReturnRequestItems)
                    .Where(r => r.ReturnRequestId == returnRequestId)
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbReturnRequests == null)
                {
                    dbReturnRequests = items;
                }
                else
                {
                    dbReturnRequests.AddRange(items);
                }
            }

            return dbReturnRequests;
        }

        private static async Task<IList<Return>> LoadReturnsAsync(QuiltContext ctx, long fulfillableId)
        {
            var returnIds = await ctx.FulfillableToReturnViews.Where(r => r.FulfillableId == fulfillableId).Select(r => r.ReturnId).ToListAsync().ConfigureAwait(false);
            List<Return> dbReturns = null;
            foreach (var returnId in returnIds)
            {
                var items = await ctx.Returns
                    .Include(r => r.ReturnItems)
                    .Where(r => r.ReturnId == returnId)
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbReturns == null)
                {
                    dbReturns = items;
                }
                else
                {
                    dbReturns.AddRange(items);
                }
            }

            return dbReturns;
        }

        private static class Create
        {
            #region Address

            public static MFulfillment_Address MFulfillment_Address(FulfillableAddress dbFulfillableAddress)
            {
                return dbFulfillableAddress != null
                    ? new MFulfillment_Address()
                    {
                        Name = dbFulfillableAddress.ShipToName,
                        AddressLine1 = dbFulfillableAddress.ShipToAddressLine1,
                        AddressLine2 = dbFulfillableAddress.ShipToAddressLine2,
                        City = dbFulfillableAddress.ShipToCity,
                        StateCode = dbFulfillableAddress.ShipToStateCode,
                        PostalCode = dbFulfillableAddress.ShipToPostalCode,
                        CountryCode = dbFulfillableAddress.ShipToCountryCode
                    }
                    : null;
            }

            public static MFulfillment_Address MFulfillment_Address(ShipmentAddress dbShipmentAddress)
            {
                return dbShipmentAddress != null
                    ? new MFulfillment_Address()
                    {
                        Name = dbShipmentAddress.ShipToName,
                        AddressLine1 = dbShipmentAddress.ShipToAddressLine1,
                        AddressLine2 = dbShipmentAddress.ShipToAddressLine2,
                        City = dbShipmentAddress.ShipToCity,
                        StateCode = dbShipmentAddress.ShipToStateCode,
                        PostalCode = dbShipmentAddress.ShipToPostalCode,
                        CountryCode = dbShipmentAddress.ShipToCountryCode
                    }
                    : null;
            }

            public static MFulfillment_Address MFulfillment_Address(ShipmentRequestAddress dbShipmentRequestAddress)
            {
                return dbShipmentRequestAddress != null
                    ? new MFulfillment_Address()
                    {
                        Name = dbShipmentRequestAddress.ShipToName,
                        AddressLine1 = dbShipmentRequestAddress.ShipToAddressLine1,
                        AddressLine2 = dbShipmentRequestAddress.ShipToAddressLine2,
                        City = dbShipmentRequestAddress.ShipToCity,
                        StateCode = dbShipmentRequestAddress.ShipToStateCode,
                        PostalCode = dbShipmentRequestAddress.ShipToPostalCode,
                        CountryCode = dbShipmentRequestAddress.ShipToCountryCode
                    }
                    : null;
            }

            #endregion

            #region Fulfillable

            public static MFulfillment_Fulfillable MFulfillment_Fulfillable(
                Fulfillable dbFulfillable,
                IList<ShipmentRequest> dbShipmentRequests,
                IList<Shipment> dbShipments,
                IList<ReturnRequest> dbReturnRequests,
                IList<Return> dbReturns)
            {
                return new MFulfillment_Fulfillable()
                {
                    FulfillableId = dbFulfillable.FulfillableId,
                    FulfillableReference = dbFulfillable.FulfillableReference,
                    Name = dbFulfillable.Name,
                    ShipToAddress = MFulfillment_Address(dbFulfillable.FulfillableAddress), // FulfillableAddressId
                    CreateDateTimeUtc = dbFulfillable.CreateDateTimeUtc,
                    FulfillableStatus = GetValue.MFulfillment_FulfillableStatus(dbFulfillable.FulfillableStatusCode),
                    StatusDateTimeUtc = dbFulfillable.FulfillableStatusDateTimeUtc,
                    UpdateDateTimeUtc = dbFulfillable.UpdateDateTimeUtc,

                    CanCreateReturnRequest = CanCreateReturnRequest(dbFulfillable),

                    FulfillableItems = MFulfillment_FulfillableItems(dbFulfillable.FulfillableItems),
                    ShipmentRequests = MFulfillment_ShipmentRequests(dbShipmentRequests, true),
                    Shipments = MFulfillment_Shipments(dbShipments, true),
                    ReturnRequests = MFulfillment_ReturnRequests(dbReturnRequests, true),
                    Returns = MFulfillment_Returns(dbReturns, true),
                };
            }

            public static MFulfillment_FulfillableItem MFulfillment_FulfillableItem(FulfillableItem dbFulfillableItem)
            {
                return new MFulfillment_FulfillableItem()
                {
                    FulfillableItemId = dbFulfillableItem.FulfillableItemId,
                    FulfillableItemReference = dbFulfillableItem.FulfillableItemReference,
                    Description = dbFulfillableItem.Description,
                    ConsumableReference = dbFulfillableItem.ConsumableReference,
                    RequestQuantity = dbFulfillableItem.RequestQuantity,
                    CompleteQuantity = dbFulfillableItem.CompleteQuantity,
                    ReturnQuantity = dbFulfillableItem.ReturnQuantity,
                    UpdateDateTimeUtc = dbFulfillableItem.UpdateDateTimeUtc,
                    FulfillableItemComponents = MFulfillment_FulfillableItemComponents(dbFulfillableItem.FulfillableItemComponents)
                };
            }

            public static IList<MFulfillment_FulfillableItem> MFulfillment_FulfillableItems(IEnumerable<FulfillableItem> dbFulfillableItems)
            {
                return dbFulfillableItems?.Select(r => MFulfillment_FulfillableItem(r)).ToList();
            }

            public static MFulfillment_FulfillableItemComponent MFulfillment_FulfillableItemComponent(FulfillableItemComponent dbFulfillableItemComponent)
            {
                return new MFulfillment_FulfillableItemComponent()
                {
                    FulfillableItemComponentId = dbFulfillableItemComponent.FulfillableItemComponentId,
                    Description = dbFulfillableItemComponent.Description,
                    ConsumableReference = dbFulfillableItemComponent.ConsumableReference,
                    Quantity = dbFulfillableItemComponent.Quantity,
                    UpdateDateTimeUtc = dbFulfillableItemComponent.UpdateDateTimeUtc
                };
            }

            public static IList<MFulfillment_FulfillableItemComponent> MFulfillment_FulfillableItemComponents(IEnumerable<FulfillableItemComponent> dbFulfillableItemComponents)
            {
                return dbFulfillableItemComponents?.Select(r => MFulfillment_FulfillableItemComponent(r)).ToList();
            }

            public static MFulfillment_FulfillableSummary MFulfillment_FulfillableSummary(Fulfillable dbFulfillable)
            {
                return new MFulfillment_FulfillableSummary()
                {
                    FulfillableId = dbFulfillable.FulfillableId,
                    Name = dbFulfillable.Name,
                    FulfillableReference = dbFulfillable.FulfillableReference,
                    FulfillableStatus = GetValue.MFulfillment_FulfillableStatus(dbFulfillable.FulfillableStatusCode),
                    CreateDateTimeUtc = dbFulfillable.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbFulfillable.FulfillableStatusDateTimeUtc,
                    TotalFulfillmentRequiredQuantity = dbFulfillable.FulfillableItems.Select(r => r.RequestQuantity).Sum(),
                    TotalFulfillmentCompleteQuantity = dbFulfillable.FulfillableItems.Select(r => r.CompleteQuantity).Sum(),
                    TotalFulfillmentReturnQuantity = dbFulfillable.FulfillableItems.Select(r => r.ReturnQuantity).Sum(),
                };
            }

            public static MFulfillment_FulfillableTransaction MFulfillment_FulfillableTransaction(FulfillableTransaction dbFulfillableTransaction)
            {
                return new MFulfillment_FulfillableTransaction()
                {
                    TransactionId = dbFulfillableTransaction.FulfillableTransactionId,
                    EntityId = dbFulfillableTransaction.FulfillableTransactionItems.Select(r => r.FulfillableItem.FulfillableId).First(),
                    TransactionDateTimeUtc = dbFulfillableTransaction.TransactionDateTimeUtc,
                    Description = dbFulfillableTransaction.Description,
                    UnitOfWork = dbFulfillableTransaction.UnitOfWork,

                    Items = MFulfillment_FulfillableTransactionItems(dbFulfillableTransaction.FulfillableTransactionItems)
                };
            }

            public static MFulfillment_FulfillableTransactionItem MFulfillment_FulfillableTransactionItem(FulfillableTransactionItem dbFulfillableransactionItem)
            {
                return new MFulfillment_FulfillableTransactionItem()
                {
                    FulfillableTransactionItemId = dbFulfillableransactionItem.FulfillableTransactionItemId,
                    FulfillableItemId = dbFulfillableransactionItem.FulfillableItemId,
                    RequestQuantity = dbFulfillableransactionItem.RequestQuantity,
                    CompleteQuantity = dbFulfillableransactionItem.CompleteQuantity,
                    ReturnQuantity = dbFulfillableransactionItem.ReturnQuantity,
                    ConsumeQuantity = dbFulfillableransactionItem.ConsumeQuantity
                };
            }

            public static IList<MFulfillment_FulfillableTransactionItem> MFulfillment_FulfillableTransactionItems(IEnumerable<FulfillableTransactionItem> dbFulfillableTransactionItems)
            {
                return dbFulfillableTransactionItems?.Select(r => MFulfillment_FulfillableTransactionItem(r)).ToList();
            }

            public static MFulfillment_FulfillableTransactionSummary MFulfillment_FulfillableTransactionSummary(FulfillableTransaction dbFulfillableTransaction)
            {
                return new MFulfillment_FulfillableTransactionSummary()
                {
                    TransactionId = dbFulfillableTransaction.FulfillableTransactionId,
                    EntityId = dbFulfillableTransaction.FulfillableTransactionItems.Select(r => r.FulfillableItem.FulfillableId).First(),
                    TransactionDateTimeUtc = dbFulfillableTransaction.TransactionDateTimeUtc,
                    Description = dbFulfillableTransaction.Description,
                    UnitOfWork = dbFulfillableTransaction.UnitOfWork
                };
            }

            public static MFulfillment_FulfillableEventLog MFulfillment_FulfillableEventLog(FulfillableEvent dbFulfillableEvent)
            {
                return new MFulfillment_FulfillableEventLog()
                {
                    EventId = dbFulfillableEvent.FulfillableEventId,
                    TransactionId = dbFulfillableEvent.FulfillableTransactionId,
                    EventTypeCode = dbFulfillableEvent.EventTypeCode,
                    EventDateTimeUtc = dbFulfillableEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFulfillableEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFulfillableEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFulfillment_FulfillableEventLogSummary MFulfillment_FulfillableEventLogSummary(FulfillableEvent dbFulfillableEvent)
            {
                return new MFulfillment_FulfillableEventLogSummary()
                {
                    EventId = dbFulfillableEvent.FulfillableEventId,
                    TransactionId = dbFulfillableEvent.FulfillableTransactionId,
                    EntityId = dbFulfillableEvent.FulfillableTransaction.FulfillableTransactionItems.Select(r => r.FulfillableItem.FulfillableId).First(),
                    EventTypeCode = dbFulfillableEvent.EventTypeCode,
                    EventDateTimeUtc = dbFulfillableEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbFulfillableEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbFulfillableEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbFulfillableEvent.FulfillableTransaction.UnitOfWork
                };
            }

            #endregion

            #region Shipment Request

            public static MFulfillment_ShipmentRequest MFulfillment_ShipmentRequest(ShipmentRequest dbShipmentRequest, bool shallow)
            {
                return new MFulfillment_ShipmentRequest()
                {
                    ShipmentRequestId = dbShipmentRequest.ShipmentRequestId,
                    ShipmentRequestNumber = dbShipmentRequest.ShipmentRequestNumber,
                    ShipmentRequestStatus = GetValue.MFulfillment_ShipmentRequestStatus(dbShipmentRequest.ShipmentRequestStatusCode),
                    StatusDateTimeUtc = dbShipmentRequest.ShipmentRequestStatusDateTimeUtc,
                    CreateDateTimeUtc = dbShipmentRequest.CreateDateTimeUtc,
                    ShipToAddress = MFulfillment_Address(dbShipmentRequest.ShipmentRequestAddress),

                    CanCreateShipment = CanCreateShipment(dbShipmentRequest),
                    CanCancel = CanCancel(dbShipmentRequest),

                    ShipmentRequestItems = shallow ? null : MFulfillment_ShipmentRequestItems(dbShipmentRequest.ShipmentRequestItems),
                };
            }

            public static IList<MFulfillment_ShipmentRequest> MFulfillment_ShipmentRequests(IEnumerable<ShipmentRequest> dbShipmentRequests, bool shallow)
            {
                return dbShipmentRequests?.Select(r => MFulfillment_ShipmentRequest(r, shallow)).ToList();
            }

            public static MFulfillment_ShipmentRequestItem MFulfillment_ShipmentRequestItem(ShipmentRequestItem dbShipmentRequestItem)
            {
                return new MFulfillment_ShipmentRequestItem()
                {
                    ShipmentRequestId = dbShipmentRequestItem.ShipmentRequestId,
                    ShipmentRequestNumber = dbShipmentRequestItem.ShipmentRequest.ShipmentRequestNumber,
                    ShipmentRequestItemId = dbShipmentRequestItem.ShipmentRequestItemId,
                    FulfillableId = dbShipmentRequestItem.FulfillableItem.FulfillableId,
                    FulfillableItemId = dbShipmentRequestItem.FulfillableItemId,
                    FulfillableItemReference = dbShipmentRequestItem.FulfillableItem.FulfillableItemReference,
                    Quantity = dbShipmentRequestItem.Quantity
                };
            }

            public static IList<MFulfillment_ShipmentRequestItem> MFulfillment_ShipmentRequestItems(IEnumerable<ShipmentRequestItem> dbShipmentRequestItems)
            {
                return dbShipmentRequestItems?.Select(r => MFulfillment_ShipmentRequestItem(r)).ToList();
            }

            public static MFulfillment_ShipmentRequestSummary MFulfillment_ShipmentRequestSummary(ShipmentRequestSummaryView dbShipmentRequestSummaryView)
            {
                return new MFulfillment_ShipmentRequestSummary()
                {
                    ShipmentRequestId = dbShipmentRequestSummaryView.ShipmentRequestId,
                    ShipmentRequestNumber = dbShipmentRequestSummaryView.ShipmentRequestNumber,
                    FulfillableId = dbShipmentRequestSummaryView.FulfillableId,
                    FulfillableName = dbShipmentRequestSummaryView.FulfillableName,
                    FulfillableReference = dbShipmentRequestSummaryView.FulfillableReference,
                    ShipmentRequestStatus = GetValue.MFulfillment_ShipmentRequestStatus(dbShipmentRequestSummaryView.ShipmentRequestStatusCode),
                    CreateDateTimeUtc = dbShipmentRequestSummaryView.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbShipmentRequestSummaryView.ShipmentRequestStatusDateTimeUtc,
                };
            }

            public static MFulfillment_ShipmentRequestTransaction MFulfillment_ShipmentRequestTransaction(ShipmentRequestTransaction dbShipmentRequestTransaction)
            {
                return new MFulfillment_ShipmentRequestTransaction()
                {
                    TransactionId = dbShipmentRequestTransaction.ShipmentRequestTransactionId,
                    EntityId = dbShipmentRequestTransaction.ShipmentRequestId,
                    TransactionDateTimeUtc = dbShipmentRequestTransaction.TransactionDateTimeUtc,
                    Description = dbShipmentRequestTransaction.Description,
                    UnitOfWork = dbShipmentRequestTransaction.UnitOfWork,

                    ShipmentRequestStatus = GetValue.MFulfillment_ShipmentRequestStatus(dbShipmentRequestTransaction.ShipmentRequestStatusCode)
                };
            }

            public static MFulfillment_ShipmentRequestTransactionSummary MFulfillment_ShipmentRequestTransactionSummary(ShipmentRequestTransaction dbShipmentRequestTransaction)
            {
                return new MFulfillment_ShipmentRequestTransactionSummary()
                {
                    TransactionId = dbShipmentRequestTransaction.ShipmentRequestTransactionId,
                    EntityId = dbShipmentRequestTransaction.ShipmentRequestId,
                    TransactionDateTimeUtc = dbShipmentRequestTransaction.TransactionDateTimeUtc,
                    Description = dbShipmentRequestTransaction.Description,
                    UnitOfWork = dbShipmentRequestTransaction.UnitOfWork,

                    ShipmentRequestStatus = GetValue.MFulfillment_ShipmentRequestStatus(dbShipmentRequestTransaction.ShipmentRequestStatusCode)
                };
            }

            public static MFulfillment_ShipmentRequestEventLog MFulfillment_ShipmentRequestEventLog(ShipmentRequestEvent dbShipmentRequestEvent)
            {
                return new MFulfillment_ShipmentRequestEventLog()
                {
                    EventId = dbShipmentRequestEvent.ShipmentRequestEventId,
                    TransactionId = dbShipmentRequestEvent.ShipmentRequestTransactionId,
                    EventTypeCode = dbShipmentRequestEvent.EventTypeCode,
                    EventDateTimeUtc = dbShipmentRequestEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbShipmentRequestEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbShipmentRequestEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFulfillment_ShipmentRequestEventLogSummary MFulfillment_ShipmentRequestEventLogSummary(ShipmentRequestEvent dbShipmentRequestEvent)
            {
                return new MFulfillment_ShipmentRequestEventLogSummary()
                {
                    EventId = dbShipmentRequestEvent.ShipmentRequestEventId,
                    TransactionId = dbShipmentRequestEvent.ShipmentRequestTransactionId,
                    EntityId = dbShipmentRequestEvent.ShipmentRequestTransaction.ShipmentRequestId,
                    EventTypeCode = dbShipmentRequestEvent.EventTypeCode,
                    EventDateTimeUtc = dbShipmentRequestEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbShipmentRequestEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbShipmentRequestEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbShipmentRequestEvent.ShipmentRequestTransaction.UnitOfWork
                };
            }

            #endregion

            #region Shipments

            public static MFulfillment_Shipment MFulfillment_Shipment(Shipment dbShipment, bool shallow)
            {
                return new MFulfillment_Shipment()
                {
                    ShipmentId = dbShipment.ShipmentId,
                    ShipmentNumber = dbShipment.ShipmentNumber,
                    ShipToAddress = MFulfillment_Address(dbShipment.ShipmentAddress), // FulfillableAddressId
                    ShipmentStatus = GetValue.MFulfillment_ShipmentStatus(dbShipment.ShipmentStatusCode),
                    ShippingVendorId = dbShipment.ShippingVendorId,
                    TrackingCode = dbShipment.TrackingCode,
                    ShipmentDateTimeUtc = dbShipment.ShipmentDateTimeUtc,
                    StatusDateTimeUtc = dbShipment.ShipmentStatusDateTimeUtc,

                    CanEdit = CanEdit(dbShipment),
                    CanPost = CanPost(dbShipment),
                    CanProcess = CanProcess(dbShipment),
                    CanCancel = CanCancel(dbShipment),

                    ShipmentItems = shallow ? null : MFulfillment_ShipmentItems(dbShipment.ShipmentItems)
                };
            }

            public static IList<MFulfillment_Shipment> MFulfillment_Shipments(IEnumerable<Shipment> dbShipments, bool shallow)
            {
                return dbShipments?.Select(r => MFulfillment_Shipment(r, shallow)).ToList();
            }

            public static MFulfillment_ShipmentItem MFulfillment_ShipmentItem(ShipmentItem dbShipmentItem)
            {
                return new MFulfillment_ShipmentItem()
                {
                    ShipmentItemId = dbShipmentItem.ShipmentItemId,
                    ShipmentRequestId = dbShipmentItem.ShipmentRequestItem.ShipmentRequestId,
                    ShipmentRequestNumber = dbShipmentItem.ShipmentRequestItem.ShipmentRequest.ShipmentRequestNumber,
                    ShipmentRequestItemId = dbShipmentItem.ShipmentRequestItemId,
                    FulfillableId = dbShipmentItem.ShipmentRequestItem.FulfillableItem.FulfillableId,
                    FulfillableItemId = dbShipmentItem.ShipmentRequestItem.FulfillableItemId,
                    FulfillableItemReference = dbShipmentItem.ShipmentRequestItem.FulfillableItem.FulfillableItemReference,
                    Quantity = dbShipmentItem.Quantity
                };
            }

            public static IList<MFulfillment_ShipmentItem> MFulfillment_ShipmentItems(IEnumerable<ShipmentItem> dbShipmentItems)
            {
                return dbShipmentItems?.Select(r => MFulfillment_ShipmentItem(r)).ToList();
            }

            public static MFulfillment_ShipmentSummary MFulfillment_ShipmentSummary(ShipmentSummaryView dbShipmentSummaryView)
            {
                return new MFulfillment_ShipmentSummary()
                {
                    ShipmentId = dbShipmentSummaryView.ShipmentId,
                    ShipmentNumber = dbShipmentSummaryView.ShipmentNumber,
                    FulfillableId = dbShipmentSummaryView.FulfillableId,
                    FulfillableName = dbShipmentSummaryView.FulfillableName,
                    FulfillableReference = dbShipmentSummaryView.FulfillableReference,
                    ShipmentStatus = GetValue.MFulfillment_ShipmentStatus(dbShipmentSummaryView.ShipmentStatusCode),
                    ShippingVendorId = dbShipmentSummaryView.ShippingVendorId,
                    TrackingCode = dbShipmentSummaryView.TrackingCode,
                    CreateDateTimeUtc = dbShipmentSummaryView.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbShipmentSummaryView.ShipmentStatusDateTimeUtc
                };
            }

            public static MFulfillment_ShipmentTransaction MFulfillment_ShipmentTransaction(ShipmentTransaction dbShipmentTransaction)
            {
                return new MFulfillment_ShipmentTransaction()
                {
                    TransactionId = dbShipmentTransaction.ShipmentTransactionId,
                    EntityId = dbShipmentTransaction.ShipmentId,
                    TransactionDateTimeUtc = dbShipmentTransaction.TransactionDateTimeUtc,
                    Description = dbShipmentTransaction.Description,
                    UnitOfWork = dbShipmentTransaction.UnitOfWork,

                    ShipmentStatus = GetValue.MFulfillment_ShipmentStatus(dbShipmentTransaction.ShipmentStatusCode)
                };
            }

            public static MFulfillment_ShipmentTransactionSummary MFulfillment_ShipmentTransactionSummary(ShipmentTransaction dbShipmentTransaction)
            {
                return new MFulfillment_ShipmentTransactionSummary()
                {
                    TransactionId = dbShipmentTransaction.ShipmentTransactionId,
                    EntityId = dbShipmentTransaction.ShipmentId,
                    TransactionDateTimeUtc = dbShipmentTransaction.TransactionDateTimeUtc,
                    Description = dbShipmentTransaction.Description,
                    UnitOfWork = dbShipmentTransaction.UnitOfWork,

                    ShipmentStatus = GetValue.MFulfillment_ShipmentStatus(dbShipmentTransaction.ShipmentStatusCode)
                };
            }

            public static MFulfillment_ShipmentEventLog MFulfillment_ShipmentEventLog(ShipmentEvent dbShipmentEvent)
            {
                return new MFulfillment_ShipmentEventLog()
                {
                    EventId = dbShipmentEvent.ShipmentEventId,
                    TransactionId = dbShipmentEvent.ShipmentTransactionId,
                    EventTypeCode = dbShipmentEvent.EventTypeCode,
                    EventDateTimeUtc = dbShipmentEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbShipmentEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbShipmentEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFulfillment_ShipmentEventLogSummary MFulfillment_ShipmentEventLogSummary(ShipmentEvent dbShipmentEvent)
            {
                return new MFulfillment_ShipmentEventLogSummary()
                {
                    EventId = dbShipmentEvent.ShipmentEventId,
                    TransactionId = dbShipmentEvent.ShipmentTransactionId,
                    EntityId = dbShipmentEvent.ShipmentTransaction.ShipmentId,
                    EventTypeCode = dbShipmentEvent.EventTypeCode,
                    EventDateTimeUtc = dbShipmentEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbShipmentEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbShipmentEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbShipmentEvent.ShipmentTransaction.UnitOfWork
                };
            }

            #endregion

            #region Return Requests

            public static MFulfillment_ReturnRequest MFulfillment_ReturnRequest(ReturnRequest dbReturnRequest, bool shallow)
            {
                return new MFulfillment_ReturnRequest()
                {
                    ReturnRequestId = dbReturnRequest.ReturnRequestId,
                    ReturnRequestNumber = dbReturnRequest.ReturnRequestNumber,
                    ReturnRequestStatus = GetValue.MFulfillment_ReturnRequestStatus(dbReturnRequest.ReturnRequestStatusCode),
                    CreateDateTimeUtc = dbReturnRequest.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbReturnRequest.ReturnRequestStatusDateTimeUtc,
                    ReturnRequestType = GetValue.MFulfillment_ReturnRequestType(dbReturnRequest.ReturnRequestTypeCode),
                    ReturnRequestReasonCode = dbReturnRequest.ReturnRequestReasonCode,

                    CanCreateReturn = CanCreateReturn(dbReturnRequest),
                    CanEdit = CanEdit(dbReturnRequest),

                    ReturnRequestItems = shallow ? null : MFulfillment_ReturnRequestItems(dbReturnRequest.ReturnRequestItems)
                };
            }

            public static IList<MFulfillment_ReturnRequest> MFulfillment_ReturnRequests(IEnumerable<ReturnRequest> dbReturnRequests, bool shallow)
            {
                return dbReturnRequests?.Select(r => MFulfillment_ReturnRequest(r, shallow)).ToList();
            }

            private static MFulfillment_ReturnRequestItem MFulfillment_ReturnRequestItem(ReturnRequestItem dbReturnRequestItem)
            {
                return new MFulfillment_ReturnRequestItem()
                {
                    ReturnRequestItemId = dbReturnRequestItem.ReturnRequestItemId,
                    ReturnRequestId = dbReturnRequestItem.ReturnRequestId,
                    ReturnRequestNumber = dbReturnRequestItem.ReturnRequest.ReturnRequestNumber,
                    FulfillableId = dbReturnRequestItem.FulfillableItem.FulfillableId,
                    FulfillableItemId = dbReturnRequestItem.FulfillableItemId,
                    FulfillableItemReference = dbReturnRequestItem.FulfillableItem.FulfillableItemReference,
                    Quantity = dbReturnRequestItem.Quantity
                };
            }

            private static IList<MFulfillment_ReturnRequestItem> MFulfillment_ReturnRequestItems(IEnumerable<ReturnRequestItem> dbReturnRequestItems)
            {
                return dbReturnRequestItems?.Select(r => MFulfillment_ReturnRequestItem(r)).ToList();
            }

            public static MFulfillment_ReturnRequestSummary MFulfillment_ReturnRequestSummary(ReturnRequestSummaryView dbReturnRequestSummaryView)
            {
                return new MFulfillment_ReturnRequestSummary()
                {
                    ReturnRequestId = dbReturnRequestSummaryView.ReturnRequestId,
                    ReturnRequestNumber = dbReturnRequestSummaryView.ReturnRequestNumber,
                    FulfillableId = dbReturnRequestSummaryView.FulfillableId,
                    FulfillableName = dbReturnRequestSummaryView.FulfillableName,
                    FulfillableReference = dbReturnRequestSummaryView.FulfillableReference,
                    CreateDateTimeUtc = dbReturnRequestSummaryView.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbReturnRequestSummaryView.ReturnRequestStatusDateTimeUtc,
                    ReturnRequestStatus = GetValue.MFulfillment_ReturnRequestStatus(dbReturnRequestSummaryView.ReturnRequestStatusCode)
                };
            }

            public static MFulfillment_ReturnRequestTransaction MFulfillment_ReturnRequestTransaction(ReturnRequestTransaction dbReturnRequestTransaction)
            {
                return new MFulfillment_ReturnRequestTransaction()
                {
                    TransactionId = dbReturnRequestTransaction.ReturnRequestTransactionId,
                    EntityId = dbReturnRequestTransaction.ReturnRequestId,
                    TransactionDateTimeUtc = dbReturnRequestTransaction.TransactionDateTimeUtc,
                    Description = dbReturnRequestTransaction.Description,
                    UnitOfWork = dbReturnRequestTransaction.UnitOfWork,

                    ReturnRequestStatus = GetValue.MFulfillment_ReturnRequestStatus(dbReturnRequestTransaction.ReturnRequestStatusCode)
                };
            }

            public static MFulfillment_ReturnRequestTransactionSummary MFulfillment_ReturnRequestTransactionSummary(ReturnRequestTransaction dbReturnRequestTransaction)
            {
                return new MFulfillment_ReturnRequestTransactionSummary()
                {
                    TransactionId = dbReturnRequestTransaction.ReturnRequestTransactionId,
                    EntityId = dbReturnRequestTransaction.ReturnRequestId,
                    TransactionDateTimeUtc = dbReturnRequestTransaction.TransactionDateTimeUtc,
                    Description = dbReturnRequestTransaction.Description,
                    UnitOfWork = dbReturnRequestTransaction.UnitOfWork,

                    ReturnRequestStatus = GetValue.MFulfillment_ReturnRequestStatus(dbReturnRequestTransaction.ReturnRequestStatusCode)
                };
            }

            public static MFulfillment_ReturnRequestEventLog MFulfillment_ReturnRequestEventLog(ReturnRequestEvent dbReturnRequestEvent)
            {
                return new MFulfillment_ReturnRequestEventLog()
                {
                    TransactionId = dbReturnRequestEvent.ReturnRequestTransactionId,
                    EventId = dbReturnRequestEvent.ReturnRequestEventId,
                    EventTypeCode = dbReturnRequestEvent.EventTypeCode,
                    EventDateTimeUtc = dbReturnRequestEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbReturnRequestEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbReturnRequestEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFulfillment_ReturnRequestEventLogSummary MFulfillment_ReturnRequestEventLogSummary(ReturnRequestEvent dbReturnRequestEvent)
            {
                return new MFulfillment_ReturnRequestEventLogSummary()
                {
                    EventId = dbReturnRequestEvent.ReturnRequestEventId,
                    TransactionId = dbReturnRequestEvent.ReturnRequestTransactionId,
                    EntityId = dbReturnRequestEvent.ReturnRequestTransaction.ReturnRequestId,
                    EventTypeCode = dbReturnRequestEvent.EventTypeCode,
                    EventDateTimeUtc = dbReturnRequestEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbReturnRequestEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbReturnRequestEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbReturnRequestEvent.ReturnRequestTransaction.UnitOfWork
                };
            }

            #endregion

            #region Returns

            public static MFulfillment_Return MFulfillment_Return(Return dbReturn, bool shallow)
            {
                return new MFulfillment_Return()
                {
                    ReturnId = dbReturn.ReturnId,
                    ReturnNumber = dbReturn.ReturnNumber,
                    ReturnStatus = GetValue.MFulfillment_ReturnStatus(dbReturn.ReturnStatusCode),
                    CreateDateTimeUtc = dbReturn.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbReturn.ReturnStatusDateTimeUtc,

                    CanEdit = CanEdit(dbReturn),
                    CanPost = CanPost(dbReturn),
                    CanProcess = CanProcess(dbReturn),
                    CanCancel = CanCancel(dbReturn),

                    ReturnItems = shallow ? null : MFulfillment_ReturnItems(dbReturn.ReturnItems)
                };
            }

            public static IList<MFulfillment_Return> MFulfillment_Returns(IEnumerable<Return> dbReturns, bool shallow)
            {
                return dbReturns?.Select(r => MFulfillment_Return(r, shallow)).ToList();
            }

            public static MFulfillment_ReturnItem MFulfillment_ReturnItem(ReturnItem dbReturnItem)
            {
                return new MFulfillment_ReturnItem()
                {
                    ReturnItemId = dbReturnItem.ReturnItemId,
                    ReturnRequestItemId = dbReturnItem.ReturnRequestItem.ReturnRequestItemId,
                    ReturnRequestId = dbReturnItem.ReturnRequestItem.ReturnRequestId,
                    ReturnRequestNumber = dbReturnItem.ReturnRequestItem.ReturnRequest.ReturnRequestNumber,
                    FulfillableId = dbReturnItem.ReturnRequestItem.FulfillableItem.FulfillableId,
                    FulfillableItemId = dbReturnItem.ReturnRequestItem.FulfillableItemId,
                    FulfillableItemReference = dbReturnItem.ReturnRequestItem.FulfillableItem.FulfillableItemReference,
                    Quantity = dbReturnItem.Quantity
                };
            }

            public static IList<MFulfillment_ReturnItem> MFulfillment_ReturnItems(IEnumerable<ReturnItem> dbReturnItems)
            {
                return dbReturnItems?.Select(r => MFulfillment_ReturnItem(r)).ToList();
            }

            public static MFulfillment_ReturnSummary MFulfillment_ReturnSummary(ReturnSummaryView dbReturnSummaryView)
            {
                return new MFulfillment_ReturnSummary()
                {
                    ReturnId = dbReturnSummaryView.ReturnId,
                    ReturnNumber = dbReturnSummaryView.ReturnNumber,
                    FulfillableId = dbReturnSummaryView.FulfillableId,
                    FulfillableName = dbReturnSummaryView.FulfillableName,
                    FulfillableReference = dbReturnSummaryView.FulfillableReference,
                    CreateDateTimeUtc = dbReturnSummaryView.CreateDateTimeUtc,
                    StatusDateTimeUtc = dbReturnSummaryView.ReturnStatusDateTimeUtc,
                    ReturnStatus = GetValue.MFulfillment_ReturnStatus(dbReturnSummaryView.ReturnStatusCode)
                };
            }

            public static MFulfillment_ReturnTransaction MFulfillment_ReturnTransaction(ReturnTransaction dbReturnTransaction)
            {
                return new MFulfillment_ReturnTransaction()
                {
                    TransactionId = dbReturnTransaction.ReturnTransactionId,
                    EntityId = dbReturnTransaction.ReturnId,
                    TransactionDateTimeUtc = dbReturnTransaction.TransactionDateTimeUtc,
                    Description = dbReturnTransaction.Description,
                    UnitOfWork = dbReturnTransaction.UnitOfWork,

                    ReturnStatus = GetValue.MFulfillment_ReturnStatus(dbReturnTransaction.ReturnStatusCode)
                };
            }

            public static MFulfillment_ReturnTransactionSummary MFulfillment_ReturnTransactionSummary(ReturnTransaction dbReturnTransaction)
            {
                return new MFulfillment_ReturnTransactionSummary()
                {
                    TransactionId = dbReturnTransaction.ReturnTransactionId,
                    EntityId = dbReturnTransaction.ReturnId,
                    TransactionDateTimeUtc = dbReturnTransaction.TransactionDateTimeUtc,
                    Description = dbReturnTransaction.Description,
                    UnitOfWork = dbReturnTransaction.UnitOfWork,

                    ReturnStatus = GetValue.MFulfillment_ReturnStatus(dbReturnTransaction.ReturnStatusCode)
                };
            }

            public static MFulfillment_ReturnEventLog MFulfillment_ReturnEventLog(ReturnEvent dbReturnEvent)
            {
                return new MFulfillment_ReturnEventLog()
                {
                    EventId = dbReturnEvent.ReturnEventId,
                    TransactionId = dbReturnEvent.ReturnTransactionId,
                    EventTypeCode = dbReturnEvent.EventTypeCode,
                    EventDateTimeUtc = dbReturnEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbReturnEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbReturnEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MFulfillment_ReturnEventLogSummary MFulfillment_ReturnEventLogSummary(ReturnEvent dbReturnEvent)
            {
                return new MFulfillment_ReturnEventLogSummary()
                {
                    EventId = dbReturnEvent.ReturnEventId,
                    TransactionId = dbReturnEvent.ReturnTransactionId,
                    EntityId = dbReturnEvent.ReturnTransaction.ReturnId,
                    EventTypeCode = dbReturnEvent.EventTypeCode,
                    EventDateTimeUtc = dbReturnEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbReturnEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbReturnEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbReturnEvent.ReturnTransaction.UnitOfWork
                };
            }

            #endregion
        }
    }
}
