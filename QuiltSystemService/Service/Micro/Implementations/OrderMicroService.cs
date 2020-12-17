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
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class OrderMicroService : MicroService, IOrderMicroService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private ILedgerMicroService LedgerMicroService { get; }

        public OrderMicroService(
            IApplicationLocale locale,
            ILogger<OrderMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IOrderEventMicroService orderEventService,
            ICommunicationMicroService communicationMicroService,
            ILedgerMicroService ledgerMicroService
            )
            : base(
                  locale,
                  logger,
                  quiltContextFactory)
        {
            OrderEventService = orderEventService ?? throw new ArgumentNullException(nameof(orderEventService));
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));
        }

        public async Task<long> AllocateOrdererAsync(string ordererReference)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(AllocateOrdererAsync), ordererReference);
            try
            {
                var ctx = CreateQuiltContext();

                var dbOrderer = await ctx.Orderers.Where(r => r.OrdererReference == ordererReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrderer == null)
                {
                    dbOrderer = new Orderer()
                    {
                        OrdererReference = ordererReference,
                        UpdateDateTimeUtc = GetUtcNow()
                    };

                    _ = ctx.Orderers.Add(dbOrderer);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = dbOrderer.OrdererId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long?> LookupOrdererAsync(string ordererReference)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(AllocateOrdererAsync), ordererReference);
            try
            {
                var ctx = CreateQuiltContext();

                var ordererId = await ctx.Orderers.Where(r => r.OrdererReference == ordererReference).Select(r => (long?)r.OrdererId).FirstOrDefaultAsync().ConfigureAwait(false);

                var result = ordererId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MOrder_AllocateOrderableResponse> AllocateOrderableAsync(MOrder_AllocateOrderable mAllocateOrderable)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(AllocateOrderableAsync), mAllocateOrderable);
            try
            {
                var utcNow = GetUtcNow();

                var ctx = CreateQuiltContext();

                var dbOrderable = await ctx.Orderables.Where(r => r.OrderableReference == mAllocateOrderable.OrderableReference).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrderable == null)
                {
                    dbOrderable = new Orderable()
                    {
                        OrderableReference = mAllocateOrderable.OrderableReference,
                        Description = mAllocateOrderable.Name,
                        Price = mAllocateOrderable.Price,
                        UpdateDateTimeUtc = utcNow
                    };
                    _ = ctx.Orderables.Add(dbOrderable);

                    foreach (var mAllocateOrderableComponent in mAllocateOrderable.Components)
                    {
                        var dbOrderableComponent = new OrderableComponent()
                        {
                            OrderableComponentReference = mAllocateOrderableComponent.OrderableComponentReference,
                            Description = mAllocateOrderableComponent.Description,
                            ConsumableReference = mAllocateOrderableComponent.ConsumableReference,
                            Quantity = mAllocateOrderableComponent.Quantity,
                            UnitPrice = mAllocateOrderableComponent.UnitPrice,
                            TotalPrice = mAllocateOrderableComponent.TotalPrice,
                            UpdateDateTimeUtc = utcNow
                        };
                        dbOrderable.OrderableComponents.Add(dbOrderableComponent);
                    }

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var components = new List<MOrder_AllocateOrderableResponseComponent>();
                foreach (var dbOrderableComponent in dbOrderable.OrderableComponents)
                {
                    components.Add(new MOrder_AllocateOrderableResponseComponent()
                    {
                        OrderableComponentId = dbOrderableComponent.OrderableComponentId,
                        OrderableComponentReference = dbOrderableComponent.OrderableComponentReference
                    });
                }

                var result = new MOrder_AllocateOrderableResponse()
                {
                    OrderableId = dbOrderable.OrderableId,
                    OrderableReference = dbOrderable.OrderableReference,
                    Components = components
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

        public async Task<MOrder_Dashboard> GetDashboardAsync()
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetDashboardAsync));
            try
            {
                using var ctx = CreateQuiltContext();

                var dashboard = new MOrder_Dashboard()
                {
                    TotalOrders = await ctx.Orders.CountAsync()
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

        public async Task<MOrder_Order> GetCartOrderAsync(long ordererId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetCartOrderAsync), ordererId);
            try
            {
                var ctx = CreateQuiltContext();

                var dbOrdererPendingOrder = await ctx.OrdererPendingOrders.Where(r => r.OrdererId == ordererId).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrdererPendingOrder == null)
                {
                    return null;
                }

                var dbOrder = dbOrdererPendingOrder.Order;

                var result = Create.MOrder_Order(dbOrder);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MOrder_Order> GetOrderAsync(long orderId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderAsync), orderId);
            try
            {
                var ctx = CreateQuiltContext();

                var result = await ctx.Orders
                    .Where(o => o.OrderId == orderId)
                    .Include(r => r.OrderItems)
                        .ThenInclude(r => r.Orderable)
                            .ThenInclude(r => r.OrderableComponents)
                    .Include(r => r.Orderer)
                    .Include(r => r.OrderShippingAddress)
                    .Select(r => Create.MOrder_Order(r))
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

        public async Task<MOrder_OrderItem> GetOrderItemAsync(long orderItemId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderItemAsync), orderItemId);
            try
            {
                var ctx = CreateQuiltContext();

                var result = await ctx.OrderItems
                    .Where(o => o.OrderItemId == orderItemId)
                    .Include(r => r.Orderable)
                        .ThenInclude(r => r.OrderableComponents)
                    .Select(r => Create.MOrder_OrderItem(r))
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

        public async Task<MOrder_OrderList> GetOrdersAsync(long ordererId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrdersAsync), ordererId);
            try
            {
                var ctx = CreateQuiltContext();

                IQueryable<Order> dbOrders = ctx.Orders;
                var dbOrderList = await dbOrders
                    .Where(r => r.OrdererId == ordererId)
                    .Where(o => o.OrderStatusCode != OrderStatusCodes.Pending)
                    .OrderByDescending(r => r.OrderStatusDateTimeUtc)
                    .Include(r => r.OrderShippingAddress)
                    .Include(r => r.OrderItems)
                        .ThenInclude(r => r.Orderable)
                            .ThenInclude(r => r.OrderableComponents)
                    .ToListAsync().ConfigureAwait(false);

                var orders = new List<MOrder_Order>();
                foreach (var dbOrder in dbOrderList)
                {
                    orders.Add(Create.MOrder_Order(dbOrder));
                }

                var result = new MOrder_OrderList()
                {
                    Orders = orders
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

        public async Task<MOrder_OrderSummaryList> GetOrderSummariesAsync(string orderNumber, DateTime? orderDateUtc, MOrder_OrderStatus orderStatus, long? ordererId, int? recordCount)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderSummariesAsync), orderNumber, orderDateUtc, orderStatus, ordererId, recordCount);
            try
            {
                var ctx = CreateQuiltContext();

                IQueryable<Order> query = ctx.Orders;

                if (!string.IsNullOrEmpty(orderNumber))
                {
                    query = query.Where(r => r.OrderNumber.Contains(orderNumber));
                }
                if (orderDateUtc != null)
                {
                    var from = orderDateUtc.Value;
                    var to = from.AddDays(1);

                    query = query.Where(r => r.SubmissionDateTimeUtc >= from && r.SubmissionDateTimeUtc < to);
                }
                if (orderStatus == MOrder_OrderStatus.MetaAll)
                {
                    // No action required.
                }
                else
                {
                    var orderStatusCode = GetCode.From(orderStatus);
                    query = query.Where(r => r.OrderStatusCode == orderStatusCode);
                }
                if (ordererId != null)
                {
                    query = query.Where(r => r.OrdererId == ordererId.Value);
                }
                if (recordCount != null)
                {
                    query = query.Take(recordCount.Value);
                }

                var orderSummaries = await query
                    .Include(r => r.Orderer)
                    .OrderByDescending(r => r.OrderStatusDateTimeUtc)
                    .Select(r => Create.MOrder_OrderSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MOrder_OrderSummaryList()
                {
                    Summaries = orderSummaries
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

        public async Task<int> ProcessEventsAsync()
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(ProcessEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var dbOrderEvents = await ctx.OrderEvents.Where(r => r.ProcessingStatusCode == EventProcessingStatusCodes.Pending).ToListAsync().ConfigureAwait(false);

                var count = 0;
                foreach (var dbOrderEvent in dbOrderEvents)
                {
                    count += 1;

                    try
                    {
                        var dbTransaction = dbOrderEvent.OrderTransaction;
                        var unitOfWork = dbTransaction.UnitOfWork;

                        var eventData = Create.MOrder_OrderEvent(dbOrderEvent, unitOfWork);

                        await OrderEventService.HandleOrderEventAsync(eventData).ConfigureAwait(false);

                        dbOrderEvent.ProcessingStatusCode = EventProcessingStatusCodes.Processed;

                        _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        log.Exception(ex);

                        dbOrderEvent.ProcessingStatusCode = EventProcessingStatusCodes.Exception;
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
            using var log = BeginFunction(nameof(OrderMicroService), nameof(CancelEventsAsync));
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var count = await ctx.Database.ExecuteSqlInterpolatedAsync($"update OrderEvent set ProcessingStatusCode = {EventProcessingStatusCodes.Cancelled} where ProcessingStatusCode = {EventProcessingStatusCodes.Pending}").ConfigureAwait(false);

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

        public async Task<bool> AddCartItemAsync(long ordererId, long orderableId, int quantity)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(AddCartItemAsync), ordererId, orderableId, quantity);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                // Search for pending order.
                //
                var dbOrder = await ctx.Orders.Where(o => o.OrdererId == ordererId && o.OrderStatusCode == OrderStatusCodes.Pending).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrder == null)
                {
                    var orderNumber = ctx.GetOrderNumber(utcNow);

                    dbOrder = new Order()
                    {
                        OrdererId = ordererId,
                        OrderNumber = orderNumber,
                        ItemSubtotal = 0,
                        Shipping = 0,
                        Discount = 0,
                        PretaxAmount = 0,
                        SalesTax = 0,
                        TotalAmount = 0,
                        FundsRequired = 0,
                        FundsReceived = 0,
                        SalexTaxJurisdiction = null,
                        TaxableAmount = 0,
                        SalesTaxRate = 0,
                        OrderStatusCode = OrderStatusCodes.Pending,
                        OrderStatusDateTimeUtc = utcNow,
                        UpdateDateTimeUtc = utcNow
                    };
                    _ = ctx.Orders.Add(dbOrder);

                    var dbOrdererPendingOrder = new OrdererPendingOrder()
                    {
                        OrdererId = ordererId,
                        CreateDateTimeUtc = utcNow,
                    };
                    dbOrder.OrdererPendingOrders.Add(dbOrdererPendingOrder);

                    // Ensures dbOrder.OrderId value is populated.
                    //
                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                // Determine maxOrderItemSequence
                //
                var orderItemSequences = await ctx.OrderItems.Where(oi => oi.OrderId == dbOrder.OrderId).Select(oi => oi.OrderItemSequence).ToListAsync().ConfigureAwait(false);
                var maxOrderItemSequence = -1;
                foreach (var orderItemSequence in orderItemSequences)
                {
                    maxOrderItemSequence = Math.Max(orderItemSequence, maxOrderItemSequence);
                }

                // Search for existing order item for orderable.
                //
                {
                    var dbOrderItem = await ctx.OrderItems
                        .Where(orderItem => orderItem.OrderId == dbOrder.OrderId && orderItem.OrderableId == orderableId).SingleOrDefaultAsync().ConfigureAwait(false);
                    if (dbOrderItem == null)
                    {
                        var dbOrderable = ctx.Orderables.Find(orderableId);

                        // Create order item.
                        //
                        dbOrderItem = new OrderItem()
                        {
                            Order = dbOrder,
                            OrderItemSequence = maxOrderItemSequence + 1,
                            Orderable = dbOrderable,
                            OrderQuantity = quantity,
                            CancelQuantity = 0,
                            FulfillmentReturnQuantity = 0,
                            FulfillmentRequiredQuantity = 0,
                            FulfillmentCompleteQuantity = 0,
                            UpdateDateTimeUtc = utcNow
                        };
                        dbOrder.OrderItems.Add(dbOrderItem);
                    }
                    else
                    {
                        dbOrderItem.OrderQuantity = quantity;
                    }

                    UpdateOrderTotals(dbOrder);
                }

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = true;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> UpdateCartItemQuantityAsync(long ordererId, long orderItemId, int quantity)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(UpdateCartItemQuantityAsync), ordererId, orderItemId, quantity);
            try
            {
                var ctx = CreateQuiltContext();

                // Search for pending order.
                //
                var dbOrder = await ctx.Orders.Where(o => o.OrdererId == ordererId && o.OrderStatusCode == OrderStatusCodes.Pending).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrder != null)
                {
                    foreach (var dbOrderItem in dbOrder.OrderItems)
                    {
                        if (dbOrderItem.OrderItemId == orderItemId)
                        {
                            dbOrderItem.OrderQuantity = quantity;
                        }
                    }

                    UpdateOrderTotals(dbOrder);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = true;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteCartItemAsync(long ordererId, int orderItemId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(DeleteCartItemAsync), ordererId, orderItemId);
            try
            {
                bool result;

                var ctx = CreateQuiltContext();

                // Search for pending order.
                //
                var dbOrder = await ctx.Orders.Where(o => o.OrdererId == ordererId && o.OrderStatusCode == OrderStatusCodes.Pending).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrder != null)
                {
                    foreach (var dbOrderItem in dbOrder.OrderItems)
                    {
                        if (dbOrderItem.OrderItemId == orderItemId)
                        {
                            _ = dbOrder.OrderItems.Remove(dbOrderItem);
                            _ = ctx.OrderItems.Remove(dbOrderItem);
                            break;
                        }
                    }

                    UpdateOrderTotals(dbOrder);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                    result = true;
                }
                else
                {
                    result = false;
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

        public async Task<bool> UpdateShippingAddressAsync(long ordererId, MCommon_Address shippingAddress)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(UpdateShippingAddressAsync), ordererId, shippingAddress);
            try
            {
                using var ctx = CreateQuiltContext();

                // Search for pending order.
                //
                var dbOrder = await ctx.Orders.Where(o => o.OrdererId == ordererId && o.OrderStatusCode == OrderStatusCodes.Pending).SingleOrDefaultAsync().ConfigureAwait(false);
                if (dbOrder != null)
                {
                    var addressUpdated = await StandardizeAddress(shippingAddress).ConfigureAwait(false);

                    var dbOrderShippingAddress = dbOrder.OrderShippingAddress;
                    if (dbOrderShippingAddress == null)
                    {
                        dbOrderShippingAddress = new OrderShippingAddress()
                        {
                            Order = dbOrder
                        };
                        _ = ctx.OrderShippingAddresses.Add(dbOrderShippingAddress);
                    }
                    dbOrderShippingAddress.ShipToName = shippingAddress.Name;
                    dbOrderShippingAddress.ShipToAddressLine1 = shippingAddress.AddressLine1;
                    dbOrderShippingAddress.ShipToAddressLine2 = shippingAddress.AddressLine2;
                    dbOrderShippingAddress.ShipToCity = shippingAddress.City;
                    dbOrderShippingAddress.ShipToStateCode = shippingAddress.StateCode;
                    dbOrderShippingAddress.ShipToPostalCode = shippingAddress.PostalCode;
                    dbOrderShippingAddress.ShipToCountryCode = shippingAddress.CountryCode;

                    var op = new SalesTaxLookupOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
                    var opResult = await op.ExecuteAsync(shippingAddress.AddressLine1, shippingAddress.City, shippingAddress.StateCode, shippingAddress.PostalCode, dbOrder.OrderStatusDateTimeUtc).ConfigureAwait(false);

                    dbOrder.SalesTaxRate = opResult.SalesTaxRate;
                    dbOrder.SalexTaxJurisdiction = opResult.SalesTaxJurisdiction;

                    UpdateOrderTotals(dbOrder);

                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                var result = true;

                log.Result(result);

                return true;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> SubmitCartAsync(long ordererId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(SubmitCartAsync), ordererId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                // Search for pending order.
                //
                var dbOrdererPendingOrder = await ctx.OrdererPendingOrders.Where(r => r.OrdererId == ordererId).SingleAsync().ConfigureAwait(false);
                if (dbOrdererPendingOrder == null)
                {
                    throw new InvalidOperationException($"Pending order not found for orderer {ordererId}.");
                }

                var dbOrder = dbOrdererPendingOrder.Order;

                var description = $"Order {dbOrder.OrderId} submitted.";
                var unitOfWork = CreateUnitOfWork.CartSubmission(dbOrder.OrderId);

                var dbOrderTransaction = ctx.CreateOrderTransactionBuilder()
                    .Begin(dbOrder.OrderId, OrderTransactionTypeCodes.Submit, utcNow)
                    .UnitOfWork(unitOfWork)
                    .PrepareForSubmission()
                    .SetOrderStatusCode(OrderStatusCodes.Submitted)
                    .Create();

                _ = ctx.OrdererPendingOrders.Remove(dbOrdererPendingOrder);

                UpdateFundsRequired(ctx, dbOrder, utcNow, unitOfWork);
                UpdateFulfillmentRequiredQuantity(ctx, dbOrder, utcNow, unitOfWork);
                UpdateClose(ctx, dbOrder, utcNow, unitOfWork);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbOrder.OrderId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> SetFundsReceivedAsync(long orderId, decimal fundsReceived, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(SetFundsReceivedAsync), orderId, fundsReceived, unitOfWorkRoot);
            try
            {
                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                using var ctx = CreateQuiltContext();

                var dbOrder = await ctx.Orders.Where(r => r.OrderId == orderId).SingleAsync().ConfigureAwait(false);

                var fundsReceivedDelta = fundsReceived - dbOrder.FundsReceived;
                if (fundsReceivedDelta != 0)
                {
                    var description = $"{fundsReceivedDelta} received for order {orderId}.";

                    var dbOrderTransaction = ctx.CreateOrderTransactionBuilder()
                        .Begin(orderId, OrderTransactionTypeCodes.FundsReceived, utcNow)
                        .UnitOfWork(unitOfWork)
                        .AddFundsReceived(fundsReceivedDelta)
                        .Create();
                }

                UpdateFulfillmentRequiredQuantity(ctx, dbOrder, utcNow, unitOfWork);
                UpdateClose(ctx, dbOrder, utcNow, unitOfWork);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                //if (fundsReceivedDelta > 0)
                //{
                //    var ledgeAccountTransactionId = await new LedgerServiceAccountTransactionBuilder(LedgerMicroService)
                //        .Begin("OrderMicrosService.SetFundsReceivedAsync", Locale.GetLocalTimeFromUtc(utcNow).Date)
                //        .UnitOfWork(unitOfWork)
                //        .Debit(LedgerAccountNumbers.FundsSuspense, fundsReceivedDelta)
                //        .Credit(LedgerAccountNumbers.AccountReceivable, fundsReceivedDelta)
                //        .CreateAsync();

                //    LogMessage($"ledgeAccountTransactionId = {ledgeAccountTransactionId}");
                //}
                //else if (fundsReceivedDelta < 0)
                //{
                //    var ledgeAccountTransactionId = await new LedgerServiceAccountTransactionBuilder(LedgerMicroService)
                //        .Begin("OrderMicrosService.SetFundsReceivedAsync", Locale.GetLocalTimeFromUtc(utcNow).Date)
                //        .UnitOfWork(unitOfWork)
                //        .Debit(LedgerAccountNumbers.AccountPayable, -fundsReceivedDelta)
                //        .Credit(LedgerAccountNumbers.FundsSuspense, -fundsReceivedDelta)
                //        .CreateAsync();

                //    LogMessage($"ledgeAccountTransactionId = {ledgeAccountTransactionId}");
                //}

                var result = true;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetFulfillmentCompleteAsync(long orderItemId, int quantity, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(SetFulfillmentCompleteAsync), orderItemId, quantity, unitOfWorkRoot);
            try
            {
                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                using var ctx = CreateQuiltContext();

                var dbOrderItem = await ctx.OrderItems.FindAsync(orderItemId).ConfigureAwait(false);
                var orderId = dbOrderItem.OrderId;

                var fulfillmentCompleteDelta = quantity - dbOrderItem.FulfillmentCompleteQuantity;
                if (fulfillmentCompleteDelta != 0)
                {
                    var description = $"{fulfillmentCompleteDelta} fulfillment completed for order item {orderId}/{orderItemId}.";

                    _ = ctx.CreateOrderTransactionBuilder()
                        .Begin(orderId, OrderTransactionTypeCodes.FulfillmentComplete, utcNow)
                        .UnitOfWork(unitOfWork)
                        .AddFulfillmentCompleteQuantity(orderItemId, fulfillmentCompleteDelta)
                        .Create();
                }

                var dbOrder = await ctx.Orders
                    .Include(r => r.OrderItems)
                    .Where(r => r.OrderId == orderId)
                    .FirstAsync().ConfigureAwait(false);

                UpdateClose(ctx, dbOrder, utcNow, unitOfWork);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task SetFulfillmentReturnAsync(long orderItemId, int quantity, string unitOfWorkRoot)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(SetFulfillmentReturnAsync), orderItemId, quantity, unitOfWorkRoot);
            try
            {
                var utcNow = GetUtcNow();
                var unitOfWork = new UnitOfWork(unitOfWorkRoot);

                using var ctx = CreateQuiltContext();

                var dbOrderItem = await ctx.OrderItems.FindAsync(orderItemId).ConfigureAwait(false);
                var orderId = dbOrderItem.OrderId;

                var fulfillmentReturnDelta = quantity - dbOrderItem.FulfillmentReturnQuantity;
                if (fulfillmentReturnDelta != 0)
                {
                    var dbOrderTransaction = ctx.CreateOrderTransactionBuilder()
                        .Begin(orderId, OrderTransactionTypeCodes.FulfillmentReturn, utcNow)
                        .UnitOfWork(unitOfWork)
                        .AddFulfillmentReturnQuantity(orderItemId, fulfillmentReturnDelta)
                        .UpdatePricing()
                        .Event(OrderEventTypeCodes.FundingUpdate)
                        .Create();
                }

                var dbOrder = await ctx.Orders
                    .Include(r => r.OrderItems)
                    .Where(r => r.OrderId == orderId)
                    .FirstAsync().ConfigureAwait(false);

                UpdateFundsRequired(ctx, dbOrder, utcNow, unitOfWork);
                UpdateClose(ctx, dbOrder, utcNow, unitOfWork);

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MOrder_OrderTransaction> GetOrderTransactionAsync(long orderEventId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderTransactionAsync), orderEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var orderTransaction = await ctx.OrderTransactions
                    .Include(r => r.OrderTransactionItems)
                    .Where(r => r.OrderTransactionId == orderEventId)
                    .Select(r => Create.MOrder_OrderTransaction(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = orderTransaction;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MOrder_OrderTransactionSummaryList> GetOrderTransactionSummariesAsync(long? orderId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderTransactionSummariesAsync), orderId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Order)
                {
                    return new MOrder_OrderTransactionSummaryList()
                    {
                        Summaries = Enumerable.Empty<MOrder_OrderTransactionSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<OrderTransaction>)ctx.OrderTransactions;
                if (orderId != null)
                {
                    query = query.Where(r => r.OrderTransactionItems.Any(r => r.OrderItem.OrderId == orderId));
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    var unitOfWorkRoot = UnitOfWork.GetRoot(unitOfWork);
                    query = query.Where(r => r.UnitOfWork.StartsWith(unitOfWorkRoot));
                }
                var summaries = await query
                    .Include(r => r.OrderTransactionItems)
                        .ThenInclude(r => r.OrderItem)
                    .Select(r => Create.MOrder_OrderTransactionSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MOrder_OrderTransactionSummaryList
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

        public async Task<MOrder_OrderEventLog> GetOrderEventLogAsync(long orderEventId)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderEventLogAsync), orderEventId);
            try
            {
                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var orderEventLog = await ctx.OrderEvents
                    .Where(r => r.OrderEventId == orderEventId)
                    .Select(r => Create.MOrder_OrderEventLog(r))
                    .SingleAsync().ConfigureAwait(false);

                var result = orderEventLog;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<MOrder_OrderEventLogSummaryList> GetOrderEventLogSummariesAsync(long? orderId, string unitOfWork, string source)
        {
            using var log = BeginFunction(nameof(OrderMicroService), nameof(GetOrderEventLogSummariesAsync), orderId, unitOfWork, source);
            try
            {
                if (source != null && source != MSources.Order)
                {
                    return new MOrder_OrderEventLogSummaryList()
                    {
                        Summaries = Enumerable.Empty<MOrder_OrderEventLogSummary>().ToList()
                    };
                }

                var utcNow = GetUtcNow();

                using var ctx = CreateQuiltContext();

                var query = (IQueryable<OrderEvent>)ctx.OrderEvents.Include(r => r.OrderTransaction);
                if (orderId != null)
                {
                    query = query.Where(r => r.OrderTransaction.OrderTransactionItems.Any(r => r.OrderItem.OrderId == orderId));
                }
                if (!string.IsNullOrEmpty(unitOfWork))
                {
                    query = query.Where(r => r.OrderTransaction.UnitOfWork == unitOfWork);
                }
                var summaries = await query
                    .Select(r => Create.MOrder_OrderEventLogSummary(r))
                    .ToListAsync().ConfigureAwait(false);

                var result = new MOrder_OrderEventLogSummaryList
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

        #region Private Members

        private IOrderEventMicroService OrderEventService { get; }

        private void UpdateOrderTotals(Order dbOrder)
        {
            var orderPricing = OrderPricing.Compute(dbOrder);
            orderPricing.Apply(dbOrder);
        }

        private void UpdateFundsRequired(QuiltContext ctx, Order dbOrder, DateTime now, UnitOfWork unitOfWork)
        {
            if (OrderIsFundable(dbOrder))
            {
                var fundsRequiredDelta = dbOrder.TotalAmount - dbOrder.FundsRequired;
                if (fundsRequiredDelta != 0)
                {
                    _ = ctx.CreateOrderTransactionBuilder()
                        .Begin(dbOrder.OrderId, OrderTransactionTypeCodes.FundsRequired, now)
                        .UnitOfWork(unitOfWork)
                        .AddFundsRequired(fundsRequiredDelta)
                        .Event(OrderEventTypeCodes.FundingUpdate)
                        .Create();
                }
            }
        }

        private void UpdateFulfillmentRequiredQuantity(QuiltContext ctx, Order dbOrder, DateTime now, UnitOfWork unitOfWork)
        {
            if (OrderIsFulfillable(dbOrder))
            {
                OrderTransactionBuilder orderTransactionBuilder = null;

                foreach (var dbOrderItem in dbOrder.OrderItems)
                {
                    var fulfillmentRequiredDelta = dbOrderItem.NetQuantity - dbOrderItem.FulfillmentRequiredQuantity;
                    if (fulfillmentRequiredDelta != 0)
                    {
                        if (orderTransactionBuilder == null)
                        {
                            var description = $"Order {dbOrder.OrderId} fulfillment required.";

                            orderTransactionBuilder = ctx.CreateOrderTransactionBuilder()
                                .Begin(dbOrder.OrderId, OrderTransactionTypeCodes.FulfillmentRequired, now)
                                .UnitOfWork(unitOfWork)
                                .Event(OrderEventTypeCodes.FulfillmentUpdate);
                        }
                        _ = orderTransactionBuilder.AddFulfillmentRequiredQuantity(dbOrderItem.OrderItemId, fulfillmentRequiredDelta);
                    }
                }

                if (orderTransactionBuilder != null)
                {
                    if (!dbOrder.FulfillmentDateTimeUtc.HasValue)
                    {
                        _ = orderTransactionBuilder.SetOrderStatusCode(OrderStatusCodes.Fulfilling);
                    }
                    _ = orderTransactionBuilder.Create();
                }
            }
        }

        private void UpdateClose(QuiltContext ctx, Order dbOrder, DateTime now, UnitOfWork unitOfWork)
        {
            if (OrderIsCloseable(dbOrder))
            {
                if (dbOrder.OrderStatusCode != OrderStatusCodes.Closed)
                {
                    _ = ctx.CreateOrderTransactionBuilder()
                        .Begin(dbOrder.OrderId, OrderTransactionTypeCodes.Close, now)
                        .UnitOfWork(unitOfWork)
                        .SetOrderStatusCode(OrderStatusCodes.Closed)
                        .Event(OrderEventTypeCodes.Close)
                        .Create();
                }
            }
        }

        private bool OrderIsFundable(Order dbOrder)
        {
            return dbOrder.OrderStatusCode != OrderStatusCodes.Pending;
        }

        private bool OrderIsFulfillable(Order dbOrder)
        {
            return dbOrder.FundsReceived >= dbOrder.TotalAmount;
        }

        private bool OrderIsCloseable(Order dbOrder)
        {
            return
                dbOrder.OrderStatusCode != OrderStatusCodes.Pending
                && dbOrder.FundsReceived >= dbOrder.TotalAmount
                && dbOrder.OrderItems.All(r => r.FulfillmentCompleteQuantity >= r.FulfillmentRequiredQuantity);
        }

        private async Task<bool> StandardizeAddress(MCommon_Address shippingAddress)
        {
            var op = new UspsAddressValidateOperation(Logger, Locale, QuiltContextFactory, CommunicationMicroService);
            var result = await op.ExecuteAsync(shippingAddress.AddressLine1, shippingAddress.AddressLine2, shippingAddress.City, shippingAddress.StateCode, shippingAddress.PostalCode).ConfigureAwait(false);

            shippingAddress.AddressLine1 = result.Address2;
            shippingAddress.AddressLine2 = result.Address1;
            shippingAddress.City = result.City;
            shippingAddress.StateCode = result.StateCode;
            shippingAddress.PostalCode = result.PostalCode;

            return false;
        }

        #endregion



        private static class Create
        {
            public static MOrder_OrderSummary MOrder_OrderSummary(Order dbOrder)
            {
                return new MOrder_OrderSummary()
                {
                    // Order
                    //
                    OrderId = dbOrder.OrderId,
                    OrdererId = dbOrder.OrdererId,
                    OrderNumber = dbOrder.OrderNumber,
                    OrderDateTimeUtc = dbOrder.OrderStatusDateTimeUtc,
                    OrderStatus = GetValue.MOrder_OrderStatus(dbOrder.OrderStatusCode),
                    UpdateDateTimeUtc = dbOrder.UpdateDateTimeUtc,
                    ItemSubtotalAmount = dbOrder.ItemSubtotal,
                    ShippingAmount = dbOrder.Shipping,
                    DiscountAmount = dbOrder.Discount,
                    TaxableAmount = dbOrder.TaxableAmount,
                    SalesTaxPercent = dbOrder.SalesTaxRate,
                    SalesTaxAmount = dbOrder.SalesTax,
                    SalesTaxJurisdiction = dbOrder.SalexTaxJurisdiction,
                    TotalAmount = dbOrder.TotalAmount,

                    // Orderer
                    //
                    OrdererReference = dbOrder.Orderer.OrdererReference
                };
            }

            public static MOrder_Order MOrder_Order(Order dbOrder)
            {
                return new MOrder_Order()
                {
                    // Order
                    //
                    OrderId = dbOrder.OrderId,
                    OrdererId = dbOrder.OrdererId,
                    OrderNumber = dbOrder.OrderNumber,
                    OrderStatus = GetValue.MOrder_OrderStatus(dbOrder.OrderStatusCode),
                    StatusDateTimeUtc = dbOrder.OrderStatusDateTimeUtc,
                    ItemSubtotalAmount = dbOrder.ItemSubtotal,
                    ShippingAmount = dbOrder.Shipping,
                    DiscountAmount = dbOrder.Discount,
                    PretaxAmount = dbOrder.PretaxAmount,
                    TaxableAmount = dbOrder.TaxableAmount,
                    SalesTaxJurisdiction = dbOrder.SalexTaxJurisdiction,
                    SalesTaxPercent = dbOrder.SalesTaxRate,
                    SalesTaxAmount = dbOrder.SalesTax,
                    TotalAmount = dbOrder.TotalAmount,
                    FundsRequired = dbOrder.FundsRequired,
                    FundsReceived = dbOrder.FundsReceived,
                    SubmissionDateTimeUtc = dbOrder.SubmissionDateTimeUtc,
                    FulfillmentDateTimeUtc = dbOrder.FulfillmentDateTimeUtc,
                    UpdateDateTimeUtc = dbOrder.UpdateDateTimeUtc,

                    // Orderer
                    //
                    OrdererReference = dbOrder.Orderer.OrdererReference,

                    CanCancel = GetCanCancel(dbOrder),
                    CanPay = GetCanPay(dbOrder),
                    CanReturn = GetCanReturn(dbOrder),

                    ShippingAddress = MOrder_OrderShippingAddress(dbOrder.OrderShippingAddress),
                    OrderItems = MOrder_OrderItems(dbOrder.OrderItems)
                };
            }

            public static MOrder_OrderShippingAddress MOrder_OrderShippingAddress(OrderShippingAddress dbOrderShippingAddress)
            {
                return dbOrderShippingAddress != null
                    ? new MOrder_OrderShippingAddress()
                    {
                        Name = dbOrderShippingAddress.ShipToName,
                        AddressLine1 = dbOrderShippingAddress.ShipToAddressLine1,
                        AddressLine2 = dbOrderShippingAddress.ShipToAddressLine2,
                        City = dbOrderShippingAddress.ShipToCity,
                        StateCode = dbOrderShippingAddress.ShipToStateCode,
                        PostalCode = dbOrderShippingAddress.ShipToPostalCode,
                        CountryCode = dbOrderShippingAddress.ShipToCountryCode
                    }
                    : null;
            }

            public static MOrder_OrderItem MOrder_OrderItem(OrderItem dbOrderItem)
            {
                var components = new List<MOrder_OrderItemComponent>();
                foreach (var dbOrderableComponent in dbOrderItem.Orderable.OrderableComponents)
                {
                    components.Add(new MOrder_OrderItemComponent()
                    {
                        // OrderableComponent
                        //
                        OrderableComponentId = dbOrderableComponent.OrderableComponentId,
                        OrderableComponentReference = dbOrderableComponent.OrderableComponentReference,
                        UnitPrice = dbOrderableComponent.UnitPrice,
                        Quantity = dbOrderableComponent.Quantity,
                        TotalPrice = dbOrderableComponent.TotalPrice,
                        Description = dbOrderableComponent.Description,
                        ConsumableReference = dbOrderableComponent.ConsumableReference,
                        UpdateDateTimeUtc = dbOrderableComponent.UpdateDateTimeUtc
                    });
                }

                return new MOrder_OrderItem()
                {
                    // OrderItem
                    //
                    OrderItemId = dbOrderItem.OrderItemId,
                    OrderItemSequence = dbOrderItem.OrderItemSequence,
                    OrderableId = dbOrderItem.OrderableId,
                    NetQuantity = dbOrderItem.NetQuantity,
                    TotalPrice = dbOrderItem.TotalPrice,
                    CancelQuantity = dbOrderItem.CancelQuantity,
                    OrderQuantity = dbOrderItem.OrderQuantity,
                    UpdateDateTimeUtc = dbOrderItem.UpdateDateTimeUtc,
                    FulfillmentRequiredQuantity = dbOrderItem.FulfillmentRequiredQuantity,
                    FulfillmentCompleteQuantity = dbOrderItem.FulfillmentCompleteQuantity,
                    FulfillmentReturnQuantity = dbOrderItem.FulfillmentReturnQuantity,

                    // Orderable
                    //
                    OrderableReference = dbOrderItem.Orderable.OrderableReference,
                    Description = dbOrderItem.Orderable.Description,
                    ConsumableReference = dbOrderItem.Orderable.ConsumableReference,
                    UnitPrice = dbOrderItem.Orderable.Price,

                    CanReturn = GetCanReturn(dbOrderItem),

                    OrderItemComponents = components
                };
            }

            public static IList<MOrder_OrderItem> MOrder_OrderItems(IEnumerable<OrderItem> dbOrderItems)
            {
                var orderItems = new List<MOrder_OrderItem>();
                foreach (var dbOrderItem in dbOrderItems)
                {
                    orderItems.Add(MOrder_OrderItem(dbOrderItem));
                }
                return orderItems;
            }

            public static MOrder_OrderEvent MOrder_OrderEvent(OrderEvent dbOrderEvent, string unitOfWork)
            {
                var dbTransaction = dbOrderEvent.OrderTransaction;
                var dbOrder = dbTransaction.Order;
                var dbOrderShippingAddress = dbOrder.OrderShippingAddress;

                var eventData = new MOrder_OrderEvent()
                {
                    EventTypeCode = dbOrderEvent.EventTypeCode,
                    UnitOfWork = unitOfWork,
                    OrderId = dbOrder.OrderId,
                    OrderNumber = dbOrder.OrderNumber,
                    OrdererId = dbOrder.OrdererId,
                    OrdererReference = dbOrder.Orderer.OrdererReference,
                    FundsRequiredIncome = dbOrder.FundsRequired - dbOrder.SalesTax,
                    FundsRequiredSalesTax = dbOrder.SalesTax,
                    FundsRequiredSalesTaxJurisdiction = dbOrder.SalexTaxJurisdiction,
                    FundsReceived = dbOrder.FundsReceived,
                    ShippingAddress = MCommon_Address(dbOrderShippingAddress),
                    OrderEventItems = MOrder_OrderEventItems(dbTransaction)
                };

                return eventData;
            }

            public static MOrder_OrderTransaction MOrder_OrderTransaction(OrderTransaction dbOrderTransaction)
            {
                return new MOrder_OrderTransaction()
                {
                    TransactionId = dbOrderTransaction.OrderTransactionId,
                    EntityId = dbOrderTransaction.OrderId,
                    TransactionDateTimeUtc = dbOrderTransaction.TransactionDateTimeUtc,
                    Description = dbOrderTransaction.Description,
                    UnitOfWork = dbOrderTransaction.UnitOfWork,

                    OrderStatus = dbOrderTransaction.OrderStatusCode != null ? (MOrder_OrderStatus?)GetValue.MOrder_OrderStatus(dbOrderTransaction.OrderStatusCode) : null,
                    FundsRequired = dbOrderTransaction.FundsRequired,
                    FundsReceived = dbOrderTransaction.FundsReceived
                };
            }

            public static MOrder_OrderTransactionSummary MOrder_OrderTransactionSummary(OrderTransaction dbOrderTransaction)
            {
                return new MOrder_OrderTransactionSummary()
                {
                    TransactionId = dbOrderTransaction.OrderTransactionId,
                    EntityId = dbOrderTransaction.OrderId,
                    TransactionDateTimeUtc = dbOrderTransaction.TransactionDateTimeUtc,
                    Description = dbOrderTransaction.Description,
                    UnitOfWork = dbOrderTransaction.UnitOfWork,

                    OrderStatus = dbOrderTransaction.OrderStatusCode != null ? (MOrder_OrderStatus?)GetValue.MOrder_OrderStatus(dbOrderTransaction.OrderStatusCode) : null,
                    FundsRequired = dbOrderTransaction.FundsRequired,
                    FundsReceived = dbOrderTransaction.FundsReceived
                };
            }

            public static MOrder_OrderEventLog MOrder_OrderEventLog(OrderEvent dbOrderEvent)
            {
                return new MOrder_OrderEventLog()
                {
                    EventId = dbOrderEvent.OrderEventId,
                    TransactionId = dbOrderEvent.OrderTransactionId,
                    EventTypeCode = dbOrderEvent.EventTypeCode,
                    EventDateTimeUtc = dbOrderEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbOrderEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbOrderEvent.ProcessingStatusDateTimeUtc
                };
            }

            public static MOrder_OrderEventLogSummary MOrder_OrderEventLogSummary(OrderEvent dbOrderEvent)
            {
                return new MOrder_OrderEventLogSummary()
                {
                    EventId = dbOrderEvent.OrderEventId,
                    TransactionId = dbOrderEvent.OrderTransactionId,
                    EntityId = dbOrderEvent.OrderTransaction.OrderId,
                    EventTypeCode = dbOrderEvent.EventTypeCode,
                    EventDateTimeUtc = dbOrderEvent.EventDateTimeUtc,
                    ProcessingStatusCode = dbOrderEvent.ProcessingStatusCode,
                    StatusDateTimeUtc = dbOrderEvent.ProcessingStatusDateTimeUtc,
                    UnitOfWork = dbOrderEvent.OrderTransaction.UnitOfWork
                };
            }

            private static MCommon_Address MCommon_Address(OrderShippingAddress dbOrderShippingAddress)
            {
                return new MCommon_Address()
                {
                    Name = dbOrderShippingAddress.ShipToName,
                    AddressLine1 = dbOrderShippingAddress.ShipToAddressLine1,
                    AddressLine2 = dbOrderShippingAddress.ShipToAddressLine2,
                    City = dbOrderShippingAddress.ShipToCity,
                    StateCode = dbOrderShippingAddress.ShipToStateCode,
                    PostalCode = dbOrderShippingAddress.ShipToPostalCode,
                    CountryCode = dbOrderShippingAddress.ShipToCountryCode
                };
            }

            private static List<MOrder_OrderEventItem> MOrder_OrderEventItems(OrderTransaction dbTransaction)
            {
                var fulfillmentEventItems = new List<MOrder_OrderEventItem>();
                foreach (var dbTransactionItem in dbTransaction.OrderTransactionItems)
                {
                    fulfillmentEventItems.Add(MOrder_OrderEventItem(dbTransactionItem));
                }

                return fulfillmentEventItems;
            }

            private static MOrder_OrderEventItem MOrder_OrderEventItem(OrderTransactionItem dbTransactionItem)
            {
                var dbOrderItem = dbTransactionItem.OrderItem;
                var dbOrderable = dbOrderItem.Orderable;

                return new MOrder_OrderEventItem()
                {
                    OrderItemId = dbOrderItem.OrderItemId,
                    RequiredQuantity = dbOrderItem.FulfillmentRequiredQuantity,
                    CompleteQuantity = dbOrderItem.FulfillmentCompleteQuantity,
                    ReturnQuantity = dbOrderItem.FulfillmentReturnQuantity,

                    OrderableId = dbOrderable.OrderableId,
                    OrderableReference = dbOrderable.OrderableReference,
                    Description = dbOrderable.Description,
                    ConsumableReference = dbOrderable.ConsumableReference,

                    OrderEventItemComponents = MOrder_OrderEventItemComponents(dbOrderable)
                };
            }

            private static List<MOrder_OrderEventItemComponent> MOrder_OrderEventItemComponents(Orderable dbOrderable)
            {
                var fulfillmentEventItemComponents = new List<MOrder_OrderEventItemComponent>();
                foreach (var dbOrderableComponent in dbOrderable.OrderableComponents)
                {
                    fulfillmentEventItemComponents.Add(MOrder_OrderEventItemComponent(dbOrderableComponent));
                }

                return fulfillmentEventItemComponents;
            }

            private static MOrder_OrderEventItemComponent MOrder_OrderEventItemComponent(OrderableComponent dbOrderableComponent)
            {
                return new MOrder_OrderEventItemComponent()
                {
                    OrderableComponentId = dbOrderableComponent.OrderableComponentId,
                    OrderableComponentReference = dbOrderableComponent.OrderableComponentReference,
                    Description = dbOrderableComponent.Description,
                    ConsumableReference = dbOrderableComponent.ConsumableReference,
                    Quantity = dbOrderableComponent.Quantity
                };
            }

            private static bool GetCanCancel(Order dbOrder)
            {
                foreach (var dbOrderItem in dbOrder.OrderItems)
                {
                    if (dbOrderItem.FulfillmentRequiredQuantity > 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            private static bool GetCanPay(Order dbOrder)
            {
                var canPay = false;
                //var dbOrderLedgerAccount = dbOrder.OrderLedgerAccounts.Where(r => r.LedgerAccountTypeCode == LedgerAccountTypes.AccountReceivable).SingleOrDefault();
                //if (dbOrderLedgerAccount != null && dbOrderLedgerAccount.Balance > 0)
                //{
                //    canPay = true;
                //}

                return canPay;
            }

            private static bool GetCanReturn(Order dbOrder)
            {
                foreach (var dbOrderItem in dbOrder.OrderItems)
                {
                    if (GetCanReturn(dbOrderItem))
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool GetCanReturn(OrderItem dbOrderItem)
            {
                return dbOrderItem.FulfillmentCompleteQuantity > 0;
            }
        }
    }
}
