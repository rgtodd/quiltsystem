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

namespace RichTodd.QuiltSystem.WebAdmin.Models.Return
{
    public class Return
    {
        public AReturn_Return AReturn { get; }
        public IApplicationLocale Locale { get; }

        public Return(
            AReturn_Return aReturn,
            IApplicationLocale locale)
        {
            AReturn = aReturn;
            Locale = locale;
        }

        private MFulfillment_Return MReturn => AReturn?.MReturn;
        private MFulfillment_ReturnTransactionSummaryList MTransactions => AReturn?.MTransactions;
        private MFulfillment_ReturnEventLogSummaryList MEvents => AReturn?.MEvents;
        private IList<MFulfillment_Fulfillable> MFulfillables => AReturn?.MFulfillables;

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

        public bool EnableEdit => AReturn.AllowEdit && MReturn.CanEdit;
        public bool EnablePost => AReturn.AllowEdit && MReturn.CanPost;
        public bool EnableProcess => AReturn.AllowEdit && MReturn.CanProcess;
        public bool EnableCancel => AReturn.AllowEdit && MReturn.CanCancel;

        private IList<ReturnFulfillable> m_returnFulfillables;
        public IList<ReturnFulfillable> ReturnFulfillables
        {
            get
            {
                if (m_returnFulfillables == null)
                {
                    if (MFulfillables == null)
                    {
                        m_returnFulfillables = new List<ReturnFulfillable>();
                    }
                    else
                    {
                        var outstandingReturnItems = new List<MFulfillment_ReturnItem>(MReturn.ReturnItems);

                        m_returnFulfillables = new List<ReturnFulfillable>();
                        foreach (var mFulfillable in MFulfillables)
                        {
                            var mFulfillableItems = new List<MFulfillment_FulfillableItem>();
                            var mReturnItems = new List<MFulfillment_ReturnItem>();

                            foreach (var mFulfillableItem in mFulfillable.FulfillableItems)
                            {
                                var mReturnItem = outstandingReturnItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).FirstOrDefault();
                                if (mReturnItem != null)
                                {
                                    mFulfillableItems.Add(mFulfillableItem);
                                    mReturnItems.Add(mReturnItem);
                                    _ = outstandingReturnItems.Remove(mReturnItem);
                                }
                            }

                            m_returnFulfillables.Add(new ReturnFulfillable(
                                mFulfillable,
                                mFulfillableItems,
                                mReturnItems,
                                Locale));
                        }

                        if (outstandingReturnItems.Count != 0)
                        {
                            throw new InvalidOperationException("One or more return items excluded from result.");
                        }
                    }
                }
                return m_returnFulfillables;
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

    public class ReturnFulfillable
    {
        public MFulfillment_Fulfillable MFulfillable { get; }
        public IList<MFulfillment_FulfillableItem> MFulfillableItems { get; }
        public IList<MFulfillment_ReturnItem> MReturnItems { get; }
        public IApplicationLocale Locale { get; }

        public ReturnFulfillable(
            MFulfillment_Fulfillable mFulfillable,
            IList<MFulfillment_FulfillableItem> mFulfillableItems,
            IList<MFulfillment_ReturnItem> mReturnItems,
            IApplicationLocale locale)
        {
            MFulfillable = mFulfillable;
            MFulfillableItems = mFulfillableItems;
            MReturnItems = mReturnItems;
            Locale = locale;
        }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => MFulfillable.Name;

        private IList<ReturnFulfillableItem> m_returnFulfillableItems;
        public IList<ReturnFulfillableItem> ReturnFulfillableItems
        {
            get
            {
                if (m_returnFulfillableItems == null)
                {
                    m_returnFulfillableItems = new List<ReturnFulfillableItem>();
                    foreach (var mFulfillableItem in MFulfillableItems)
                    {
                        var mReturnItem = MReturnItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).First();
                        m_returnFulfillableItems.Add(new ReturnFulfillableItem(
                            mFulfillableItem,
                            mReturnItem,
                            Locale));
                    }

                }
                return m_returnFulfillableItems;
            }
        }
    }

    public class ReturnFulfillableItem
    {
        public MFulfillment_FulfillableItem MFulfillableItem { get; }
        public MFulfillment_ReturnItem MReturnItem { get; }
        public IApplicationLocale Locale { get; }

        public ReturnFulfillableItem(
            MFulfillment_FulfillableItem mFulfillableItem,
            MFulfillment_ReturnItem mReturnItem,
            IApplicationLocale locale)
        {
            MFulfillableItem = mFulfillableItem;
            MReturnItem = mReturnItem;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"SI:{MReturnItem.ReturnItemId}:FI:{MFulfillableItem.FulfillableItemId}";

        [Display(Name = "Return Item ID")]
        public long ReturnItemId => MReturnItem.ReturnItemId;

        [Display(Name = "Quantity")]
        public int Quantity => MReturnItem.Quantity;

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
                            MReturnItem.Quantity,
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
            ItemQuantity = itemQuantity;
            Locale = locale;
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
        public MFulfillment_ReturnTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public TransactionModel(
            MFulfillment_ReturnTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"FT:{MTransaction.ReturnTransactionId}";

        public long ReturnTransactionId => MTransaction.ReturnTransactionId;

        public long ReturnId => MTransaction.ReturnId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class EventModel
    {
        public MFulfillment_ReturnEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public EventModel(
            MFulfillment_ReturnEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"FE:{MEvent.ReturnEventId}";

        public long ReturnEventId => MEvent.ReturnEventId;

        public long ReturnTransactionId => MEvent.ReturnTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}