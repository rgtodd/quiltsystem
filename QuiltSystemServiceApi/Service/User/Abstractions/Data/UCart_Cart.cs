//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.User.Abstractions.Data
{
    public class UCart_Cart
    {
        public UOrder_Order Order { get; set; }
        public Cart_PaymentStatus PaymentStatus { get; set; }
    }

    public enum Cart_PaymentStatus
    {
        Required,
        InProgress,
        Complete
    };
}
