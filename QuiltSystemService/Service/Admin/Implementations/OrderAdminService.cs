//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class OrderAdminService : BaseService, IOrderAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IFundingMicroService FundingMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public OrderAdminService(
            IApplicationRequestServices requestServices,
            ILogger<OrderAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IFundingMicroService fundingMicroService,
            IOrderMicroService orderMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task EnsureOrderAcceptedAsync(long orderId)
        {
            using var log = BeginFunction(nameof(OrderAdminService), nameof(EnsureOrderAcceptedAsync), orderId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AOrder_Order> GetOrderAsync(long orderId)
        {
            using var log = BeginFunction(nameof(OrderAdminService), nameof(GetOrderAsync), orderId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var mOrder = await OrderMicroService.GetOrderAsync(orderId).ConfigureAwait(false);
                var mTransactions = await OrderMicroService.GetOrderTransactionSummariesAsync(orderId, null, null);
                var mEvents = await OrderMicroService.GetOrderEventLogSummariesAsync(orderId, null, null);

                var fulfillableReference = CreateFulfillableReference.FromOrderId(orderId);
                var fulfillableId = await FulfillmentMicroService.LookupFulfillableAsync(fulfillableReference);
                var mFulfillable = fulfillableId != null
                    ? await FulfillmentMicroService.GetFulfillableAsync(fulfillableId.Value).ConfigureAwait(false)
                    : null;

                var fundableReference = CreateFundableReference.FromOrderId(orderId);
                var fundableId = await FundingMicroService.LookupFundableAsync(fundableReference);
                var mFundable = fundableId != null
                    ? await FundingMicroService.GetFundableAsync(fundableId.Value).ConfigureAwait(false)
                    : null;

                var mUser = TryParseUserId.FromOrdererReference(mOrder.OrdererReference, out string userId)
                    ? await UserMicroService.GetUserAsync(userId)
                    : null;

                var result = Create.AOrder_Order(mOrder, mTransactions, mEvents, mFulfillable, mFundable, mUser);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<AOrder_OrderSummary>> GetOrderSummariesAsync(string orderNumber, DateTime? orderDateUtc, MOrder_OrderStatus orderStatus, string userName, int? recordCount)
        {
            using var log = BeginFunction(nameof(OrderAdminService), nameof(GetOrderSummariesAsync), orderNumber, orderDateUtc, orderStatus, userName, recordCount);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                long? ordererId;
                if (!string.IsNullOrEmpty(userName))
                {
                    var userId = await UserMicroService.LookupUserIdAsync(userName).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var ordererReference = CreateOrdererReference.FromUserId(userId);
                        ordererId = await OrderMicroService.LookupOrdererAsync(ordererReference);
                    }
                    else
                    {
                        ordererId = null;
                    }
                }
                else
                {
                    ordererId = null;
                }

                var mOrderSummaryList = await OrderMicroService.GetOrderSummariesAsync(orderNumber, orderDateUtc, orderStatus, ordererId, recordCount).ConfigureAwait(false);

                var orderSummaries = new List<AOrder_OrderSummary>();
                foreach (var mOrderSummary in mOrderSummaryList.Summaries)
                {
                    orderSummaries.Add(new AOrder_OrderSummary()
                    {
                        OrderId = mOrderSummary.OrderId,
                        OrderNumber = mOrderSummary.OrderNumber,
                        OrderDateTimeUtc = mOrderSummary.OrderDateTimeUtc,
                        StatusDateTimeUtc = mOrderSummary.UpdateDateTimeUtc,
                        OrderStatusType = mOrderSummary.OrderStatus.ToString(),
                        Total = mOrderSummary.TotalAmount,
                        UserId = "",
                        UserName = ""
                    });
                }

                var result = orderSummaries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> PostOrderTransactionAsync(AOrder_PostOrderTransaction transaction)
        {
            using var log = BeginFunction(nameof(OrderAdminService), nameof(PostOrderTransactionAsync), transaction);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                if (transaction == null) throw new ArgumentNullException(nameof(transaction));

                //using var ctx = QuiltContextFactory.Create();

                //var dbOrder = await ctx.Orders.Where(r => r.OrderId == transaction.OrderId).SingleAsync().ConfigureAwait(false);

                //var description = string.Format("Posting manual transaction for order {0}.", transaction.OrderId);

                // HACK: OrderStatus
                /*
            OrderTransaction dbOrderTransaction;
            {
                var dbOrderTransactionBuilder = dbOrder.CreateOrderTransactionBuilder(GetUtcNow())
                    .Begin(transaction.OrderTransactionTypeCode, description);

                            foreach (var item in transaction.Items)
                            {
                                _ = dbOrderTransactionBuilder.AddItem(item.OrderItemId, item.Quantity, item.OrderItemStatusTypeCode);
                            }

                dbOrderTransaction = dbOrderTransactionBuilder.Create();
            }

                ctx.OnOrderTransactionCreated(dbOrderTransaction, Locale.GetUtcNow());

                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);

                var result = dbOrderTransaction.OrderTransactionId;
                            */
                var result = 0L;

                log.Result(result);
                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public static class Create
        {
            public static AOrder_Order AOrder_Order(
                MOrder_Order mOrder,
                MOrder_OrderTransactionSummaryList mTransactions,
                MOrder_OrderEventLogSummaryList mEvents,
                MFulfillment_Fulfillable mFulfillable,
                MFunding_Fundable mFundable,
                MUser_User mUser)
            {
                if (mOrder == null) throw new ArgumentNullException(nameof(mOrder));

                return new AOrder_Order()
                {
                    MOrder = mOrder,
                    MTransactions = mTransactions,
                    MEvents = mEvents,
                    MFulfillable = mFulfillable,
                    MFundable = mFundable,
                    MUser = mUser
                };
            }
        }
    }
}
