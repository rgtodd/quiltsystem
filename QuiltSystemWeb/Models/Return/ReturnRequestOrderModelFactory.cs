//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Models.Return
{
    public static class ReturnRequestOrderModelFactory
    {
        #region Methods

        public static ReturnRequestOrderItemModel CreateReturnRequestOrderItemModel(MOrder_OrderItem from)
        {
            var to = new ReturnRequestOrderItemModel();
            CopyReturnOrderDetailItemModel(to, from);
            return to;
        }

        private static void CopyReturnOrderDetailItemModel(ReturnRequestOrderItemModel to, MOrder_OrderItem from)
        {
            to.OrderItemId = from.OrderItemId;
            to.OrderItemSequence = from.OrderItemSequence;
            to.OrderableReference = from.OrderableReference;
            to.Description = from.Description;
            to.Sku = from.Sku;
            to.Quantity = from.NetQuantity;
            to.UnitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;
            //to.Status = from.Status;
            //to.StatusDateTime = from.StatusDateTimeUtc;
            to.Components = CreateReturnRequestOrderItemComponentModels(from.OrderItemComponents);
        }

        private static void CopyReturnOrderItemComponentModel(ReturnRequestOrderItemComponentModel to, MOrder_OrderItemComponent from)
        {
            to.OrderableProjectComponentId = from.OrderableComponentId;
            to.Name = from.Description;
            //to.Sku = from.Sku;
            to.Quantity = from.Quantity;
            to.UnitPrice = from.UnitPrice;
            to.TotalPrice = from.TotalPrice;
        }

        private static ReturnRequestOrderItemComponentModel CreateReturnRequestOrderItemComponentModel(MOrder_OrderItemComponent from)
        {
            var to = new ReturnRequestOrderItemComponentModel();
            CopyReturnOrderItemComponentModel(to, from);
            return to;
        }

        private static IList<ReturnRequestOrderItemComponentModel> CreateReturnRequestOrderItemComponentModels(IEnumerable<MOrder_OrderItemComponent> from)
        {
            var to = new List<ReturnRequestOrderItemComponentModel>();
            foreach (var fromItem in from)
            {
                to.Add(CreateReturnRequestOrderItemComponentModel(fromItem));
            }
            return to;
        }

        #endregion Methods
    }
}