//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AEvent_EventLogList
    {
        public MFunding_FunderEventLogSummaryList MFunderEventLogs { get; set; }
        public MFunding_FundableEventLogSummaryList MFundableEventLogs { get; set; }
        public MFulfillment_FulfillableEventLogSummaryList MFulfillableEventLogs { get; set; }
        public MFulfillment_ShipmentRequestEventLogSummaryList MShipmentRequestEventLogs { get; set; }
        public MFulfillment_ShipmentEventLogSummaryList MShipmentEventLogs { get; set; }
        public MFulfillment_ReturnRequestEventLogSummaryList MReturnRequestTrnsactions { get; set; }
        public MFulfillment_ReturnEventLogSummaryList MReturnEventLogs { get; set; }
        public MOrder_OrderEventLogSummaryList MOrderEventLogs { get; set; }
        public MSquare_PaymentEventLogSummaryList MSquarePaymentEventLogs { get; set; }
        public MSquare_RefundEventLogSummaryList MSquareRefundEventLogs { get; set; }
    }
}