//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class ADashboard_Summary
    {
        public MCommunication_Dashboard MCommunication_Dashboard { get; set; }
        public MDesign_Dashboard MDesign_Dashboard { get; set; }
        public MFulfillment_Dashboard MFulfillment_Dashboard { get; set; }
        public MFunding_Dashboard MFunding_Dashboard { get; set; }
        public MLedger_Dashboard MLedger_Dashboard { get; set; }
        public MOrder_Dashboard MOrder_Dashboard { get; set; }
        public MProject_Dashboard MProject_Dashboard { get; set; }
        public MSquare_Dashboard MSquare_Dashboard { get; set; }
        public MUser_Dashboard MUser_Dashboard { get; set; }

        public IList<ADashboard_SummaryItem> OrderStatusCounts { get; set; }
        public IList<ADashboard_SummaryItem> OrderReturnStatusCounts { get; set; }
        public IList<ADashboard_SummaryItem> OrderReturnRequestStatusCounts { get; set; }
        public IList<ADashboard_SummaryItem> OrderShipmentStatusCounts { get; set; }
        public IList<ADashboard_SummaryItem> OrderShipmentRequestStatusCounts { get; set; }
    }
}
