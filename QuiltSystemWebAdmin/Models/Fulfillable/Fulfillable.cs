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
using RichTodd.QuiltSystem.Web.Bootstrap;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Fulfillable
{
    public class Fulfillable
    {
        public AFulfillable_Fulfillable AFulfillable { get; }
        public IApplicationLocale Locale { get; }

        public Fulfillable(
            AFulfillable_Fulfillable aFulfillable,
            IApplicationLocale locale)
        {
            AFulfillable = aFulfillable;
            Locale = locale;
        }

        public MFulfillment_Fulfillable MFulfillable => AFulfillable.MFulfillable;
        public MFulfillment_FulfillableTransactionSummaryList MTransactions => AFulfillable.MTransactions;
        public MFulfillment_FulfillableEventLogSummaryList MEvents => AFulfillable.MEvents;

        [Display(Name = "Fulfillable ID")]
        public long FulfillableId => MFulfillable.FulfillableId;

        [Display(Name = "Fulfillable Reference")]
        public string FulfillableReference => MFulfillable.FulfillableReference;

        [Display(Name = "Name")]
        public string FulfillableName => MFulfillable.Name;

        [Display(Name = "Creation Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MFulfillable.CreateDateTimeUtc);

        [Display(Name = "Fulfillable Status")]
        public string FulfillableStatus => MFulfillable.FulfillableStatus.ToString();

        [Display(Name = "Fulfillable Status Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MFulfillable.StatusDateTimeUtc);

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFulfillable.UpdateDateTimeUtc);

        [Display(Name = "Ship To")]
        public IBootstrapAddress ShipToAddress => BootstrapAddressWrapper.Wrap(MFulfillable.ShipToAddress);

        public bool EnableCreateReturnRequest => AFulfillable.AllowEdit && MFulfillable.CanCreateReturnRequest;

        private IList<FulfillableItem> m_fulfillableItems;
        public IList<FulfillableItem> FulfillableItems
        {
            get
            {
                if (m_fulfillableItems == null)
                {
                    m_fulfillableItems = MFulfillable.FulfillableItems != null
                        ? MFulfillable.FulfillableItems
                            .Select(r => new FulfillableItem(r, Locale))
                            .ToList()
                        : new List<FulfillableItem>(0);
                }
                return m_fulfillableItems;
            }
        }

        private IList<FulfillableShipmentRequest> m_shipmentRequests;
        public IList<FulfillableShipmentRequest> ShipmentRequests
        {
            get
            {
                if (m_shipmentRequests == null)
                {
                    m_shipmentRequests = MFulfillable.ShipmentRequests != null
                        ? MFulfillable.ShipmentRequests
                            .Select(r => new FulfillableShipmentRequest(r, Locale))
                            .ToList()
                        : new List<FulfillableShipmentRequest>(0);
                }
                return m_shipmentRequests;
            }
        }

        private IList<FulfillableShipment> m_shipments;
        public IList<FulfillableShipment> Shipments
        {
            get
            {
                if (m_shipments == null)
                {
                    m_shipments = MFulfillable.Shipments != null
                        ? MFulfillable.Shipments
                            .Select(r => new FulfillableShipment(r, Locale))
                            .ToList()
                        : new List<FulfillableShipment>(0);
                }
                return m_shipments;
            }
        }

        private IList<FulfillableReturnRequest> m_returnRequests;
        public IList<FulfillableReturnRequest> ReturnRequests
        {
            get
            {
                if (m_returnRequests == null)
                {
                    m_returnRequests = MFulfillable.ReturnRequests != null
                        ? MFulfillable.ReturnRequests
                            .Select(r => new FulfillableReturnRequest(r, Locale))
                            .ToList()
                        : new List<FulfillableReturnRequest>(0);
                }
                return m_returnRequests;
            }
        }

        private IList<FulfillableReturn> m_returns;
        public IList<FulfillableReturn> Returns
        {
            get
            {
                if (m_returns == null)
                {
                    m_returns = MFulfillable.Returns != null
                        ? MFulfillable.Returns
                            .Select(r => new FulfillableReturn(r, Locale))
                            .ToList()
                        : new List<FulfillableReturn>(0);
                }
                return m_returns;
            }
        }

        private IList<FulfillableTransaction> m_transactions;
        public IList<FulfillableTransaction> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions != null
                        ? MTransactions.Summaries
                            .Select(r => new FulfillableTransaction(r, Locale))
                            .ToList()
                        : new List<FulfillableTransaction>(0);
                }
                return m_transactions;
            }
        }

        private IList<FulfillableEvent> m_events;
        public IList<FulfillableEvent> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents != null
                        ? MEvents.Summaries
                            .Select(r => new FulfillableEvent(r, Locale))
                            .ToList()
                        : new List<FulfillableEvent>(0);
                }
                return m_events;
            }
        }
    }

    public class FulfillableItem
    {
        public MFulfillment_FulfillableItem MFulfillableItem { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableItem(
            MFulfillment_FulfillableItem mFulfillableItem,
            IApplicationLocale locale)
        {
            MFulfillableItem = mFulfillableItem;
            Locale = locale;
        }

        [Display(Name = "Fulfillable Item ID")]
        public long FulfillableItemId => MFulfillableItem.FulfillableItemId;

        [Display(Name = "Fulfillable Item Reference")]
        public string FulfillableItemReference => MFulfillableItem.FulfillableItemReference;

        [Display(Name = "Description")]
        public string Description => MFulfillableItem.Description;

        [Display(Name = "Consumable Reference")]
        public string ConsumableReference => MFulfillableItem.ConsumableReference;

        [Display(Name = "Request Quantity")]
        public int RequestQuantity => MFulfillableItem.RequestQuantity;

        [Display(Name = "Complete Quantity")]
        public int CompleteQuantity => MFulfillableItem.CompleteQuantity;

        [Display(Name = "Return Quantity")]
        public int ReturnQuantity => MFulfillableItem.ReturnQuantity;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFulfillableItem.UpdateDateTimeUtc);

        private IList<FulfillableItemComponent> m_fulfillableItemComponents;
        public IList<FulfillableItemComponent> FulfillableItemComponents
        {
            get
            {
                if (m_fulfillableItemComponents == null)
                {
                    m_fulfillableItemComponents = MFulfillableItem.FulfillableItemComponents != null
                        ? MFulfillableItem.FulfillableItemComponents
                            .Select(r => new FulfillableItemComponent(r, Locale, MFulfillableItem.RequestQuantity))
                            .ToList()
                        : new List<FulfillableItemComponent>(0);
                }
                return m_fulfillableItemComponents;
            }
        }
    }

    public class FulfillableItemComponent
    {
        public MFulfillment_FulfillableItemComponent MFulfillableItemComponent { get; }
        public IApplicationLocale Locale { get; }
        public int ItemQuantity { get; }

        public FulfillableItemComponent(
            MFulfillment_FulfillableItemComponent mFulfillableItemComponent,
            IApplicationLocale locale,
            int itemQuantity)
        {
            MFulfillableItemComponent = mFulfillableItemComponent;
            Locale = locale;
            ItemQuantity = itemQuantity;
        }

        [Display(Name = "Fulfillable Item Component ID")]
        public long FulfillableItemComponentId => MFulfillableItemComponent.FulfillableItemComponentId;

        [Display(Name = "Description")]
        public string Description => MFulfillableItemComponent.Description;

        [Display(Name = "Consumable Reference")]
        public string ConsumableReference => MFulfillableItemComponent.ConsumableReference;

        [Display(Name = "Component Quantity")]
        public int ComponentQuantity => MFulfillableItemComponent.Quantity;

        [Display(Name = "Total Quantity")]
        public int TotalQuantity => MFulfillableItemComponent.Quantity * ItemQuantity;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFulfillableItemComponent.UpdateDateTimeUtc);
    }

    public class FulfillableShipmentRequest
    {
        public MFulfillment_ShipmentRequest MShipmentRequest { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableShipmentRequest(
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

    public class FulfillableShipment
    {
        public MFulfillment_Shipment MShipment { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableShipment(
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

    public class FulfillableReturnRequest
    {
        public MFulfillment_ReturnRequest MReturnRequest { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableReturnRequest(
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

    public class FulfillableReturn
    {
        public MFulfillment_Return MReturn { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableReturn(
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

    public class FulfillableTransaction
    {
        public MFulfillment_FulfillableTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableTransaction(
            MFulfillment_FulfillableTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.FulfillableTransactionId}";

        public long FulfillableTransactionId => MTransaction.FulfillableTransactionId;

        public long FulfillableId => MTransaction.FulfillableId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class FulfillableEvent
    {
        public MFulfillment_FulfillableEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableEvent(
            MFulfillment_FulfillableEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.FulfillableEventId}";

        public long FulfillableEventId => MEvent.FulfillableEventId;

        public long FulfillableTransactionId => MEvent.FulfillableTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}
