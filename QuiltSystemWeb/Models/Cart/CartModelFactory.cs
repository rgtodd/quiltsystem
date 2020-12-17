//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Models.Cart
{
    public class CartModelFactory : ApplicationModelFactory
    {
        public CartDetailModel CreateCartDetailModel(UOrder_Order from)
        {
            var to = new CartDetailModel();
            Copy(to, from);
            return to;
        }

        public CartEditModel CreateCartEditModel(UOrder_Order from)
        {
            var to = new CartEditModel();
            Copy(to, from);
            return to;
        }

        public CartEditShippingAddressModel CreateCartShippingAddressModel(UOrder_Order from)
        {
            if (from == null) return null;

            var to = new CartEditShippingAddressModel();
            Copy(to, from);
            return to;
        }

        private void Copy(CartEditModel to, UOrder_Order from)
        {
            to.OrderId = from.MOrder.OrderId;
            to.OrderNumber = from.MOrder.OrderNumber;
            to.Items = CreateCartEditItemModels(from.MOrder.OrderItems);
            to.ItemSubtotalAmount = from.MOrder.ItemSubtotalAmount;
            to.DiscountAmount = from.MOrder.DiscountAmount;
            to.ShippingAmount = from.MOrder.ShippingAmount;
            to.TaxableSubtotal = to.ItemSubtotalAmount - to.DiscountAmount + to.ShippingAmount;
            to.SalesTaxPercent = from.MOrder.SalesTaxPercent;
            to.SalesTaxAmount = from.MOrder.SalesTaxAmount;
            to.TotalAmount = from.MOrder.TotalAmount;
            to.SubmissionDateTime = Locale.GetLocalTimeFromUtc(from.MOrder.SubmissionDateTimeUtc);
            to.OrderStatus = from.MOrder.OrderStatus.ToString();
        }

        private void Copy(CartDetailModel to, UOrder_Order from)
        {
            to.OrderId = from.MOrder.OrderId;
            to.OrderNumber = from.MOrder.OrderNumber;
            to.Items = CreateCartDetailItemModels(from.MOrder.OrderItems);
            to.ItemSubtotalAmount = from.MOrder.ItemSubtotalAmount;
            to.DiscountAmount = from.MOrder.DiscountAmount;
            to.ShippingAmount = from.MOrder.ShippingAmount;
            to.TaxableSubtotal = to.ItemSubtotalAmount - to.DiscountAmount + to.ShippingAmount;
            to.SalesTaxPercent = from.MOrder.SalesTaxPercent;
            to.SalesTaxAmount = from.MOrder.SalesTaxAmount;
            to.TotalAmount = from.MOrder.TotalAmount;
            to.SubmissionDateTime = Locale.GetLocalTimeFromUtc(from.MOrder.SubmissionDateTimeUtc);
            to.OrderStatus = from.MOrder.OrderStatus.ToString();
            to.ShippingName = from.MOrder.ShippingAddress.Name;
            to.ShippingAddressLines =
                FormatAddress(
                    from.MOrder.ShippingAddress.AddressLine1,
                    from.MOrder.ShippingAddress.AddressLine2,
                    from.MOrder.ShippingAddress.City,
                    from.MOrder.ShippingAddress.StateCode,
                    from.MOrder.ShippingAddress.PostalCode,
                    from.MOrder.ShippingAddress.CountryCode);
        }

        private void Copy(CartEditShippingAddressModel to, UOrder_Order from)
        {
            to.OrderId = from.MOrder.OrderId;
            to.Name = from.MOrder.ShippingAddress?.Name;
            to.AddressLine1 = from.MOrder.ShippingAddress?.AddressLine1;
            to.AddressLine2 = from.MOrder.ShippingAddress?.AddressLine2;
            to.City = from.MOrder.ShippingAddress?.City;
            to.StateCode = from.MOrder.ShippingAddress?.StateCode;
            to.PostalCode = from.MOrder.ShippingAddress?.PostalCode;
            to.CountryCode = from.MOrder.ShippingAddress?.CountryCode;
        }

        private void Copy(CartEditItemModel to, MOrder_OrderItem from)
        {
            to.OrderableReference = from.OrderableReference;
            to.OriginalQuantity = from.OrderQuantity;
            to.OrderItemId = from.OrderItemId;
            to.Name = from.Description;
            to.Sku = from.Sku;
            to.Quantity = from.NetQuantity;
            to.KitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;
            to.Components = CreateCartItemComponentModels(from.OrderItemComponents);
        }

        private void Copy(CartDetailItemModel to, MOrder_OrderItem from)
        {
            to.OrderableReference = from.OrderableReference;
            to.OrderItemId = from.OrderItemId;
            to.Name = from.Description;
            to.Sku = from.Sku;
            to.Quantity = from.NetQuantity;
            to.KitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;

            to.Components = CreateCartItemComponentModels(from.OrderItemComponents);
        }

        private void Copy(CartItemComponentModel to, MOrder_OrderItemComponent from)
        {
            to.Description = from.Description;
            to.Quantity = from.Quantity;
            //to.Sku = from.Sku;
            to.TotalPrice = from.TotalPrice;
            to.UnitPrice = from.UnitPrice;
        }

        private CartDetailItemModel CreateCartDetailItemModel(MOrder_OrderItem from)
        {
            var to = new CartDetailItemModel();
            Copy(to, from);
            return to;
        }

        private IList<CartDetailItemModel> CreateCartDetailItemModels(IEnumerable<MOrder_OrderItem> from)
        {
            var to = new List<CartDetailItemModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateCartDetailItemModel(fromItem));
            }
            return to;
        }

        private CartEditItemModel CreateCartEditItemModel(MOrder_OrderItem from)
        {
            var to = new CartEditItemModel();
            Copy(to, from);
            return to;
        }

        private IList<CartEditItemModel> CreateCartEditItemModels(IEnumerable<MOrder_OrderItem> from)
        {
            var to = new List<CartEditItemModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateCartEditItemModel(fromItem));
            }
            return to;
        }

        private CartItemComponentModel CreateCartItemComponentModel(MOrder_OrderItemComponent from)
        {
            var to = new CartItemComponentModel();
            Copy(to, from);
            return to;
        }

        private IList<CartItemComponentModel> CreateCartItemComponentModels(IEnumerable<MOrder_OrderItemComponent> from)
        {
            var to = new List<CartItemComponentModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateCartItemComponentModel(fromItem));
            }
            return to;
        }

    }
}