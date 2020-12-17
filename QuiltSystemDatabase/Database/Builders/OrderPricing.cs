//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;

using RichTodd.QuiltSystem.Database.Model;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class OrderPricing
    {
        public decimal ItemSubtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal PreTaxAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal TotalAmount { get; set; }

        public IList<OrderItemPricing> OrderItemPricings { get; set; }

        public static OrderPricing Compute(Order dbOrder)
        {
            var orderPricing = new OrderPricing()
            {
                OrderItemPricings = new List<OrderItemPricing>()
            };

            foreach (var dbOrderItem in dbOrder.OrderItems)
            {
                orderPricing.OrderItemPricings.Add(ComputeOrderItemPricing(dbOrderItem));
            }

            var shipping = dbOrder.OrderItems.Sum(r => r.NetQuantity) * 5.00m;
            orderPricing.Shipping = shipping;

            // Compute item subtotal.
            //
            {
                var itemSubtotal = 0m;
                foreach (var orderItemPricing in orderPricing.OrderItemPricings)
                {
                    itemSubtotal += orderItemPricing.TotalPrice;
                }

                orderPricing.ItemSubtotal = itemSubtotal;
            }

            // Compute pre-tax amount.
            //
            {
                var pretaxAmount = orderPricing.ItemSubtotal + orderPricing.Shipping + dbOrder.Discount;

                orderPricing.PreTaxAmount = pretaxAmount;
            }

            // Compute taxable amount.
            //
            {
                // HACK: Assuming shipping is taxable.
                //
                var shippingTaxable = true;

                decimal taxableAmount;
                taxableAmount = shippingTaxable
                    ? orderPricing.ItemSubtotal + orderPricing.Shipping + dbOrder.Discount
                    : orderPricing.ItemSubtotal + dbOrder.Discount;

                orderPricing.TaxableAmount = taxableAmount;
            }

            // Compute sales tax.
            //
            {
                var salesTax = Math.Round(orderPricing.TaxableAmount * dbOrder.SalesTaxRate, 2, MidpointRounding.AwayFromZero);

                orderPricing.SalesTax = salesTax;
            }

            // Update total amount.
            {
                var totalAmount = orderPricing.PreTaxAmount + orderPricing.SalesTax;

                orderPricing.TotalAmount = totalAmount;
            }

            return orderPricing;
        }

        private static OrderItemPricing ComputeOrderItemPricing(OrderItem orderItem)
        {
            var orderItemPricing = new OrderItemPricing()
            {
                OrderItemId = orderItem.OrderItemId
            };

            orderItemPricing.NetQuantity = orderItem.OrderQuantity - orderItem.CancelQuantity - orderItem.FulfillmentReturnQuantity;
            orderItemPricing.UnitPrice = orderItem.Orderable.Price;
            orderItemPricing.TotalPrice = orderItemPricing.UnitPrice * orderItemPricing.NetQuantity;

            return orderItemPricing;
        }

        public void Apply(Order dbOrder)
        {
            dbOrder.ItemSubtotal = ItemSubtotal;
            dbOrder.Shipping = Shipping;
            dbOrder.PretaxAmount = PreTaxAmount;
            dbOrder.TaxableAmount = TaxableAmount;
            dbOrder.SalesTax = SalesTax;
            dbOrder.TotalAmount = TotalAmount;

            foreach (var dbOrderItem in dbOrder.OrderItems)
            {
                OrderItemPricings.Where(r => r.OrderItemId == dbOrderItem.OrderItemId).Single().Apply(dbOrderItem);
            }
        }
    }

    public class OrderItemPricing
    {
        public long OrderItemId { get; set; }

        public int NetQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public void Apply(OrderItem dbOrderItem)
        {
            dbOrderItem.NetQuantity = NetQuantity;
            dbOrderItem.UnitPrice = UnitPrice;
            dbOrderItem.TotalPrice = TotalPrice;
        }
    }
}
