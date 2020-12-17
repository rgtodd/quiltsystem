//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Implementations
{
    internal class CartUserService : BaseService, ICartUserService
    {
        private IEventProcessorMicroService EventProcessorMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }
        private IOrderUserService ProjectOrderService { get; }
        private ISquareMicroService SquareMicroService { get; set; }
        private IUserMicroService UserMicroService { get; }

        public CartUserService(
            IApplicationRequestServices requestServices,
            ILogger<CartUserService> logger,
            IEventProcessorMicroService eventProcessorMicroService,
            IOrderMicroService orderMicroService,
            IProjectMicroService projectMicroService,
            IOrderUserService projectOrderService,
            ISquareMicroService squareMicroService,
            IUserMicroService userMicroService)
            : base(requestServices, logger)
        {
            EventProcessorMicroService = eventProcessorMicroService ?? throw new ArgumentNullException(nameof(eventProcessorMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
            ProjectOrderService = projectOrderService ?? throw new ArgumentNullException(nameof(projectOrderService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
        }

        public async Task<UOrder_Order> GetCartOrderAsync(string userId)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(GetCartOrderAsync), userId);
            try
            {
                // HACK: Review logic
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);
                var mOrder = await OrderMicroService.GetCartOrderAsync(ordererId).ConfigureAwait(false);
                if (mOrder == null)
                {
                    return null;
                }

                //var mProjectSnapshots = new List<MProject_ProjectSnapshot>();
                //foreach (var mOrderItem in mOrder.OrderItems)
                //{
                //    var projectSnapshotId = ParseProjectSnapshotId.FromOrderableReference(mOrderItem.OrderableReference);
                //    var mProjectSnapshot = await ProjectMicroService.GetProjectSnapshotAsync(projectSnapshotId).ConfigureAwait(false);
                //    mProjectSnapshots.Add(mProjectSnapshot);
                //}

                var result = OrderUserService.Create.UOrder_Order(mOrder, null);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<UCart_Cart> GetCartAsync(string userId)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(GetCartAsync), userId);
            try
            {
                // HACK: Review logic
                //AssertIsEndUser(userId);
                //await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);
                var mOrder = await OrderMicroService.GetCartOrderAsync(ordererId).ConfigureAwait(false);
                if (mOrder == null)
                {
                    return null;
                }

                var mProjectOrder = OrderUserService.Create.UOrder_Order(mOrder, null);

                var squareCustomerReference = CreateSquareCustomerReference.FromUserId(userId);
                var squareCustomerId = await SquareMicroService.AllocateSquareCustomerAsync(squareCustomerReference);

                var squarePaymentReference = CreateSquarePaymentReference.FromOrderId(mOrder.OrderId);
                var squarePaymentId = await SquareMicroService.AllocateSquarePaymentAsync(squarePaymentReference, squareCustomerId);

                var mSquarePayment = await SquareMicroService.GetSquarePaymentAsync(squarePaymentId);
                var mWebPaymentRequest = mSquarePayment.WebPaymentRequests.Where(r =>
                    r.WebRequestTypeCode == SquarePaymentWebRequestTypeCodes.Payment &&
                    r.ProcessingStatusCode != SquareProcessingStatusCodes.Cancelled).SingleOrDefault();

                Cart_PaymentStatus paymentStatus;
                if (mWebPaymentRequest == null)
                {
                    paymentStatus = Cart_PaymentStatus.Required;
                }
                else
                {
                    switch (mWebPaymentRequest.ProcessingStatusCode)
                    {
                        case SquareProcessingStatusCodes.Rejected:
                            paymentStatus = Cart_PaymentStatus.Required;
                            break;

                        case SquareProcessingStatusCodes.Processed:
                            paymentStatus = Cart_PaymentStatus.Complete;
                            break;

                        case SquareProcessingStatusCodes.Pending:
                        case SquareProcessingStatusCodes.Processing:
                        case SquareProcessingStatusCodes.Transmitting:
                        case SquareProcessingStatusCodes.Transmitted:
                        case SquareProcessingStatusCodes.Exception:
                            paymentStatus = Cart_PaymentStatus.InProgress;
                            break;

                        default:
                            // Includes SquarePaymentWebRequestStatusTypes.Cancelled - should have been filtered out above.
                            //
                            throw new InvalidOperationException($"Unknown payment transaction status type {mWebPaymentRequest.ProcessingStatusCode}.");
                    }
                }

                var result = new UCart_Cart()
                {
                    Order = mProjectOrder,
                    PaymentStatus = paymentStatus
                };

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> AddProjectAsync(string userId, Guid projectId, int quantity)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(AddProjectAsync), userId, projectId, quantity);
            try
            {
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference).ConfigureAwait(false);

                var projectSnapshotId = await ProjectMicroService.GetCurrentSnapshotIdAsync(projectId).ConfigureAwait(false);
                var mProjectSnapshotDetail = await ProjectMicroService.GetProjectSnapshotAsync(projectSnapshotId).ConfigureAwait(false);
                var mAllocateOrderable = MicroDataFactory.MOrder_AllocateOrderable(mProjectSnapshotDetail);
                var mAllocateOrderableResponseData = await OrderMicroService.AllocateOrderableAsync(mAllocateOrderable).ConfigureAwait(false);
                var orderableId = mAllocateOrderableResponseData.OrderableId;

                var result = await OrderMicroService.AddCartItemAsync(ordererId, orderableId, quantity).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteCartItemAsync(string userId, int orderItemId)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(DeleteCartItemAsync), userId, orderItemId);
            try
            {
                //AssertIsEndUser(userId);
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference).ConfigureAwait(false);

                var result = await OrderMicroService.DeleteCartItemAsync(ordererId, orderItemId).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public Task<bool> EmptyCartAsync()
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(EmptyCartAsync));
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> SubmitCartOrderAsync(string userId)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(SubmitCartOrderAsync), userId);
            try
            {
                //AssertIsEndUser(userId);
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference).ConfigureAwait(false);

                var orderId = await OrderMicroService.SubmitCartAsync(ordererId).ConfigureAwait(false);

                var result = orderId;

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string userId, long orderItemId, int quantity)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(UpdateCartItemQuantityAsync), userId, orderItemId, quantity);
            try
            {
                //AssertIsEndUser(userId);
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference).ConfigureAwait(false);

                var result = await OrderMicroService.UpdateCartItemQuantityAsync(ordererId, orderItemId, quantity).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> UpdateShippingAddressAsync(string userId, UCart_ShippingAddress shippingAddress)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(UpdateShippingAddressAsync), userId, shippingAddress);
            try
            {
                //AssertIsEndUser(userId);
                await Assert(SecurityPolicy.IsAuthorized, userId).ConfigureAwait(false);

                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference).ConfigureAwait(false);

                var result = await OrderMicroService.UpdateShippingAddressAsync(ordererId, shippingAddress).ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<UCart_CreateSquarePaymentResponse> CreateSquarePaymentAsync(string userId, decimal paymentAmount, string nonce)
        {
            using var log = BeginFunction(nameof(CartUserService), nameof(CreateSquarePaymentAsync), userId, paymentAmount, nonce);
            try
            {
                var ordererReference = CreateOrdererReference.FromUserId(userId);
                var ordererId = await OrderMicroService.AllocateOrdererAsync(ordererReference);

                var uOrder = await GetCartOrderAsync(userId);
                if (uOrder == null)
                {
                    throw new InvalidOperationException("Shopping cart is empty.");
                }

                var squareCustomerReference = CreateSquareCustomerReference.FromUserId(userId);
                var squareCustomerId = await SquareMicroService.AllocateSquareCustomerAsync(squareCustomerReference);

                var squarePaymentReference = CreateSquarePaymentReference.FromOrderId(uOrder.MOrder.OrderId);
                var squarePaymentId = await SquareMicroService.AllocateSquarePaymentAsync(squarePaymentReference, squareCustomerId);

                var squareWebPaymentRequestId = await SquareMicroService.CreateSquareWebPaymentRequestAsync(squarePaymentId, paymentAmount, nonce);

                var mProcessWebPaymentTransactionResponse = await SquareMicroService.ProcessWebPaymentRequestAsync(squareWebPaymentRequestId);

                IList<Cart_CreateSquarePaymentResponseErrorData> errors;
                if (mProcessWebPaymentTransactionResponse.Errors == null)
                {
                    errors = null;

                    _ = await OrderMicroService.SubmitCartAsync(ordererId);
                }
                else
                {
                    errors = new List<Cart_CreateSquarePaymentResponseErrorData>();
                    foreach (var mError in mProcessWebPaymentTransactionResponse.Errors)
                    {
                        errors.Add(new Cart_CreateSquarePaymentResponseErrorData()
                        {
                            Category = mError.Category,
                            Code = mError.Code,
                            Detail = mError.Detail,
                            Field = mError.Field
                        });
                    }
                }

                var result = new UCart_CreateSquarePaymentResponse()
                {
                    Errors = errors
                };

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }
    }
}