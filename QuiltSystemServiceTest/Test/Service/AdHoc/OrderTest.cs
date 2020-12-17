//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Test.Service.AdHoc
{
    [TestClass]
    public class OrderTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize(mockEvents: false);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        [TestMethod]
        public async Task CreateAndShipOrder()
        {
            var services = ServiceScope.ServiceProvider;
            var logger = services.GetService<ILogger<OrderTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            // Get the user ID.
            //
            string userId;
            {
                var identityUser = await UserManager.FindByNameAsync("user@richtodd.com");
                userId = identityUser.Id;
                logger.LogInformation("User ID = {0}", userId);
            }

            // Get the orderer ID.
            //
            long ordererId;
            {
                var ordererReference = CreateOrdererReference.FromTimestamp(GetUniqueNow());
                ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);
                logger.LogInformation("Orderer ID = {0}", ordererId);
            }

            // Get the funder ID.
            //
            long funderId;
            {
                var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
                funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
                logger.LogInformation("Funder ID = {0}", funderId);
            }

            // Create the design.
            //
            Guid designId;
            {
                var designData = Factory.CreateDesign();
                designId = await DesignAjaxService.SaveDesignAsync(userId, designData);
                logger.LogInformation($"Design ID = {designId}");
            }

            // Create the project.
            //
            string projectId;
            {
                projectId = await ProjectUserService.CreateProjectAsync(userId, ProjectUserService.ProjectType_Kit, "Test Project", designId);
                logger.LogInformation($"Project ID = {projectId}");
            }

            // Create the orderable ID.
            //
            long orderableId;
            {
                var projectSnapshotId = await ProjectMicroService.GetCurrentSnapshotIdAsync(Guid.Parse(projectId));
                var mProjectSnapshotDetail = await ProjectMicroService.GetProjectSnapshotAsync(projectSnapshotId);
                var mAllocateOrderable = MicroDataFactory.MOrder_AllocateOrderable(mProjectSnapshotDetail);
                var mAllocateOrderableResponseData = await OrderMicroService.AllocateOrderableAsync(mAllocateOrderable);
                orderableId = mAllocateOrderableResponseData.OrderableId;
                logger.LogInformation($"Orderable ID = {orderableId}");
            }

            var shippingAddress = new MCommon_Address()
            {
                Name = "RICH TODD",
                AddressLine1 = "17340 W 156 TER",
                City = "OLATHE",
                StateCode = "KS",
                PostalCode = "66062",
                CountryCode = "US"
            };

            // Create the order.
            //
            long orderId;
            {
                // Add item to cart.
                //
                _ = await OrderMicroService.AddCartItemAsync(ordererId, orderableId, 2);

                // Update shipping address.
                //
                _ = await OrderMicroService.UpdateShippingAddressAsync(ordererId, shippingAddress);

                // Submit order.
                //
                orderId = await OrderMicroService.SubmitCartAsync(ordererId);
                logger.LogInformation($"Order ID = {orderId}");
            }

            _ = await EventProcessorMicroService.ProcessPendingEvents();

            // Create fundable for order.
            //
            long fundableId;
            {
                var fundableReference = CreateFundableReference.FromOrderId(orderId);
                fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
                logger.LogInformation($"Fundable ID = {fundableId}");
            }

            // Retrieve the fundable detail.
            //
            MFunding_Fundable fundableDetail;
            {
                fundableDetail = await FundingMicroService.GetFundableAsync(fundableId);
                logger.LogInformation($"Fundable Detail = {fundableDetail}");
            }

            // Post a receipt for the order.
            //
            {
                var fundsRequiredDelta = fundableDetail.FundsRequiredTotal - fundableDetail.FundsReceived;
                await FundingMicroService.SetFundsReceivedAsync(funderId, fundableDetail.FundableReference, fundableDetail.FundsRequiredTotal, unitOfWork.Next());
                //await fundingService.TransferFundsAsync(funderId, fundableId, fundsRequiredDelta);
                logger.LogInformation($"{fundsRequiredDelta} funds applied.");
            }

            _ = await EventProcessorMicroService.ProcessPendingEvents();

            // Lookup fulfillable.
            //
            long fulfillableId;
            {
                var fulfillableReference = CreateFulfillableReference.FromOrderId(orderId);
                fulfillableId = (await FulfillmentMicroService.LookupFulfillableAsync(fulfillableReference)).Value;
                await FulfillmentMicroService.SetFulfillableShippingAddress(fulfillableId, shippingAddress);
                logger.LogInformation($"Fulfillable ID = {fulfillableId}");
            }

            // Lookup pending shipment request.
            //
            long? shipmentRequestId;
            {
                shipmentRequestId = await FulfillmentMicroService.GetPendingShipmentRequestAsync(fulfillableId);
                logger.LogInformation($"Shipment Request ID = {shipmentRequestId}");
            }

            // Open it.
            //
            {
                await FulfillmentMicroService.OpenShipmentRequestAsync(shipmentRequestId.Value);
            }

            // Lookup shipment request.
            //
            MFulfillment_ShipmentRequest shipmentRequestDetail;
            {
                shipmentRequestDetail = await FulfillmentMicroService.GetShipmentRequestAsync(shipmentRequestId.Value);
                logger.LogInformation($"Shipment Request Detail = {shipmentRequestDetail}");
            }

            // Create shipment.
            //
            long shipmentId;
            {
                var items = new List<MFulfillment_CreateShipmentItem>();
                foreach (var shipmentRequestItemDetail in shipmentRequestDetail.ShipmentRequestItems)
                {
                    items.Add(new MFulfillment_CreateShipmentItem()
                    {
                        ShipmentRequestItemId = shipmentRequestItemDetail.ShipmentRequestItemId,
                        Quantity = shipmentRequestItemDetail.Quantity
                    });
                }

                var data = new MFulfillment_CreateShipment()
                {
                    ShippingVendorId = ShippingVendorIds.Usps,
                    ShipmentDateTimeUtc = Locale.GetUtcNow(),
                    TrackingCode = "123123",
                    CreateShipmentItems = items
                };

                shipmentId = await FulfillmentMicroService.CreateShipmentAsync(data);
                await FulfillmentMicroService.PostShipmentAsync(shipmentId);
                await FulfillmentMicroService.ProcessShipmentAsync(shipmentId);

                logger.LogInformation($"Shipment ID = {shipmentId}");
            }

            _ = await EventProcessorMicroService.ProcessPendingEvents();
        }

        [TestMethod]
        public async Task CreateOrder()
        {
            var services = ServiceScope.ServiceProvider;
            var logger = services.GetService<ILogger<OrderTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            // Get the user ID.
            //
            string userId;
            {
                var identityUser = await UserManager.FindByNameAsync("user@richtodd.com");
                userId = identityUser.Id;
                logger.LogInformation("User ID = {0}", userId);
            }

            // Get the orderer ID.
            //
            long ordererId;
            {
                var ordererReference = CreateOrdererReference.FromTimestamp(GetUniqueNow());
                ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);
                logger.LogInformation("Orderer ID = {0}", ordererId);
            }

            // Get the funder ID.
            //
            long funderId;
            {
                var funderReference = CreateFunderReference.FromTimestamp(GetUniqueNow());
                funderId = await FundingMicroService.AllocateFunderAsync(funderReference);
                logger.LogInformation("Funder ID = {0}", funderId);
            }

            // Create the design.
            //
            Guid designId;
            {
                var designData = Factory.CreateDesign();
                designId = await DesignAjaxService.SaveDesignAsync(userId, designData);
                logger.LogInformation($"Design ID = {designId}");
            }

            for (int idx = 0; idx < 3; ++idx)
            {
                // Create the project.
                //
                string projectId;
                {
                    projectId = await ProjectUserService.CreateProjectAsync(userId, ProjectUserService.ProjectType_Kit, "Test Project", designId);
                    logger.LogInformation($"Project ID = {projectId}");
                }

                // Create the orderable ID.
                //
                long orderableId;
                {
                    var projectSnapshotId = await ProjectMicroService.GetCurrentSnapshotIdAsync(Guid.Parse(projectId));
                    var mProjectSnapshotDetail = await ProjectMicroService.GetProjectSnapshotAsync(projectSnapshotId);
                    var mAllocateOrderable = MicroDataFactory.MOrder_AllocateOrderable(mProjectSnapshotDetail);
                    var mAllocateOrderableResponseData = await OrderMicroService.AllocateOrderableAsync(mAllocateOrderable);
                    orderableId = mAllocateOrderableResponseData.OrderableId;
                    logger.LogInformation($"Orderable ID = {orderableId}");
                }

                // Add orderable to cart
                //
                {
                    // Add item to cart.
                    //
                    _ = await OrderMicroService.AddCartItemAsync(ordererId, orderableId, (idx + 1) * 2);
                }
            }

            var shippingAddress = new MCommon_Address()
            {
                Name = "RICH TODD",
                AddressLine1 = "17340 W 156 TER",
                City = "OLATHE",
                StateCode = "KS",
                PostalCode = "66062",
                CountryCode = "US"
            };

            // Create the order.
            //
            long orderId;
            {
                // Update shipping address.
                //
                _ = await OrderMicroService.UpdateShippingAddressAsync(ordererId, shippingAddress);

                // Submit order.
                //
                orderId = await OrderMicroService.SubmitCartAsync(ordererId);
                logger.LogInformation($"Order ID = {orderId}");
            }

            _ = await EventProcessorMicroService.ProcessPendingEvents();

            // Create fundable for order.
            //
            long fundableId;
            {
                var fundableReference = CreateFundableReference.FromOrderId(orderId);
                fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference);
                logger.LogInformation($"Fundable ID = {fundableId}");
            }

            // Retrieve the fundable detail.
            //
            MFunding_Fundable fundableDetail;
            {
                fundableDetail = await FundingMicroService.GetFundableAsync(fundableId);
                logger.LogInformation($"Fundable Detail = {fundableDetail}");
            }

            // Post a receipt for the order.
            //
            {
                var fundsRequiredDelta = fundableDetail.FundsRequiredTotal - fundableDetail.FundsReceived;
                await FundingMicroService.SetFundsReceivedAsync(funderId, fundableDetail.FundableReference, fundableDetail.FundsRequiredTotal, unitOfWork.Next());
                //await fundingService.TransferFundsAsync(funderId, fundableId, fundsRequiredDelta);
                logger.LogInformation($"{fundsRequiredDelta} funds applied.");
            }

            _ = await EventProcessorMicroService.ProcessPendingEvents();
        }

        //private async Task ProcessEvents(IServiceProvider services, ILogger logger)
        //{
        //    var eventsRaised = true;
        //    while (eventsRaised)
        //    {
        //        eventsRaised = false;

        //        // Process order events.
        //        //
        //        {
        //            var orderMicroService = services.GetService<IOrderMicroService>();
        //            var count = await orderMicroService.ProcessEventsAsync();
        //            logger.LogInformation($"Processed {count} order events.");
        //            eventsRaised = eventsRaised || count > 0;
        //        }

        //        // Process funding events.
        //        //
        //        {
        //            var fundingMicroService = services.GetService<IFundingMicroService>();
        //            var count = await fundingMicroService.ProcessEventsAsync();
        //            logger.LogInformation($"Processed {count} funding events.");
        //            eventsRaised = eventsRaised || count > 0;
        //        }

        //        // Process fulfillment events.
        //        //
        //        {
        //            var fulfillmentMicroService = services.GetService<IFulfillmentMicroService>();
        //            var count = await fulfillmentMicroService.ProcessEventsAsync();
        //            logger.LogInformation($"Processed {count} fulfillment events.");
        //            eventsRaised = eventsRaised || count > 0;
        //        }
        //    }
        //}
    }
}
