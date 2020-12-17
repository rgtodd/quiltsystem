//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.WebAdmin.Models.Domain;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Order
{
    public class Order
    {
        public AOrder_Order AOrder { get; }
        public IApplicationLocale Locale { get; }
        public DomainModel ShippingVendors { get; }

        public Order(
            AOrder_Order aOrder,
            IApplicationLocale locale,
            DomainModel shippingVendors)
        {
            AOrder = aOrder;
            Locale = locale;
            ShippingVendors = shippingVendors;
        }

        public MOrder_Order MOrder => AOrder.MOrder;
        public MOrder_OrderTransactionSummaryList MTransactions => AOrder.MTransactions;
        public MOrder_OrderEventLogSummaryList MEvents => AOrder.MEvents;
        public MFunding_Fundable MFundable => AOrder.MFundable;
        public MFulfillment_Fulfillable MFulfillable => AOrder.MFulfillable;
        public MUser_User MUser => AOrder.MUser;

        [Display(Name = "Fundable ID")]
        public long? FundableId => MFundable?.FundableId;

        [Display(Name = "Fulfillable ID")]
        public long? FulfillableId => MFulfillable?.FulfillableId;

        [Display(Name = "Order ID")]
        public long OrderId => MOrder.OrderId;

        [Display(Name = "Order Number")]
        public string OrderNumber => MOrder.OrderNumber;

        [Display(Name = "Order Status")]
        public string OrderStatusType => MOrder.OrderStatus.ToString();

        [Display(Name = "Submission Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime? SubmissionDateTime => Locale.GetLocalTimeFromUtc(MOrder.SubmissionDateTimeUtc);

        [Display(Name = "Item Subtotal")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ItemSubtotalAmount => MOrder.ItemSubtotalAmount;

        [Display(Name = "Shipping")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal ShippingAmount => MOrder.ShippingAmount;

        [Display(Name = "Discount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal DiscountAmount => MOrder.DiscountAmount;

        [Display(Name = "Taxable Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal TaxableAmount => MOrder.TaxableAmount;

        [Display(Name = "Sales Tax %")]
        public decimal SalesTaxPercent => MOrder.SalesTaxPercent;

        [Display(Name = "Sales Tax")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal SalesTaxAmount => MOrder.SalesTaxAmount;

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        public decimal TotalAmount => MOrder.TotalAmount;

        [Display(Name = "User ID")]
        public string UserId => MUser?.UserId;

        [Display(Name = "User")]
        public string Email => MUser?.Email;

        private IList<OrderItem> m_orderItems;
        public IList<OrderItem> OrderItems
        {
            get
            {
                if (m_orderItems == null)
                {
                    m_orderItems = MOrder?.OrderItems != null
                        ? MOrder.OrderItems
                            .Select(r => new OrderItem(r, Locale))
                            .ToList()
                        : new List<OrderItem>(0);
                }
                return m_orderItems;
            }
        }

        private IList<OrderShipmentRequest> m_shipmentRequests;
        public IList<OrderShipmentRequest> ShipmentRequests
        {
            get
            {
                if (m_shipmentRequests == null)
                {
                    m_shipmentRequests = MFulfillable?.ShipmentRequests != null
                        ? MFulfillable.ShipmentRequests
                            .Select(r => new OrderShipmentRequest(r, Locale))
                            .ToList()
                        : new List<OrderShipmentRequest>(0);
                }
                return m_shipmentRequests;
            }
        }

        private IList<OrderShipment> m_shipments;
        public IList<OrderShipment> Shipments
        {
            get
            {
                if (m_shipments == null)
                {
                    m_shipments = MFulfillable?.Shipments != null
                        ? MFulfillable.Shipments
                            .Select(r => new OrderShipment(r, Locale))
                            .ToList()
                        : new List<OrderShipment>(0);
                }
                return m_shipments;
            }
        }

        private IList<OrderReturnRequest> m_returnRequests;
        public IList<OrderReturnRequest> ReturnRequests
        {
            get
            {
                if (m_returnRequests == null)
                {
                    m_returnRequests = MFulfillable?.ReturnRequests != null
                        ? MFulfillable.ReturnRequests
                            .Select(r => new OrderReturnRequest(r, Locale))
                            .ToList()
                        : new List<OrderReturnRequest>(0);
                }
                return m_returnRequests;
            }
        }

        private IList<OrderReturn> m_returns;
        public IList<OrderReturn> Returns
        {
            get
            {
                if (m_returns == null)
                {
                    m_returns = MFulfillable?.Returns != null
                        ? MFulfillable.Returns
                            .Select(r => new OrderReturn(r, Locale))
                            .ToList()
                        : new List<OrderReturn>(0);
                }
                return m_returns;
            }
        }


        private IList<OrderTransaction> m_transactions;
        public IList<OrderTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions?.Summaries != null
                        ? MTransactions.Summaries
                            .Select(r => new OrderTransaction(r, Locale))
                            .ToList()
                        : new List<OrderTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<OrderEvent> m_events;
        public IList<OrderEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents?.Summaries != null
                        ? MEvents.Summaries
                            .Select(r => new OrderEvent(r, Locale))
                            .ToList()
                        : new List<OrderEvent>(0);
                }
                return m_events;
            }
        }
    }

    public class OrderItem
    {
        public MOrder_OrderItem MOrderItem { get; }
        public IApplicationLocale Locale { get; }

        public OrderItem(
            MOrder_OrderItem mOrderItem,
            IApplicationLocale locale)
        {
            MOrderItem = mOrderItem;
            Locale = locale;
        }

        [Display(Name = "Order Item ID")]
        public long OrderItemId => MOrderItem.OrderItemId;

        [Display(Name = "Ordererable Reference")]
        public string OrderableReference => MOrderItem.OrderableReference;

        [Display(Name = "Order Item Sequence")]
        public int Sequence => MOrderItem.OrderItemSequence;

        [Display(Name = "Order Quantity")]
        public int OrderQuantity => MOrderItem.OrderQuantity;

        [Display(Name = "Cancel Quantity")]
        public int CancelQuantity => MOrderItem.CancelQuantity;

        [Display(Name = "Return Quantity")]
        public int FulfillmentReturnQuantity => MOrderItem.FulfillmentReturnQuantity;

        [Display(Name = "Net Quantity")]
        public int NetQuantity => MOrderItem.NetQuantity;

        [Display(Name = "Shipping Quantity")]
        public int FulfillmentRequiredQuantity => MOrderItem.FulfillmentRequiredQuantity;

        [Display(Name = "Shipped Quantity")]
        public int FulfillmentCompleteQuantity => MOrderItem.FulfillmentCompleteQuantity;

        [Display(Name = "Description")]
        public string Description => MOrderItem.Description;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice => MOrderItem.UnitPrice;

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = Standard.CurrencyFormat)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice => MOrderItem.TotalPrice;

        private IList<OrderItemComponent> m_orderItemComponents;
        public IList<OrderItemComponent> OrderItemComponents
        {
            get
            {
                if (m_orderItemComponents == null)
                {
                    m_orderItemComponents = MOrderItem.OrderItemComponents != null
                        ? MOrderItem.OrderItemComponents
                            .Select(r => new OrderItemComponent(r, MOrderItem.NetQuantity, Locale))
                            .ToList()
                        : new List<OrderItemComponent>(0);
                }
                return m_orderItemComponents;
            }
        }
    }

    public class OrderItemComponent
    {
        public MOrder_OrderItemComponent MOrderItemComponent { get; }
        public int ItemQuantity { get; }
        public IApplicationLocale Locale { get; }

        public OrderItemComponent(
            MOrder_OrderItemComponent mOrderItemComponent,
            int itemQuantity,
            IApplicationLocale locale)
        {
            MOrderItemComponent = mOrderItemComponent;
            ItemQuantity = itemQuantity;
            Locale = locale;
        }

        [Display(Name = "Orderable Component ID")]
        public long OrderableComponentId => MOrderItemComponent.OrderableComponentId;

        [Display(Name = "Description")]
        public string Description => MOrderItemComponent.Description;

        [Display(Name = "Consumable Reference")]
        public string ConsumableReference => MOrderItemComponent.ConsumableReference;

        [Display(Name = "Component Quantity")]
        public int ComponentQuantity => MOrderItemComponent.Quantity;

        [Display(Name = "Total Quantity")]
        public int TotalQuantity => MOrderItemComponent.Quantity * ItemQuantity;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MOrderItemComponent.UpdateDateTimeUtc);
    }

    public class OrderShipmentRequest
    {
        public MFulfillment_ShipmentRequest MShipmentRequest { get; }
        public IApplicationLocale Locale { get; }

        public OrderShipmentRequest(
            MFulfillment_ShipmentRequest mShipmentRequest,
            IApplicationLocale locale)
        {
            MShipmentRequest = mShipmentRequest;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"SR:{MShipmentRequest.ShipmentRequestId}";

        [Display(Name = "Ship Request ID")]
        public long ShipmentRequestId => MShipmentRequest.ShipmentRequestId;

        [Display(Name = "Shipment Request Number")]
        public string ShipmentRequestNumber => MShipmentRequest.ShipmentRequestNumber;

        [Display(Name = "Shipment Request Status")]
        public string ShipmentRequestStatus => MShipmentRequest.ShipmentRequestStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MShipmentRequest.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MShipmentRequest.CreateDateTimeUtc);
    }

    public class OrderShipment
    {
        public MFulfillment_Shipment MShipment { get; }
        public IApplicationLocale Locale { get; }

        public OrderShipment(
            MFulfillment_Shipment mShipment,
            IApplicationLocale locale)
        {
            MShipment = mShipment;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"S:{MShipment.ShipmentId}";

        [Display(Name = "Shipment ID")]
        public long ShipmentId => MShipment.ShipmentId;

        [Display(Name = "Shipment Status")]
        public string ShipmentStatus => MShipment.ShipmentStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MShipment.StatusDateTimeUtc);

        [Display(Name = "Shipping Vendor ID")]
        public string ShippingVendorId => MShipment.ShippingVendorId;

        [Display(Name = "Tracking Code")]
        public string TrackingCode => MShipment.TrackingCode;

        [Display(Name = "Shipment Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ShipmentDateTime => Locale.GetLocalTimeFromUtc(MShipment.ShipmentDateTimeUtc);

        public bool CanEdit => MShipment.CanEdit;
        public bool CanPost => MShipment.CanPost;
        public bool CanProcess => MShipment.CanProcess;
        public bool CanCancel => MShipment.CanCancel;
    }

    public class OrderReturnRequest
    {
        public MFulfillment_ReturnRequest MReturnRequest { get; }
        public IApplicationLocale Locale { get; }

        public OrderReturnRequest(
            MFulfillment_ReturnRequest mReturnRequest,
            IApplicationLocale locale)
        {
            MReturnRequest = mReturnRequest;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"RR:{MReturnRequest.ReturnRequestId}";

        [Display(Name = "Return Request ID")]
        public long ReturnRequestId => MReturnRequest.ReturnRequestId;

        [Display(Name = "Return Request Number")]
        public string ReturnRequestNumber => MReturnRequest.ReturnRequestNumber;

        [Display(Name = "Return Request Type")]
        public string ReturnRequestType => MReturnRequest.ReturnRequestType.ToString();

        [Display(Name = "Return Request Status")]
        public string ReturnRequestStatus => MReturnRequest.ReturnRequestStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MReturnRequest.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MReturnRequest.CreateDateTimeUtc);
    }

    public class OrderReturn
    {
        public MFulfillment_Return MReturn { get; }
        public IApplicationLocale Locale { get; }

        public OrderReturn(
            MFulfillment_Return mReturn,
            IApplicationLocale locale)
        {
            MReturn = mReturn;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"R:{MReturn.ReturnId}";

        [Display(Name = "Return ID")]
        public long ReturnId => MReturn.ReturnId;

        [Display(Name = "Return Number")]
        public string ReturnNumber => MReturn.ReturnNumber;

        [Display(Name = "Return Status")]
        public string ReturnStatus => MReturn.ReturnStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MReturn.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MReturn.CreateDateTimeUtc);

        public bool CanEdit => MReturn.CanEdit;
        public bool CanCancel => MReturn.CanCancel;
        public bool CanPost => MReturn.CanPost;
        public bool CanProcess => MReturn.CanProcess;
    }

    public class OrderTransaction
    {
        public MOrder_OrderTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public OrderTransaction(
            MOrder_OrderTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"OT:{MTransaction.OrderTransactionId}";

        public long OrderTransactionId => MTransaction.OrderTransactionId;

        public long OrderId => MTransaction.OrderId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class OrderEvent
    {
        public MOrder_OrderEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public OrderEvent(
            MOrder_OrderEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"OE:{MEvent.OrderEventId}";

        public long OrderEventId => MEvent.OrderEventId;

        public long OrderTransactionId => MEvent.OrderTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}