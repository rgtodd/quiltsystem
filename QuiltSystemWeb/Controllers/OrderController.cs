//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Models.Order;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class OrderController : ApplicationController<OrderModelFactory>
    {
        private IOrderUserService OrderUserService { get; }

        public OrderController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IOrderUserService orderUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            OrderUserService = orderUserService ?? throw new ArgumentNullException(nameof(orderUserService));
        }

        public async Task<ActionResult> Index(long? id)
        {
            if (id != null)
            {
                var svcOrder = await OrderUserService.GetOrderAsync(id.Value);

                var model = ModelFactory.CreateOrderDetailModel(svcOrder);

                model.Collapsable = false;

                return View("Detail", model);
            }
            else
            {
                var svcOrders = await OrderUserService.GetOrdersAsync(GetUserId());

                var model = ModelFactory.CreateOrderDetalListModel(svcOrders, this.GetPagingState(0));

                return View("List", model);
            }
        }
    }
}