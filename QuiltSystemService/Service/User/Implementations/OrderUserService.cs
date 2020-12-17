//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class OrderUserService : BaseService, IOrderUserService
    {
        private IDesignMicroService DesignMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }

        public OrderUserService(
            IApplicationRequestServices requestServices,
            ILogger<OrderUserService> logger,
            IDesignMicroService designMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IOrderMicroService orderMicroService)
            : base(requestServices, logger)
        {
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
        }

        public async Task<UOrder_Order> GetOrderAsync(long orderId)
        {
            using var log = BeginFunction(nameof(OrderUserService), nameof(GetOrderAsync), orderId);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var mOrder = await OrderMicroService.GetOrderAsync(orderId).ConfigureAwait(false);

                //var fulfillableReference = CreateFulfillableReference.FromOrderId(orderId);
                //var fulfillableId = await FulfillmentMicroService.AllocateFulfillableAsync(fulfillableReference);

                var result = Create.UOrder_Order(mOrder, null);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<IReadOnlyList<UOrder_Order>> GetOrdersAsync(string userId)
        {
            using var log = BeginFunction(nameof(OrderUserService), nameof(GetOrdersAsync), userId);
            try
            {
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.LookupOrdererAsync(ordererReference);
                if (ordererId == null)
                {
                    return null;
                }

                var mOrderList = await OrderMicroService.GetOrdersAsync(ordererId.Value).ConfigureAwait(false);

                var orders = new List<UOrder_Order>();
                foreach (var mOrder in mOrderList.Orders)
                {
                    orders.Add(Create.UOrder_Order(mOrder, null));
                }

                var result = orders;

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
            public static UOrder_Order UOrder_Order(MOrder_Order mOrder, MFulfillment_Fulfillable mFulfillable)
            {
                if (mOrder == null) throw new ArgumentNullException(nameof(mOrder));

                return new UOrder_Order()
                {
                    MOrder = mOrder,
                    MFulfillable = mFulfillable
                };
            }
        }
    }
}