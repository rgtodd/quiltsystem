//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MOrder_OrderEvent
    {
        public string EventTypeCode { get; set; }
        public string UnitOfWork { get; set; }
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public long OrdererId { get; set; }
        public string OrdererReference { get; set; }
        public decimal FundsRequiredIncome { get; set; }
        public decimal FundsRequiredSalesTax { get; set; }
        public string FundsRequiredSalesTaxJurisdiction { get; set; }
        public decimal FundsReceived { get; set; }

        public MCommon_Address ShippingAddress { get; set; }
        public IList<MOrder_OrderEventItem> OrderEventItems { get; set; }
    }

    public class MOrder_OrderEventItem
    {
        public long OrderItemId { get; set; }
        public int ReturnQuantity { get; set; }
        public int RequiredQuantity { get; set; }
        public int CompleteQuantity { get; set; }

        public long OrderableId { get; set; }
        public string OrderableReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }

        public IList<MOrder_OrderEventItemComponent> OrderEventItemComponents { get; set; }
    }

    public class MOrder_OrderEventItemComponent
    {
        public long OrderableComponentId { get; set; }
        public string OrderableComponentReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
    }
}
