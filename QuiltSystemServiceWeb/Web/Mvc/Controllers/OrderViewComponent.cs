//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web.Mvc.Models;

namespace RichTodd.QuiltSystem.Web.Mvc.Controllers
{
    public class OrderViewComponent : ApplicationViewComponent<OrderVcModelFactory>
    {
        private IOrderMicroService OrderMicroService { get; }

        public OrderViewComponent(
            IApplicationLocale applicationLocale,
            IOrderMicroService orderMicroService)
            : base(applicationLocale)
        {
            OrderMicroService = orderMicroService;
        }

        public async Task<IViewComponentResult> InvokeAsync(long orderId)
        {
            var mOrder = await OrderMicroService.GetOrderAsync(orderId);

            var model = ModelFactory.CreateOrderVcModel(mOrder, Locale);

            return View(model);
        }
    }
}
