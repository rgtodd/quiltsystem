//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Linq;

using PagedList;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Paging;

namespace RichTodd.QuiltSystem.Web.Models.Order
{
    public class OrderModelFactory : ApplicationModelFactory
    {
        public OrderDetailModel CreateOrderDetailModel(UOrder_Order from)
        {
            var to = new OrderDetailModel();
            CopyOrderDetailModel(to, from);
            return to;
        }

        public OrderDetailListModel CreateOrderDetalListModel(IReadOnlyList<UOrder_Order> svcOrders, PagingState pagingState)
        {
            var orders = new List<OrderDetailModel>();
            foreach (var svcOrder in svcOrders)
            {
                var order = CreateOrderDetailModel(svcOrder);
                order.Collapsable = true;
                orders.Add(order);
            }

            IList<OrderDetailModel> sortedOrders = orders.OrderByDescending(r => r.SubmissionDateTime).ToList();

            var pageSize = 10;
            var pageNumber = WebMath.GetPageNumber(pagingState.Page, sortedOrders.Count, pageSize);
            var pagedOrders = sortedOrders.ToPagedList(pageNumber, pageSize);

            var model = new OrderDetailListModel()
            {
                Orders = pagedOrders
            };

            return model;
        }

        public OrderEditModel CreateOrderEditModel(UOrder_Order from)
        {
            var to = new OrderEditModel();
            CopyOrderEditModel(to, from);
            return to;
        }

        public OrderEditShippingAddressModel CreateOrderShippingAddressModel(UOrder_Order from)
        {
            if (from == null) return null;

            var to = new OrderEditShippingAddressModel();
            CopyOrderEditShippingAddressModel(to, from);
            return to;
        }

        private void CopyOrderDetailItemModel(OrderDetailItemModel to, MOrder_OrderItem from)
        {
            to.OrderItemSequence = from.OrderItemSequence;
            to.OrderItemId = from.OrderItemId;
            to.OrderableReference = from.OrderableReference;
            to.Name = from.Description;
            to.Sku = from.Sku;
            to.Quantity = from.NetQuantity;
            to.UnitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;
            to.Components = CreateOrderItemComponentModels(from.OrderItemComponents);
        }

        private void CopyOrderDetailModel(OrderDetailModel to, UOrder_Order from)
        {
            var toItems = CreateOrderDetailItemModels(from.MOrder.OrderItems);

            var toShipments = new List<OrderDetailShipmentModel>();
            var fromShipments = from.MFulfillable?.Shipments;
            if (fromShipments != null)
            {
                foreach (var fromShipment in fromShipments)
                {
                    var toShipment = new OrderDetailShipmentModel()
                    {
                        ShipmentDateTime = Locale.GetLocalTimeFromUtc(fromShipment.ShipmentDateTimeUtc),
                        ShippingVendor = fromShipment.ShippingVendorId,
                        TrackingCode = fromShipment.TrackingCode
                    };

                    var toShipmentItems = new List<OrderDetailItemModel>();
                    foreach (var fromShipmentItem in fromShipment.ShipmentItems)
                    {
                        var fromOrderItemId = ParseOrderItemId.FromFulfillableItemReference(fromShipmentItem.FulfillableItemReference);
                        var toItem = toItems.Where(r => r.OrderItemId == fromOrderItemId).Single();
                        toShipmentItems.Add(toItem);
                        _ = toItems.Remove(toItem);
                    }
                    toShipment.Items = toShipmentItems;

                    toShipments.Add(toShipment);
                }
            }

            var toReturns = new List<OrderDetailReturnModel>();
            var fromReturns = from.MFulfillable?.Returns;
            if (fromReturns != null)
            {
                foreach (var fromReturn in fromReturns)
                {
                    var toReturn = new OrderDetailReturnModel()
                    {
                        ReturnDateTime = Locale.GetLocalTimeFromUtc(fromReturn.CreateDateTimeUtc)
                    };

                    var toReturnItems = new List<OrderDetailItemModel>();
                    foreach (var fromReturnItem in fromReturn.ReturnItems)
                    {
                        var fromOrderItemId = ParseOrderItemId.FromFulfillableItemReference(fromReturnItem.FulfillableItemReference);
                        var toItem = toItems.Where(r => r.OrderItemId == fromOrderItemId).Single();
                        toReturnItems.Add(toItem);
                        _ = toItems.Remove(toItem);
                    }
                    toReturn.Items = toReturnItems;

                    toReturns.Add(toReturn);
                }
            }

            to.OrderId = from.MOrder.OrderId;
            to.OrderNumber = from.MOrder.OrderNumber;
            to.ItemSubtotal = from.MOrder.ItemSubtotalAmount;
            to.Shipping = from.MOrder.ShippingAmount;
            to.Discount = from.MOrder.DiscountAmount;
            to.TaxableAmount = from.MOrder.TaxableAmount;
            to.SalesTaxPercent = from.MOrder.SalesTaxPercent;
            to.SalesTax = from.MOrder.SalesTaxAmount;
            to.Total = from.MOrder.TotalAmount;
            to.OrderStatus = from.MOrder.OrderStatus.ToString();
            to.StatusDateTime = Locale.GetLocalTimeFromUtc(from.MOrder.UpdateDateTimeUtc);
            to.SubmissionDateTime = Locale.GetLocalTimeFromUtc(from.MOrder.SubmissionDateTimeUtc);
            to.ShippingName = from.MOrder.ShippingAddress.Name;
            to.ShippingAddressLines =
                FormatAddress(
                    from.MOrder.ShippingAddress.AddressLine1,
                    from.MOrder.ShippingAddress.AddressLine2,
                    from.MOrder.ShippingAddress.City,
                    from.MOrder.ShippingAddress.StateCode,
                    from.MOrder.ShippingAddress.PostalCode,
                    from.MOrder.ShippingAddress.CountryCode).ToArray();
            to.PendingItems = toItems;
            to.Shipments = toShipments;
            to.Returns = toReturns;
            to.CanCancel = from.MOrder.CanCancel;
            to.CanPay = from.MOrder.CanPay;
            to.CanReturn = from.MOrder.CanReturn;
        }

        private void CopyOrderEditItemModel(OrderEditItemModel to, MOrder_OrderItem from)
        {
            to.Components = CreateOrderItemComponentModels(from.OrderItemComponents);
            to.OrderItemId = from.OrderItemId;
            to.OrderableReference = from.OrderableReference;
            to.Description = from.Description;
            to.Quantity = from.NetQuantity;
            to.OriginalQuantity = from.OrderQuantity;
            to.Sku = from.Sku;
            to.TotalPrice = from.TotalPrice;
            to.KitPrice = from.UnitPrice;
        }

        private void CopyOrderEditModel(OrderEditModel to, UOrder_Order from)
        {
            to.OrderId = from.MOrder.OrderId;
            to.OrderNumber = from.MOrder.OrderNumber;
            to.Items = CreateOrderEditItemModels(from.MOrder.OrderItems);
            to.ItemSubtotal = from.MOrder.ItemSubtotalAmount;
            to.Discount = from.MOrder.DiscountAmount;
            to.Shipping = from.MOrder.ShippingAmount;
            to.TaxableSubtotal = to.ItemSubtotal - to.Discount + to.Shipping;
            to.SalesTaxPercent = from.MOrder.SalesTaxPercent;
            to.SalesTax = from.MOrder.SalesTaxAmount;
            to.Total = from.MOrder.TotalAmount;
            to.SubmissionDateTime = Locale.GetLocalTimeFromUtc(from.MOrder.SubmissionDateTimeUtc);
            to.OrderStatus = from.MOrder.OrderStatus.ToString();
        }

        private void CopyOrderEditShippingAddressModel(OrderEditShippingAddressModel to, UOrder_Order from)
        {
            to.OrderId = from.MOrder.OrderId;
            to.Name = from.MOrder.ShippingAddress.Name;
            to.AddressLine1 = from.MOrder.ShippingAddress.AddressLine1;
            to.AddressLine2 = from.MOrder.ShippingAddress.AddressLine2;
            to.City = from.MOrder.ShippingAddress.City;
            to.StateCode = from.MOrder.ShippingAddress.StateCode;
            to.PostalCode = from.MOrder.ShippingAddress.PostalCode;
            to.CountryCode = from.MOrder.ShippingAddress.CountryCode;
        }

        private void CopyOrderItemComponentModel(OrderItemComponentModel to, MOrder_OrderItemComponent from)
        {
            to.OrderableProjectComponentId = from.OrderableComponentId;
            to.Description = from.Description;
            //to.Sku = from.Sku;
            to.Quantity = from.Quantity;
            to.UnitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;
        }

        private OrderDetailItemModel CreateOrderDetailItemModel(MOrder_OrderItem from)
        {
            var to = new OrderDetailItemModel();
            CopyOrderDetailItemModel(to, from);
            return to;
        }

        private IList<OrderDetailItemModel> CreateOrderDetailItemModels(IEnumerable<MOrder_OrderItem> from)
        {
            var to = new List<OrderDetailItemModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateOrderDetailItemModel(fromItem));
            }
            return to;
        }

        private OrderEditItemModel CreateOrderEditItemModel(MOrder_OrderItem from)
        {
            var to = new OrderEditItemModel();
            CopyOrderEditItemModel(to, from);
            return to;
        }

        private IList<OrderEditItemModel> CreateOrderEditItemModels(IEnumerable<MOrder_OrderItem> from)
        {
            var to = new List<OrderEditItemModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateOrderEditItemModel(fromItem));
            }
            return to;
        }

        private OrderItemComponentModel CreateOrderItemComponentModel(MOrder_OrderItemComponent from)
        {
            var to = new OrderItemComponentModel();
            CopyOrderItemComponentModel(to, from);
            return to;
        }

        private IList<OrderItemComponentModel> CreateOrderItemComponentModels(IEnumerable<MOrder_OrderItemComponent> from)
        {
            var to = new List<OrderItemComponentModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateOrderItemComponentModel(fromItem));
            }
            return to;
        }

    }
}