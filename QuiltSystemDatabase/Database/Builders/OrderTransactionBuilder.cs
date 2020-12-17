//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;

namespace RichTodd.QuiltSystem.Database.Builders
{
    public class OrderTransactionBuilder : ITransactionBuilder<OrderTransaction, OrderTransactionBuilder>
    {
        private readonly QuiltContext m_ctx;

        private Order m_order;
        private DateTime m_utcNow;
        private OrderTransaction m_orderTransaction;
        private readonly TransactionDescriptionBuilder m_description = new TransactionDescriptionBuilder();

        public OrderTransactionBuilder(QuiltContext ctx)
        {
            m_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public OrderTransactionBuilder Begin(long orderId, string orderTransactionTypeCode, DateTime utcNow)
        {
            m_order = m_ctx.Orders.Where(r => r.OrderId == orderId).Single();
            m_utcNow = utcNow;

            m_orderTransaction = new OrderTransaction()
            {
                Order = m_order,
                OrderTransactionTypeCode = orderTransactionTypeCode,
                TransactionDateTimeUtc = m_utcNow,
                ItemSubtotal = 0,
                Shipping = 0,
                Discount = 0,
                SalesTax = 0,
                FundsRequired = 0,
                FundsReceived = 0
            };
            m_order.OrderTransactions.Add(m_orderTransaction);

            return this;
        }

        public OrderTransactionBuilder UnitOfWork(UnitOfWork unitOfWork)
        {
            m_orderTransaction.UnitOfWork = unitOfWork.Next();

            return this;
        }

        public OrderTransactionBuilder SetOrderStatusCode(string orderStatusCode)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (!string.IsNullOrEmpty(m_orderTransaction.OrderStatusCode))
            {
                throw new InvalidOperationException("Order status type already specified.");
            }

            var dbOrder = m_orderTransaction.Order;

            // Validate order status code.
            //
            switch (orderStatusCode)
            {
                case OrderStatusCodes.Pending:
                    {
                        if (dbOrder.SubmissionDateTimeUtc.HasValue)
                        {
                            throw new InvalidOperationException($"Cannot move submitted order {dbOrder.OrderId} to pending status.");
                        }
                    }
                    break;

                case OrderStatusCodes.Submitted:
                    {
                        if (dbOrder.SubmissionDateTimeUtc.HasValue)
                        {
                            throw new InvalidOperationException($"Order {dbOrder.OrderId} already submitted.");
                        }
                        dbOrder.SubmissionDateTimeUtc = m_utcNow;
                    }
                    break;

                case OrderStatusCodes.Fulfilling:
                    {
                        {
                            if (!dbOrder.SubmissionDateTimeUtc.HasValue)
                            {
                                throw new InvalidOperationException($"Order {dbOrder.OrderId} not submitted.");
                            }
                            if (dbOrder.FulfillmentDateTimeUtc.HasValue)
                            {
                                throw new InvalidOperationException($"Order {dbOrder.OrderId} already fulfilling.");
                            }
                            dbOrder.FulfillmentDateTimeUtc = m_utcNow;
                        }
                    }
                    break;

                case OrderStatusCodes.Closed:
                    {
                        // Always allowed.
                    }
                    break;
            }

            m_orderTransaction.OrderStatusCode = orderStatusCode;
            dbOrder.OrderStatusCode = orderStatusCode;
            dbOrder.OrderStatusDateTimeUtc = m_utcNow;

            m_description.Append($"Order status set to {orderStatusCode}.");

            return this;
        }

        public OrderTransactionBuilder SetSalesTaxJurisdiction(string salesTaxJurisdiction)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (!string.IsNullOrEmpty(m_orderTransaction.SalesTaxJurisdiction))
            {
                throw new InvalidOperationException("Sales tax jurisdiction already specified.");
            }

            var dbOrder = m_orderTransaction.Order;

            m_orderTransaction.SalesTaxJurisdiction = salesTaxJurisdiction;
            dbOrder.SalexTaxJurisdiction = salesTaxJurisdiction;

            m_description.Append($"Sales tax jurisdiction set to {salesTaxJurisdiction}.");

            return this;
        }

        public OrderTransactionBuilder SetSalesTaxRate(decimal salesTaxRate)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.SalesTaxRate != null)
            {
                throw new InvalidOperationException("Sales tax rate already specified.");
            }

            var dbOrder = m_orderTransaction.Order;

            m_orderTransaction.SalesTaxRate = salesTaxRate;
            dbOrder.SalesTaxRate = salesTaxRate;

            m_description.Append($"Sales tax rate set to {salesTaxRate:p}.");

            return this;
        }

        public OrderTransactionBuilder PrepareForSubmission()
        {
            _ = CaptureAll();

            m_order.ItemSubtotal = 0m;
            m_order.Shipping = 0m;
            m_order.SalesTax = 0m;

            _ = UpdatePricing();

            return this;
        }

        public OrderTransactionBuilder UpdatePricing()
        {
            var orderPricing = OrderPricing.Compute(m_order);

            foreach (var dbOrderItem in m_order.OrderItems)
            {
                var orderItemPricing = orderPricing.OrderItemPricings.Where(r => r.OrderItemId == dbOrderItem.OrderItemId).Single();

                dbOrderItem.NetQuantity = orderItemPricing.NetQuantity;
                dbOrderItem.UnitPrice = orderItemPricing.UnitPrice;
                dbOrderItem.TotalPrice = orderItemPricing.TotalPrice;
            }

            _ = AddItemSubtotal(orderPricing.ItemSubtotal - m_order.ItemSubtotal);
            _ = AddShipping(orderPricing.Shipping - m_order.Shipping);
            _ = AddSalesTax(orderPricing.SalesTax - m_order.SalesTax);
            m_order.PretaxAmount = orderPricing.PreTaxAmount;
            m_order.TaxableAmount = orderPricing.TaxableAmount;
            m_order.TotalAmount = orderPricing.TotalAmount;

            return this;
        }

        public OrderTransactionBuilder AddItemSubtotal(decimal itemSubtotalDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.ItemSubtotal != 0)
            {
                throw new InvalidOperationException("Item subtotal already specified.");
            }

            if (itemSubtotalDelta != 0)
            {
                m_orderTransaction.ItemSubtotal = itemSubtotalDelta;
                m_orderTransaction.Order.ItemSubtotal += itemSubtotalDelta;

                m_description.Append($"Item subtotal updated by {itemSubtotalDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddShipping(decimal shippingDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.Shipping != 0)
            {
                throw new InvalidOperationException("Shipping already specified.");
            }

            if (shippingDelta != 0)
            {
                m_orderTransaction.Shipping = shippingDelta;
                m_orderTransaction.Order.Shipping += shippingDelta;

                m_description.Append($"Shipping updated by {shippingDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddDiscount(decimal discountDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.Discount != 0)
            {
                throw new InvalidOperationException("Discount already specified.");
            }

            if (discountDelta != 0)
            {
                m_orderTransaction.Discount = discountDelta;
                m_orderTransaction.Order.Discount += discountDelta;

                m_description.Append($"Discount updated by {discountDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddSalesTax(decimal salesTaxDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.SalesTax != 0)
            {
                throw new InvalidOperationException("Sales tax already specified.");
            }

            if (salesTaxDelta != 0)
            {
                m_orderTransaction.SalesTax = salesTaxDelta;
                m_orderTransaction.Order.SalesTax += salesTaxDelta;

                m_description.Append($"Sales tax updated by {salesTaxDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddFundsRequired(decimal fundsRequiredDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.FundsRequired != 0)
            {
                throw new InvalidOperationException("Funding required already specified.");
            }

            if (fundsRequiredDelta != 0)
            {
                m_orderTransaction.FundsRequired = fundsRequiredDelta;
                m_orderTransaction.Order.FundsRequired += fundsRequiredDelta;

                m_description.Append($"Funds required updated by {fundsRequiredDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddFundsReceived(decimal fundsReceivedDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.FundsReceived != 0)
            {
                throw new InvalidOperationException("Funding received already specified.");
            }

            if (fundsReceivedDelta != 0)
            {
                m_orderTransaction.FundsReceived = fundsReceivedDelta;
                m_orderTransaction.Order.FundsReceived += fundsReceivedDelta;

                m_description.Append($"Funds received updated by {fundsReceivedDelta:c}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddOrderQuantity(long orderItemId, int orderQuantityDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.OrderQuantity != 0)
            {
                throw new InvalidOperationException("Order quantity already specified.");
            }

            if (orderQuantityDelta != 0)
            {
                dbOrderTransactionItem.OrderQuantity = orderQuantityDelta;
                dbOrderTransactionItem.OrderItem.OrderQuantity += orderQuantityDelta;

                m_description.Append($"Item {orderItemId} order quantity updated by {orderQuantityDelta}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddCancelQuantity(long orderItemId, int cancelQuantityDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.CancelQuantity != 0)
            {
                throw new InvalidOperationException("Cancel quantity already specified.");
            }

            if (cancelQuantityDelta != 0)
            {
                dbOrderTransactionItem.CancelQuantity = cancelQuantityDelta;
                dbOrderTransactionItem.OrderItem.CancelQuantity += cancelQuantityDelta;

                m_description.Append($"Item {orderItemId} cancel quantity updated by {cancelQuantityDelta}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddFulfillmentRequiredQuantity(long orderItemId, int fulfillmentRequiredDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.FulfillmentRequiredQuantity != 0)
            {
                throw new InvalidOperationException("Fulfillment request quantity already specified.");
            }

            if (fulfillmentRequiredDelta != 0)
            {
                dbOrderTransactionItem.FulfillmentRequiredQuantity = fulfillmentRequiredDelta;
                dbOrderTransactionItem.OrderItem.FulfillmentRequiredQuantity += fulfillmentRequiredDelta;

                m_description.Append($"Item {orderItemId} fulfillment required updated by {fulfillmentRequiredDelta}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddFulfillmentCompleteQuantity(long orderItemId, int fulfillmentCompleteDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.FulfillmentCompleteQuantity != 0)
            {
                throw new InvalidOperationException("Fulfillment complete quantity already specified.");
            }

            if (fulfillmentCompleteDelta != 0)
            {
                dbOrderTransactionItem.FulfillmentCompleteQuantity = fulfillmentCompleteDelta;
                dbOrderTransactionItem.OrderItem.FulfillmentCompleteQuantity += fulfillmentCompleteDelta;

                m_description.Append($"Item {orderItemId} fulfillment complete updated by {fulfillmentCompleteDelta}.");
            }

            return this;
        }

        public OrderTransactionBuilder AddFulfillmentReturnQuantity(long orderItemId, int fulfillmentReturnDelta)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.FulfillmentReturnQuantity != 0)
            {
                throw new InvalidOperationException("Fulfillment return quantity already specified.");
            }

            if (fulfillmentReturnDelta != 0)
            {
                dbOrderTransactionItem.FulfillmentReturnQuantity = fulfillmentReturnDelta;
                dbOrderTransactionItem.OrderItem.FulfillmentReturnQuantity += fulfillmentReturnDelta;

                m_description.Append($"Item {orderItemId} fulfillment return updated by {fulfillmentReturnDelta}.");
            }

            return this;
        }

        public OrderTransactionBuilder CaptureAll()
        {
            _ = CaptureSalesTaxJurisdiction();
            _ = CaptureSalesTaxRate();
            _ = CaptureDiscount();

            foreach (var orderItemId in m_order.OrderItems.Select(r => r.OrderItemId))
            {
                _ = CaptureOrderQuantity(orderItemId);
                _ = CaptureCancelQuantity(orderItemId);
            }

            return this;
        }

        public OrderTransactionBuilder CaptureSalesTaxJurisdiction()
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (!string.IsNullOrEmpty(m_orderTransaction.SalesTaxJurisdiction))
            {
                throw new InvalidOperationException("Sales tax jurisdiction already specified.");
            }

            var salesTaxJurisdiction = m_orderTransaction.Order.SalexTaxJurisdiction;
            if (!string.IsNullOrEmpty(salesTaxJurisdiction))
            {
                m_orderTransaction.SalesTaxJurisdiction = salesTaxJurisdiction;

                m_description.Append($"Sales tax jurisdiction {salesTaxJurisdiction} captured.");
            }

            return this;
        }

        public OrderTransactionBuilder CaptureSalesTaxRate()
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.SalesTaxRate != null)
            {
                throw new InvalidOperationException("Sales tax rate already specified.");
            }

            var salesTaxRate = m_orderTransaction.Order.SalesTaxRate;
            if (salesTaxRate != 0)
            {
                m_orderTransaction.SalesTaxRate = salesTaxRate;

                m_description.Append($"Sales tax rate {salesTaxRate:p} captured.");
            }

            return this;
        }

        public OrderTransactionBuilder CaptureDiscount()
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }
            if (m_orderTransaction.Discount != 0)
            {
                throw new InvalidOperationException("Discount already specified.");
            }

            var discountDelta = m_orderTransaction.Order.Discount;
            if (discountDelta != 0)
            {
                m_orderTransaction.Discount = discountDelta;

                m_description.Append($"Discount {discountDelta:c} captured.");
            }

            return this;
        }

        public OrderTransactionBuilder CaptureOrderQuantity(long orderItemId)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.OrderQuantity != 0)
            {
                throw new InvalidOperationException("Order quantity already specified.");
            }

            var orderQuantity = dbOrderTransactionItem.OrderItem.OrderQuantity;
            if (orderQuantity != 0)
            {
                dbOrderTransactionItem.OrderQuantity = orderQuantity;

                m_description.Append($"Item {orderItemId} order quantity {orderQuantity} captured.");
            }

            return this;
        }

        public OrderTransactionBuilder CaptureCancelQuantity(long orderItemId)
        {
            if (m_orderTransaction == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            var dbOrderTransactionItem = GetOrderTransactionItem(orderItemId);
            if (dbOrderTransactionItem.CancelQuantity != 0)
            {
                throw new InvalidOperationException("Cancel quantity already specified.");
            }

            var cancelQuantity = dbOrderTransactionItem.OrderItem.CancelQuantity;
            if (cancelQuantity != 0)
            {
                dbOrderTransactionItem.CancelQuantity = cancelQuantity;

                m_description.Append($"Item {orderItemId} cancel quantity {cancelQuantity} captured.");
            }

            return this;
        }

        public OrderTransactionBuilder Event(string eventTypeCode)
        {
            var dbOrderEvent = new OrderEvent()
            {
                OrderTransaction = m_orderTransaction,
                EventTypeCode = eventTypeCode,
                ProcessingStatusCode = EventProcessingStatusCodes.Pending,
                ProcessingStatusDateTimeUtc = m_utcNow,
                EventDateTimeUtc = m_utcNow,
            };
            m_orderTransaction.OrderEvents.Add(dbOrderEvent);

            return this;
        }

        public OrderTransaction Create()
        {
            m_order.UpdateDateTimeUtc = m_utcNow;

            m_orderTransaction.Description = m_description.ToString();

            return m_orderTransaction;
        }

        private OrderTransactionItem GetOrderTransactionItem(long orderItemId)
        {
            var dbOrderTransactionItem = m_orderTransaction.OrderTransactionItems.Where(r => r.OrderItemId == orderItemId).SingleOrDefault();
            if (dbOrderTransactionItem == null)
            {
                var dbOrderItem = m_order.OrderItems.Where(r => r.OrderItemId == orderItemId).Single();

                dbOrderTransactionItem = new OrderTransactionItem()
                {
                    OrderTransaction = m_orderTransaction,
                    OrderItemId = orderItemId,
                    OrderItem = dbOrderItem,
                    FulfillmentRequiredQuantity = 0,
                    FulfillmentCompleteQuantity = 0,
                    FulfillmentReturnQuantity = 0,
                    CancelQuantity = 0
                };
                m_orderTransaction.OrderTransactionItems.Add(dbOrderTransactionItem);
            }

            return dbOrderTransactionItem;
        }
    }
}
