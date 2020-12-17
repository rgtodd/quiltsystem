//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Service.Micro.Implementations
{
    internal class EventProcessorMicroService : BaseService, IEventProcessorMicroService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IFundingMicroService FundingMicroService { get; }
        private IInventoryMicroService InventoryMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public EventProcessorMicroService(
            IApplicationRequestServices requestServices,
            ILogger<EventProcessorMicroService> logger,
            ICommunicationMicroService communicationMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IFundingMicroService fundingMicroService,
            IInventoryMicroService inventoryMicroService,
            IOrderMicroService orderMicroService,
            IProjectMicroService projectMicroService,
            ISquareMicroService squareMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            InventoryMicroService = inventoryMicroService ?? throw new ArgumentNullException(nameof(inventoryMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task<int> ProcessPendingEvents()
        {
            using var log = BeginFunction(nameof(EventProcessorMicroService), nameof(ProcessPendingEvents));
            try
            {
                var totalCount = 0;

                int iterationCount;
                do
                {
                    iterationCount = 0;
                    iterationCount += await CommunicationMicroService.ProcessEventsAsync();
                    iterationCount += await FulfillmentMicroService.ProcessEventsAsync();
                    iterationCount += await FundingMicroService.ProcessEventsAsync();
                    iterationCount += await InventoryMicroService.ProcessEventsAsync();
                    iterationCount += await OrderMicroService.ProcessEventsAsync();
                    iterationCount += await SquareMicroService.ProcessEventsAsync();
                    iterationCount += await UserMicroService.ProcessEventsAsync();

                    //Task<int>[] tasks = {
                    //CommunicationMicroService.ProcessEventsAsync(),
                    //FulfillmentMicroService.ProcessEventsAsync(),
                    //FundingMicroService.ProcessEventsAsync(),
                    //InventoryMicroService.ProcessEventsAsync(),
                    //OrderMicroService.ProcessEventsAsync(),
                    //PayPalMicroService.ProcessEventsAsync(),
                    //SquareMicroService.ProcessEventsAsync(),
                    //UserMicroService.ProcessEventsAsync() };
                    //var counts = await Task.WhenAll(tasks);
                    //iterationCount = counts.Sum();

                    totalCount += iterationCount;

                } while (iterationCount > 0);

                var result = totalCount;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public Task<int> CancelPendingEvents()
        {
            throw new NotImplementedException();
        }
    }
}