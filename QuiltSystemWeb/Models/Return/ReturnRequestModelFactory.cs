//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Service.User.Abstractions.Data;

namespace RichTodd.QuiltSystem.Web.Models.Return
{
    public class ReturnRequestModelFactory : ApplicationModelFactory
    {
        public ReturnRequestEditModel CreateReturnRequestEditModel(UOrder_Order fromOrder, MFulfillment_ReturnRequest fromReturnRequest, IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        {
            var to = new ReturnRequestEditModel();
            if (fromReturnRequest != null)
            {
                //CopyReturnRequestEditModel(to, fromOrder, fromReturnRequest, fromReturnRequestReasons);
            }
            else
            {
                CopyReturnRequestEditModel(to, fromOrder, fromReturnRequestReasons);
            }
            return to;
        }

        public ReturnRequestViewModel CreateReturnRequestViewModel(UOrder_Order fromOrder, MFulfillment_ReturnRequest fromReturnRequest, IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        {
            var to = new ReturnRequestViewModel();
            if (fromReturnRequest != null)
            {
                CopyReturnRequestViewModel(to, fromOrder, fromReturnRequest, fromReturnRequestReasons);
            }
            else
            {
                CopyReturnRequestViewModel(to, fromOrder);
            }
            return to;
        }

        //private void CopyReturnRequestEditModel(ReturnRequestEditModel to, UOrder_Order fromOrder, MFulfillment_ReturnRequest fromReturnRequest, IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        //{
        //    var toReturnRequestItems = new List<ReturnRequestEditItemModel>();
        //    foreach (var fromReturnRequestItem in fromReturnRequest.ReturnRequestItems)
        //    {
        //        var fromOrderItemId = ParseOrderItemId.FromFulfillableItemReference(fromReturnRequestItem.FulfillableItemReference);
        //        var fromOrderItem = fromOrder.MOrder.OrderItems.Where(r => r.OrderItemId == fromOrderItemId).Single();
        //        if (fromOrderItem.CanReturn)
        //        {
        //            var toReturnRequestItem = new ReturnRequestEditItemModel()
        //            {
        //                OrderReturnRequestItemId = fromReturnRequestItem.ReturnRequestItemId,
        //                Quantity = fromReturnRequestItem.Quantity,
        //                MaximumQuantity = fromOrderItem.FulfillmentCompleteQuantity,
        //                OrderItem = ReturnRequestOrderModelFactory.CreateReturnRequestOrderItemModel(fromOrder.MOrder.OrderItems.Where(r => r.OrderItemId == fromOrderItem.OrderItemId).Single())
        //            };
        //            toReturnRequestItems.Add(toReturnRequestItem);
        //        }
        //    }

        //    to.OrderReturnRequestId = fromReturnRequest.ReturnRequestId;
        //    to.ReturnTypeCode = fromReturnRequest.ReturnRequestType;
        //    to.ReturnTypeName = fromReturnRequest.ReturnRequestTypeCode;
        //    //to.ReasonTypeCode = fromReturnRequest.re;
        //    //to.ReasonTypeName = fromReturnRequestReasons.Where(r => r.ReturnRequestReasonTypeCode == fromReturnRequest.ReasonTypeCode).SingleOrDefault()?.Name;
        //    to.ReasonTypes = CreateReasonTypes(fromReturnRequestReasons);
        //    //to.Notes = fromReturnRequest.;
        //    to.Items = toReturnRequestItems;
        //    to.OrderId = fromOrder.MOrder.OrderId;
        //    to.OrderNumber = fromOrder.MOrder.OrderNumber;
        //}

        private void CopyReturnRequestEditModel(ReturnRequestEditModel to, UOrder_Order fromOrder, IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        {
            var toReturnRequestItems = new List<ReturnRequestEditItemModel>();
            foreach (var fromOrderItem in fromOrder.MOrder.OrderItems)
            {
                if (fromOrderItem.CanReturn)
                {
                    var toReturnRequestItem = new ReturnRequestEditItemModel()
                    {
                        OrderReturnRequestItemId = null,
                        Quantity = 0,
                        MaximumQuantity = fromOrderItem.NetQuantity,
                        OrderItem = ReturnRequestOrderModelFactory.CreateReturnRequestOrderItemModel(fromOrder.MOrder.OrderItems.Where(r => r.OrderItemId == fromOrderItem.OrderItemId).Single())
                    };
                    toReturnRequestItems.Add(toReturnRequestItem);
                }
            }

            to.OrderReturnRequestId = null;
            //to.ReturnTypeCode = 0;
            to.ReturnTypeName = null;
            to.ReasonTypeCode = null;
            to.ReasonTypeName = null;
            to.ReasonTypes = CreateReasonTypes(fromReturnRequestReasons);
            to.Notes = null;
            to.Items = toReturnRequestItems;
            to.OrderId = fromOrder.MOrder.OrderId;
            to.OrderNumber = fromOrder.MOrder.OrderNumber;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private void CopyReturnRequestViewModel(ReturnRequestViewModel to, UOrder_Order fromOrder, MFulfillment_ReturnRequest fromReturnRequest, IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        {
            //var toReturnRequestItems = new List<ReturnRequestViewItemModel>();
            //foreach (var fromReturnRequestItem in fromReturnRequest.Items)
            //{
            //    var fromOrderItem = fromOrder.OrderItems.Where(r => r.OrderItem.OrderItemId == fromReturnRequestItem.OrderItemId).Single();
            //    if (fromOrderItem.OrderItem.CanReturn)
            //    {
            //        var toReturnRequestItem = new ReturnRequestViewItemModel()
            //        {
            //            OrderReturnRequestItemId = fromReturnRequestItem.OrderReturnRequestItemId,
            //            Quantity = fromReturnRequestItem.Quantity,
            //            OrderItem = ReturnRequestOrderModelFactory.CreateReturnRequestOrderItemModel(fromOrder.OrderItems.Where(r => r.OrderItem.OrderItemId == fromOrderItem.OrderItem.OrderItemId).Single())
            //        };
            //        toReturnRequestItems.Add(toReturnRequestItem);
            //    }
            //}

            //to.OrderReturnRequestId = fromReturnRequest.OrderReturnRequestId;
            //to.ReturnRequestNumber = fromReturnRequest.ReturnRequestNumber;
            //to.ReturnTypeCode = (int)fromReturnRequest.ReturnType;
            //to.ReturnTypeName = GetReturnTypeName(fromReturnRequest.ReturnType);
            //to.ReasonTypeCode = fromReturnRequest.ReasonTypeCode;
            //to.ReasonTypeName = fromReturnRequestReasons.Where(r => r.ReturnRequestReasonTypeCode == fromReturnRequest.ReasonTypeCode).Single().Name;
            //to.Notes = fromReturnRequest.Notes;
            //to.Items = toReturnRequestItems;
            //to.OrderId = fromOrder.MOrder.OrderId;
            //to.OrderNumber = fromOrder.MOrder.OrderNumber;
        }

        private void CopyReturnRequestViewModel(ReturnRequestViewModel to, UOrder_Order fromOrder)
        {
            to.OrderId = fromOrder.MOrder.OrderId;
            to.OrderNumber = fromOrder.MOrder.OrderNumber;
        }

        private List<SelectListItem> CreateReasonTypes(IReadOnlyList<MFulfillment_ReturnRequestReason> fromReturnRequestReasons)
        {
            var returnReasons = new List<SelectListItem>();

            // Create default entry.
            //
            {
                var returnReason = new SelectListItem()
                {
                    Value = "",
                    Text = "(Select One)"
                };
                returnReasons.Add(returnReason);
            }

            foreach (var svcReturnReason in fromReturnRequestReasons)
            {
                var returnReason = new SelectListItem()
                {
                    Value = svcReturnReason.ReturnRequestReasonTypeCode.ToString(),
                    Text = svcReturnReason.Name
                };
                returnReasons.Add(returnReason);
            }

            return returnReasons;
        }

        //private string GetReturnTypeName(UOrder_OrderReturnRequest.ReturnTypes returnType)
        //{
        //    return returnType switch
        //    {
        //        UOrder_OrderReturnRequest.ReturnTypes.Return => "Return",
        //        UOrder_OrderReturnRequest.ReturnTypes.Replace => "Replace",
        //        UOrder_OrderReturnRequest.ReturnTypes.Manual => "Manual",
        //        _ => "*UNKNOWN*",
        //    };
        //}

        //private string GetReturnTypeCode(UOrder_OrderReturnRequest.ReturnTypes returnType)
        //{
        //    return returnType switch
        //    {
        //        UOrder_OrderReturnRequest.ReturnTypes.Return => "Return",
        //        UOrder_OrderReturnRequest.ReturnTypes.Replace => "Replace",
        //        UOrder_OrderReturnRequest.ReturnTypes.Manual => "Manual",
        //        _ => "*UNKNOWN*",
        //    };
        //}

    }
}