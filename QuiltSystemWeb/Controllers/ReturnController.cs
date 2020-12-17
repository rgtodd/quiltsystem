//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Models.Return;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    // Views:
    //
    // * ReturnStart
    // * ReturnEditReason
    // * ReturnEditItems
    // * ReturnConfirm
    // * ReturnComplete
    public class ReturnController : ApplicationController<ReturnRequestModelFactory>
    {
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IOrderUserService OrderUserService { get; }

        public ReturnController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IOrderUserService orderUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            OrderUserService = orderUserService ?? throw new ArgumentNullException(nameof(OrderUserService));
        }

        public async Task<ActionResult> Index(long id)
        {
            var svcOrder = await OrderUserService.GetOrderAsync(id);

            var model = ModelFactory.CreateReturnRequestViewModel(svcOrder, null, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

            return View("ReturnStart", model);
        }

        [HttpGet]
        public async Task<ActionResult> ReturnComplete(long id)
        {
            var svcOrder = await OrderUserService.GetOrderAsync(id);
            var svcReturnRequest = GetActiveReturnRequest(svcOrder);

            var model = ModelFactory.CreateReturnRequestViewModel(svcOrder, svcReturnRequest, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ReturnConfirm(long id)
        {
            var svcOrder = await OrderUserService.GetOrderAsync(id);
            var svcReturnRequest = GetActiveReturnRequest(svcOrder);

            var model = ModelFactory.CreateReturnRequestViewModel(svcOrder, svcReturnRequest, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ReturnConfirm(ReturnRequestEditModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Continue:
                    {
                        await FulfillmentMicroService.PostReturnRequestAsync(model.OrderReturnRequestId.Value);

                        return RedirectToAction("ReturnComplete", new { id = model.OrderId });
                    }

                case Actions.Back:
                    {
                        return RedirectToAction("ReturnEditItems", new { id = model.OrderId });
                    }
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReturnEditItems(long id)
        {
            var svcOrder = await OrderUserService.GetOrderAsync(id);
            var svcReturnRequest = GetActiveReturnRequest(svcOrder);

            var model = ModelFactory.CreateReturnRequestEditModel(svcOrder, svcReturnRequest, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ReturnEditItems(ReturnRequestEditModel model)
        {
            if (ModelState.IsValid)
            {
                var quantitySpecified = false;// Assume failure.
                foreach (var item in model.Items)
                {
                    if (item.Quantity > 0)
                    {
                        quantitySpecified = true;
                        break;
                    }
                }
                if (!quantitySpecified)
                {
                    ModelState.AddModelError("", "No items have been selected.");
                }
            }
            if (!ModelState.IsValid)
            {
                var svcOrder = await OrderUserService.GetOrderAsync(model.OrderId);
                var svcReturnRequest = GetActiveReturnRequest(svcOrder);

                model = ModelFactory.CreateReturnRequestEditModel(svcOrder, svcReturnRequest, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

                return View(model);
            }

            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Continue:
                    {
                        //if (svcReturnRequest.ReturnRequestId.HasValue)
                        //{
                        //    var svcReturnRequest = await CreateReturnRequestAsync(model);

                        //    await FulfillmentMicroService.UpdateReturnRequestAsync(svcReturnRequest);
                        //}
                        //else
                        //{
                        //    var svcReturnRequest = await CreateReturnRequestAsync(model);

                        //    _ = await FulfillmentMicroService.CreateReturnRequestAsync(svcReturnRequest);
                        //}

                        return RedirectToAction("ReturnConfirm", new { id = model.OrderId });
                    }

                case Actions.Back:
                    {
                        return RedirectToAction("Index", new { id = model.OrderId });
                    }
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReturnEditReason(long id)
        {
            var svcOrder = await OrderUserService.GetOrderAsync(id);
            var svcReturnRequest = GetActiveReturnRequest(svcOrder);

            var model = ModelFactory.CreateReturnRequestEditModel(svcOrder, svcReturnRequest, await FulfillmentMicroService.GetReturnRequestReasonsAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ReturnEditReason(ReturnRequestEditModel model)
        {
            var actionData = this.GetActionData();
            switch (actionData.ActionName)
            {
                case Actions.Continue:
                    {
                        if (!ModelState.IsValid)
                        {
                            return View();
                        }

                        await Task.CompletedTask;

                        //if (svcReturnRequest.ReturnRequestId.HasValue)
                        //{
                        //    await FulfillmentMicroService.UpdateReturnRequestAsync(svcReturnRequest);
                        //}
                        //else
                        //{
                        //    _ = await FulfillmentMicroService.CreateReturnRequestAsync(svcReturnRequest);
                        //}

                        return RedirectToAction("ReturnEditItems", new { id = model.OrderId });
                    }

                case Actions.Back:
                    {
                        return RedirectToAction("Index", new { id = model.OrderId });
                    }

                default:
                    throw new InvalidOperationException(string.Format("Unknown action {0}", actionData.ActionName));
            }
        }

        #region Methods

        //private async Task<MFulfillment_CreateReturnRequest> CreateReturnRequestAsync(ReturnRequestEditModel model)
        //{
        //    var mCreateReturnRequestItems = new List<MFulfillment_CreateReturnRequestItem>();
        //    foreach (var item in model.Items)
        //    {
        //        var fulfillableItemReference = CreateFulfillableItemReference.FromOrderItemId(item.OrderItem.OrderItemId);
        //        var fulfillableItemId = (await FulfillmentMicroService.LookupFulfillableItemAsync(fulfillableItemReference)).Value;

        //        var mCreateReturnRequestItem = new MFulfillment_CreateReturnRequestItem()
        //        {
        //            FulfillableItemId = fulfillableItemId,
        //            Quantity = item.Quantity
        //        };
        //        mCreateReturnRequestItems.Add(mCreateReturnRequestItem);
        //    }

        //    var mCreateReturnRequest = new MFulfillment_CreateReturnRequest()
        //    {
        //        ReturnRequestReasonCode = model.ReasonTypeCode,
        //        ReturnRequestType = Enum.Parse<MFulfillment_ReturnRequestTypes>(model.ReturnTypeCode),
        //        Notes = model.Notes,
        //        CreateReturnRequestItems = mCreateReturnRequestItems
        //    };
        //    return mCreateReturnRequest;
        //}

        //private async Task<MFulfillment_UpdateReturnRequest> UpdateReturnRequestAsync(ReturnRequestEditModel model)
        //{
        //    var fulfillableReference = CreateFulfillableReference.FromOrderId(model.OrderId);
        //    var fulfillableId = (await FulfillmentMicroService.LookupFulfillableAsync(fulfillableReference)).Value;

        //    var svcItems = new List<MFulfillment_UpdateReturnRequestItem>();
        //    foreach (var item in model.Items)
        //    {
        //        var fulfillableItemReference = CreateFulfillableItemReference.FromOrderItemId(item.OrderItem.OrderItemId);
        //        var fulfillableItemId = (await FulfillmentMicroService.LookupFulfillableItemAsync(fulfillableItemReference)).Value;

        //        var svcItem = new MFulfillment_UpdateReturnRequestItem()
        //        {
        //            ReturnRequestItemId = item.OrderReturnRequestItemId.Value,
        //            Quantity = item.Quantity
        //        };
        //        svcItems.Add(svcItem);
        //    }

        //    var svcReturnRequest = new MFulfillment_UpdateReturnRequest()
        //    {
        //        ReturnRequestId = model.OrderReturnRequestId.Value,
        //        //ReturnType = (MFulfillment_ReturnRequestTypes)model.ReturnTypeCode,
        //        ReturnRequestReasonCode = model.ReasonTypeCode,
        //        Notes = model.Notes,
        //        UpdateReturnRequestItems = svcItems
        //    };
        //    return svcReturnRequest;
        //}

        private MFulfillment_ReturnRequest GetActiveReturnRequest(UOrder_Order svcOrder)
        {
            return svcOrder.MFulfillable.ReturnRequests.Count != 0 ?
                svcOrder.MFulfillable.ReturnRequests[0]
                : null;

            //return null;
            //var svcOpenReturnRequest = svcOrder.ReturnRequests.Where(r => r.StatusType.In(UOrder_OrderReturnRequest.StatusTypes.Open)).SingleOrDefault();
            //if (svcOpenReturnRequest != null)
            //{
            //    return svcOpenReturnRequest;
            //}

            //var svcPostedReturnRequest = svcOrder.ReturnRequests.Where(r => r.StatusType.In(UOrder_OrderReturnRequest.StatusTypes.Posted)).SingleOrDefault();
            //return svcPostedReturnRequest;
        }

        #endregion Methods
    }
}