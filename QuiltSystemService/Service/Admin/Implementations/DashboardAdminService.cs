//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class DashboardAdminService : BaseService, IDashboardAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IDesignMicroService DesignMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IFundingMicroService FundingMicroService { get; }
        private ILedgerMicroService LedgerMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }
        private IUserMicroService UserMicroService { get; }

        public DashboardAdminService(
            IApplicationRequestServices requestServices,
            ILogger<DashboardAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IDesignMicroService designMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IFundingMicroService fundingMicroService,
            ILedgerMicroService ledgerMicroService,
            IOrderMicroService orderMicroService,
            IProjectMicroService projectMicroService,
            ISquareMicroService squareMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            DesignMicroService = designMicroService ?? throw new ArgumentNullException(nameof(designMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            FundingMicroService = fundingMicroService ?? throw new ArgumentNullException(nameof(fundingMicroService));
            LedgerMicroService = ledgerMicroService ?? throw new ArgumentNullException(nameof(ledgerMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        #region IAdmin_DashboardService

        public async Task<ADashboard_Summary> GetDashboardDataAsync()
        {
            using var log = BeginFunction(nameof(DashboardAdminService), nameof(GetDashboardDataAsync));
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                var result = new ADashboard_Summary()
                {
                    MCommunication_Dashboard = await CommunicationMicroService.GetDashboardAsync(),
                    MDesign_Dashboard = await DesignMicroService.GetDashboardAsync(),
                    MFulfillment_Dashboard = await FulfillmentMicroService.GetDashboardAsync(),
                    MFunding_Dashboard = await FundingMicroService.GetDashboardAsync(),
                    MLedger_Dashboard = await LedgerMicroService.GetDashboardAsync(),
                    MOrder_Dashboard = await OrderMicroService.GetDashboardAsync(),
                    MProject_Dashboard = await ProjectMicroService.GetDashboardAsync(),
                    MSquare_Dashboard = await SquareMicroService.GetDashboardAsync(),
                    MUser_Dashboard = await UserMicroService.GetDashboardAsync()
                };

                //using (var ctx = QuiltContextFactory.Create())
                //{
                //    using var conn = QuiltContextFactory.CreateConnection();

                //    conn.Open();

                //    // Order
                //    //
                //    {
                //        var statusCounts = new List<Admin_Dashboard_StatusCountData>();
                //        foreach (var dbOrder in GetDashboardOrders(conn).ToList())
                //        {
                //            var statusCount = new Admin_Dashboard_StatusCountData()
                //            {
                //                StatusName = ctx.OrderStatusType(dbOrder.OrderStatusTypeCode).Name,
                //                Count = dbOrder.RecordCount.GetValueOrDefault()
                //            };
                //            statusCounts.Add(statusCount);
                //        }
                //        result.OrderStatusCounts = statusCounts;
                //    }

                //    // Order Return Request
                //    //
                //    {
                //        var statusCounts = new List<Admin_Dashboard_StatusCountData>();
                //        foreach (var dbOrderReturnRequest in GetDashboardOrderReturnRequests(conn).ToList())
                //        {
                //            var statusCount = new Admin_Dashboard_StatusCountData()
                //            {
                //                StatusName = ctx.ReturnRequestStatusType(dbOrderReturnRequest.OrderReturnRequestStatusTypeCode).Name,
                //                Count = dbOrderReturnRequest.RecordCount.GetValueOrDefault()
                //            };
                //            statusCounts.Add(statusCount);
                //        }
                //        result.OrderReturnRequestStatusCounts = statusCounts;
                //    }

                //    // Order Return
                //    //
                //    {
                //        var statusCounts = new List<Admin_Dashboard_StatusCountData>();
                //        foreach (var dbOrderReturn in GetDashboardOrderReturns(conn).ToList())
                //        {
                //            var statusCount = new Admin_Dashboard_StatusCountData()
                //            {
                //                StatusName = ctx.ReturnStatusType(dbOrderReturn.OrderReturnStatusTypeCode).Name,
                //                Count = dbOrderReturn.RecordCount.GetValueOrDefault()
                //            };
                //            statusCounts.Add(statusCount);
                //        }
                //        result.OrderReturnStatusCounts = statusCounts;
                //    }

                //    // Order Shipment Request
                //    //
                //    {
                //        var statusCounts = new List<Admin_Dashboard_StatusCountData>();
                //        foreach (var dbOrderShipmentRequest in GetDashboardOrderShipmentRequests(conn).ToList())
                //        {
                //            var statusCount = new Admin_Dashboard_StatusCountData()
                //            {
                //                StatusName = ctx.ShipmentRequestStatusType(dbOrderShipmentRequest.OrderShipmentRequestStatusTypeCode).Name,
                //                Count = dbOrderShipmentRequest.RecordCount.GetValueOrDefault()
                //            };
                //            statusCounts.Add(statusCount);
                //        }
                //        result.OrderShipmentRequestStatusCounts = statusCounts;
                //    }

                //    // Order Shipment
                //    //
                //    {
                //        var statusCounts = new List<Admin_Dashboard_StatusCountData>();
                //        foreach (var dbOrderShipment in GetDashboardOrderShipments(conn).ToList())
                //        {
                //            var statusCount = new Admin_Dashboard_StatusCountData()
                //            {
                //                StatusName = ctx.ShipmentStatusType(dbOrderShipment.OrderShipmentStatusTypeCode).Name,
                //                Count = dbOrderShipment.RecordCount.GetValueOrDefault()
                //            };
                //            statusCounts.Add(statusCount);
                //        }
                //        result.OrderShipmentStatusCounts = statusCounts;
                //    }
                //}

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion IAdmin_DashboardService
    }
}