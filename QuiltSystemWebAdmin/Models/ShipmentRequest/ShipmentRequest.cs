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

namespace RichTodd.QuiltSystem.WebAdmin.Models.ShipmentRequest
{
    public class ShipmentRequest
    {
        public AShipment_ShipmentRequest AShipmentRequest { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentRequest(
            AShipment_ShipmentRequest aShipmentRequest,
            IApplicationLocale locale)
        {
            AShipmentRequest = aShipmentRequest;
            Locale = locale;
        }

        public MFulfillment_ShipmentRequest MShipmentRequest => AShipmentRequest.MShipmentRequest;
        public MFulfillment_ShipmentRequestTransactionSummaryList MTransactions => AShipmentRequest.MTransactions;
        public MFulfillment_ShipmentRequestEventLogSummaryList MEvents => AShipmentRequest.MEvents;

        [Display(Name = "Shipment Request ID")]
        public long ShipmentRequestId => MShipmentRequest.ShipmentRequestId;

        [Display(Name = "Shipment Request Number")]
        public string ShipmentRequestNumber => MShipmentRequest.ShipmentRequestNumber;

        [Display(Name = "Shipment Request Status")]
        public string ShipmentRequestStatus => MShipmentRequest.ShipmentRequestStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MShipmentRequest.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreationDateTime => Locale.GetLocalTimeFromUtc(MShipmentRequest.CreateDateTimeUtc);

        [Display(Name = "Ship To")]
        public IBootstrapAddress ShipToAddress => BootstrapAddressWrapper.Wrap(AShipmentRequest.MShipmentRequest.ShipToAddress);

        public bool EnableEdit => AShipmentRequest.AllowEdit;
        public bool EnableCreateShipment => AShipmentRequest.AllowEdit && MShipmentRequest.CanCreateShipment;
        public bool EnableCancel => AShipmentRequest.AllowEdit && MShipmentRequest.CanCancel;

        private IList<ShipmentRequestFulfillable> m_shipmentRequestFulfillables;
        public IList<ShipmentRequestFulfillable> ShipmentRequestFulfillables
        {
            get
            {
                if (m_shipmentRequestFulfillables == null)
                {
                    var outstandingShipmentRequestItems = new List<MFulfillment_ShipmentRequestItem>(AShipmentRequest.MShipmentRequest.ShipmentRequestItems);

                    m_shipmentRequestFulfillables = new List<ShipmentRequestFulfillable>();
                    foreach (var mFulfillable in AShipmentRequest.MFulfillables)
                    {
                        var mFulfillableItems = new List<MFulfillment_FulfillableItem>();
                        var mShipmentRequestItems = new List<MFulfillment_ShipmentRequestItem>();

                        foreach (var mFulfillableItem in mFulfillable.FulfillableItems)
                        {
                            var mShipmentRequestItem = outstandingShipmentRequestItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).FirstOrDefault();
                            if (mShipmentRequestItem != null)
                            {
                                mFulfillableItems.Add(mFulfillableItem);
                                mShipmentRequestItems.Add(mShipmentRequestItem);
                                _ = outstandingShipmentRequestItems.Remove(mShipmentRequestItem);
                            }
                        }

                        m_shipmentRequestFulfillables.Add(new ShipmentRequestFulfillable(
                            mFulfillable,
                            mFulfillableItems,
                            mShipmentRequestItems,
                            Locale));
                    }

                    if (outstandingShipmentRequestItems.Count != 0)
                    {
                        throw new InvalidOperationException("One or more shipment request items excluded from result.");
                    }
                }
                return m_shipmentRequestFulfillables;
            }
        }

        private IList<TransactionModel> m_transactions;
        public IList<TransactionModel> Transactions
        {
            get
            {
                if (m_transactions == null)
                {
                    m_transactions = MTransactions != null
                        ? MTransactions.Summaries
                            .Select(r => new TransactionModel(r, Locale))
                            .ToList()
                        : new List<TransactionModel>(0);
                }
                return m_transactions;
            }
        }

        private IList<EventModel> m_events;
        public IList<EventModel> Events
        {
            get
            {
                if (m_events == null)
                {
                    m_events = MEvents != null
                        ? MEvents.Summaries
                            .Select(r => new EventModel(r, Locale))
                            .ToList()
                        : new List<EventModel>(0);
                }
                return m_events;
            }
        }
    }

    public class ShipmentRequestFulfillable
    {
        public MFulfillment_Fulfillable MFulfillable { get; }
        public IList<MFulfillment_FulfillableItem> MFulfillableItems { get; }
        public IList<MFulfillment_ShipmentRequestItem> MShipmentRequestItems { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentRequestFulfillable(
            MFulfillment_Fulfillable mFulfillable,
            IList<MFulfillment_FulfillableItem> mFulfillableItems,
            IList<MFulfillment_ShipmentRequestItem> mShipmentRequestItems,
            IApplicationLocale locale)
        {
            MFulfillable = mFulfillable;
            MFulfillableItems = mFulfillableItems;
            MShipmentRequestItems = mShipmentRequestItems;
            Locale = locale;
        }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => MFulfillable.Name;

        private IList<ShipmentRequestFulfillableItem> m_shipmentRequestFulfillableItems;
        public IList<ShipmentRequestFulfillableItem> ShipmentRequestFulfillableItems
        {
            get
            {
                if (m_shipmentRequestFulfillableItems == null)
                {
                    m_shipmentRequestFulfillableItems = new List<ShipmentRequestFulfillableItem>();
                    foreach (var mFulfillableItem in MFulfillableItems)
                    {
                        var mShipmentRequestItem = MShipmentRequestItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).First();
                        m_shipmentRequestFulfillableItems.Add(new ShipmentRequestFulfillableItem(
                            mFulfillableItem,
                            mShipmentRequestItem,
                            Locale));
                    }

                }
                return m_shipmentRequestFulfillableItems;
            }
        }
    }

    public class ShipmentRequestFulfillableItem
    {
        public MFulfillment_FulfillableItem MFulfillableItem { get; }
        public MFulfillment_ShipmentRequestItem MShipmentRequestItem { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentRequestFulfillableItem(
            MFulfillment_FulfillableItem mFulfillableItem,
            MFulfillment_ShipmentRequestItem mShipmentRequestItem,
            IApplicationLocale locale)
        {
            MFulfillableItem = mFulfillableItem;
            MShipmentRequestItem = mShipmentRequestItem;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"SRI:{MShipmentRequestItem.ShipmentRequestItemId}:FI:{MFulfillableItem.FulfillableItemId}";

        [Display(Name = "Shipment Request Item ID")]
        public long ShipmentRequestItemId => MShipmentRequestItem.ShipmentRequestItemId;

        [Display(Name = "Quantity")]
        public int Quantity => MShipmentRequestItem.Quantity;

        [Display(Name = "Fulfillable Item ID")]
        public long FulfillableItemId => MFulfillableItem.FulfillableItemId;

        [Display(Name = "Fulfillable Item Reference")]
        public string FulfillableItemReference => MFulfillableItem.FulfillableItemReference;

        [Display(Name = "Description")]
        public string Description => MFulfillableItem.Description;

        [Display(Name = "Consumable Reference")]
        public string ConsumableReference => MFulfillableItem.ConsumableReference;

        private IList<FulfillableItemComponent> m_FulfillableItemComponents;
        public IList<FulfillableItemComponent> FulfillableItemComponents
        {
            get
            {
                if (m_FulfillableItemComponents == null)
                {
                    m_FulfillableItemComponents = new List<FulfillableItemComponent>();
                    foreach (var mFulfillableItemComponent in MFulfillableItem.FulfillableItemComponents)
                    {
                        m_FulfillableItemComponents.Add(new FulfillableItemComponent(
                            mFulfillableItemComponent,
                            MShipmentRequestItem.Quantity,
                            Locale));
                    }
                }
                return m_FulfillableItemComponents;
            }
        }
    }

    public class FulfillableItemComponent
    {
        public MFulfillment_FulfillableItemComponent MFulfillableItemComponent { get; }
        public int ItemQuantity { get; }
        public IApplicationLocale Locale { get; }

        public FulfillableItemComponent(
            MFulfillment_FulfillableItemComponent mFulfillableItemComponent,
            int itemQuantity,
            IApplicationLocale locale)
        {
            MFulfillableItemComponent = mFulfillableItemComponent;
            Locale = locale;
            ItemQuantity = itemQuantity;
        }

        [Display(Name = "ID")]
        public string Id => $"FIC:{MFulfillableItemComponent.FulfillableItemComponentId}";

        [Display(Name = "Fulfillable Item Component ID")]
        public long FulfillableItemComponentId => MFulfillableItemComponent.FulfillableItemComponentId;

        [Display(Name = "Description")]
        public string Description => MFulfillableItemComponent.Description;

        [Display(Name = "Consumable Reference")]
        public string ConsumableReference => MFulfillableItemComponent.ConsumableReference;

        [Display(Name = "Component Quantity")]
        public int ComponentQuantity => MFulfillableItemComponent.Quantity;

        [Display(Name = "Quantity")]
        public int Quantity => MFulfillableItemComponent.Quantity;

        [Display(Name = "Total Quantity")]
        public int TotalQuantity => MFulfillableItemComponent.Quantity * ItemQuantity;

        [Display(Name = "Update Date/Time")]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime UpdateDateTime => Locale.GetLocalTimeFromUtc(MFulfillableItemComponent.UpdateDateTimeUtc);
    }


    public class TransactionModel
    {
        public MFulfillment_ShipmentRequestTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public TransactionModel(
            MFulfillment_ShipmentRequestTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.ShipmentRequestTransactionId}";

        public long ShipmentRequestTransactionId => MTransaction.ShipmentRequestTransactionId;

        public long ShipmentRequestId => MTransaction.ShipmentRequestId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class EventModel
    {
        public MFulfillment_ShipmentRequestEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public EventModel(
            MFulfillment_ShipmentRequestEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.ShipmentRequestEventId}";

        public long ShipmentRequestEventId => MEvent.ShipmentRequestEventId;

        public long ShipmentRequestTransactionId => MEvent.ShipmentRequestTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}