//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using PagedList;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Resources.Web;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.WebAdmin.Models.Domain;

namespace RichTodd.QuiltSystem.WebAdmin.Models.Order
{
    public class OrderModelFactory : ApplicationModelFactory
    {
        public Order CreateOrder(AOrder_Order aOrder, IDomainMicroService domainMicroService)
        {
            var domainFactory = Create<DomainModelFactory>(Context);

            var model = new Order(
                aOrder,
                Locale,
                domainFactory.CreateShippingVendorDomainModel(domainMicroService));

            return model;
        }

        public OrderList CreateOrderList(IList<AOrder_OrderSummary> aSummaries, PagingState pagingState)
        {
            var summaries = aSummaries.Select(r => CreateOrderListItem(r)).ToList();

            var sortFunction = GetSortFunction(pagingState.Sort);
            var sortedSummaries = sortFunction != null
                ? pagingState.Descending
                    ? summaries.OrderByDescending(sortFunction).ToList()
                    : summaries.OrderBy(sortFunction).ToList()
                : summaries;

            var pageSize = PagingState.PageSize;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedSummaries.Count, pageSize);
            var pagedSummaries = sortedSummaries.ToPagedList(pageNumber, pageSize);

            var (orderNumber, orderDate, orderStatus, userName, recordCount) = ParsePagingStateFilter(pagingState.Filter);

            var model = new OrderList()
            {
                Items = pagedSummaries,
                Filter = new OrderListFilter()
                {
                    OrderNumber = orderNumber,
                    OrderDate = orderDate,
                    UserName = userName,
                    OrderStatus = orderStatus,
                    RecordCount = recordCount,

                    OrderStatusList = new List<SelectListItem>
                    {
                        new SelectListItem() { Text = "All", Value = MOrder_OrderStatus.MetaAll.ToString() },
                        new SelectListItem() { Text = "Pending", Value = MOrder_OrderStatus.Pending.ToString() },
                        new SelectListItem() { Text = "Submitted", Value = MOrder_OrderStatus.Submitted.ToString() },
                        new SelectListItem() { Text = "Fulfilling", Value = MOrder_OrderStatus.Fulfilling.ToString() },
                        new SelectListItem() { Text = "Closed", Value = MOrder_OrderStatus.Closed.ToString() }
                    },
                    RecordCountList = CreateRecordCountList()
                }
            };

            return model;
        }

        public OrderListItem CreateOrderListItem(AOrder_OrderSummary aSummary)
        {
            var model = new OrderListItem(aSummary, Locale);

            return model;
        }

        public EditTransaction CreateOrderEditTransactionModel(AOrder_Order svcOrder)
        {
            var transactionTypes = new List<DomainValue>
            {
                new DomainValue() { Value = OrderTransactionTypeCodes.Submit, Text = "Submit" },
                new DomainValue() { Value = OrderTransactionTypeCodes.FundsRequired, Text = "Funds Required" },
                new DomainValue() { Value = OrderTransactionTypeCodes.FundsReceived, Text = "Funds Received" },
                new DomainValue() { Value = OrderTransactionTypeCodes.FulfillmentRequired, Text = "Fulfillment Required" },
                new DomainValue() { Value = OrderTransactionTypeCodes.FulfillmentComplete, Text = "Fulfillment Complete" }
            };

            var model = new EditTransaction()
            {
                OrderId = svcOrder.MOrder.OrderId,
                TransactionTypes = new SelectList(transactionTypes, "Value", "Text")
            };

            var items = new List<EditTransaction.TransactionEntry>();
            foreach (var svcItem in svcOrder.MOrder.OrderItems)
            {
                var item = new EditTransaction.TransactionEntry()
                {
                    OrderItemId = svcItem.OrderItemId,
                    Quantity = svcItem.NetQuantity,
                    Selected = false
                };
                items.Add(item);
            }
            model.TransactionEntries = items;

            return model;
        }

        public string GetDefaultSort()
        {
            return ListItemMetadata.GetDisplayName(m => m.OrderNumber);
        }

        public string CreatePagingStateFilter(string orderNumber, DateTime? orderDate, MOrder_OrderStatus orderStatus, string userName, int recordCount)
        {
            return $"{orderNumber}|{orderDate}|{orderStatus}|{userName}|{recordCount}";
        }

        public string CreatePagingStateFilter(OrderListFilter orderListFilter)
        {
            return CreatePagingStateFilter(orderListFilter.OrderNumber, orderListFilter.OrderDate, orderListFilter.OrderStatus, orderListFilter.UserName, orderListFilter.RecordCount);
        }

        public (string unitOfWork, DateTime? orderDate, MOrder_OrderStatus orderStatus, string userName, int recordCount) ParsePagingStateFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return (null, null, MOrder_OrderStatus.MetaAll, null, DefaultRecordCount);
            }

            var fields = filter.Split('|');

            var orderNumber = fields.Length >= 1 ? fields[0] : null;

            var orderDate = fields.Length >= 2 && DateTime.TryParse(fields[1], out var orderDateField)
                ? (DateTime?)orderDateField
                : null;

            var orderStatus = fields.Length >= 3 && Enum.TryParse(fields[2], out MOrder_OrderStatus orderStatusField)
               ? orderStatusField
               : MOrder_OrderStatus.MetaAll;

            var userName = fields.Length >= 4 ? fields[3] : null;

            var recordCount = fields.Length >= 5 && int.TryParse(fields[4], out var recordCountField)
                ? recordCountField
                : DefaultRecordCount;

            return (orderNumber, orderDate, orderStatus, userName, recordCount);
        }

        private ModelMetadata<OrderListItem> m_listItemMetadata;
        private ModelMetadata<OrderListItem> ListItemMetadata
        {
            get
            {
                if (m_listItemMetadata == null)
                {
                    m_listItemMetadata = ModelMetadata<OrderListItem>.Create(HttpContext);
                }

                return m_listItemMetadata;
            }
        }

        private static IDictionary<string, Func<OrderListItem, object>> s_sortFunctions;
        private IDictionary<string, Func<OrderListItem, object>> SortFunctions
        {
            get
            {
                if (s_sortFunctions == null)
                {
                    var heading = new AOrder_OrderSummary();

                    var sortFunctions = new Dictionary<string, Func<OrderListItem, object>>
                    {
                        { ListItemMetadata.GetDisplayName(m => m.OrderDateTime), r => r.OrderDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.OrderId), r => r.OrderId },
                        { ListItemMetadata.GetDisplayName(m => m.OrderNumber), r => r.OrderNumber },
                        { ListItemMetadata.GetDisplayName(m => m.OrderStatusType), r => r.OrderStatusType },
                        { ListItemMetadata.GetDisplayName(m => m.StatusDateTime), r => r.StatusDateTime },
                        { ListItemMetadata.GetDisplayName(m => m.Total), r => r.Total },
                        { ListItemMetadata.GetDisplayName(m => m.UserId), r => r.UserId },
                        { ListItemMetadata.GetDisplayName(m => m.UserName), r => r.UserName }
                    };

                    s_sortFunctions = sortFunctions;
                }

                return s_sortFunctions;
            }
        }

        private Func<OrderListItem, object> GetSortFunction(string sort)
        {
            return !string.IsNullOrEmpty(sort)
                ? SortFunctions[sort]
                : null;
        }

        private class DomainValue
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}