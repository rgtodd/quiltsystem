//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Extensions;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Models.Cart;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class CartController : ApplicationController<CartModelFactory>
    {
        private ICartUserService CartUserService { get; }

        public CartController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            ICartUserService cartUserService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            CartUserService = cartUserService ?? throw new ArgumentNullException(nameof(cartUserService));
        }

        public async Task<ActionResult> Index()
        {
            var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
            if (svcOrder == null)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                return RedirectToAction("Index", "Order");
            }

            var order = ModelFactory.CreateCartEditModel(svcOrder);
            return View(order);
        }

        [HttpPost]
        public async Task<ActionResult> Index(CartEditModel model)
        {
            if (!ModelState.IsValid)
            {
                var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
                if (svcOrder == null)
                {
                    AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                    return RedirectToAction("Index", "Order");
                }

                var order = ModelFactory.CreateCartEditModel(svcOrder);
                return View(order);
            }

            // Apply changes to cart.
            //
            var actionData = this.GetActionData();
            switch (actionData.ActionName)
            {
                case Actions.Delete:
                    {
                        var orderItemId = int.Parse(actionData.ActionParameter);
                        var result = await CartUserService.DeleteCartItemAsync(GetUserId(), orderItemId);
                        if (!result)
                        {
                            throw new Exception("DeleteCartItemAsync failure.");
                        }

                        var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
                        if (svcOrder == null)
                        {
                            AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                            return RedirectToAction("Index", "Order");
                        }

                        model = ModelFactory.CreateCartEditModel(svcOrder);

                        ModelState.Clear();
                        AddFeedbackMessage(FeedbackMessageTypes.Informational, "Order updated.  Please review and click Continue to confirm.");
                        return View("Index", model);
                    }

                case Actions.Continue:
                    {
                        var orderUpdated = false; // Assume failure
                        foreach (var item in model.Items)
                        {
                            if (item.Quantity != item.OriginalQuantity)
                            {
                                _ = await CartUserService.UpdateCartItemQuantityAsync(GetUserId(), item.OrderItemId, item.Quantity);
                                orderUpdated = true;
                            }
                        }

                        if (orderUpdated)
                        {
                            var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
                            if (svcOrder == null)
                            {
                                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                                return RedirectToAction("Index", "Order");
                            }

                            model = ModelFactory.CreateCartEditModel(svcOrder);

                            ModelState.Clear();
                            AddFeedbackMessage(FeedbackMessageTypes.Informational, "Order updated.  Please review and click Continue to confirm.");
                            return View("Index", model);
                        }
                        else
                        {
                            return RedirectToAction("Shipping");
                        }
                    }

                default:
                    throw new InvalidOperationException(string.Format("Unknown action {0}", actionData.ActionName));
            }
        }

        public async Task<ActionResult> Shipping()
        {
            var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
            if (svcOrder == null)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                return RedirectToAction("Index", "Order");
            }

            var model = ModelFactory.CreateCartShippingAddressModel(svcOrder);

            model.StateCodes = GetStateCodes(string.IsNullOrEmpty(model.StateCode));

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Shipping(CartEditShippingAddressModel model)
        {
            try
            {
                var actionData = this.GetActionData();
                switch (actionData.ActionName)
                {
                    case Actions.Continue:
                        //if (!ControllerModelFactory.IsValidPostalCode(model.PostalCode))
                        //{
                        //    ModelState.AddModelError("PostalCode", "Invalid postal code.");
                        //}
                        if (!ModelState.IsValid)
                        {
                            if (model.StateCodes == null)
                            {
                                model.StateCodes = GetStateCodes(string.IsNullOrEmpty(model.StateCode));
                            }
                            return View(model);
                        }

                        model.CountryCode = "US"; // BUG: CartEditShippingAddressModel does not contain country code.

                        var shippingAddress = new UCart_ShippingAddress()
                        {
                            Name = model.Name,
                            AddressLine1 = model.AddressLine1,
                            AddressLine2 = model.AddressLine2,
                            City = model.City,
                            StateCode = model.StateCode,
                            PostalCode = model.PostalCode,
                            CountryCode = model.CountryCode
                        };

                        _ = await CartUserService.UpdateShippingAddressAsync(GetUserId(), shippingAddress);

                        return RedirectToAction("Confirm");

                    case Actions.Back:
                        return RedirectToAction("Index");

                    default:
                        throw new InvalidOperationException(string.Format("Unknown action {0}", actionData.ActionName));
                        //{
                        //    var svcOrder = await ServicePool.OrderService.GetCartOrderAsync(GetUserId());
                        //    model = CartModelFactory.CreateCartShippingAddressModel(svcOrder);

                        //    ModelState.Clear();
                        //    return View("Shipping", model);
                        //}
                }
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> Confirm()
        {
            var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
            if (svcOrder == null)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                return RedirectToAction("Index", "Order");
            }

            var model = ModelFactory.CreateCartDetailModel(svcOrder);
            return View(model);
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Avoid breaking changes.")]
        public async Task<ActionResult> Confirm(CartDetailModel order)
        {
            var actionData = this.GetActionData();
            switch (actionData?.ActionName)
            {
                case Actions.Continue:
                    //var orderId = await CartService.SubmitCartOrderAsync(GetUserId());
                    return RedirectToAction("Square");

                case Actions.Back:
                    return RedirectToAction("Shipping");

                default:
                    var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
                    if (svcOrder == null)
                    {
                        AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                        return RedirectToAction("Index", "Order");
                    }

                    var model = ModelFactory.CreateCartDetailModel(svcOrder);
                    return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Square()
        {
            var svcOrder = await CartUserService.GetCartOrderAsync(GetUserId());
            if (svcOrder == null)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Your shopping cart is empty.");
                return RedirectToAction("Index", "Order");
            }


            var model = new CartSquareModel()
            {
                OrderTotal = svcOrder.MOrder.TotalAmount
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Square(CartSquareModel model)
        {
            if (string.IsNullOrEmpty(model.Nonce))
            {
                if (!string.IsNullOrEmpty(model.Errors))
                {
                    var errors = DeserializeSquareErrors(model.Errors);
                    foreach (var e in errors)
                    {
                        AddFeedbackMessage(FeedbackMessageTypes.Error, $"{e.Type}: {e.Message} ({e.Field})");
                    }
                }

                model = new CartSquareModel()
                {
                    OrderTotal = model.OrderTotal
                };
                return View(model);
            }

            UCart_CreateSquarePaymentResponse response;
            try
            {
                response = await CartUserService.CreateSquarePaymentAsync(GetUserId(), model.OrderTotal, model.Nonce);
            }
            catch (Exception ex)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Error, ex.Message);
                return RedirectToAction("Index", "Order");
            }

            var error = response.Errors?.FirstOrDefault();
            if (error != null)
            {
                AddFeedbackMessage(FeedbackMessageTypes.Error, error.Detail);

                model = new CartSquareModel();
                return View(model);
            }
            else
            {
                AddFeedbackMessage(FeedbackMessageTypes.Informational, "Order submitted.");
                return RedirectToAction("Index", "Order");
            }
        }

        private SquareError[] DeserializeSquareErrors(string json)
        {
            var errors = JsonConvert.DeserializeObject<SquareError[]>(json);
            return errors;
        }

        private class SquareError
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public string Field { get; set; }
        }
    }
}