//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Mvc.Models
{
    public class OrderVcModelFactory : ApplicationModelFactory
    {
        #region Methods

        public OrderVcModel CreateOrderVcModel(MOrder_Order from, IApplicationLocale locale)
        {
            var to = new OrderVcModel();
            CopyOrderVcModel(to, from, locale);
            return to;
        }

        private void CopyOrderVcModel(OrderVcModel to, MOrder_Order from, IApplicationLocale locale)
        {
            to.OrderId = from.OrderId;
            to.OrderNumber = from.OrderNumber;
            to.ItemSubtotal = from.ItemSubtotalAmount;
            to.Shipping = from.ShippingAmount;
            to.Discount = from.DiscountAmount;
            to.TaxableAmount = from.TaxableAmount;
            to.SalesTaxPercent = from.SalesTaxPercent;
            to.SalesTax = from.SalesTaxAmount;
            to.Total = from.TotalAmount;
            to.OrderStatus = from.OrderStatus.ToString();
            to.StatusDateTime = locale.GetLocalTimeFromUtc(from.UpdateDateTimeUtc);
            to.SubmissionDateTime = locale.GetLocalTimeFromUtc(from.SubmissionDateTimeUtc);
            to.ShippingName = from.ShippingAddress.Name;
            to.ShippingAddressLines =
                FormatAddress(
                    from.ShippingAddress.AddressLine1,
                    from.ShippingAddress.AddressLine2,
                    from.ShippingAddress.City,
                    from.ShippingAddress.StateCode,
                    from.ShippingAddress.PostalCode,
                    from.ShippingAddress.CountryCode);
            to.Items = CreateOrderItemVcModels(from.OrderItems);
        }

        private IList<OrderItemVcModel> CreateOrderItemVcModels(IEnumerable<MOrder_OrderItem> from)
        {
            var to = new List<OrderItemVcModel>();
            foreach (var fromItem in from)
            {
                var toItem = new OrderItemVcModel()
                {
                    Description = fromItem.Description,
                    OrderableReference = fromItem.OrderableReference,
                    OrderItemId = fromItem.OrderItemId,
                    Quantity = fromItem.NetQuantity,
                    UnitPrice = fromItem.UnitPrice,
                    TotalPrice = fromItem.TotalPrice,
                    OrderItemSequence = fromItem.OrderItemSequence,
                    Sku = fromItem.Sku
                };
                to.Add(toItem);
            }
            return to;
        }

        #endregion Methods
    }
}