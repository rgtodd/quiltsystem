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
using RichTodd.QuiltSystem.Utility;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Bootstrap;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Shipment
{
    public class Shipment
    {
        public AShipment_Shipment AShipment { get; }
        public IApplicationLocale Locale { get; }

        public Shipment(
            AShipment_Shipment aShipment,
            IApplicationLocale locale)
        {
            AShipment = aShipment;
            Locale = locale;
        }

        public MFulfillment_Shipment MShipment => AShipment.MShipment;
        public MFulfillment_ShipmentTransactionSummaryList MTransactions => AShipment.MTransactions;
        public MFulfillment_ShipmentEventLogSummaryList MEvents => AShipment.MEvents;

        [Display(Name = "Shipment ID")]
        public long ShipmentId => MShipment.ShipmentId;

        [Display(Name = "Shipment Number")]
        public string ShipmentNumber => MShipment.ShipmentNumber;

        [Display(Name = "Shipment Status")]
        public string ShipmentStatus => MShipment.ShipmentStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MShipment.StatusDateTimeUtc);

        [Display(Name = "Shipment Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime ShipmentDateTime => Locale.GetLocalTimeFromUtc(MShipment.ShipmentDateTimeUtc);

        [Display(Name = "Ship To")]
        public IBootstrapAddress ShipToAddress => BootstrapAddressWrapper.Wrap(MShipment.ShipToAddress);

        [Display(Name = "Shipping Vendor ID")]
        public string ShippingVendorId => MShipment.ShippingVendorId;

        [Display(Name = "Tracking Code")]
        public string TrackingCode => MShipment.TrackingCode;

        public bool EnableEdit => AShipment.AllowEdit && MShipment.CanEdit;
        public bool EnablePost => AShipment.AllowEdit && MShipment.CanPost;
        public bool EnableProcess => AShipment.AllowEdit && MShipment.CanProcess;
        public bool EnableCancel => AShipment.AllowEdit && MShipment.CanCancel;

        private IList<ShipmentFulfillable> m_shipmentFulfillables;
        public IList<ShipmentFulfillable> ShipmentFulfillables
        {
            get
            {
                if (m_shipmentFulfillables == null)
                {
                    m_shipmentFulfillables = new List<ShipmentFulfillable>();
                    if (Is.Populated(AShipment.MFulfillables))
                    {
                        var outstandingShipmentItems = new List<MFulfillment_ShipmentItem>(AShipment.MShipment.ShipmentItems);

                        foreach (var mFulfillable in AShipment.MFulfillables)
                        {
                            var mFulfillableItems = new List<MFulfillment_FulfillableItem>();
                            var mShipmentItems = new List<MFulfillment_ShipmentItem>();

                            foreach (var mFulfillableItem in mFulfillable.FulfillableItems)
                            {
                                var mShipmentItem = outstandingShipmentItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).FirstOrDefault();
                                if (mShipmentItem != null)
                                {
                                    mFulfillableItems.Add(mFulfillableItem);
                                    mShipmentItems.Add(mShipmentItem);
                                    _ = outstandingShipmentItems.Remove(mShipmentItem);
                                }
                            }

                            m_shipmentFulfillables.Add(new ShipmentFulfillable(
                                mFulfillable,
                                mFulfillableItems,
                                mShipmentItems,
                                Locale));
                        }

                        if (outstandingShipmentItems.Count != 0)
                        {
                            throw new InvalidOperationException("One or more shipment items excluded from result.");
                        }
                    }
                }
                return m_shipmentFulfillables;
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

    public class ShipmentFulfillable
    {
        public MFulfillment_Fulfillable MFulfillable { get; }
        public IList<MFulfillment_FulfillableItem> MFulfillableItems { get; }
        public IList<MFulfillment_ShipmentItem> MShipmentItems { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentFulfillable(
            MFulfillment_Fulfillable mFulfillable,
            IList<MFulfillment_FulfillableItem> mFulfillableItems,
            IList<MFulfillment_ShipmentItem> mShipmentItems,
            IApplicationLocale locale)
        {
            MFulfillable = mFulfillable;
            MFulfillableItems = mFulfillableItems;
            MShipmentItems = mShipmentItems;
            Locale = locale;
        }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => MFulfillable.Name;

        private IList<ShipmentFulfillableItem> m_shipmentFulfillableItems;
        public IList<ShipmentFulfillableItem> ShipmentFulfillableItems
        {
            get
            {
                if (m_shipmentFulfillableItems == null)
                {
                    m_shipmentFulfillableItems = new List<ShipmentFulfillableItem>();
                    foreach (var mFulfillableItem in MFulfillableItems)
                    {
                        var mShipmentItem = MShipmentItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).First();
                        m_shipmentFulfillableItems.Add(new ShipmentFulfillableItem(
                            mFulfillableItem,
                            mShipmentItem,
                            Locale));
                    }

                }
                return m_shipmentFulfillableItems;
            }
        }
    }

    public class ShipmentFulfillableItem
    {
        public MFulfillment_FulfillableItem MFulfillableItem { get; }
        public MFulfillment_ShipmentItem MShipmentItem { get; }
        public IApplicationLocale Locale { get; }

        public ShipmentFulfillableItem(
            MFulfillment_FulfillableItem mFulfillableItem,
            MFulfillment_ShipmentItem mShipmentItem,
            IApplicationLocale locale)
        {
            MFulfillableItem = mFulfillableItem;
            MShipmentItem = mShipmentItem;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"SI:{MShipmentItem.ShipmentItemId}:FI:{MFulfillableItem.FulfillableItemId}";

        [Display(Name = "Shipment Item ID")]
        public long ShipmentItemId => MShipmentItem.ShipmentItemId;

        [Display(Name = "Quantity")]
        public int Quantity => MShipmentItem.Quantity;

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
                            Locale,
                            MShipmentItem.Quantity));
                    }
                }
                return m_FulfillableItemComponents;
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
        public MFulfillment_ShipmentTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public TransactionModel(
            MFulfillment_ShipmentTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.ShipmentTransactionId}";

        public long ShipmentTransactionId => MTransaction.ShipmentTransactionId;

        public long ShipmentId => MTransaction.ShipmentId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class EventModel
    {
        public MFulfillment_ShipmentEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public EventModel(
            MFulfillment_ShipmentEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.ShipmentEventId}";

        public long ShipmentEventId => MEvent.ShipmentEventId;

        public long ShipmentTransactionId => MEvent.ShipmentTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}