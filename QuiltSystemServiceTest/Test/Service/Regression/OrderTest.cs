//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Test.Service.Regression
{
    [TestClass]
    public class OrderTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateOrder()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<OrderTest>>();

            var ordererReference = CreateOrdererReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Orderer reference = {ordererReference}");

            var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);
            logger.LogInformation($"Orderer ID = {ordererId}");

            var orderableReference = CreateOrderableReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Orderable reference = {orderableReference}");

            var orderableComponentReference = CreateOrderableCompnentReference.FromTimestamp(GetUniqueNow());
            logger.LogInformation($"Orderable component reference = {orderableComponentReference}");

            var mAllocateOrderable = new MOrder_AllocateOrderable()
            {
                OrderableReference = orderableReference,
                Name = "Orderable Item",
                Price = 100,
                Components = new List<MOrder_AllocateOrderableComponent>()
                {
                    new MOrder_AllocateOrderableComponent()
                    {
                        OrderableComponentReference = orderableComponentReference,
                        Description = "Orderable Item Component",
                        Quantity = 2,
                        ConsumableReference = CreateConsumableReference.FromTimestamp(GetUniqueNow()),
                        UnitPrice = 25,
                        TotalPrice = 50
                    }
                }
            };

            var mAllocateOrderableResponse = await OrderMicroService.AllocateOrderableAsync(mAllocateOrderable);
            logger.LogInformation($"Orderable ID = {mAllocateOrderableResponse.OrderableId}");

            _ = await OrderMicroService.AddCartItemAsync(ordererId, mAllocateOrderableResponse.OrderableId, 2);
            logger.LogInformation("Item added to cart.");

            var shippingAddress = new MCommon_Address()
            {
                Name = "RICHARD TODD",
                AddressLine1 = "17340 W 156 TER",
                City = "OLATHE",
                StateCode = "KS",
                PostalCode = "66062",
                CountryCode = "US"
            };
            _ = await OrderMicroService.UpdateShippingAddressAsync(ordererId, shippingAddress);
            logger.LogInformation("Shipping address updated.");

            var orderId = await OrderMicroService.SubmitCartAsync(ordererId);
            logger.LogInformation($"Order {orderId} submitted.");
        }

    }
}
