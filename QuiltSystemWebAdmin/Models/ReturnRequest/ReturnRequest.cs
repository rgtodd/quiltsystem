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

namespace RichTodd.QuiltSystem.WebAdmin.Models.ReturnRequest
{
    public class ReturnRequest
    {
        public AReturn_ReturnRequest AReturnRequest { get; }
        public IApplicationLocale Locale { get; }

        public ReturnRequest(AReturn_ReturnRequest aReturnRequest, IApplicationLocale locale)
        {
            AReturnRequest = aReturnRequest;
            Locale = locale;
        }

        public MFulfillment_ReturnRequest MReturnRequest => AReturnRequest.MReturnRequest;
        public MFulfillment_ReturnRequestTransactionSummaryList MTransactions => AReturnRequest.MTransactions;
        public MFulfillment_ReturnRequestEventLogSummaryList MEvents => AReturnRequest.MEvents;

        [Display(Name = "Return Request ID")]
        public long ReturnRequestId => MReturnRequest.ReturnRequestId;

        [Display(Name = "Return Request Number")]
        public string ReturnRequestNumber => MReturnRequest.ReturnRequestNumber;

        [Display(Name = "Return Request Type")]
        public string ReturnRequestType => MReturnRequest.ReturnRequestType.ToString();

        [Display(Name = "Return Request Reason")]
        public string ReturnRequestReasonCode => MReturnRequest.ReturnRequestReasonCode;

        [Display(Name = "Return Request Status")]
        public string ReturnRequestStatus => MReturnRequest.ReturnRequestStatus.ToString();

        [Display(Name = "Status Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MReturnRequest.StatusDateTimeUtc);

        [Display(Name = "Creation Date/Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime CreateDateTime => Locale.GetLocalTimeFromUtc(MReturnRequest.CreateDateTimeUtc);

        public bool EnableEdit => AReturnRequest.AllowEdit && MReturnRequest.CanEdit;

        public bool EnableCreateReturn => AReturnRequest.AllowEdit && MReturnRequest.CanCreateReturn;

        private IList<ReturnRequestFulfillable> m_returnRequestFulfillables;
        public IList<ReturnRequestFulfillable> ReturnRequestFulfillables
        {
            get
            {
                if (m_returnRequestFulfillables == null)
                {
                    var outstandingReturnRequestItems = new List<MFulfillment_ReturnRequestItem>(AReturnRequest.MReturnRequest.ReturnRequestItems);

                    m_returnRequestFulfillables = new List<ReturnRequestFulfillable>();
                    foreach (var mFulfillable in AReturnRequest.MFulfillables)
                    {
                        var mFulfillableItems = new List<MFulfillment_FulfillableItem>();
                        var mReturnRequestItems = new List<MFulfillment_ReturnRequestItem>();

                        foreach (var mFulfillableItem in mFulfillable.FulfillableItems)
                        {
                            var mReturnRequestItem = outstandingReturnRequestItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).FirstOrDefault();
                            if (mReturnRequestItem != null)
                            {
                                mFulfillableItems.Add(mFulfillableItem);
                                mReturnRequestItems.Add(mReturnRequestItem);
                                _ = outstandingReturnRequestItems.Remove(mReturnRequestItem);
                            }
                        }

                        m_returnRequestFulfillables.Add(new ReturnRequestFulfillable(
                            mFulfillable,
                            mFulfillableItems,
                            mReturnRequestItems,
                            Locale));
                    }

                    if (outstandingReturnRequestItems.Count != 0)
                    {
                        throw new InvalidOperationException("One or more return request items excluded from result.");
                    }
                }
                return m_returnRequestFulfillables;
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

    public class ReturnRequestFulfillable
    {
        public MFulfillment_Fulfillable MFulfillable { get; }
        public IList<MFulfillment_FulfillableItem> MFulfillableItems { get; }
        public IList<MFulfillment_ReturnRequestItem> MReturnRequestItems { get; }
        public IApplicationLocale Locale { get; }

        public ReturnRequestFulfillable(
            MFulfillment_Fulfillable mFulfillable,
            IList<MFulfillment_FulfillableItem> mFulfillableItems,
            IList<MFulfillment_ReturnRequestItem> mReturnRequestItems,
            IApplicationLocale locale)
        {
            MFulfillable = mFulfillable;
            MFulfillableItems = mFulfillableItems;
            MReturnRequestItems = mReturnRequestItems;
            Locale = locale;
        }

        [Display(Name = "Fulfillable Name")]
        public string FulfillableName => MFulfillable.Name;

        private IList<ReturnRequestFulfillableItem> m_ReturnRequestFulfillableItems;
        public IList<ReturnRequestFulfillableItem> ReturnRequestFulfillableItems
        {
            get
            {
                if (m_ReturnRequestFulfillableItems == null)
                {
                    m_ReturnRequestFulfillableItems = new List<ReturnRequestFulfillableItem>();
                    foreach (var mFulfillableItem in MFulfillableItems)
                    {
                        var mReturnRequestItem = MReturnRequestItems.Where(r => r.FulfillableItemId == mFulfillableItem.FulfillableItemId).First();
                        m_ReturnRequestFulfillableItems.Add(new ReturnRequestFulfillableItem(
                            mFulfillableItem,
                            mReturnRequestItem,
                            Locale));
                    }

                }
                return m_ReturnRequestFulfillableItems;
            }
        }
    }

    public class ReturnRequestFulfillableItem
    {
        public MFulfillment_FulfillableItem MFulfillableItem { get; }
        public MFulfillment_ReturnRequestItem MReturnRequestItem { get; }
        public IApplicationLocale Locale { get; }

        public ReturnRequestFulfillableItem(
            MFulfillment_FulfillableItem mFulfillableItem,
            MFulfillment_ReturnRequestItem mReturnRequestItem,
            IApplicationLocale locale)
        {
            MFulfillableItem = mFulfillableItem;
            MReturnRequestItem = mReturnRequestItem;
            Locale = locale;
        }

        [Display(Name = "ID")]
        public string Id => $"RRI:{MReturnRequestItem.ReturnRequestItemId}:FI:{MFulfillableItem.FulfillableItemId}";

        [Display(Name = "Return Request Item ID")]
        public long ReturnRequestItemId => MReturnRequestItem.ReturnRequestItemId;

        [Display(Name = "Quantity")]
        public int Quantity => MReturnRequestItem.Quantity;

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
                            MReturnRequestItem.Quantity,
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
        public MFulfillment_ReturnRequestTransactionSummary MTransaction { get; }
        public IApplicationLocale Locale { get; }

        public TransactionModel(
            MFulfillment_ReturnRequestTransactionSummary mTransaction,
            IApplicationLocale locale)
        {
            MTransaction = mTransaction;
            Locale = locale;
        }

        public string Id => $"RR:{MTransaction.ReturnRequestTransactionId}";

        public long ReturnRequestTransactionId => MTransaction.ReturnRequestTransactionId;

        public long ReturnRequestId => MTransaction.ReturnRequestId;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime TransactionDateTime => Locale.GetLocalTimeFromUtc(MTransaction.TransactionDateTimeUtc);

        public string Description => MTransaction.Description;

        public string UnitOfWork => MTransaction.UnitOfWork;
    }

    public class EventModel
    {
        public MFulfillment_ReturnRequestEventLogSummary MEvent { get; }
        public IApplicationLocale Locale { get; }

        public EventModel(
            MFulfillment_ReturnRequestEventLogSummary mEvent,
            IApplicationLocale locale)
        {
            MEvent = mEvent;
            Locale = locale;
        }

        public string Id => $"RrE:{MEvent.ReturnRequestEventId}";

        public long ReturnRequestEventId => MEvent.ReturnRequestEventId;

        public long ReturnRequestTransactionId => MEvent.ReturnRequestTransactionId;

        public string EventTypeCode => MEvent.EventTypeCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime EventDateTime => Locale.GetLocalTimeFromUtc(MEvent.EventDateTimeUtc);

        public string ProcessingStatusCode => MEvent.ProcessingStatusCode;

        [DisplayFormat(DataFormatString = Standard.DateTimeFormat)]
        public DateTime StatusDateTime => Locale.GetLocalTimeFromUtc(MEvent.StatusDateTimeUtc);

        public string UnitOfWork => MEvent.UnitOfWork;
    }
}