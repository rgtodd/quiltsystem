//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.WebAdmin.Models.Order;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class OrderController : ApplicationController<OrderModelFactory>
    {
        private IOrderAdminService OrderAdminService { get; }

        public OrderController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IOrderAdminService orderAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            OrderAdminService = orderAdminService ?? throw new ArgumentNullException(nameof(orderAdminService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id != null)
            {
                var model = await GetOrderAsync(id.Value);

                return View("Detail", model);
            }
            else
            {
                this.SetSortDefault(ModelFactory.GetDefaultSort());
                this.SetFilterDefault(ModelFactory.CreatePagingStateFilter(null, null, MOrder_OrderStatus.MetaAll, null, ModelFactory.DefaultRecordCount));

                var model = await GetOrderListAsync();

                return View("List", model);
            }
        }

        public Task<ActionResult> DetailSubmit(long id)
        {
            return Index(id);
        }

        [HttpPost]
        public async Task<ActionResult> DetailSubmit(OrderDetailSubmit model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case "processReceipts":
                    {
                        //await m_orderService.ProcessOrderReceiptsAsync(model.Order.OrderId);

                        return RedirectToAction("Index", new { id = model.OrderId });
                    }
            }

            return await Index(model.OrderId);
        }

        [HttpGet]
        public async Task<ActionResult> EditTransaction(string id)
        {
            var orderId = long.Parse(id);

            var svcOrderDetail = await OrderAdminService.GetOrderAsync(orderId);

            var model = ModelFactory.CreateOrderEditTransactionModel(svcOrderDetail);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditTransaction(EditTransaction model)
        {
            var svcData = new AOrder_PostOrderTransaction()
            {
                OrderId = model.OrderId,
                OrderTransactionTypeCode = model.OrderTransactionTypeCode
            };
            var svcItems = new List<AOrder_PostOrderTransactionItem>();
            foreach (var item in model.TransactionEntries)
            {
                if (item.Selected)
                {
                    var svcItem = new AOrder_PostOrderTransactionItem()
                    {
                        OrderItemId = item.OrderItemId,
                        OrderItemStatusTypeCode = model.OrderItemStatusTypeCode,
                        Quantity = item.Quantity
                    };
                    svcItems.Add(svcItem);
                }
            }
            svcData.Items = svcItems;

            _ = await OrderAdminService.PostOrderTransactionAsync(svcData);

            return RedirectToAction("Index", new { id = model.OrderId });
        }

        public Task<ActionResult> ListSubmit()
        {
            return Index(null);
        }

        [HttpPost]
        public async Task<ActionResult> ListSubmit(OrderList model)
        {
            this.SetPagingState(ModelFactory.CreatePagingStateFilter(model.Filter));

            model = await GetOrderListAsync();

            return View("List", model);
        }

        private async Task<Order> GetOrderAsync(long orderId)
        {
            var aOrder = await OrderAdminService.GetOrderAsync(orderId);

            var model = ModelFactory.CreateOrder(aOrder, DomainMicroService);

            return model;
        }

        private async Task<OrderList> GetOrderListAsync()
        {
            var pagingState = this.GetPagingState(0);

            var (orderNumber, orderDate, orderStatus, userName, recordCount) = ModelFactory.ParsePagingStateFilter(pagingState.Filter);

            var orderDateUtc = orderDate != null
                ? (DateTime?)Locale.GetUtcFromLocalTime(orderDate.Value)
                : null;

            var aOrderSummaries = new List<AOrder_OrderSummary>(await OrderAdminService.GetOrderSummariesAsync(orderNumber, orderDateUtc, orderStatus, userName, recordCount));

            var model = ModelFactory.CreateOrderList(aOrderSummaries, pagingState);

            return model;
        }

    }
}