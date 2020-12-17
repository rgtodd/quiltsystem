//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Home
{
    public class DashboardModel
    {
        public ADashboard_Summary ADashboard_Summary { get; set; }
        public IList<DashboardStatusCountModel> OrderStatusCounts { get; set; }
        public IList<DashboardStatusCountModel> OrderReturnStatusCounts { get; set; }
        public IList<DashboardStatusCountModel> OrderReturnRequestStatusCounts { get; set; }
        public IList<DashboardStatusCountModel> OrderShipmentStatusCounts { get; set; }
        public IList<DashboardStatusCountModel> OrderShipmentRequestStatusCounts { get; set; }
    }
}