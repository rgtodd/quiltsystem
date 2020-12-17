//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.User.Abstractions
{
    public interface ICartUserService
    {
        Task<UOrder_Order> GetCartOrderAsync(string userId);

        Task<UCart_Cart> GetCartAsync(string userId);

        Task<bool> AddProjectAsync(string userId, Guid projectId, int quantity);

        Task<bool> UpdateCartItemQuantityAsync(string userId, long orderItemId, int quantity);

        Task<bool> DeleteCartItemAsync(string userId, int orderItemId);

        Task<bool> EmptyCartAsync();

        Task<bool> UpdateShippingAddressAsync(string userId, UCart_ShippingAddress shippingAddress);

        Task<long> SubmitCartOrderAsync(string userId);

        Task<UCart_CreateSquarePaymentResponse> CreateSquarePaymentAsync(string userId, decimal paymentAmount, string nonce);
    }
}