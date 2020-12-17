//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RichTodd.QuiltSystem.Database.Builders;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Test.Service.Regression
{
    [TestClass]
    public class FulfillmentTest : BaseTest
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
        public async Task CreateFulfillable()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FulfillmentTest>>();
            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            var allocateFulfillableResponse = await FulfillmentMicroService.AllocateFulfillableAsync(RandomFulfillable());
            var fulfillableId = allocateFulfillableResponse.FulfillableId;
            logger.LogInformation($"Fulfillable ID = {fulfillableId}");
            foreach (var allowcateFulfillableItemResponse in allocateFulfillableResponse.FulfillableItemResponses)
            {
                var fulfillableItemId = allowcateFulfillableItemResponse.FulfillableItemId;
                logger.LogInformation($"Fulfillable Item ID = {fulfillableItemId}");
            }

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            foreach (var allowcateFulfillableItemResponse in allocateFulfillableResponse.FulfillableItemResponses)
            {
                var quantity = Random.Next(9) + 1;
                var fulfillableItemId = allowcateFulfillableItemResponse.FulfillableItemId;
                logger.LogInformation($"Fulfillable Item ID = {fulfillableItemId}");
                await FulfillmentMicroService.SetFulfillmentRequestQuantity(fulfillableItemId, quantity, unitOfWork.Next());
                logger.LogInformation($"Fulfillment item {fulfillableItemId} required quantity updated to {quantity}.");
            }

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            var mShipmentRequestSummaryList = await FulfillmentMicroService.GetShipmentRequestSummariesAsync(MFulfillment_ShipmentRequestStatus.Pending, null);
            Assert.IsTrue(mShipmentRequestSummaryList.Summaries.Any(r => r.FulfillableId == fulfillableId));
        }

        [TestMethod]
        public async Task CreateShipment()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FulfillmentTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());
            int maxQuantity = 10;

            var mShipmentRequest = await SetupShipmentRequest(unitOfWork, maxQuantity);

            var mCreateShipmentItems = new List<MFulfillment_CreateShipmentItem>();
            foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
            {
                mCreateShipmentItems.Add(new MFulfillment_CreateShipmentItem()
                {
                    ShipmentRequestItemId = mShipmentRequestItem.ShipmentRequestItemId,
                    Quantity = mShipmentRequestItem.Quantity
                });
            }
            var mCreateShipment = new MFulfillment_CreateShipment()
            {
                ShipmentDateTimeUtc = DateTime.Today,
                TrackingCode = "1234567890",
                ShippingVendorId = "USPS",
                CreateShipmentItems = mCreateShipmentItems
            };
            var shipmentId = await FulfillmentMicroService.CreateShipmentAsync(mCreateShipment);
            logger.LogInformation($"Shipment {shipmentId} created.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            await FulfillmentMicroService.PostShipmentAsync(shipmentId);
            logger.LogInformation($"Shipment {shipmentId} posted.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            await FulfillmentMicroService.ProcessShipmentAsync(shipmentId);
            logger.LogInformation($"Shipment {shipmentId} processed.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");
        }

        [TestMethod]
        public async Task CreateShipmentPartial()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FulfillmentTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());
            int maxQuantity = 10;

            var firstShippedQuantity = 5;

            var mShipmentRequest = await SetupShipmentRequest(unitOfWork, maxQuantity);

            // Create first shipment.
            {
                var mCreateShipmentItems = new List<MFulfillment_CreateShipmentItem>();
                foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
                {
                    mCreateShipmentItems.Add(new MFulfillment_CreateShipmentItem()
                    {
                        ShipmentRequestItemId = mShipmentRequestItem.ShipmentRequestItemId,
                        Quantity = Math.Min(mShipmentRequestItem.Quantity, firstShippedQuantity)
                    });
                }
                var mCreateShipment = new MFulfillment_CreateShipment()
                {
                    ShipmentDateTimeUtc = DateTime.Today,
                    TrackingCode = "1234567890",
                    ShippingVendorId = "USPS",
                    CreateShipmentItems = mCreateShipmentItems
                };
                var shipmentId = await FulfillmentMicroService.CreateShipmentAsync(mCreateShipment);
                logger.LogInformation($"Shipment {shipmentId} created.");

                var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                logger.LogInformation($"{eventCount} events processed.");

                await FulfillmentMicroService.PostShipmentAsync(shipmentId);
                logger.LogInformation($"Shipment {shipmentId} posted.");

                eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                logger.LogInformation($"{eventCount} events processed.");

                await FulfillmentMicroService.ProcessShipmentAsync(shipmentId);
                logger.LogInformation($"Shipment {shipmentId} processed.");

                eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                logger.LogInformation($"{eventCount} events processed.");
            }

            // Create second shipment.
            {
                var mCreateShipmentItems = new List<MFulfillment_CreateShipmentItem>();
                foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
                {
                    if (mShipmentRequestItem.Quantity > firstShippedQuantity)
                    {
                        mCreateShipmentItems.Add(new MFulfillment_CreateShipmentItem()
                        {
                            ShipmentRequestItemId = mShipmentRequestItem.ShipmentRequestItemId,
                            Quantity = mShipmentRequestItem.Quantity - firstShippedQuantity
                        });
                    }
                }
                if (mCreateShipmentItems.Count > 0)
                {
                    var mCreateShipment = new MFulfillment_CreateShipment()
                    {
                        ShipmentDateTimeUtc = DateTime.Today,
                        TrackingCode = "1234567890",
                        ShippingVendorId = "USPS",
                        CreateShipmentItems = mCreateShipmentItems
                    };
                    var shipmentId = await FulfillmentMicroService.CreateShipmentAsync(mCreateShipment);
                    logger.LogInformation($"Shipment {shipmentId} created.");

                    var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                    logger.LogInformation($"{eventCount} events processed.");

                    await FulfillmentMicroService.PostShipmentAsync(shipmentId);
                    logger.LogInformation($"Shipment {shipmentId} posted.");

                    eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                    logger.LogInformation($"{eventCount} events processed.");

                    await FulfillmentMicroService.ProcessShipmentAsync(shipmentId);
                    logger.LogInformation($"Shipment {shipmentId} processed.");

                    eventCount = await EventProcessorMicroService.ProcessPendingEvents();
                    logger.LogInformation($"{eventCount} events processed.");
                }
            }
        }

        [TestMethod]
        public async Task CreateReturnRequest()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FulfillmentTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());
            int maxQuantity = 10;

            var mShipment = await SetupShipment(unitOfWork, maxQuantity);

            var mCreateReturnRequestItems = new List<MFulfillment_CreateReturnRequestItem>();
            foreach (var mShipmentItem in mShipment.ShipmentItems)
            {
                mCreateReturnRequestItems.Add(new MFulfillment_CreateReturnRequestItem()
                {
                    FulfillableItemId = mShipmentItem.FulfillableItemId,
                    Quantity = mShipmentItem.Quantity
                });
            }
            var mCreateReturnRequest = new MFulfillment_CreateReturnRequest()
            {
                ReturnRequestType = MFulfillment_ReturnRequestTypes.Return,
                ReturnRequestReasonCode = "ITEM-DEFECTIVE",
                Notes = "Notes",
                CreateReturnRequestItems = mCreateReturnRequestItems
            };

            var returnRequestId = await FulfillmentMicroService.CreateReturnRequestAsync(mCreateReturnRequest);
            logger.LogInformation($"Return request {returnRequestId} created.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            await FulfillmentMicroService.PostReturnRequestAsync(returnRequestId);
            logger.LogInformation($"Return request {returnRequestId} posted.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");
        }

        [TestMethod]
        public async Task CreateReturn()
        {
            var logger = ServiceScope.ServiceProvider.GetService<ILogger<FulfillmentTest>>();

            var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());
            int maxQuantity = 10;

            var mReturnRequest = await SetupReturnRequest(unitOfWork, maxQuantity);

            var mCreateReturn = new MFulfillment_CreateReturn()
            {
                CreateDateTimeUtc = DateTime.Now,
                CreateReturnItems = mReturnRequest.ReturnRequestItems
                    .Select(r => new MFulfillment_CreateReturnItem()
                    {
                        ReturnRequestItemId = r.ReturnRequestItemId,
                        Quantity = r.Quantity
                    }).ToList()
            };
            var returnId = await FulfillmentMicroService.CreateReturnAsync(mCreateReturn);
            logger.LogInformation($"Return {returnId} created.");

            var eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            await FulfillmentMicroService.PostReturnAsync(returnId);
            logger.LogInformation($"Return {returnId} posted.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");

            await FulfillmentMicroService.ProcessReturnAsync(returnId);
            logger.LogInformation($"Return {returnId} processed.");

            eventCount = await EventProcessorMicroService.ProcessPendingEvents();
            logger.LogInformation($"{eventCount} events processed.");
        }

        private async Task<MFulfillment_AllocateFulfillableResponse> SetupFulfillable()
        {
            var allocateFulfillable = RandomFulfillable();
            var allocateFulfillableResponse = await FulfillmentMicroService.AllocateFulfillableAsync(allocateFulfillable);
            return allocateFulfillableResponse;
        }

        private async Task<MFulfillment_ShipmentRequest> SetupShipmentRequest(UnitOfWork unitOfWork, int maxQuantity)
        {
            //var unitOfWork = CreateUnitOfWork.Timestamp(GetUniqueNow());

            //var utcNow = DateTime.UtcNow;
            //var name = $"Fulfillable {utcNow.Year - 2000:00}-{utcNow.DayOfYear:000}-{utcNow.Hour * utcNow.Minute:0000}.{utcNow.Millisecond:000}";

            var allocateFulfillableResponse = await SetupFulfillable();

            foreach (var fulfillableItem in allocateFulfillableResponse.FulfillableItemResponses)
            {
                int quantity = Random.Next(maxQuantity) + 1;
                await FulfillmentMicroService.SetFulfillmentRequestQuantity(fulfillableItem.FulfillableItemId, quantity, unitOfWork.Next());
            }

            var shipmentRequestId = await FulfillmentMicroService.GetPendingShipmentRequestAsync(allocateFulfillableResponse.FulfillableId);

            await FulfillmentMicroService.OpenShipmentRequestAsync(shipmentRequestId.Value);

            var mShipmentRequest = await FulfillmentMicroService.GetShipmentRequestAsync(shipmentRequestId.Value);
            return mShipmentRequest;
        }

        private async Task<MFulfillment_Shipment> SetupShipment(UnitOfWork unitOfWork, int maxQuantity)
        {
            var mShipmentRequest = await SetupShipmentRequest(unitOfWork, maxQuantity);

            var mCreateShipmentItems = new List<MFulfillment_CreateShipmentItem>();
            foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
            {
                mCreateShipmentItems.Add(new MFulfillment_CreateShipmentItem()
                {
                    ShipmentRequestItemId = mShipmentRequestItem.ShipmentRequestItemId,
                    Quantity = mShipmentRequestItem.Quantity
                });
            }
            var mCreateShipment = new MFulfillment_CreateShipment()
            {
                ShipmentDateTimeUtc = DateTime.Today,
                TrackingCode = "1234567890",
                ShippingVendorId = "USPS",
                CreateShipmentItems = mCreateShipmentItems
            };
            var shipmentId = await FulfillmentMicroService.CreateShipmentAsync(mCreateShipment);

            await FulfillmentMicroService.PostShipmentAsync(shipmentId);
            await FulfillmentMicroService.ProcessShipmentAsync(shipmentId);

            var mShipment = await FulfillmentMicroService.GetShipmentAsync(shipmentId);
            return mShipment;
        }

        private async Task<MFulfillment_ReturnRequest> SetupReturnRequest(UnitOfWork unitOfWork, int quantity)
        {
            var mShipment = await SetupShipment(unitOfWork, quantity);

            var mCreateReturnRequestItems = new List<MFulfillment_CreateReturnRequestItem>();
            foreach (var mShipmentItem in mShipment.ShipmentItems)
            {
                mCreateReturnRequestItems.Add(new MFulfillment_CreateReturnRequestItem()
                {
                    FulfillableItemId = mShipmentItem.FulfillableItemId,
                    Quantity = mShipmentItem.Quantity
                });
            }
            var mCreateReturnRequest = new MFulfillment_CreateReturnRequest()
            {
                ReturnRequestType = MFulfillment_ReturnRequestTypes.Return,
                ReturnRequestReasonCode = "ITEM-DEFECTIVE",
                Notes = "Notes",
                CreateReturnRequestItems = mCreateReturnRequestItems
            };
            var returnRequestId = await FulfillmentMicroService.CreateReturnRequestAsync(mCreateReturnRequest);

            await FulfillmentMicroService.PostReturnRequestAsync(returnRequestId);

            var mReturnRequest = await FulfillmentMicroService.GetReturnRequestAsync(returnRequestId);
            return mReturnRequest;
        }

        private MFulfillment_AllocateFulfillable RandomFulfillable()
        {
            var fulfillableReference = CreateFulfillableReference.FromTimestamp(GetUniqueNow());
            var utcNow = DateTime.UtcNow;
            var name = $"Fulfillable {utcNow.Year - 2000:00}-{utcNow.DayOfYear:000}-{utcNow.Hour * utcNow.Minute:0000}.{utcNow.Millisecond:000}";

            var items = new List<MFulfillment_AllocateFulfillableItem>();
            int max = Random.Next(9) + 1;
            for (int idx = 0; idx < max; ++idx)
            {
                items.Add(RandomFulfillableItem());
            }

            return new MFulfillment_AllocateFulfillable()
            {
                Name = name,
                ShippingAddress = new MCommon_Address()
                {
                    Name = "RICHARD TODD",
                    AddressLine1 = "17340 W 156 TER",
                    City = "OLATHE",
                    StateCode = "KS",
                    PostalCode = "66062",
                    CountryCode = "US"
                },
                FulfillableReference = fulfillableReference,
                FulfillableItems = items
            };
        }

        private MFulfillment_AllocateFulfillableItem RandomFulfillableItem()
        {
            var description = $"{Words.GetRandomAdjective()} {Words.GetRandomNoun()}";
            var fulfillableItemReference = CreateFulfillableItemReference.FromTimestamp(GetUniqueNow());
            var consumableReference = CreateConsumableReference.FromTimestamp(GetUniqueNow());

            var components = new List<MFulfillment_AllocateFulfillableItemComponent>();
            var max = Random.Next(9);
            for (int idx = 0; idx < max; ++idx) // Note: allow zero items
            {
                components.Add(RandomFulfillableItemComponent());
            }

            return new MFulfillment_AllocateFulfillableItem()
            {
                Description = description,
                FulfillableItemReference = fulfillableItemReference,
                ConsumableReference = consumableReference,
                FulfillableItemComponents = components
            };
        }

        private MFulfillment_AllocateFulfillableItemComponent RandomFulfillableItemComponent()
        {
            var description = $"{Words.GetRandomAdjective()} {Words.GetRandomNoun()}";
            var consumableReference = CreateConsumableReference.FromTimestamp(GetUniqueNow());

            return new MFulfillment_AllocateFulfillableItemComponent()
            {
                Description = description,
                Quantity = Random.Next(9) + 1,
                ConsumableReference = consumableReference
            };
        }
    }
}
