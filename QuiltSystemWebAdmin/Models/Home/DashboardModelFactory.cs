//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Web;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Home
{
    public class DashboardModelFactory : ApplicationModelFactory
    {

        public DashboardModel CreateDashboardModel(ADashboard_Summary svcDashboard)
        {
            var model = new DashboardModel
            {
                ADashboard_Summary = svcDashboard,
                OrderStatusCounts = CreateDashboardStatusCountModels(svcDashboard.OrderStatusCounts),
                OrderReturnStatusCounts = CreateDashboardStatusCountModels(svcDashboard.OrderReturnStatusCounts),
                OrderReturnRequestStatusCounts = CreateDashboardStatusCountModels(svcDashboard.OrderReturnRequestStatusCounts),
                OrderShipmentStatusCounts = CreateDashboardStatusCountModels(svcDashboard.OrderShipmentStatusCounts),
                OrderShipmentRequestStatusCounts = CreateDashboardStatusCountModels(svcDashboard.OrderShipmentRequestStatusCounts),
            };

            return model;
        }

        private List<DashboardStatusCountModel> CreateDashboardStatusCountModels(IList<ADashboard_SummaryItem> svcStatusCounts)
        {
            var statusCounts = new List<DashboardStatusCountModel>();

            if (svcStatusCounts != null)
            {
                foreach (var svcStatusCount in svcStatusCounts)
                {
                    var statusCount = new DashboardStatusCountModel()
                    {
                        Status = svcStatusCount.StatusName,
                        Count = svcStatusCount.Count
                    };
                    statusCounts.Add(statusCount);
                }
            }

            return statusCounts;
        }

    }
}